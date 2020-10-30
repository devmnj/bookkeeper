using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;
using Xceed.Document;
using Xceed.Words.NET;

namespace accounts
{
    /// <summary>
    /// Interaction logic for journalposting.xaml
    /// </summary>
    public partial class journalposting : Window, IDataErrorInfo, INotifyPropertyChanged
    {
        double crLimit1, CrLimit2, DrLimit1, DrLimit2;
        string wordFname = null;
        string excelFname = null;
        string xpsFname = null;
        string pdfFname = null;
        void ClearPrintCache()
        {
              wordFname = null;
              excelFname = null;
              xpsFname = null;
              pdfFname = null;
        }


        List<int> jnos = new List<int>();
        int cindex = 0;

        public event PropertyChangedEventHandler PropertyChanged;
        private string _jpCrac = null;
        private string _jpDrac = null;
        private string _jpDrAm = "0.00";
        private string _jpCrAm = "0.00";
        private string _jpNarration = null;


        public string jpCrAc
        {
            get { return _jpCrac; }
            set { _jpCrac = value; OnPropertyChanged("jpCrAc"); }
        }
        public string jpDrAc
        {
            get { return _jpDrac; }
            set { _jpDrac = value; OnPropertyChanged("jpDrAc"); }
        }
        public string jpDrAm
        {
            get { return _jpDrAm; }
            set { _jpDrAm = value; OnPropertyChanged("jpDrAm"); }
        }
        public string jpCrAm
        {
            get { return _jpCrAm; }
            set { _jpCrAm = value; OnPropertyChanged("jpCrAm"); }
        }
        public string jpNarration
        {
            get { return _jpNarration; }
            set { _jpNarration = value; OnPropertyChanged("jpNarration"); }
        }



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
                    case "jpCrAc":
                        result = public_members.AccountValidattor(jpCrAc);
                        break;
                    case "jpDrAc":
                        result = public_members.AccountValidattor(jpDrAc);
                        break;
                    case "jpCrAm":
                        result = public_members.NumberValidator(jpCrAm, 0);
                        break;
                    case "jpDrAm":
                        result = public_members.NumberValidator(jpDrAm, 0);
                        break;
                    case "jpNarration":
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
        public journalposting()
        {
            InitializeComponent();


        }
        //void DatePicker1_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    e.Handled = false;
        //}
        void NewButtonState()
        {
            ClearPrintCache();
            btn_doc.IsEnabled = false;
            btn_pdf.IsEnabled = false;
            btn_xps.IsEnabled = false;

            btn_print.IsEnabled = false;
            txt_jno.Text = DB.Connection.NewEntryno("journal_entry", "j_no").ToString();
            jnos = (from p in ViewModels_Variables.ModelViews.Journals select p.jno).ToList();
            //taskgroup.IsEnabled = false;
            txt_crname.Text = "";
            jpCrAc = null;
            jpCrAm = "0.00";
            jpDrAm = "0.00";
            jpDrAc = null;
            jpNarration = null;
            btn_task_add.IsEnabled = false;
            btn_task_del.IsEnabled = false;
            taskgroup.IsEnabled = false;
            txt_task_Amount.Text = "";
            txt_task_label.Text = "";
            chk_isrecurr.IsChecked = false;
            txt_task_Amount.Text = "";
            txt_task_label.Text = "";
            //chk_active.IsChecked = false;
            txt_cramount.Text = "0.00";
            txt_dramount.Text = "0.00";
            txt_narration.Text = "";
            txt_jnofind.Text = "";
            tr_history_list.ItemsSource = null;
            txt_jinvno.Text = "";

            CMB_CRACC.Text = ""; cmb_dr_short_name.Text = "";

            btn_save.IsEnabled = true;
            btn_update.IsEnabled = false;
            btn_del.IsEnabled = false;


            lbl_taskflag.Visibility = Visibility.Collapsed;
            lst_tasks.Visibility = Visibility.Visible;
            var taskList = ViewModels_Variables.ModelViews.Tasks.Where((t) => t.ENTRY == "JOURNAL");
            //taskList.Add(public_members.tasks.Select("entry='JOURNAL'")[0]);
            CollectionView collectionView = new CollectionView(taskList);
            lst_tasks.DataContext = collectionView;
            hist_task_lbl.Content = "TASKS";
            dtp_jdate.SelectedDate = ViewModels_Variables._sysDate[0];

            CMB_CRACC.Focus();
        }
        void FindButtonState()
        {
            ClearPrintCache();
            btn_doc.IsEnabled = true;
            btn_pdf.IsEnabled = true;
            btn_xps.IsEnabled = true;

            //btn_task_add.IsEnabled = true;

            btn_save.IsEnabled = false;
            btn_update.IsEnabled = true;
            btn_del.IsEnabled = true;
            btn_print.IsEnabled = true;
            dtp_jdate.Focus();
            CMB_CRACC.Focus();


        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewButtonState();
                Random rnd = new Random(5);
                txt_jinvno.Text = "#" + rnd.Next().ToString();
            }
            catch (Exception)
            {

                throw;
            }
       
        }


        private void btn_save_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                double _amount, _damount;
                double.TryParse(txt_cramount.Text.ToString(), out _amount);
                double.TryParse(txt_dramount.Text.ToString(), out _damount);
                bool flag = true;





                if ((_amount) > crLimit1 && crLimit1 != 0)
                {
                    flag = false; MessageBox.Show("Credit lock reached");
                }



                if ((_damount) > DrLimit1 && DrLimit1 != 0)
                {
                    flag = false; MessageBox.Show("Debit lock reached");
                }



                if ((_amount > 0 || _damount > 0) && flag == true && CMB_CRACC.SelectedItem != null && CMB_CRACC.SelectedItem != null)
                {

                    Model.JournalModel journal = new Model.JournalModel();


                    journal.Isrecurring = (bool)chk_isrecurr.IsChecked;
                    //journal.IsActive = (bool)chk_active.IsChecked;
                    //journal.IsDaily = (bool)rbtn_weekly1.IsChecked;
                    //journal.IsWeekly = (bool)rbtn_weekly1.IsChecked;
                    //journal.IsMonthly = (bool)rbtn_monthly1.IsChecked;

                    journal.Invoice = txt_jinvno.Text.ToString().ToUpper();
                    journal.CrAccount = ((Model.AccountModel)CMB_CRACC.SelectedItem);
                    journal.DrAccount = ((Model.AccountModel)cmb_dr_short_name.SelectedItem);


                    flag = DB.Connection.CheckCreditLocks((_amount), DrLimit1, DrLimit2, journal.CrAccount.ID);
                    flag = DB.Connection.CheckCreditLocks((_damount), crLimit1, CrLimit2, journal.DrAccount.ID);

                    //Save
                    if (flag == false)
                    {
                        MessageBox.Show("Debit lock reached");

                    }

                    journal.Narration = txt_narration.Text.ToString();
                    journal.Date = dtp_jdate.SelectedDate.Value;
                    double cram, dram;
                    double.TryParse(txt_cramount.Text.ToString(), out cram);
                    double.TryParse(txt_dramount.Text.ToString(), out dram);
                    journal.Cr_Amount = cram;
                    journal.Dr_Amount = dram;

                    var jno = DB.Journal.Save(journal, flag);
                    if (jno > 0)
                    {


                        MessageBox.Show("Journal Posted successfully");
                        btn_Reset_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong");
                    }
                }


                else
                {
                    MessageBox.Show("Enter data correctly");

                }
            }
            catch (SqlException e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }
        }


        public void Find(int jno)
        {
            NewButtonState();

            if (jno > 0)
            {

                var rows = ViewModels_Variables.ModelViews.Journals.Where((j) => j.jno == jno).FirstOrDefault();
                if (rows != null)
                {
                    FindButtonState();
                    txt_jno.Text = rows.jno.ToString();
                    dtp_jdate.SelectedDate = rows.Date;
                    txt_jinvno.Text = rows.Invoice;

                    var cracc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.ID == rows.CrAccount.ID).FirstOrDefault();
                    var dracc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.ID == rows.DrAccount.ID).FirstOrDefault();

                    CMB_CRACC.SelectedItem = cracc;
                    cmb_dr_short_name.SelectedItem = dracc;
                    txt_cramount.Text = rows.Cr_Amount.ToString("0.00");
                    txt_dramount.Text = rows.Dr_Amount.ToString("0.00");
                    txt_narration.Text = rows.Narration;

                    chk_isrecurr.IsChecked = rows.Isrecurring;


                    lbl_taskflag.Visibility = Visibility.Collapsed;
                    if (rows.Task_Id > 0) { lbl_taskflag.Visibility = Visibility.Visible; }



                    FindTask(eno: jno);
                }
                else
                {
                    MessageBox.Show("Not a Journal No");
                }

            }

        }
        private void txt_jnofind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txt_jnofind.Text.Length > 0)
            {
                int en = Convert.ToInt32(txt_jnofind.Text.ToString());
                this.Find(en);
            }
        }
        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                double _amount, _damount;
                double.TryParse(txt_cramount.Text.ToString(), out _amount);
                double.TryParse(txt_dramount.Text.ToString(), out _damount);
                bool flag = true;



                if ((_amount) > crLimit1 && crLimit1 != 0)
                {
                    flag = false; MessageBox.Show("Credit lock reached");
                }



                if ((_damount) > DrLimit1 && DrLimit1 != 0)
                {
                    flag = false; MessageBox.Show("Debit lock reached");
                }



                if ((_amount > 0 || _damount > 0) && flag == true && CMB_CRACC.SelectedItem != null && CMB_CRACC.SelectedItem != null)
                {

                    Model.JournalModel journal = new Model.JournalModel();


                    journal.Isrecurring = (bool)chk_isrecurr.IsChecked;
                    //journal.IsActive = (bool)chk_active.IsChecked;
                    //journal.IsDaily = (bool)rbtn_weekly1.IsChecked;
                    //journal.IsWeekly = (bool)rbtn_weekly1.IsChecked;
                    //journal.IsMonthly = (bool)rbtn_monthly1.IsChecked;

                    journal.Invoice = txt_jinvno.Text.ToString().ToUpper();
                    journal.CrAccount = ((Model.AccountModel)CMB_CRACC.SelectedItem);
                    journal.DrAccount = ((Model.AccountModel)cmb_dr_short_name.SelectedItem);
                    int.TryParse(txt_jno.Text.ToString(), out int jno);
                    journal.jno = jno;

                    flag = DB.Connection.CheckCreditLocks((_amount), DrLimit1, DrLimit2, journal.CrAccount.ID);
                    flag = DB.Connection.CheckCreditLocks((_damount), crLimit1, CrLimit2, journal.DrAccount.ID);

                    //Save
                    if (flag == false)
                    {
                        MessageBox.Show("Debit lock reached");
                    }

                    journal.Narration = txt_narration.Text.ToString();
                    journal.Date = dtp_jdate.SelectedDate.Value;
                    double cram, dram;
                    double.TryParse(txt_cramount.Text.ToString(), out cram);
                    double.TryParse(txt_dramount.Text.ToString(), out dram);
                    journal.Cr_Amount = cram;
                    journal.Dr_Amount = dram;

                    var isj = DB.Journal.Update(journal, flag);
                    if (isj == true)
                    {
                        MessageBox.Show("Journal Updated successfully");
                        btn_Reset_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong");
                    }
                }


                else
                {
                    MessageBox.Show("Enter data correctly");

                }
            }
            catch (SqlException e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }
        }
        private void btn_del_Click(object sender, RoutedEventArgs e)
        {

            if (cmb_dr_short_name.Text.Length > 0 && CMB_CRACC.Text.Length > 0)
            {

                int.TryParse(txt_jno.Text.ToString(), out int jno);
                var rr = DB.Journal.Remove(jno);
                if (rr == true)
                {
                    MessageBox.Show("Journal Removed");
                    btn_Reset_Click(sender, e);

                }
                else
                {
                    MessageBox.Show("DataBase connection lost");
                }


            }
        }
        void CR_Status()
        {
            int crid;
            try
            {
                if (CMB_CRACC.SelectedItem != null)
                {

                    var selected = (Model.AccountModel)CMB_CRACC.SelectedItem;



                    if (selected != null)
                    {
                        txt_crname.Text = selected.Name;


                        crLimit1 = selected.CrLimit;
                        crid = selected.ID;


                        var p = ViewModels_Variables.ModelViews.AccountGroups.Where((p1) => p1.ID == selected.ParentGroup).FirstOrDefault();
                        if (p != null)
                        {
                            CrLimit2 = p.Cr_Loc;
                        }

                        string infotxt;
                        infotxt = selected.Name;
                        if (crLimit1 > 0)
                        {
                            infotxt += " Cr.Lock : " + crLimit1;
                        }
                        else if (CrLimit2 > 0)
                        {
                            infotxt += " Cr.Lock : " + CrLimit2;
                        }

                        var ob = DB.Connection.GetActBalance(crid );

                        if (ob > 0) { infotxt += ", OB: " + ob; }
                        txt_crname.Text = infotxt;


                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void CMB_CRACC_KeyDown(object sender, KeyEventArgs e)
        {
            int crid;
            try
            {
                if (e.Key == Key.Enter) { CR_Status(); }
               
            }
            catch (Exception r)
            {

            }



            try
            {
                if (CMB_CRACC.SelectedItem != null)
                {
                    if (CMB_CRACC.Text.Trim().ToString().Length > 0)
                    {

                        var row = (Model.AccountModel)CMB_CRACC.SelectedItem;
                        if (row != null)
                        {
                            crid = row.ID;
                            var list = (from r in ViewModels_Variables.ModelViews.Journals
                                        where (r.jno != int.Parse(txt_jno.Text.ToString()) && r.CrAccount.ID == crid)
                                        select new { eno = r.jno, edate = r.Date, amount = r.Cr_Amount });

                            tr_history_list.ItemsSource = list;
                            lst_tasks.Visibility = Visibility.Collapsed;
                            hist_task_lbl.Content = " JOUNRNAL HISTORY";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            public_members._TabPress(e);
        }
        private void CMB_CRACC_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (CMB_CRACC.Text != null)
                {
                    var parent = (Model.AccountModel)CMB_CRACC.SelectedItem;


                    if (parent != null)
                    {
                        txt_crname.Text = parent.Name;
                    }
                }
            }
            catch (Exception r)
            {

            }
        }
        private void cmb_dr_short_name_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (cmb_dr_short_name.Text != null)
                {
                    var parent = (Model.AccountModel)cmb_dr_short_name.SelectedItem;


                    if (parent != null)
                    {
                        txt_crname.Text = parent.Name;
                    }
                }
                if (e.Key == Key.Enter)
                {
                    CheckJInvoice();
                }
            }
            catch (Exception r)
            {

            }
        }
        private void btn_movelast_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (jnos.Count > 0)
                {
                    int pno = jnos[jnos.Count - 1];
                    cindex = jnos.Count - 1;
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
        private void btn_movenext_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (jnos.Count > 0 && cindex < jnos.Count - 1)
                {

                    cindex++;

                    Find(jnos[cindex]);
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

                if (jnos.Count > 0 && cindex >= 0 && cindex < jnos.Count)
                {
                    if (cindex != 0) { cindex--; }

                    Find(jnos[cindex]);
                }


                else
                {
                    MessageBox.Show("No entey found");
                }
            }
            catch (Exception rr)
            {

            }
        }
        private void btn_movefirst_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (jnos.Count > 0 && cindex >= 0 && cindex < jnos.Count)
                {
                    cindex = 1;
                    if (cindex != 0) { cindex--; }

                    Find(jnos[cindex]);
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
            if (txt_jnofind.Text.Length > 0)
            {
                int en = Convert.ToInt32(txt_jnofind.Text.ToString());
                this.Find(en);
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

        private void dtp_jdate_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void dtp_jdate_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void CMB_CRACC_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void CMB_CRACC_LostFocus(object sender, RoutedEventArgs e)
        {

        }
        void Dr_status()
        {
            int crid;
            try
            {
                if (cmb_dr_short_name.SelectedItem != null)
                {

                    var row = (Model.AccountModel)cmb_dr_short_name.SelectedItem;


                    if (row != null)
                    {

                        crid = row.ID;
                        var p = ViewModels_Variables.ModelViews.AccountGroups.Where((pp) => pp.ID == row.ParentGroup).FirstOrDefault();
                        if (p != null)
                        {
                            DrLimit2 = p.Dr_Loc;
                        }




                        DrLimit1 = row.DrLimit;
                        crid = row.ID;

                        string infotxt;
                        infotxt = row.Name;
                        if (DrLimit1 > 0)
                        {
                            infotxt += " Dr.Lock : " + DrLimit1;
                        }
                        else if (DrLimit2 > 0)
                        {
                            infotxt += " Dr.Lock : " + DrLimit2;
                        }

                        var ob = DB.Connection.GetActBalance(crid);

                        if (ob > 0) { infotxt += ", OB: " + ob; }
                        txt_crname.Text = infotxt;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void cmb_dr_short_name_KeyDown(object sender, KeyEventArgs e)
        {
            int crid;
            try
            {
                if (e.Key == Key.Enter) { Dr_status(); }

                if (cmb_dr_short_name.SelectedItem != null)
                {

                    var row = (Model.AccountModel)cmb_dr_short_name.SelectedItem;


                    if (row != null)
                    {

                        crid = row.ID;





                        var list = (from r in ViewModels_Variables.ModelViews.Journals.AsEnumerable()
                                    where (r.jno != int.Parse(txt_jno.Text.ToString()) && r.DrAccount.ID == crid)
                                    select new { eno = r.jno, edate = r.Date, amount = r.Dr_Amount });

                        tr_history_list.ItemsSource = list;
                        lst_tasks.Visibility = Visibility.Collapsed;
                        hist_task_lbl.Content = "JOUNRNAL HISTORY";

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            public_members._TabPress(e);
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

        private void txt_narration_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TabToSave();
            }
        }

        private void txt_cramount_KeyDown(object sender, KeyEventArgs e)

        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    double am;
                    double.TryParse(txt_cramount.Text.ToString(), out am);
                    if (crLimit1 != 0 && crLimit1 < am)
                    {
                        MessageBox.Show("Cr Limit Reached ! [" + crLimit1 + "]");
                    }
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }
            public_members._TabPress(e);
        }

        private void txt_dramount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                double am;
                double.TryParse(txt_dramount.Text.ToString(), out am);
                if (DrLimit1 != 0 && DrLimit1 < am)
                {
                    MessageBox.Show("Dr Limit Reached ! [" + DrLimit1 + "]");
                }
            }
            public_members._TabPress(e);
        }

        private void dtp_jdate_KeyUp(object sender, KeyEventArgs e)
        {
            public_members._TabPress(e);
        }
        string  CreateWordDoc( )
        {
            string fileName = null;
            try
            {
                   fileName = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\doc", ".doc");
                public_members.AutoCreateDirectory(@"DocTemplates\");
                using (var document = DocX.Create(fileName))
                {
                    // Add a title
                    document.ApplyTemplate(@"DocTemplates\JournalPosting.dotx");

                    //Footer
                    document.ReplaceText("[MyCompany]", ViewModels_Variables.ModelViews.CompanyProfile[0].company);
                    document.ReplaceText("[Clandmark]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].lmark);
                    document.ReplaceText("[CCity]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].place);
                    document.ReplaceText("[CPhone]", "Phone :" + ViewModels_Variables.ModelViews.CompanyProfile[0].officeno);

                    //customer Information
                     var parties =(Model.AccountModel) cmb_dr_short_name.SelectedItem;
                    if (parties!=null)
                    {
                        document.ReplaceText("[DrName]", parties.Name);
                       document.ReplaceText("[DrGroup]", parties.Parent.Name);
                        document.ReplaceText("[DrAddress]", parties.Address);
                        document.ReplaceText("[DrMob]", "Phone :" + parties.Mob);
                    }

                    parties = (Model.AccountModel)CMB_CRACC.SelectedItem;
                    if (parties!=null)
                    {
                        document.ReplaceText("[CrName]", parties.Name);
                          document.ReplaceText("[CrGroup]", parties.Parent.Name);
                        document.ReplaceText("[CrAddress]", parties.Address);
                        document.ReplaceText("[CrMob]", "Phone :" + parties.PhoneNo);
                    }
                    //Employee Info

                    document.ReplaceText("[VNo]", txt_jno.Text.ToString());
                    document.ReplaceText("[Date]", dtp_jdate.SelectedDate.Value.ToShortDateString());
                    string jinv = null;
                    if (txt_jinvno.Text != null && txt_jinvno.Text.Length > 0)
                    {
                        jinv = txt_jinvno.Text.ToString();
                    }
                    else { jinv = " "; }
                    document.ReplaceText("[InvNo]", jinv);
                    document.ReplaceText("[DrAccount]", cmb_dr_short_name.Text.ToString());
                    document.ReplaceText("[CrAccount]", CMB_CRACC.Text.ToString());
                    document.ReplaceText("[DrAmount]", txt_dramount.Text.ToString());
                    document.ReplaceText("[CrAmount]", txt_cramount.Text.ToString());
                    document.ReplaceText("[DrTotal]", txt_dramount.Text.ToString());
                    document.ReplaceText("[CrTotal]", txt_cramount.Text.ToString());
                    document.ReplaceText("[Narration]", txt_narration.Text.ToString());

                    document.Save();
                    document.Dispose();
                }
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
            return fileName;
        }
        private void lst_tasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lst_tasks.SelectedItem != null)
                {
                    var t = (Model.Task)lst_tasks.SelectedItem;
                    MessageBoxResult res = new MessageBoxResult();
                    res = MessageBox.Show("Do you want to execute this task", "Task", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        if (t != null)
                        {
                            var r = DB.Journal.Tasker(t.ID, dtp_jdate.SelectedDate.Value);
                            if (r == true)
                            {
                                MessageBox.Show("Task completed");
                                btn_Reset_Click(sender, e);
                            }
                            else
                            {
                                MessageBox.Show("Something went wrong");
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

        private void txt_crname_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txt_cramount_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                double t = 0;
                double.TryParse(txt_dramount.Text.ToString(), out t);
                if (t == 0)
                {
                    txt_dramount.Text = txt_cramount.Text;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        void TextSelect(TextBox t)
        {
            var l = t.Text.Length;
            t.Select(0, l);
        }
        private void txt_cramount_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txt_jinvno_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter) { CheckJInvoice(); }
                public_members._TabPress(e);
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
                    wordFname= CreateWordDoc();
                }

                if (wordFname != null)
                {
                   xpsFname= ReportExport.WordExport.CreateXPS(wordFname);
                }


                if (xpsFname != null)
                {

                    XpsDocument xps = new XpsDocument(xpsFname, FileAccess.Read);
                    accounts.PrintDialogue print = new PrintDialogue(xps, this.Title);
                    print.Show();
                    xps.Close();
                }

            }
            catch (Exception w1)
            {

                MessageBox.Show(w1.Message.ToString());
            }
        }

        private void btn_doc_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (wordFname == null)
                {
                  wordFname=  CreateWordDoc();
                }
             if(wordFname!=null)   Process.Start("winword.exe", wordFname);
            }
            catch (Exception e1)

            {
                MessageBox.Show(e1.Message.ToString());

            }
            finally
            {

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
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }


                if (xpsFname != null)
                {
                    pdfFname = ReportExport.WordExport.CreatePDF(wordFname);
                }
                if(pdfFname!=null)Process.Start("AcroRd32.exe", pdfFname);
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
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


                if (xpsFname != null)
                {
                    Process.Start("explorer.exe",xpsFname);
                }
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {



                if (ViewModels_Variables.ModelViews.Journals.Count() <= 0)
                {
                    DB.Journal.Fetch();
                    ViewModels_Variables.ModelViews.Journals_To_list();
                }

                var vm = ViewModels_Variables.ModelViews;

                DataContext = vm;
                dtp_jdate.SelectedDate = ViewModels_Variables._sysDate[0];
                dtp_jdate.Focus();
                NewButtonState();
                Random rnd = new Random(5);
                txt_jinvno.Text = "#" + rnd.Next().ToString();
               
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }

        }

        private void lst_tasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        void FindTask(int eno)
        {
            try
            {
                var t1 = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ENO == eno && ts.ENTRY == "JOURNAL").FirstOrDefault();
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
                    int.TryParse(txt_jno.Text.ToString(), out int t);
                    decimal.TryParse(txt_task_Amount.Text.ToString(), out decimal a);
                    task.T_LABEL = txt_task_label.Text.ToString();
                    task.T_AMOUNT = a;
                    task.ENO = t;
                    task.ENTRY = "JOURNAL";
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
                    int.TryParse(txt_jno.Text.ToString(), out int t);
                    var t1 = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ENO == t && ts.ENTRY == "JOURNAL").FirstOrDefault();
                    if (t1 != null)
                    {
                        var tt = DB.Task.DeleteTask(eno: t1.ENO, entry: "JOURNAL");
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

        private void chk_isrecurr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chk_isrecurr.IsChecked == true)
                {
                    var cr = (Model.AccountModel)CMB_CRACC.SelectedItem;
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
        void CheckJInvoice()
        {
            try
            {
                if (btn_save.IsEnabled == true && (Model.AccountModel)cmb_dr_short_name.SelectedItem != null && txt_jno.Text != null && txt_jinvno.Text.Length > 0)
                {
                    var jinvoice = ViewModels_Variables.ModelViews.Journals.Where((j) => j.Invoice == txt_jinvno.Text.ToUpper().ToString() && j.DrAccount.ID == ((Model.AccountModel)cmb_dr_short_name.SelectedItem).ID).FirstOrDefault();
                    if (jinvoice != null)
                    {
                        MessageBox.Show("Invoice already in Use, replaced with system generated #");
                        Random rnd = new Random(100);
                        txt_jinvno.Text = "#" + rnd.Next().ToString();
                        txt_jinvno.Focus();
                    }
                }

            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        private void txt_jinvno_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                CheckJInvoice();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmb_dr_short_name_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Dr_status();
                CheckJInvoice();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void CMB_CRACC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CR_Status();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txt_jinvno_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void CMB_CRACC_GotFocus_1(object sender, RoutedEventArgs e)
        {
            try
            {
                CMB_CRACC.IsDropDownOpen = true;
            }
            catch (Exception er)
            {

                throw;
            }
        }

        private void cmb_dr_short_name_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                cmb_dr_short_name.IsDropDownOpen = true;
            }
            catch (Exception er)
            {

                throw;
            }
        }

        private void btn_cash_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CMB_CRACC.SelectedItem != null)
                {
                    var id = ((Model.AccountModel)cmb_dr_short_name.SelectedItem).ID;
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

        private void btn_dr_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CMB_CRACC.SelectedItem!=null)
                {
                    var id =((Model.AccountModel)CMB_CRACC.SelectedItem).ID;
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

        private void btn_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReportListTemplateForm jr = new ReportListTemplateForm(); jr.Owner = this;
                jr.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
