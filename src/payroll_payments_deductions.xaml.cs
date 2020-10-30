using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data;
using Xceed.Words.NET;
using System.Diagnostics;
using System.IO;
using System.Windows.Xps.Packaging;

namespace accounts
{
    /// <summary>
    /// Interaction logic for payroll_payments_deductions.xaml
    /// </summary>
    public partial class payroll_payments_deductions : Window
    {
        IEnumerable<Model.PayRollVoucherModel> report;
        int cindex = 0;
        List<int> p_nos = new List<int>();
        ListCollectionView listCollectionView;
        double CrLimit, gCrLimit, DrLimit, gDrlimit;
        string wordFname = null;
        string xpsFname = null;
        string pdfFname = null;
        string excelFname = null;
        void ClearPrintCache()
        {
            try
            {

                wordFname = null;
                xpsFname = null;
                pdfFname = null;
                excelFname = null;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public payroll_payments_deductions()
        {
            InitializeComponent();

        }
        void FindButtonState()
        {
            btn_doc.IsEnabled = true;
            btn_pdf.IsEnabled = true;
            btn_xps.IsEnabled = true;
            ClearPrintCache();

            btn_save.IsEnabled = false;
            btn_update1.IsEnabled = true;
            btn_delete.IsEnabled = true;
            btn_print.IsEnabled = true;
            cmb_employee.Focus();
        }
        void Transactions()
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }
        public void TabToSave()
        {

            if (btn_save.IsEnabled == true)
            {
                btn_save.Focus();
            }
            else
            {
                btn_update1.Focus();
            }
        }
        void NewButtonState()
        {
            btn_doc.IsEnabled = false;
            btn_pdf.IsEnabled = false;
            btn_xps.IsEnabled = false;
            btn_task_add.IsEnabled = false;
            btn_task_del.IsEnabled = false;
            ClearPrintCache();
            txt_task_Amount.Text = "";
            txt_task_label.Text = "";

            btn_print.IsEnabled = false;
            cmb_employee.Text = "";

            chk_isrecurr.IsChecked = false;
            txt_cramount.Text = "";
            txt_narration.Text = "";
            txt_total.Text = "0.00";
            txt_empid.Text = "";
            cmb_entry.Text = "";
            lbl_taskflag.Visibility = Visibility.Collapsed;
            txt_info.Text = "";
            cmb_cash.Text = "";
            var tlist = ViewModels_Variables.ModelViews.Tasks.Where((tasks) => tasks.ENTRY == "PAYROLL VOUCHER");

            DataTemplate tasksTemplate = this.FindResource("Tasks") as DataTemplate;
            lst_tasks.ItemTemplate = tasksTemplate;

            lst_tasks.ItemsSource = tlist;
            lst_tasks.Visibility = Visibility.Visible;
            hist_task_lbl.Content = "Task".ToUpper();
            btn_save.IsEnabled = true;
            btn_update1.IsEnabled = false;
            btn_delete.IsEnabled = false;
            txt_pno.Text = DB.Connection.NewEntryno(table: "payroll_posting", field: "pp_no").ToString();
            dtp_pdate.SelectedDate = ViewModels_Variables._sysDate[0];
            p_nos = (from p in ViewModels_Variables.ModelViews.PayrollVouchers select p.VNO).ToList<int>();
            cmb_employee.Focus();
        }


        private void cmb_entry_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmb_employee_KeyDown(object sender, KeyEventArgs e)
        {





            int crid;
            try
            {
                if (cmb_employee.SelectedItem != null)
                {


                    var row = (Model.EmployeeModel)cmb_employee.SelectedItem;
                    if (row != null)
                    {
                        var emp_acc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.ID == row.Account.ID).FirstOrDefault();
                        if (emp_acc != null)
                        {

                            txt_info.Text = emp_acc.Name;
                            crid = emp_acc.ID;
                            var p = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.ID == emp_acc.ParentGroup).FirstOrDefault();
                            if (p != null)
                            {
                                gDrlimit = p.ID;
                            }
                            txt_empid.Text = row.Emp_Id;
                            string infotxt;
                            infotxt = emp_acc.Name;
                            if (DrLimit > 0)
                            {
                                infotxt += " Dr.Lock : " + DrLimit;
                            }
                            else if (gDrlimit > 0)
                            {
                                infotxt += " Dr.Lock : " + gDrlimit;
                            }

                            var ob = DB.Connection.GetActBalance(crid, "");
                            if (ob > 0) { infotxt += ", OB: " + ob; }
                            txt_info.Text = infotxt;
                            var list = (from r in ViewModels_Variables.ModelViews.PayrollVouchers
                                        where (r.VNO) != int.Parse(txt_pno.Text.ToString()) && r.DrAccount.Account.ID == crid
                                        select new { eno = r.VNO, edate = r.PPDate, amount = r.Amount });
                            lst_tasks.ItemsSource = list;
                            DataTemplate pre = this.FindResource("tr_History2") as DataTemplate;
                            lst_tasks.ItemTemplate = pre;

                        }

                    }
                    public_members._TabPress(e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void txt_cramount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                txt_total.Text = txt_cramount.Text.ToString();
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txt_narration_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter) TabToSave();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btn_movefirst_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                //int feno;

                if (p_nos.Count > 0)
                {
                    int pno = p_nos[0];
                    cindex = 0;
                    Find(pno);
                }


                else
                {
                    MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                MessageBox.Show(perror.Message.ToString());
            }
        }

        private void btn_moveprevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (p_nos.Count > 0 && cindex >= 0 && cindex < p_nos.Count)
                {
                    if (cindex != 0) { cindex--; }

                    Find(p_nos[cindex]);
                }


                else
                {
                    MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                MessageBox.Show(perror.Message.ToString());
            }
        }

        private void btn_movenext_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (p_nos.Count > 0 && cindex < p_nos.Count - 1)
                {

                    cindex++;

                    Find(p_nos[cindex]);
                }


                else
                {
                    MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                MessageBox.Show(perror.Message.ToString());
            }
        }

        private void btn_movelast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (p_nos.Count > 0)
                {
                    int pno = p_nos[p_nos.Count - 1];
                    cindex = p_nos.Count - 1;
                    Find(pno);
                }


                else
                {
                    MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                MessageBox.Show(perror.Message.ToString());
            }
        }

        private void btn_find_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txt_pnofind_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dtp_pdate_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                accountRegistration acc = new accountRegistration();
                acc.Owner = this;
                acc.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            //delete
            if (cmb_employee.SelectedItem != null && cmb_cash.SelectedItem != null)
            {

                int.TryParse(txt_pno.Text.ToString(), out int jno);
                var rr = DB.PayrollVoucher.Remove(jno);
                if (rr == true)
                {
                    MessageBox.Show("Voucher Removed");
                    btn_Reset_Click(sender, e);

                }
                else
                {
                    MessageBox.Show("DataBase connection lost");
                }


            }

        }

        private void btn_update1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double amount;
                Model.PayRollVoucherModel payRoll = new Model.PayRollVoucherModel();
                double.TryParse(txt_cramount.Text.ToString(), out amount);
                var emp_acc = (Model.EmployeeModel)cmb_employee.SelectedItem;
                var cash_acc = (Model.AccountModel)cmb_cash.SelectedItem;
                int.TryParse(txt_pno.Text.ToString(), out int ppno);

                if (cmb_employee.SelectedItem != null && amount > 0 && cmb_entry.Text != null && cmb_cash.SelectedItem != null)
                {
                    payRoll.Isrecurring = (bool)chk_isrecurr.IsChecked;
                    payRoll.DrAccount = emp_acc ;
                    payRoll.CrAccount = cash_acc ;
                    payRoll.VoucherType = cmb_entry.Text.ToString();
                    payRoll.Amount = amount;
                    payRoll.Narration = txt_narration.Text.ToString();
                    payRoll.Task_ID = 0;
                    payRoll.VNO = ppno;

                    var r = DB.PayrollVoucher.Update(payRoll);
                    if (r == true)
                    {
                        MessageBox.Show("Voucher Updated");
                        btn_Reset_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Something went Wrong ");
                    }


                }

                else
                {
                    MessageBox.Show("Enter data correctly");
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            NewButtonState();
        }
        void FindTask(int eno)
        {
            try
            {
                var t1 = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ENO == eno && ts.ENTRY == "PAYROLL VOUCHER").FirstOrDefault();
                if (t1 != null)
                {
                    taskgroup.IsEnabled = true;
                    btn_task_del.IsEnabled = true;
                    btn_task_add.IsEnabled = true;
                    txt_task_Amount.Text = t1.T_AMOUNT.ToString("0.00");
                    txt_task_label.Text = t1.T_LABEL;
                    chk_isrecurr.IsChecked = true;
                }
                else
                {
                    btn_task_add.IsEnabled = true;
                    taskgroup.IsEnabled = true;
                    btn_task_del.IsEnabled = false;
                    chk_isrecurr.IsChecked = false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void Find(int no)
        {
            try
            {
                NewButtonState();
                var entries = ViewModels_Variables.ModelViews.PayrollVouchers.Where((pp) => pp.VNO == no).FirstOrDefault();
                if (entries != null)
                {
                    txt_pno.Text = entries.VNO.ToString();
                    txt_empid.Text = entries.DrAccount.Account.ID.ToString();
                    txt_cramount.Text = entries.Amount.ToString("0.00");
                    txt_total.Text = entries.Amount.ToString("0.00");
                    txt_narration.Text = entries.Narration;
                    dtp_pdate.SelectedDate = entries.PPDate;
                    cmb_entry.SelectedItem = (ComboBoxItem)cmb_entry.Items.Cast<ComboBoxItem>().Where((s, b) => s.Content.Equals(entries.VoucherType)).SingleOrDefault();
                    
                    if (entries.DrAccount != null) cmb_employee.SelectedItem = entries.DrAccount;
                     
                    if (entries.CrAccount != null) { cmb_cash.SelectedItem = entries.CrAccount; }

                    chk_isrecurr.IsChecked = entries.Isrecurring;

                    lbl_taskflag.Visibility = Visibility.Collapsed;
                    if (entries.Task_ID > 0) { lbl_taskflag.Visibility = Visibility.Visible; }

                    FindTask(entries.VNO);
                    FindButtonState();

                }
                else
                {
                    MessageBox.Show("Entry not found");
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double amount;
                Model.PayRollVoucherModel payRoll = new Model.PayRollVoucherModel();
                double.TryParse(txt_cramount.Text.ToString(), out amount);
                var emp_acc = (Model.EmployeeModel)cmb_employee.SelectedItem;
                var cash_acc = (Model.AccountModel)cmb_cash.SelectedItem;


                if (cmb_employee.SelectedItem != null && amount > 0 && cmb_entry.Text != null && cmb_cash.SelectedItem != null)
                {
                    payRoll.Isrecurring = (bool)chk_isrecurr.IsChecked;
                    payRoll.DrAccount = emp_acc;
                    payRoll.CrAccount = cash_acc;
                    payRoll.VoucherType = cmb_entry.Text.ToString();
                    payRoll.Amount = amount;
                    payRoll.Narration = txt_narration.Text.ToString();
                    payRoll.Task_ID = 0;
                    payRoll.PPDate = dtp_pdate.SelectedDate.Value;

                    var r = DB.PayrollVoucher.Save(payRoll);
                    if (r > 0)
                    {
                        MessageBox.Show("Entry Saved");
                        btn_Reset_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Something went Wrong ");
                    }


                }

                else
                {
                    MessageBox.Show("Enter data correctly");
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MessageBoxResult res = new MessageBoxResult();
                res = MessageBox.Show("Do you want close this Window", "Close the Window", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
        }

        private void btn_cash_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmb_employee.SelectedItem!=null)
                {
                    var emp = (Model.EmployeeModel)cmb_employee.SelectedItem;
                    if (emp.Account.ID > 0)
                    {
                    CashBook1 cashsow = new CashBook1(emp.Account.ID);
                    cashsow.Show();
                    }
                    else
                    {
                        MessageBox.Show("Not a valid account");
                    }
                }
                else
                {
                    MessageBox.Show("Not a valid account");
                }
            }
            catch (Exception eee)
            {
                MessageBox.Show(eee.Message.ToString());
            }

        }

        private void lst_tasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
var t = (Model.Task)lst_tasks.SelectedItem;
                if (t != null)
                {
                    
                    MessageBoxResult res1 = new MessageBoxResult();
                    res1 = MessageBox.Show("Do you want to execute this task", "Task", MessageBoxButton.YesNo);
                    if (res1 == MessageBoxResult.Yes)
                    {
                        
                            var res = DB.PayrollVoucher.Tasker(t, dtp_pdate.SelectedDate.Value);
                            if (res == true)
                            {
                                MessageBox.Show("Task completed");
                                btn_Reset_Click(sender, e);
                            }
                         
                    }
                }
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void btn_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ppReport pp = new ppReport(); pp.Owner = this;
                pp.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (wordFname == null)
                {
                    wordFname = CreateWordDoc();
                }
                if (wordFname != null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }

                if (xpsFname != null)
                {

                    XpsDocument xps = new XpsDocument(xpsFname, FileAccess.Read);
                    accounts.PrintDialogue print = new PrintDialogue(xps, this.Title + " " + txt_pno.Text.ToString());
                    print.Show();
                    xps.Close();
                }



            }
            catch (Exception w1)
            {

                MessageBox.Show(w1.Message.ToString());
            }
        }
        string CreateWordDoc( )
        {
            string fileName = null;
            try
            {
                  fileName = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\doc", ".doc");
                using (var document = DocX.Create(fileName))
                {
                    // Add a title
                    document.ApplyTemplate(@"DocTemplates\PayrollVoucher.dotx");

                    //Footer
                    document.ReplaceText("[MyCompany]", ViewModels_Variables.ModelViews.CompanyProfile[0].company);
                    document.ReplaceText("[Clandmark]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].lmark);
                    document.ReplaceText("[CCity]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].place);
                    document.ReplaceText("[CPhone]", "Phone :" + ViewModels_Variables.ModelViews.CompanyProfile[0].officeno);


                    var cust = ((Model.EmployeeModel)cmb_employee.SelectedItem).Account;
                    if (cust!=null)
                    {
                        document.ReplaceText("[Employee]", cust.Name);
                        document.ReplaceText("[Address]", cust.Address);
                        document.ReplaceText("[City]", cust.City);
                        document.ReplaceText("[Phone]", cust.PhoneNo);

                    }

                    //Employee Info
                    document.ReplaceText("[EID]", txt_empid.Text.ToString());
                    document.ReplaceText("[VNo]", txt_pno.Text.ToString());
                    document.ReplaceText("[Date]", dtp_pdate.SelectedDate.Value.ToShortDateString());
                    string jinv = null;
                    document.ReplaceText("[Voucher]", cmb_entry.Text.ToString());
                    document.ReplaceText("[Amount]", txt_cramount.Text.ToString());
                    document.ReplaceText("[Total]", txt_cramount.Text.ToString());


                    string narration = null;
                    if (txt_narration.Text != null && txt_narration.Text.Length > 0) { narration = txt_narration.Text.ToString(); } else { narration = ""; }
                    document.ReplaceText("[Narration]", "Note:" + narration);
                    document.Save();
                    document.Dispose();
                }
            }
            catch (Exception ee)
            {

                MessageBox.Show(ee.Message.ToString());
            }
            return fileName;
        }
        private void btn_doc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wordFname == null)
                {
                 wordFname=   CreateWordDoc();
                }

               if(wordFname!=null) Process.Start("winword.exe", wordFname);
            }
            catch (Exception w1)
            {

                MessageBox.Show(w1.Message.ToString());
            }
        }

        private void btn_pdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wordFname == null)
                {
                    wordFname = CreateWordDoc();
                }
                if (wordFname != null)
                {
                    pdfFname = ReportExport.WordExport.CreatePDF(wordFname);
                }
                if(pdfFname!=null)Process.Start("AcroRd32.exe", pdfFname);


            }
            catch (Exception w1)
            {

                MessageBox.Show(w1.Message.ToString());
            }
        }

        private void btn_xps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wordFname == null)
                {
                    wordFname = CreateWordDoc();
                }
                if (wordFname != null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }

                if(xpsFname!=null)Process.Start("explorer.exe", xpsFname);


            }
            catch (Exception w1)
            {

                MessageBox.Show(w1.Message.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (ViewModels_Variables.ModelViews.Accounts.Count <= 0)
                //{
                //    DB.Accounts.Fetch();
                //    ViewModels_Variables.ModelViews.AccountToCollection();
                //}
                //if (ViewModels_Variables.ModelViews.Employees.Count <= 0)
                //{
                //    DB.Employee.Fetch();
                //    ViewModels_Variables.ModelViews.Employees_To_List();
                //}
                //if (ViewModels_Variables.ModelViews.PayrollVouchers.Count <= 0)
                //{
                //    DB.PayrollVoucher.Fetch();
                //    ViewModels_Variables.ModelViews.PayRolls_To_List();
                //}

                var v = ViewModels_Variables.ModelViews;
                DataContext = v;
                NewButtonState();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void chk_isrecurr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chk_isrecurr.IsChecked == true)
                {
                    var cr = (Model.EmployeeModel)cmb_employee.SelectedItem;
                    if (cr != null)
                    {
                        txt_task_label.Text = cr.Account.Name;
                        txt_task_Amount.Text = txt_cramount.Text.ToString();
                    }
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void btn_task_del_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btn_save.IsEnabled == false)
                {
                    int.TryParse(txt_pno.Text.ToString(), out int t);
                    var t1 = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ENO == t && ts.ENTRY == "PAYROLL VOUCHER").FirstOrDefault();
                    if (t1 != null)
                    {
                        var tt = DB.Task.DeleteTask(eno: t1.ENO, entry: "PAYROLL VOUCHER");
                        if (tt == true)
                        {
                            MessageBox.Show("Task removed");
                            btn_Reset_Click(sender, e);
                        }
                    }
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void chk_isrecurr_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chk_isrecurr.IsChecked == true)
                {
                    var cr = (Model.EmployeeModel)cmb_employee.SelectedItem;
                    if (cr != null)
                    {
                        txt_task_label.Text = cr.Account.Name;
                        txt_task_Amount.Text = txt_cramount.Text.ToString();
                    }
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void btn_task_add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btn_save.IsEnabled == false && chk_isrecurr.IsChecked == true)
                {
                    Model.Task task = new Model.Task();
                    int.TryParse(txt_pno.Text.ToString(), out int t);
                    decimal.TryParse(txt_task_Amount.Text.ToString(), out decimal a);
                    task.T_LABEL = txt_task_label.Text.ToString();
                    task.T_AMOUNT = a;
                    task.ENO = t;
                    task.ENTRY = "PAYROLL VOUCHER";
                    var r = DB.Task.AddTask(task);
                    if (r > 0)
                    {
                        MessageBox.Show("Task Added");
                        btn_Reset_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Some thing went wrong");
                    }

                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void cmb_employee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int crid;
            try
            {
                if (cmb_employee.SelectedItem != null)
                {


                    var row = (Model.EmployeeModel)cmb_employee.SelectedItem;
                    if (row != null)
                    {
                        var emp_acc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.ID == row.Account.ID).FirstOrDefault();
                        if (emp_acc != null)
                        {

                            txt_info.Text = emp_acc.Name;
                            crid = emp_acc.ID;
                            var p = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.ID == emp_acc.ParentGroup).FirstOrDefault();
                            if (p != null)
                            {
                                gDrlimit = p.ID;
                            }
                            txt_empid.Text = row.Emp_Id;
                            string infotxt;
                            infotxt = emp_acc.Name;
                            if (DrLimit > 0)
                            {
                                infotxt += " Dr.Lock : " + DrLimit;
                            }
                            else if (gDrlimit > 0)
                            {
                                infotxt += " Dr.Lock : " + gDrlimit;
                            }

                            var ob = DB.Connection.GetActBalance(crid, "");
                            if (ob > 0) { infotxt += ", OB: " + ob; }
                            txt_info.Text = infotxt;
                            var list = (from r in ViewModels_Variables.ModelViews.PayrollVouchers
                                        where (r.VNO) != int.Parse(txt_pno.Text.ToString()) && r.DrAccount.Account.ID == crid
                                        select new { eno = r.VNO, edate = r.PPDate, amount = r.Amount });
                            lst_tasks.ItemsSource = list;
                            DataTemplate pre = this.FindResource("tr_History2") as DataTemplate;
                            lst_tasks.ItemTemplate = pre;

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void cmb_cash_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int drid;
            try
            {


                if (cmb_cash.SelectedItem != null)
                {
                    var row = (Model.AccountModel)cmb_cash.SelectedItem;

                    if (row != null)
                    {
                        var cash_acc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.ID == row.ID).FirstOrDefault();
                        if (cash_acc != null)
                        {
                            CrLimit = cash_acc.CrLimit;

                            var p = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.ID == cash_acc.ParentGroup).FirstOrDefault();
                            if (p != null)
                            {
                                gCrLimit = p.Cr_Loc;
                            }
                            string infotxt;
                            infotxt = row.Name;
                            if (CrLimit > 0)
                            {
                                infotxt += " Cr.Lock : " + CrLimit;
                            }
                            else if (gCrLimit > 0)
                            {
                                infotxt += " Cr.Lock : " + gCrLimit;
                            }

                            var ob = DB.Connection.GetActBalance(row.ID, "");

                            if (ob > 0) { infotxt += ", OB: " + ob; }
                            txt_info.Text = infotxt;
                        }
                    }
                }



            }
            catch (Exception)
            {

                throw;
            }
        }

        private void lst_tasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmb_cash_KeyDown(object sender, KeyEventArgs e)
        {
            int drid;
            try
            {

                if (e.Key == Key.Enter)
                {
                    if (cmb_cash.SelectedItem != null)
                    {
                        var row = (Model.AccountModel)cmb_cash.SelectedItem;

                        if (row != null)
                        {
                            var cash_acc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.ID == row.ID).FirstOrDefault();
                            if (cash_acc != null)
                            {
                                CrLimit = cash_acc.CrLimit;

                                var p = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.ID == cash_acc.ParentGroup).FirstOrDefault();
                                if (p != null)
                                {
                                    gCrLimit = p.Cr_Loc;
                                }
                                string infotxt;
                                infotxt = row.Name;
                                if (CrLimit > 0)
                                {
                                    infotxt += " Cr.Lock : " + CrLimit;
                                }
                                else if (gCrLimit > 0)
                                {
                                    infotxt += " Cr.Lock : " + gCrLimit;
                                }

                                var ob = DB.Connection.GetActBalance(row.ID, "");

                                if (ob > 0) { infotxt += ", OB: " + ob; }
                                txt_info.Text = infotxt;
                            }
                        }
                    }


                }
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }



        }

        private void txt_empid_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cmb_employee_KeyDown_1(object sender, KeyEventArgs e)
        {

        }
    }
}
