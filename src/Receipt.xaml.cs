using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using Xceed.Words.NET;
using System.Diagnostics;
using System.IO;
using System.Windows.Xps.Packaging;

namespace accounts
{
    /// <summary>
    /// Interaction logic for Receipt.xaml
    /// </summary>
    public partial class Receipt : Window, IDataErrorInfo, INotifyPropertyChanged
    {
        SqlConnection con = new SqlConnection();

        List<int> rnos = new List<int>();
        int cindex = 0;
        int crid = 0;
        Double discLimit, creditLimit, gcriditLock;
        Double dreditLimit, gdriditLock;

        private string _rcCashAc = null;
        private string _rcCrAc = null;
        private string _rcAmount = "0.00";
        private string _rcDisc = "0.00";
        private string _rcDAmount = "0.00";
        private string _rcNarration = null;

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
        public string rcCashAc
        {
            get { return _rcCashAc; }
            set { _rcCashAc = value; OnPropertyChanged("rcCashAc"); }
        }
        public string rcCrAc
        {
            get { return _rcCrAc; }
            set { _rcCrAc = value; OnPropertyChanged("rcCrAc"); }
        }
        public string rcAmount
        {
            get { return _rcAmount; }
            set { _rcAmount = value; OnPropertyChanged("rcAmount"); }
        }
        public string rcDAmount
        {
            get { return _rcDAmount; }
            set { _rcDAmount = value; OnPropertyChanged("rcDAmount"); }
        }

        public string rcDisc
        {
            get { return _rcDisc; }
            set { _rcDisc = value; OnPropertyChanged("rcDisc"); }
        }
        public string rcNarration
        {
            get { return _rcNarration; }
            set { _rcNarration = value; OnPropertyChanged("rcNarration"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Error
        {
            get { return string.Empty; }
        }


        public string this[string columnName]
        {
            get
            {
                string result = String.Empty;
                int id;

                switch (columnName)
                {
                    case "rcCrAc":
                        result = public_members.AccountValidattor(rcCrAc);
                        break;
                    case "rcCashAc":
                        result = public_members.AccountValidattor(rcCashAc);
                        break;
                    case "rcAmount":
                        result = public_members.NumberValidator(rcAmount, 0);
                        break;
                    case "rcDAmount":
                        result = public_members.NumberValidator(rcDAmount, 0);
                        break;
                    case "rcDisc":
                        result = public_members.NumberValidator(rcDisc, 0);
                        break;
                    case "rcNarration":
                        result = "A quick narrtion recommended";
                        break;
                }
                return result;
            }
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public Receipt()
        {
            InitializeComponent();







        }
        void NewButtonState()
        {

            btn_save.IsEnabled = true;
            btn_update.IsEnabled = false;
            btn_del.IsEnabled = false;
            btn_print.IsEnabled = false;
            btn_doc.IsEnabled = false;
            btn_pdf.IsEnabled = false;
            btn_xps.IsEnabled = false;
            ClearPrintCache();
            chk_isrecurr.IsChecked = false;
            txt_task_Amount.Text = "";
            txt_task_label.Text = "";
            lbl_taskflag.Visibility = Visibility.Collapsed;
            lst_tasks.Visibility = Visibility.Visible;
            hist_task_lbl.Content = "TASKS";
            var taskList = ViewModels_Variables.ModelViews.Tasks.Where((t) => t.ENTRY == "RECEIPT");
            //taskList.Add(public_members.tasks.Select("entry='JOURNAL'")[0]);
            //if (taskList.Count() > 0)
            //{
            CollectionView collectionView = new CollectionView(taskList);

            //}
            lst_tasks.DataContext = taskList;
            rnos = (from r in ViewModels_Variables.ModelViews.Receipts select r.rno).ToList<int>();
            dtp_rdate.SelectedDate = ViewModels_Variables._sysDate[0];
            txt_rno.Text = DB.Connection.NewEntryno("receipts", "r_no").ToString();
            cmb_cashaccount.Focus();
        }
        public void Find(int rno)
        {
            //open          

            if (rno > 0)
            {
                var rows = ViewModels_Variables.ModelViews.Receipts.Where((r) => r.rno == rno).FirstOrDefault();
                if (rows != null)
                {
                    var cracc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.ID == rows.CrAccount.ID).FirstOrDefault();
                    var dracc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.ID == rows.DrAccount.ID).FirstOrDefault();
                    if (cracc != null)
                    {
                        cmb_craccount.SelectedItem = cracc;
                    }
                    if (dracc != null)
                    {
                        cmb_cashaccount.SelectedItem = dracc;
                    }
                    cmb_jinv.Text = rows.Invno;
                    txt_cramount.Text = rows.DrAmount.ToString("0.00");
                    txt_narration.Text = rows.Narration;
                    dtp_rdate.SelectedDate = rows.Date;
                    txt_rno.Text = rows.rno.ToString();
                    txt_disc.Text = rows.DiscP.ToString("0.00");
                    txt_discamount.Text = rows.DAmount.ToString("0.00");
                    txt_total.Text = string.Format("{0:0.00}", rows.DrAmount - rows.DAmount);
                    lbl_taskflag.Visibility = Visibility.Collapsed;
                    if (rows.Task_Id > 0) { lbl_taskflag.Visibility = Visibility.Visible; }
                    chk_isrecurr.IsChecked = rows.isRecurr;



                    FindButtonState();
                    FindTask(eno: rno);
                }
                else
                {
                    MessageBox.Show("Not a Receipt No");
                }

            }
            else
            {
                MessageBox.Show("Enter a valid Receipt No");
                txt_rnofind.Focus();
            }
        }
        void FindButtonState()
        {
            btn_doc.IsEnabled = true;
            btn_pdf.IsEnabled = true;
            btn_xps.IsEnabled = true;
            ClearPrintCache();
            btn_save.IsEnabled = false;
            btn_update.IsEnabled = true;
            btn_del.IsEnabled = true;
            btn_print.IsEnabled = true;
            cmb_cashaccount.Focus();

        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {

            rcAmount = "0.00";
            rcDAmount = "0.00";
            rcDisc = "0.00";
            txt_cramount.Text = "";
            info.Text = "";
            txt_disc.Text = "";
            txt_discamount.Text = "";
            txt_total.Text = "0.00";
            rcCashAc = null;
            cmb_jinv.Text = "";
            rcNarration = null;

            chk_isrecurr.IsChecked = false;
            rcCrAc = null;
            cmb_cashaccount.Text = "";
            cmb_craccount.Text = "";
            txt_rnofind.Text = "";

            txt_rno.Text = DB.Connection.NewEntryno("receipts", "r_no").ToString();
            NewButtonState();

        }
        private void txt_rnofind_KeyDown(object sender, KeyEventArgs e)
        {
            int eno = 0;
            if (txt_rnofind.Text.Length > 0 && e.Key == Key.Enter)
            {
                Int32.TryParse(txt_rnofind.Text.ToString(), out eno);
                Find(eno);
            }

        }


        private void btn_save_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                //Save
                double _disc = 0, _damount = 0, _amount;
                double.TryParse(txt_cramount.Text.ToString(), out _amount);
                double.TryParse(txt_discamount.Text.ToString(), out _damount);
                bool flag = true;


                if (cmb_cashaccount.SelectedItem != null && cmb_craccount.SelectedItem != null && _amount > 0)
                {
                    var drid = ((Model.AccountModel)cmb_cashaccount.SelectedItem);
                    var crid = ((Model.AccountModel)cmb_craccount.SelectedItem);
                    flag = DB.Connection.CheckCreditLocks((_amount - _damount), creditLimit, gcriditLock, crid.ID);
                    flag = DB.Connection.CheckCreditLocks((_amount - _damount), dreditLimit, gdriditLock, drid.ID);
                    //Save
                    if (flag == false)
                    {
                        MessageBox.Show("Debit lock reached");
                    }

                    if (flag == true)

                    {
                        Model.ReceiptModel receipt = new Model.ReceiptModel();
                        receipt.Invno = cmb_jinv.Text.ToString().Trim();
                        receipt.isRecurr = (bool)chk_isrecurr.IsChecked;
                        receipt.DrAccount = drid;
                        receipt.CrAccount = crid;
                        double.TryParse(txt_cramount.Text.ToString(), out _amount);
                        receipt.DrAmount = _amount;
                        double.TryParse(txt_disc.Text.ToString(), out _disc);
                        double.TryParse(txt_discamount.Text.ToString(), out _damount);
                        receipt.DiscP = _disc;
                        receipt.DAmount = _damount;
                        receipt.Narration = txt_narration.Text.ToString().Trim();
                        receipt.Date = dtp_rdate.SelectedDate.Value;

                        var new_rno = DB.Receipt.Save(receipt, flag);
                        if (new_rno > 0)
                        {
                            MessageBox.Show("Receipt Saved");
                            btn_Reset_Click(sender, e);
                        }
                        else
                        {

                            MessageBox.Show("Something went wrong");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Enter data correctly");
                }
            }
            catch (SqlException rr)
            {
                MessageBox.Show(rr.Message.ToString());
            }

        }
        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            //Edit


            try
            {
                //Save
                double _disc = 0, _damount = 0, _amount;
                double.TryParse(txt_cramount.Text.ToString(), out _amount);
                double.TryParse(txt_discamount.Text.ToString(), out _damount);
                bool flag = true;


                if (cmb_cashaccount.SelectedItem != null && cmb_craccount.SelectedItem != null && _amount > 0)
                {
                    var drid = ((Model.AccountModel)cmb_cashaccount.SelectedItem);
                    var crid = ((Model.AccountModel)cmb_craccount.SelectedItem);
                    flag = DB.Connection.CheckCreditLocks((_amount - _damount), creditLimit, gcriditLock, crid.ID);
                    flag = DB.Connection.CheckCreditLocks((_amount - _damount), dreditLimit, gdriditLock, drid.ID);
                    //Save
                    if (flag == false)
                    {
                        MessageBox.Show("Debit lock reached");
                    }

                    if (flag == true)

                    {
                        Model.ReceiptModel receipt = new Model.ReceiptModel();
                        receipt.Invno = cmb_jinv.Text.ToString().Trim();

                        receipt.DrAccount = drid;
                        receipt.CrAccount = crid;
                        receipt.isRecurr = (bool)chk_isrecurr.IsChecked;

                        double.TryParse(txt_cramount.Text.ToString(), out _amount);
                        int.TryParse(txt_rno.Text.ToString(), out int rno);
                        receipt.rno = rno;
                        receipt.DrAmount = _amount;
                        double.TryParse(txt_disc.Text.ToString(), out _disc);
                        double.TryParse(txt_discamount.Text.ToString(), out _damount);
                        receipt.DiscP = _disc;
                        receipt.DAmount = _damount;
                        receipt.Narration = txt_narration.Text.ToString().Trim();
                        receipt.Date = dtp_rdate.SelectedDate.Value;

                        var res = DB.Receipt.Update(receipt, flag);
                        if (res == true)
                        {
                            MessageBox.Show("Receipt Updated");
                            btn_Reset_Click(sender, e);
                        }
                        else
                        {

                            MessageBox.Show("Something went wrong");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Enter data correctly");
                }
            }
            catch (SqlException rr)
            {
                MessageBox.Show(rr.Message.ToString());
            }

        }
        private void btn_del_Click(object sender, RoutedEventArgs e)
        {

            if (cmb_cashaccount.Text.Length > 0 && cmb_craccount.Text.Length > 0)
            {

                int.TryParse(txt_rno.Text.ToString(), out int rno);
                var rr = DB.Receipt.Remove(rno);
                if (rr == true)
                {
                    MessageBox.Show("Receipt Removed");
                    btn_Reset_Click(sender, e);

                }
                else
                {
                    MessageBox.Show("DataBase connection lost");
                }


            }
        }

        private void btn_cash_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmb_cashaccount.SelectedItem != null)
                {
                    var id = ((Model.AccountModel)cmb_cashaccount.SelectedItem).ID;
                    if (id > 0)
                    {
                        CashBook1 cashsow = new CashBook1(id);
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
        private void btn_movefirst_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rnos.Count > 0)
                {
                    int pno = rnos[0];
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

                if (rnos.Count > 0 && cindex >= 0 && cindex < rnos.Count)
                {
                    if (cindex != 0) { cindex--; }

                    Find(rnos[cindex]);
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


                if (rnos.Count > 0 && cindex < rnos.Count - 1)
                {

                    cindex++;

                    Find(rnos[cindex]);
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
                if (rnos.Count > 0)
                {
                    int pno = rnos[rnos.Count - 1];
                    cindex = rnos.Count - 1;
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
        private void btn_account_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmb_craccount.SelectedItem != null)
                {
                    var id = ((Model.AccountModel)cmb_craccount.SelectedItem).ID;
                    if (id > 0)
                    {
                        CashBook1 cashsow = new CashBook1(id);
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
        private void btn_find_Click(object sender, RoutedEventArgs e)
        {
            int eno = 0;
            if (txt_rnofind.Text.Length > 0)
            {
                Int32.TryParse(txt_rnofind.Text.ToString(), out eno);
                Find(eno);
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
        private void cmb_craccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Cr_Status();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.ToString());
            }
        }
        void Cr_Status()
        {
            try
            {
                var row = (Model.AccountModel)cmb_craccount.SelectedItem;
                if (row != null)
                {
                    crid = row.ID;
                    creditLimit = row.CrLimit;
                    discLimit = row.DrLimit;

                    var p = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.ID == row.ParentGroup).FirstOrDefault();

                    if (p != null)
                    {
                        gcriditLock = p.Cr_Loc;
                    }
                    string infotxt;
                    infotxt = row.Name;
                    if (creditLimit > 0)
                    {
                        infotxt += " Dr.Lock : " + creditLimit;
                    }
                    else if (gcriditLock > 0)
                    {
                        infotxt += " Dr.Lock : " + gcriditLock;
                    }

                    var ob = DB.Connection.GetActBalance(crid);

                    if (ob > 0) { infotxt += ", OB: " + ob; }
                    info.Text = infotxt;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void cmb_craccount_KeyDown(object sender, KeyEventArgs e)
        {


            try
            {
                Cr_Status();

                if (cmb_craccount.SelectedItem != null)
                {
                    if (cmb_craccount.Text.Trim().ToString().Length > 0)
                    {
                        if (e.Key == Key.Enter)
                        {
                            var jlist = (from j in ViewModels_Variables.ModelViews.Journals.AsEnumerable() where j.DrAccount.ID == crid && DB.Connection.GetActBalance(crid, j.Invoice) > 0 select j.Invoice).ToList();
                            cmb_jinv.ItemsSource = jlist;

                        }





                        var list = (from r in ViewModels_Variables.ModelViews.Receipts.AsEnumerable()
                                    where (r.rno != int.Parse(txt_rno.Text.ToString()) && r.CrAccount.ID == crid)
                                    select new { eno = r.rno, edate = r.Date, amount = r.DrAmount });

                        tr_history_list.ItemsSource = list;
                        lst_tasks.Visibility = Visibility.Collapsed;
                        hist_task_lbl.Content = "RECEIPT HISTORY";




                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            public_members._TabPress(e);
        }

        private void dtp_rdate_KeyUp(object sender, KeyEventArgs e)
        {
            public_members._TabPress(e);
        }
        void Cash_Blance_Cr_Status()
        {
            int drid;
            try
            {
                if (cmb_cashaccount.SelectedItem != null)
                {


                    var row = (Model.AccountModel)cmb_cashaccount.SelectedItem;
                    if (row != null)
                    {
                        drid = row.ID;
                        dreditLimit = row.DrLimit;
                        discLimit = row.MaxDisc;
                        var p = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.ID == row.ParentGroup).FirstOrDefault();
                        if (p != null)
                        {
                            gdriditLock = p.Dr_Loc;
                        }
                        string infotxt;
                        infotxt = row.Name;
                        if (dreditLimit > 0)
                        {
                            infotxt += " Dr.Lock : " + dreditLimit;
                        }
                        else if (gcriditLock > 0)
                        {
                            infotxt += " Dr.Lock : " + gdriditLock;
                        }

                        var ob = DB.Connection.GetActBalance(drid);

                        if (ob > 0) { infotxt += ", OB: " + ob; }
                        info.Text = infotxt;
                    }

                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        private void cmb_cashaccount_KeyDown(object sender, KeyEventArgs e)
        {
            int drid;
            try
            {

                if (e.Key == Key.Enter)
                {
                    Cash_Blance_Cr_Status();

                    public_members._TabPress(e);
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        private void txt_narration_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TabToSave();
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
                btn_update.Focus();
            }
        }

        private void txt_cramount_Copy_KeyDown(object sender, KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void txt_cramount_Copy1_KeyDown(object sender, KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void txt_narration_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txt_cramount_KeyDown(object sender, KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void txt_disc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    double am = 0, disc = 0, per = 0;
                    double.TryParse(txt_cramount.Text.ToString(), out am);
                    double.TryParse(txt_disc.Text.ToString(), out per);
                    if (discLimit > 0 && per > discLimit)
                    {
                        per = discLimit;

                    }

                    if (per > 0 && am > 0)
                    {
                        disc = am * per / 100;
                    }
                    txt_disc.Text = per.ToString();
                    txt_discamount.Text = disc.ToString();

                }

                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txt_discamount_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                double am = 0, disc = 0, per = 0;
                double.TryParse(txt_cramount.Text.ToString(), out am);
                double.TryParse(txt_discamount.Text.ToString(), out disc);

                if (am > 0 && disc > 0)
                {
                    per = disc / am * 100;
                }
                if (discLimit > 0 && per > discLimit) { per = 0; disc = 0.00; }
                txt_discamount.Text = disc.ToString();
                txt_disc.Text = per.ToString();
                txt_total.Text = string.Format("{0:0.00}", am - disc);

            }

            public_members._TabPress(e);
        }

        private void txt_disc_TextChanged()
        {

        }

        private void txt_disc_KeyDown_1(object sender, KeyEventArgs e)
        {

        }

        private void cmb_jinv_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && cmb_jinv.Text != null && cmb_jinv.Text.Length > 0 && crid > 0 && cmb_craccount.SelectedItem != null)
                {
                    info.Text = "Inv. Balance for " + cmb_jinv.Text.ToString() + " : " + DB.Connection.GetActBalance(crid, cmb_jinv.Text.ToString());
                    var jinvoice = ViewModels_Variables.ModelViews.Journals.Where((j) => j.Invoice == cmb_jinv.Text.ToString() && (j.DrAccount.ID == ((Model.AccountModel)cmb_craccount.SelectedItem).ID)).FirstOrDefault();
                    if (jinvoice != null && dtp_rdate.SelectedDate < jinvoice.Date)
                    {
                        MessageBox.Show("The invoice is generated on " + jinvoice.Date.ToShortDateString());
                        if (public_members._sysDate[0] >= jinvoice.Date)
                        {
                            dtp_rdate.SelectedDate = public_members._sysDate[0];
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid date");
                        }
                    }
                }
                public_members._TabPress(e);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }

        private void btn_print_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (wordFname == null)
                {
                    wordFname = CreateDocument();
                }
                if (wordFname != null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }
                if (xpsFname != null)
                {
                    XpsDocument xps = new XpsDocument(xpsFname, FileAccess.Read);
                    accounts.PrintDialogue print = new PrintDialogue(xps, this.Title + " " + txt_rno.Text.ToString());
                    print.Show();
                    xps.Close();
                }

            }
            catch (Exception w1)
            {

                MessageBox.Show(w1.Message.ToString());
            }
        }

        string CreateDocument()
        {
            string fileName = null;
            try
            {
                fileName = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\doc", ".docx");
                using (var document = DocX.Create(fileName))
                {

                    // Add a title

                    document.ApplyTemplate(@"DocTemplates\Receipt.dotx");

                    //Footer
                    document.ReplaceText("[MyCompany]", ViewModels_Variables.ModelViews.CompanyProfile[0].company);
                    document.ReplaceText("[Clandmark]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].lmark);
                    document.ReplaceText("[CCity]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].place);
                    document.ReplaceText("[CPhone]", "Phone :" + ViewModels_Variables.ModelViews.CompanyProfile[0].officeno);

                    //customer Information
                    var cust = (Model.AccountModel)cmb_craccount.SelectedItem;
                    if (cust != null)
                    {
                        document.ReplaceText("[Customer]", cust.Name);
                        document.ReplaceText("[Company]", "");
                        document.ReplaceText("[Address]", cust.Address);
                        document.ReplaceText("[City]", cust.City);
                        document.ReplaceText("[Phone]", cust.Mob);
                        document.ReplaceText("[CustomerID]", cust.ID.ToString());
                    }
                    //Receipt Info
                    document.ReplaceText("[VNo]", txt_rno.Text.ToString());
                    document.ReplaceText("[Date]", dtp_rdate.SelectedDate.Value.ToShortDateString());
                    string jinv = null;
                    if (cmb_jinv.Text != null && cmb_jinv.Text.Length > 0) { jinv = cmb_jinv.Text.ToString(); } else { jinv = ""; }
                    document.ReplaceText("[InvNo]", jinv);
                    document.ReplaceText("[Discount]", txt_discamount.Text.ToString());
                    document.ReplaceText("[Amount]", txt_cramount.Text.ToString());
                    document.ReplaceText("[Total]", txt_total.Text.ToString());
                    string narration = null;
                    if (txt_narration.Text != null && txt_narration.Text.Length > 0) { narration = txt_narration.Text.ToString(); } else { narration = ""; }
                    document.ReplaceText("[Narration]", "Note:" + narration);
                    document.Save();
                }
            }
            catch (Exception er)
            {

                throw;
            }
            return fileName;
        }

        private void btn_doc_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (wordFname == null)
                {
                    wordFname = CreateDocument();
                }


                if (wordFname != null) Process.Start("winword.exe", wordFname);



            }
            catch (Exception e1)

            {
                MessageBox.Show(e1.Message.ToString());

            }
            finally
            {

            }
        }

        private void btn_excel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_pdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wordFname == null)
                {
                    wordFname = CreateDocument();
                }
                if (wordFname != null)
                {
                    pdfFname = ReportExport.WordExport.CreatePDF(wordFname);
                }
                if (pdfFname != null) Process.Start("AcroRd32.exe", pdfFname);
             }
            catch (Exception e1)

            {
                MessageBox.Show(e1.Message.ToString());

            }
            finally
            {

            }
        }

        private void btn_xps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wordFname == null)
                {
                    wordFname = CreateDocument();
                }
                if (wordFname != null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }
                if (xpsFname != null)
                {
                    Process.Start("explorer.exe", xpsFname);

                }

            }
            catch (Exception e1)

            {
                MessageBox.Show(e1.Message.ToString());
            }
            finally
            {
            }
        }

        private void lst_tasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (ViewModels_Variables.ModelViews.Accounts.Count() <= 0)
                //{
                //    DB.Accounts.Fetch();
                //    ViewModels_Variables.ModelViews.AccountToCollection();
                //}

                if (ViewModels_Variables.ModelViews.Receipts.Count() <= 0)
                {
                    DB.Receipt.Fetch();
                    ViewModels_Variables.ModelViews.Reciepts_To_List();
                }
                //if (ViewModels_Variables.ModelViews.Journals.Count() <= 0)
                //{
                //    DB.Journal.Fetch();
                //    ViewModels_Variables.ModelViews.Journals_To_list();
                //}
                NewButtonState();

                var view = ViewModels_Variables.ModelViews;
                DataContext = view;
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
                    var cr = (Model.AccountModel)cmb_craccount.SelectedItem;
                    if (cr != null)
                    {
                        txt_task_label.Text = cr.Name;
                        txt_task_Amount.Text = txt_cramount.Text.ToString();
                    }
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        void FindTask(int eno)
        {
            try
            {
                var t1 = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ENO == eno && ts.ENTRY == "RECEIPT").FirstOrDefault();
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
        private void btn_task_add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btn_save.IsEnabled == false && chk_isrecurr.IsChecked == true)
                {
                    Model.Task task = new Model.Task();
                    int.TryParse(txt_rno.Text.ToString(), out int t);
                    decimal.TryParse(txt_task_Amount.Text.ToString(), out decimal a);
                    task.T_LABEL = txt_task_label.Text.ToString();
                    task.T_AMOUNT = a;
                    task.ENO = t;
                    task.ENTRY = "RECEIPT";
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

        private void btn_task_del_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btn_save.IsEnabled == false)
                {
                    int.TryParse(txt_rno.Text.ToString(), out int t);
                    var t1 = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ENO == t && ts.ENTRY == "RECEIPT").FirstOrDefault();
                    if (t1 != null)
                    {
                        var tt = DB.Task.DeleteTask(eno: t1.ENO, entry: "RECEIPT");
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

        private void cmb_cashaccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Cash_Blance_Cr_Status();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void cmb_cashaccount_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                cmb_cashaccount.IsDropDownOpen = true;
            }
            catch (Exception er)
            {

                throw;
            }
        }

        private void cmb_craccount_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                cmb_craccount.IsDropDownOpen = true;
            }
            catch (Exception er)
            {

                throw;
            }
        }

        private void cmb_jinv_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                cmb_jinv.IsDropDownOpen = true;
            }
            catch (Exception er)
            {

                throw;
            }
        }

        private void lst_tasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lst_tasks.SelectedItem != null)
                {
                    var t = (Model.Task)lst_tasks.SelectedItem;
                    MessageBoxResult res1 = new MessageBoxResult();
                    res1 = MessageBox.Show("Do you want to execute this task", "Task", MessageBoxButton.YesNo);
                    if (res1 == MessageBoxResult.Yes)
                    {
                        if (t != null)
                        {
                            var res = DB.Receipt.Tasker(t, dtp_rdate.SelectedDate.Value);
                            if (res == true)
                            {
                                MessageBox.Show("Task completed");
                                btn_Reset_Click(sender, e);
                            }
                        }
                    }
                }
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
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

        private void btn_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReceiptReport r = new ReceiptReport(); r.Owner = this; r.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
