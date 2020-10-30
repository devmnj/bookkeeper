using accounts.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Xps.Packaging;
using Xceed.Words.NET;

namespace accounts
{
    /// <summary>
    /// Interaction logic for payment.xaml
    /// </summary>
    public partial class payment : Window, IDataErrorInfo, INotifyPropertyChanged
    {
        double drLimit, DiscLimit, gdrLimit;
        double crLimit, cdrLimit, gcrLimit;
        SqlConnection con = new SqlConnection();
        int crid = 0;

        List<Task> taskList = new List<Task>();
        //DataTable ledgers = new DataTable();
        List<int> p_nos = new List<int>();
        int cindex = 0;
        private string _paCasAc = null;
        private string _paCrAc = null;
        private string _paAmount = "0.00";
        private string _paDisc = "0.00";
        private string _paDAmount = "0.00";
        private string _paNarratiom = null;

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


        public string paCashAc
        {
            set
            {
                _paCasAc = value;
                OnPropertyChanged("paCashAc");
            }
            get { return _paCasAc; }
        }
        public string paCrAc
        {
            set
            {
                _paCrAc = value;
                OnPropertyChanged("paCrAc");
            }
            get { return _paCrAc; }
        }
        public string paAmount
        {
            set
            {
                _paAmount = value;
                OnPropertyChanged("paAmount");
            }
            get { return _paAmount; }
        }
        //public string paDisc
        //{
        //    set
        //    {
        //        value = _paDisc;
        //        OnPropertyChanged("paDisc");
        //    }
        //    get { return _paDisc; }
        //}
        //public string paDAmount
        //{
        //    set
        //    {
        //        value = _paDAmount;
        //        OnPropertyChanged("paDAmount");
        //    }
        //    get { return _paDAmount; }
        //}
        public string paNarration
        {
            set
            {
                _paNarratiom = value;
                OnPropertyChanged("paNarration");
            }
            get { return _paNarratiom; }
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
                switch (columnName)
                {
                    case "paCashAc":
                        result = public_members.AccountValidattor(paCashAc);
                        break;
                    case "paCrAc":
                        result = public_members.AccountValidattor(paCrAc);
                        break;
                    case "paAmount":
                        result = public_members.NumberValidator(paAmount, 0);
                        break;
                    //case "paDisc":
                    //    result = public_members.NumberValidator(paDisc, 0);
                    //    break;
                    //case "paDAmount":
                    //    result = public_members.NumberValidator(paDAmount, 0);
                    //    break;
                    case "paNarration":
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
        public event PropertyChangedEventHandler PropertyChanged;
        public payment()
        {
            InitializeComponent();




        }
        void NewButtonState()
        {
            btn_doc.IsEnabled = false;
            btn_pdf.IsEnabled = false;
            btn_xps.IsEnabled = false;
             
            txt_cramount.Text = "0.00";
            paCashAc = null;
            paCrAc = null;
            txt_total.Text = "0.00";
            cmb_jinv.Text = "";
            cmb_cashaccount.SelectedItem = null;
            cmb_draccount.SelectedItem = null;
            paAmount = "0.00";
            ClearPrintCache();
            info.Text = "";
            txt_task_Amount.Text = "";
            txt_task_label.Text = "";
            paNarration = null;
            txt_pno.Text = "";
            txt_disc.Text = "";
            chk_isrecurr.IsChecked = false;
            txt_discamount.Text = "";
            btn_print.IsEnabled = false;
            tr_history_list.ItemsSource = null;
            btn_save.IsEnabled = true;
            btn_update1.IsEnabled = false;
            btn_delete.IsEnabled = false;
            cmb_cashaccount.Focus();
            lbl_taskflag.Visibility = Visibility.Collapsed;
            p_nos = (from p in ViewModels_Variables.ModelViews.Payments select p.pno).ToList();

            var taskList = ViewModels_Variables.ModelViews.Tasks.Where((t) => t.ENTRY == "PAYMENT");
            
                CollectionView collectionView = new CollectionView(taskList);
                lst_tasks.DataContext = taskList;
             
            txt_pno.Text = DB.Connection.NewEntryno("payments", "p_no").ToString();
            lst_tasks.Visibility = Visibility.Visible;
            hist_task_lbl.Content = "Task".ToUpper();
            dtp_pdate.SelectedDate = ViewModels_Variables._sysDate[0];
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
            cmb_cashaccount.Focus();
        }



        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                NewButtonState();

            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }


        }
        public void Find(int rno)
        {
            try
            {

                if (rno > 0)
                {
                    var rows = ViewModels_Variables.ModelViews.Payments.Where((p) => p.pno == rno).FirstOrDefault();
                    if (rows != null)
                    {
                        NewButtonState();

                        var cracc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.ID == rows.CrAccount.ID).FirstOrDefault();
                        var dracc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.ID == rows.DrAccount.ID).FirstOrDefault();

                        if (dracc != null)
                        {
                            cmb_draccount.SelectedItem = dracc;
                        }
                        if (cracc != null)
                        {
                            cmb_cashaccount.SelectedItem = cracc;
                        }
                        cmb_jinv.Text = rows.Invno;
                        txt_cramount.Text = rows.Amount.ToString("0.00");
                        txt_narration.Text = rows.Narration;
                        dtp_pdate.SelectedDate = rows.Date;
                        txt_pno.Text = rows.pno.ToString();
                        txt_disc.Text = rows.Disc.ToString("0.00");
                        txt_discamount.Text = rows.DiscAmount.ToString("0.00");
                        chk_isrecurr.IsChecked = rows.IsRecurring;

                        int? tid = rows.Task_Id;
                        lbl_taskflag.Visibility = Visibility.Collapsed;
                        txt_total.Text = string.Format("{0:0.00}", Convert.ToDouble(rows.Amount) - Convert.ToDouble(rows.DiscAmount));
                        if (tid > 0) { lbl_taskflag.Visibility = Visibility.Visible; }

                        FindButtonState();

                    }
                    else
                    {
                        MessageBox.Show("Not a Payment No");
                    }
                }
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }

        }

        private void txt_pnofind_KeyDown(object sender, KeyEventArgs e)
        {
            int eno = 0;
            if (txt_pnofind.Text.Length > 0 && e.Key == Key.Enter)
            {
                Int32.TryParse(txt_pnofind.Text.ToString(), out eno);
                Find(eno);
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            int crid = 0, drid = 0;
            double _amount, _damount;
            bool flag = true;
            try
            {
                double.TryParse(txt_cramount.Text.ToString(), out _amount);
                double.TryParse(txt_discamount.Text.ToString(), out _damount);


                //Save


                if (_amount > 0 && cmb_draccount.SelectedItem != null && cmb_cashaccount.SelectedItem != null)
                {
                    Model.PaymentModel payment = new PaymentModel();

                    var cracc = (Model.AccountModel)cmb_cashaccount.SelectedItem;
                    var dracc = (Model.AccountModel)cmb_draccount.SelectedItem;


                    flag = DB.Connection.CheckCreditLocks((_amount - _damount), drLimit, gdrLimit, dracc.ID);
                    flag = DB.Connection.CheckCreditLocks((_amount - _damount), crLimit, gcrLimit, cracc.ID);

                    //Save
                    if (flag == false)
                    {
                        MessageBox.Show("Debit/Credit lock reached");

                    }
                    if (dracc.ID > 0 && cracc.ID > 0 && flag == true)
                    {
                        payment.CrAccount = cracc;
                        payment.DrAccount = dracc;
                        payment.Invno = cmb_jinv.Text.ToString().ToUpper();
                        payment.Amount = _amount;
                        payment.Narration = txt_narration.Text.ToString().Trim();
                        payment.Date = dtp_pdate.SelectedDate.Value;
                        double _disc;
                        double.TryParse(txt_disc.Text.ToString(), out _disc);
                        double.TryParse(txt_discamount.Text.ToString(), out _damount);
                        payment.Disc = _disc;
                        payment.DiscAmount = _damount;



                        var r = DB.Payment.Save(payment, flag);
                        if (r > 0)
                        {


                            MessageBox.Show("Payment Voucher saved!"); btn_Reset_Click(sender, e);
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
            //delete

            if (cmb_draccount.Text.Length > 0 && cmb_cashaccount.Text.Length > 0)
            {
                int.TryParse(txt_pno.Text.ToString(), out int no);
                var r = DB.Payment.Remove(no);
                if (r == true)
                {
                    MessageBox.Show("Payment succesfully removed");
                    btn_Reset_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Something went wrong");
                }

            }
        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            //Edit
            int crid = 0, drid = 0;
            double _amount, _damount;
            bool flag = true;
            try
            {
                double.TryParse(txt_cramount.Text.ToString(), out _amount);
                double.TryParse(txt_discamount.Text.ToString(), out _damount);
                int.TryParse(txt_pno.Text.ToString(), out int pno);


                

                //Save


                if (_amount > 0 && cmb_draccount.SelectedItem != null && cmb_cashaccount.SelectedItem != null)
                {
                    Model.PaymentModel payment = new PaymentModel();

                    var cracc = (Model.AccountModel)cmb_cashaccount.SelectedItem;
                    var dracc = (Model.AccountModel)cmb_draccount.SelectedItem;


                    flag = DB.Connection.CheckCreditLocks((_amount - _damount), drLimit, gdrLimit, dracc.ID);
                    flag =  DB.Connection.CheckCreditLocks((_amount - _damount), crLimit, gcrLimit, cracc.ID);

                    //Save
                    if (flag == false)
                    {
                        MessageBox.Show("Debit/Credit lock reached");

                    }
                    if (cracc.ID > 0 && dracc.ID > 0 && flag == true && pno > 0)
                    {

                        payment.CrAccount = cracc;
                        payment.pno = pno;
                        payment.DrAccount = dracc;
                        payment.Invno = cmb_jinv.Text.ToString().ToUpper();
                        payment.Amount = _amount;
                        payment.Narration = txt_narration.Text.ToString().Trim();
                        payment.Date = dtp_pdate.SelectedDate.Value;
                        double _disc;
                        double.TryParse(txt_disc.Text.ToString(), out _disc);
                        double.TryParse(txt_discamount.Text.ToString(), out _damount);
                        payment.Disc = _disc;
                        payment.DiscAmount = _damount;



                        var r = DB.Payment.Update(payment, flag);
                        if (r == true)
                        {


                            MessageBox.Show("Payment Updated!"); btn_Reset_Click(sender, e);
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

        private void btn_PRINT_Click(object sender, RoutedEventArgs e)
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

        private void btn_movefirst_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int feno;

                if (p_nos.Count > 0)
                {
                    int pno = p_nos[0];
                    cindex = 0;
                    Find(pno);
                }


                else
                {
                    MessageBox.Show("No entry found");
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
                    MessageBox.Show("No entry found");
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
                    MessageBox.Show("No entry found");
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
                    MessageBox.Show("No entry found");
                }
            }
            catch (Exception perror)
            {
                MessageBox.Show(perror.Message.ToString());
            }

        }

        private void btn_find_Click(object sender, RoutedEventArgs e)
        {
            int eno = 0;
            if (txt_pnofind.Text.Length > 0)
            {
                Int32.TryParse(txt_pnofind.Text.ToString(), out eno);
                Find(eno);
            }
        }

        private void btn_cash_report_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception eee)
            {
                MessageBox.Show(eee.Message.ToString());
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

        private void post_date_keydown(object sender, KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void cash_key_down(object sender, KeyEventArgs e)
        {

            public_members._TabPress(e);

        }
        void Dr_Status()
        {
            int crid;
            try
            {
                if (cmb_draccount.SelectedItem != null)
                {

                    var row = (Model.AccountModel)cmb_draccount.SelectedItem;
                    if (row != null)
                    {

                        drLimit = row.DrLimit;
                        info.Text = row.Name;


                        crid = row.ID;

                        var p = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.ID == row.ParentGroup).FirstOrDefault();
                        if (p != null)
                        {
                            gdrLimit = p.Dr_Loc;
                        }
                        string infotxt;
                        infotxt = row.Name;
                        if (drLimit > 0)
                        {
                            infotxt += " Dr.Lock : " + drLimit;
                        }
                        else if (gdrLimit > 0)
                        {
                            infotxt += " Dr.Lock : " + gdrLimit;
                        }

                        var ob = DB.Connection.GetActBalance(crid );

                        if (ob > 0) { infotxt += ", OB: " + ob; }
                        info.Text = infotxt;


                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void dracc_keydown(object sender, KeyEventArgs e)
        {
            public_members._TabPress(e);

            int crdi=0;

            try
            {
                if (cmb_draccount.SelectedItem != null)
                {
                    if (cmb_draccount.Text.Trim().ToString().Length > 0)
                    {

                        var row = (Model.AccountModel)cmb_draccount.SelectedItem;
                        if (row != null)
                        {
                            crid = row.ID;
                            drLimit = row.DrLimit;
                            info.Text = row.Name;


                            crid = row.ID;
                            if (e.Key == Key.Enter)
                            {
                                var jlist = (from j in ViewModels_Variables.ModelViews.Journals where j.CrAccount.ID == crid && DB.Connection.GetActBalance(crid, j.Invoice) > 0 select j.Invoice).ToList();
                                cmb_jinv.ItemsSource = jlist;
                                Dr_Status();
                            }
                            

                            var list = (from r in ViewModels_Variables.ModelViews.Payments
                                        where (r.pno != int.Parse(txt_pno.Text.ToString()) && r.DrAccount.ID == crid)
                                        select new { eno = r.pno, edate = r.Date, amount = r.Amount });

                            tr_history_list.ItemsSource = list;
                            lst_tasks.Visibility = Visibility.Collapsed;
                            hist_task_lbl.Content = "Payment History".ToUpper();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void txt_cramount_KeyDown(object sender, KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void txt_narration_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { TabToSave(); }
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

        private void dtp_pdate_KeyUp(object sender, KeyEventArgs e)
        {
            public_members._TabPress(e);
        }
        void Cash_Status()
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
                        crLimit = row.CrLimit;
                        var p = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.ID == row.ParentGroup).FirstOrDefault();

                        if (p != null)
                        {
                            gcrLimit = p.Cr_Loc;
                        }
                        string infotxt;
                        infotxt = row.Name;
                        if (crLimit > 0)
                        {
                            infotxt += " Cr.Lock : " + crLimit;
                        }
                        else if (gcrLimit > 0)
                        {
                            infotxt += " Cr.Lock : " + gcrLimit;
                        }

                        var ob = DB.Connection.GetActBalance(drid);

                        if (ob > 0) { infotxt += ", OB: " + ob; }
                        info.Text = infotxt;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void cmb_cashaccount_KeyDown(object sender, KeyEventArgs e)
        {
            int drid;
            try
            {

                if (e.Key == Key.Enter)
                {
                    Cash_Status();
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
            try
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
                    if (DiscLimit > 0 && per > DiscLimit) { per = 0; disc = 0.00; }
                    txt_discamount.Text = disc.ToString();
                    txt_disc.Text = per.ToString();
                    txt_total.Text = string.Format("{0:0.00}", am - disc);

                }

                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txt_disc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                double am = 0, disc = 0, per = 0;
                double.TryParse(txt_cramount.Text.ToString(), out am);
                double.TryParse(txt_disc.Text.ToString(), out per);
                if (DiscLimit > 0 && per > DiscLimit)
                {
                    per = DiscLimit;

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

        private void btn_dr_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmb_draccount.SelectedItem != null)
                {
                    var id = ((Model.AccountModel)cmb_draccount.SelectedItem).ID;
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

        private void txt_narration_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
        void JournalDate_Validation()
        { int crid;
            try
            {

                if  ( cmb_jinv.Text != null   && cmb_draccount.SelectedItem != null)
                {
                    var ac = (Model.AccountModel)cmb_draccount.SelectedItem;
                    crid = ac.ID;
                    if (cmb_jinv.Text.Length <= 0) { info.Text = "Inv. Balance for " + cmb_jinv.Text.ToString() + " : " + DB.Connection.GetActBalance(crid); }
                    else
                    {
                        info.Text = "Inv. Balance for " + cmb_jinv.Text.ToString() + " : " + DB.Connection.GetActBalance(crid, cmb_jinv.Text.ToString());
                    }
                    var jinvoice = ViewModels_Variables.ModelViews.Journals.Where((j) => j.Invoice == cmb_jinv.Text.ToString() && (j.CrAccount.ID == ((Model.AccountModel)cmb_draccount.SelectedItem).ID)).FirstOrDefault();
                    if (jinvoice != null && dtp_pdate.SelectedDate <= jinvoice.Date)
                    {
                        MessageBox.Show("The invoice is generated on " + jinvoice.Date.ToShortDateString());
                        if (public_members._sysDate[0] >= jinvoice.Date)
                        {
                            dtp_pdate.SelectedDate = public_members._sysDate[0];
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid date");
                        }
                    }
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        private void cmb_jinv_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {


                if (e.Key == Key.Enter  )
                {
                    JournalDate_Validation();
                }

                public_members._TabPress(e);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }

        private void btn_print_Click_1(object sender, RoutedEventArgs e)
        {

        }
        string CreateWordDoc()
        {
            string fileName = null;
            try
            {
                fileName = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\doc", ".docx");
                using (var document = DocX.Create(fileName))
                {
                    // Add a title
                    document.ApplyTemplate(@"DocTemplates\payment.dotx");

                    //Footer
                    document.ReplaceText("[MyCompany]", ViewModels_Variables.ModelViews.CompanyProfile[0].company);
                    document.ReplaceText("[Clandmark]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].lmark);
                    document.ReplaceText("[CCity]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].place);
                    document.ReplaceText("[CPhone]", "Phone :" + ViewModels_Variables.ModelViews.CompanyProfile[0].officeno);

                     
                    var cust = (Model.AccountModel) cmb_draccount.SelectedItem;
                    if (cust!=null)
                    {
                        document.ReplaceText("[Customer]", cust.Name);
                        document.ReplaceText("[Company]", "");
                        document.ReplaceText("[Address]", cust.Address);
                        document.ReplaceText("[City]", cust.City);
                        document.ReplaceText("[Phone]", cust.PhoneNo);
                        document.ReplaceText("[CustomerID]", cust.ID.ToString());
                    }
                    //Receipt Info
                    document.ReplaceText("[VNo]", txt_pno.Text.ToString());
                    document.ReplaceText("[Date]", dtp_pdate.SelectedDate.Value.ToShortDateString());
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
            catch (Exception)
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
                 wordFname=   CreateWordDoc();
                }
               if(wordFname!=null) Process.Start("winword.exe", wordFname);



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
                    wordFname = CreateWordDoc();
                }
                if (wordFname != null)
                {
                    pdfFname = ReportExport.WordExport.CreatePDF(wordFname);
                }

             if(pdfFname!=null)   Process.Start("AcroRd32.exe", pdfFname);



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
                    wordFname = CreateWordDoc();
                }
                if (wordFname != null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }

               if(xpsFname!=null) Process.Start("explorer.exe", xpsFname);



            }
            catch (Exception e1)

            {
                MessageBox.Show(e1.Message.ToString());

            }
            finally
            {

            }
        }

        private void lst_tasks_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                if (ViewModels_Variables.ModelViews.Payments.Count() <= 0)
                {
                    DB.Payment.Fetch();
                    ViewModels_Variables.ModelViews.Payments_To_List();
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
            catch (Exception e1)

            {
                MessageBox.Show(e1.Message.ToString());

            }
        }

        private void btn_task_del_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btn_save.IsEnabled == false)
                {
                    int.TryParse(txt_pno.Text.ToString(), out int t);
                    var t1 = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ENO == t && ts.ENTRY == "PAYMENT").FirstOrDefault();
                    if (t1 != null)
                    {
                        var tt = DB.Task.DeleteTask(eno: t1.ENO, entry: "PAYMENT");
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
                    task.ENTRY = "PAYMENT";
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

        private void chk_isrecurr_Click(object sender, RoutedEventArgs e)
        {

        }

        private void chk_isrecurr_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chk_isrecurr.IsChecked == true)
                {
                    var cr = (Model.AccountModel)cmb_cashaccount.SelectedItem;
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

        private void cmb_cashaccount_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                Cash_Status();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmb_draccount_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                Dr_Status();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmb_cashaccount_GotFocus_1(object sender, RoutedEventArgs e)
        {

        }

        private void cmb_draccount_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                cmb_draccount.IsDropDownOpen = true;
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
                    var t = (Task)lst_tasks.SelectedItem;
                    MessageBoxResult res1 = new MessageBoxResult();
                    res1 = MessageBox.Show("Do you want to execute this task", "Task", MessageBoxButton.YesNo);
                    if (res1 == MessageBoxResult.Yes)
                    {
                        if (t != null)
                        {
                            var res = DB.Payment.Tasker(t, dtp_pdate.SelectedDate.Value);
                            if (res == true)
                            {
                                MessageBox.Show("Task completed");
                               // public_members.Refresh_Payments();
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

        private void cmb_cashaccount_GotFocus(object sender, RoutedEventArgs e)
        {
            cmb_cashaccount.IsDropDownOpen = true; ;
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
                PaymentReport rp = new PaymentReport();
                rp.Owner = this;
                rp.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
