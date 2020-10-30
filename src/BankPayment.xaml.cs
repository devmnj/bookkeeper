
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Xps.Packaging;
using Xceed.Words.NET;

namespace accounts
{
    /// <summary>
    /// Interaction logic for BankPayment.xaml
    /// </summary>
    /// 
    public class TextboxText
    {
        public string textdata { get; set; }

    }
    public partial class BankPayment : Window

    {
        double dreditLimit, creditLinit, gdreditLimit, gCrLimit, discLimit;
        int crid = 0;
        SqlConnection con = new SqlConnection();
        List<int> br_nos = new List<int>();
        ObservableCollection<int> status_data1 = new ObservableCollection<int>();
        int cindex = 0;
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

        public BankPayment()
        {
            InitializeComponent();



        }
        void Status()
        {
            try
            {
                //var x = Convert.ToInt32( (from p in bpayments.AsEnumerable() select p.Field<string>("bp_no")).Count<string>());
                //var y = Convert.ToInt32( (from p in bpayments.AsEnumerable() select p.Field<string>("bp_no")).Count<string>());
                //c = x;p = y;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString());
            }
        }

        void FindButtonState()
        {
            btn_doc.IsEnabled = true;
            btn_pdf.IsEnabled = true;
            ClearPrintCache();
            btn_xps.IsEnabled = true;

            btn_save.IsEnabled = false;
            btn_update1.IsEnabled = true;
            btn_delete.IsEnabled = true;
            btn_print.IsEnabled = true;
            cmb_cashaccount.Focus();
        }
        void NewButtonState()
        {
            btn_doc.IsEnabled = false;
            btn_pdf.IsEnabled = false;
            ClearPrintCache();
            btn_xps.IsEnabled = false;

            btn_print.IsEnabled = false;
            dtp_brdate.SelectedDate = ViewModels_Variables._sysDate[0];
            dtp_chdate.SelectedDate = ViewModels_Variables._sysDate[0];

            txt_total.Text = "0.00";
            txt_amount.Text = "";
            txt_bcharge.Text = "";
            txt_brno.Text = "";
            txt_chqNo.Text = "";
            txt_disc.Text = "";
            cmb_jinv.Text = "";
            info.Text = " ";
            txt_discamount.Text = "";
            cmb_cashaccount.SelectedItem = null;
            cmb_draccount.SelectedItem = null;
            cmb_status.Text = "";
            cmb_typeofpayment.Text = "";
            txt_narration.Text = "";
            btn_save.IsEnabled = true;
            btn_update1.IsEnabled = false;
            btn_delete.IsEnabled = false;
            cmb_cashaccount.Focus();
            txt_brno.Text = DB.Connection.NewEntryno("bank_payments", "bp_no").ToString();
            br_nos = (from pb in ViewModels_Variables.ModelViews.BankPayments select pb.pno).ToList<int>();


        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
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
            catch (Exception)
            {

                throw;
            }
        }
        private void btn_cr_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmb_cashaccount.SelectedItem != null)
                {
                    var acc = (Model.AccountModel)cmb_cashaccount.SelectedItem;
                    if (acc != null)
                    {
                        CashBook1 cashsow = new CashBook1(acc.ID);
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
                if (cmb_draccount.Text.Length > 0)
                {
                    var id = (Model.AccountModel)cmb_draccount.SelectedItem;
                    if (id != null)
                    {
                        CashBook1 cashsow = new CashBook1(id.ID);
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

        private void dtp_chdate_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                accountRegistration ac = new accountRegistration(); ac.Owner = this.Owner; ac.Show();
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
                bpReport br = new bpReport();
                br.Owner = this;
                br.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            //delete

            if (cmb_draccount.Text.Length > 0 && cmb_cashaccount.Text.Length > 0)
            {
                int.TryParse(txt_brno.Text.ToString(), out int no);
                var r = DB.BankPayment.Remove(no);
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

        private void btn_update1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double _amount = 0, _damount = 0, _disc = 0, _bcahrge = 0;
                double.TryParse(txt_amount.Text.ToString(), out _amount);
                double.TryParse(txt_discamount.Text.ToString(), out _damount);
                double.TryParse(txt_bcharge.Text.ToString(), out _bcahrge);
                int.TryParse(txt_brno.Text.ToString(), out int bpno);
                if (cmb_status.Text != null && cmb_typeofpayment.Text != null && cmb_cashaccount.Text != null && cmb_draccount.Text != null && txt_chqNo.Text != null && _amount > 0 && cmb_draccount.Text.Length > 0 && cmb_cashaccount.Text.Length > 0 && cmb_status.Text.Length > 0 && cmb_typeofpayment.Text.Length > 0)
                {

                    Model.BankPaymentModel bankPayment = new Model.BankPaymentModel();
                    int crid, drid;
                    var cracc = (Model.AccountModel)cmb_cashaccount.SelectedItem;
                    var dacc = (Model.AccountModel)cmb_draccount.SelectedItem;
                    bool flag;
                    flag = true;
                    flag = DB.Connection.CheckCreditLocks((_amount - _damount), dreditLimit, gdreditLimit, dacc.ID);
                    flag = DB.Connection.CheckCreditLocks((_amount - _damount), gCrLimit, gCrLimit, cracc.ID);
                    if (flag == false) { MessageBox.Show("Credit/Debit lock reached"); }
                    if (flag = true && cracc.ID > 0 && dacc.ID > 0)
                    {
                        bankPayment.pno = bpno;
                        bankPayment.Crid = cracc.ID;
                        bankPayment.Drid = dacc.ID;
                        bankPayment.CrAccount = cracc;
                        bankPayment.DrAccount = dacc;
                        bankPayment.Amount = _amount;
                        double.TryParse(txt_disc.Text.ToString(), out _disc);
                        bankPayment.Disc = _disc;
                        bankPayment.DiscAmount = _damount;
                        bankPayment.BankCharge = _bcahrge;
                        bankPayment.Narration = txt_narration.Text.ToString();
                        bankPayment.Type = cmb_typeofpayment.Text.ToString();
                        bankPayment.status = cmb_status.Text.ToString();
                        bankPayment.CheqNo = txt_chqNo.Text.ToString();
                        bankPayment.Date = dtp_brdate.SelectedDate.Value;
                        bankPayment.CheqDate = dtp_chdate.SelectedDate.Value;
                        bankPayment.Invno = cmb_jinv.Text.ToString();
                        var r = DB.BankPayment.Update(bankPayment, flag);
                        if (r == true)
                        {
                            MessageBox.Show("Payment Updated");
                            btn_Reset_Click(sender, e);
                        }
                        else { MessageBox.Show("Something went wrong"); }

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
            catch (Exception ee)
            {

                MessageBox.Show(ee.Message.ToString());
            }
        }
        public void Find(int eno)
        {
            try
            {
                var rows = ViewModels_Variables.ModelViews.BankPayments.Where((bp) => bp.pno == eno).FirstOrDefault();
                if (rows != null)
                {
                    NewButtonState();
                    txt_brno.Text = rows.pno.ToString();

                    cmb_jinv.Text = rows.Invno;
                    var dracc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.ID == rows.Drid).FirstOrDefault();
                    var cracc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.ID == rows.Crid).FirstOrDefault();

                    if (cracc != null) cmb_cashaccount.SelectedItem = cracc;
                    if (dracc != null) cmb_draccount.SelectedItem = dracc;
                    dtp_brdate.SelectedDate = rows.Date;
                    dtp_chdate.SelectedDate = rows.CheqDate;
                    txt_chqNo.Text = rows.CheqNo;
                    cmb_status.Text = rows.status; ;
                    cmb_typeofpayment.Text = rows.Type;
                    txt_disc.Text = rows.Disc.ToString("0.00");
                    txt_discamount.Text = rows.DiscAmount.ToString("0.00");
                    txt_bcharge.Text = rows.BankCharge.ToString("0.00");
                    txt_amount.Text = rows.Amount.ToString("0.00");

                    txt_narration.Text = rows.Narration;
                    txt_total.Text = string.Format("{0:0.00}", Convert.ToDouble(txt_amount.Text.ToString()) + Convert.ToDouble(txt_bcharge.Text.ToString()) - Convert.ToDouble(txt_discamount.Text.ToString())).ToString();
                    FindButtonState();
                }
                else
                {
                    MessageBox.Show("Entry not found");
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewButtonState();
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
        private void txt_narration_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter) { TabToSave(); }
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
                double _amount = 0, _damount = 0, _disc = 0, _bcahrge = 0;
                double.TryParse(txt_amount.Text.ToString(), out _amount);
                double.TryParse(txt_discamount.Text.ToString(), out _damount);
                double.TryParse(txt_bcharge.Text.ToString(), out _bcahrge);
                if (cmb_status.Text != null && cmb_typeofpayment.Text != null && cmb_cashaccount.Text != null && cmb_draccount.Text != null && txt_chqNo.Text != null && _amount > 0 && cmb_draccount.Text.Length > 0 && cmb_cashaccount.Text.Length > 0 && cmb_status.Text.Length > 0 && cmb_typeofpayment.Text.Length > 0)
                {

                    Model.BankPaymentModel bankPayment = new Model.BankPaymentModel();
                    int crid, drid;
                    var cracc = (Model.AccountModel)cmb_cashaccount.SelectedItem;
                    var dacc = (Model.AccountModel)cmb_draccount.SelectedItem;
                    bool flag;
                    flag = true;
                    flag = DB.Connection.CheckCreditLocks((_amount - _damount), dreditLimit, gdreditLimit, dacc.ID);
                    flag = DB.Connection.CheckCreditLocks((_amount - _damount), gCrLimit, gCrLimit, cracc.ID);
                    if (flag == false) { MessageBox.Show("Credit/Debit lock reached"); }
                    if (flag = true && cracc.ID > 0 && dacc.ID > 0)
                    {
                        bankPayment.Crid = cracc.ID;
                        bankPayment.Crid = cracc.ID;
                        bankPayment.CrAccount = cracc;
                        bankPayment.DrAccount = dacc;
                        bankPayment.Amount = _amount;
                        double.TryParse(txt_disc.Text.ToString(), out _disc);
                        bankPayment.Disc = _disc;
                        bankPayment.DiscAmount = _damount;
                        bankPayment.BankCharge = _bcahrge;
                        bankPayment.Narration = txt_narration.Text.ToString();
                        bankPayment.Type = cmb_typeofpayment.Text.ToString();
                        bankPayment.status = cmb_status.Text.ToString();
                        bankPayment.CheqNo = txt_chqNo.Text.ToString();
                        bankPayment.Date = dtp_brdate.SelectedDate.Value;
                        bankPayment.CheqDate = dtp_chdate.SelectedDate.Value;
                        bankPayment.Invno = cmb_jinv.Text.ToString();
                        var r = DB.BankPayment.Save(bankPayment, flag);
                        if (r > 0)
                        {

                            MessageBox.Show("Payment Saved");
                            btn_Reset_Click(sender, e);
                        }
                        else { MessageBox.Show("Something went wrong"); }

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
            catch (Exception ee)
            {

                MessageBox.Show(ee.Message.ToString());
            }
        }

        private void cmb_typeofpayment_KeyDown(object sender, KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void cmb_status_KeyDown(object sender, KeyEventArgs e)
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
                    double.TryParse(txt_amount.Text.ToString(), out am);
                    double.TryParse(txt_disc.Text.ToString(), out per);
                    if (discLimit > 0 && per > discLimit)
                    {
                        per = discLimit;

                    }

                    if (per > 0 && am > 0)
                    {
                        disc = am * per / 100;
                    }
                    txt_disc.Text = string.Format("{0:0.00}", per);
                    txt_discamount.Text = string.Format("{0:0.00}", disc);

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
                    double am = 0, disc = 0, per = 0, bc = 0;
                    double.TryParse(txt_amount.Text.ToString(), out am);
                    double.TryParse(txt_discamount.Text.ToString(), out disc);
                    double.TryParse(txt_bcharge.Text.ToString(), out bc);

                    if (am > 0 && disc > 0)
                    {
                        per = disc / am * 100;
                    }
                    if (discLimit > 0 && per > discLimit) { per = 0; disc = 0.00; }
                    txt_discamount.Text = string.Format("{0:0.00}", disc);
                    txt_disc.Text = string.Format("{0:0.00}", per);
                    txt_total.Text = string.Format("{0:0.00}", (am - disc + bc));

                }

                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txt_bcharge_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_amount_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_chqNo_KeyDown(object sender, KeyEventArgs e)
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

        private void btn_movelast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (br_nos.Count > 0)
                {
                    int pno = br_nos[br_nos.Count - 1];
                    cindex = br_nos.Count - 1;
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


                if (br_nos.Count > 0 && cindex < br_nos.Count - 1)
                {

                    cindex++;

                    Find(br_nos[cindex]);
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

        private void btn_movefirst_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int feno;

                if (br_nos.Count > 0)
                {
                    int pno = br_nos[0];
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

        private void txt_brnofind_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (txt_brnofind.Text != null && txt_brnofind.Text.Length > 0)
                    {
                        int n;
                        int.TryParse(txt_brnofind.Text.ToString(), out n);
                        if (n > 0)
                        {
                            Find(n);
                        }
                    }
                }
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }


        private void btn_find_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dtp_brdate_KeyUp(object sender, KeyEventArgs e)
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

        private void btn_moveprevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (br_nos.Count > 0 && cindex >= 0 && cindex < br_nos.Count)
                {
                    if (cindex != 0) { cindex--; }

                    Find(br_nos[cindex]);
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
        void Bank_status()
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
                        var p = ViewModels_Variables.ModelViews.AccountGroups.Where((pp) => pp.ID == row.ParentGroup).FirstOrDefault();
                        if (p != null)
                        {
                            gdreditLimit = p.Dr_Loc;
                        }
                        string infotxt;
                        infotxt = row.Name;
                        if (dreditLimit > 0)
                        {
                            infotxt += " Dr.Lock : " + dreditLimit;
                        }
                        else if (gCrLimit > 0)
                        {
                            infotxt += " Dr.Lock : " + gdreditLimit;
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
            try
            {
                int drid;

                if (e.Key == Key.Enter)
                {
                    Bank_status();
                }

                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmb_jinv_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && cmb_jinv.Text != null && cmb_jinv.Text.Length > 0 && crid > 0)
                {
                    info.Text = "Inv. Balance for " + cmb_jinv.Text.ToString() + " : " + DB.Connection.GetActBalance(crid, cmb_jinv.Text.ToString());
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
                    wordFname = CreateWordDoc();
                }
                if (wordFname != null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }

                if (xpsFname != null)
                {
                    XpsDocument xps = new XpsDocument(xpsFname, FileAccess.Read);
                    accounts.PrintDialogue print = new PrintDialogue(xps, this.Title + " " + txt_brno.Text.ToString());
                    print.Show();
                    xps.Close();
                }
            }
            catch (Exception w1)
            {

                MessageBox.Show(w1.Message.ToString());
            }
        }
        string CreateWordDoc()
        {
            string fileName = null;
            try
            {
                fileName = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\doc", ".doc");
                using (var document = DocX.Create(fileName))
                {
                    // Add a title
                    document.ApplyTemplate(@"DocTemplates\bankpayment.dotx");

                    //Footer
                    document.ReplaceText("[MyCompany]", ViewModels_Variables.ModelViews.CompanyProfile[0].company);
                    document.ReplaceText("[Clandmark]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].lmark);
                    document.ReplaceText("[CCity]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].place);
                    document.ReplaceText("[CPhone]", "Phone :" + ViewModels_Variables.ModelViews.CompanyProfile[0].officeno);

                    //                    customer Information
                    var cust = (Model.AccountModel)cmb_draccount.SelectedItem;
                    if (cust != null)
                    {
                        document.ReplaceText("[Supplier]", cust.Name);
                        document.ReplaceText("[Company]", "");
                        document.ReplaceText("[Address]", cust.Address.ToString());
                        document.ReplaceText("[City]", cust.City.ToString());
                        document.ReplaceText("[Phone]", cust.Mob.ToString());
                        document.ReplaceText("[SupplierID]", cust.ID.ToString());
                    }

                    document.ReplaceText("[Method]", cmb_typeofpayment.Text.ToString());
                    document.ReplaceText("[DDDate]", dtp_chdate.SelectedDate.Value.ToShortDateString());
                    document.ReplaceText("[CheqNo]", txt_chqNo.Text.ToString());
                    document.ReplaceText("[Status]", cmb_status.Text.ToString());
                    document.ReplaceText("[BankCharge]", txt_bcharge.Text.ToString());

                    var bank = (Model.AccountModel)cmb_cashaccount.SelectedItem;
                    if (bank != null)
                    {
                        document.ReplaceText("[BankName]", bank.Name);
                        document.ReplaceText("[Branch]", bank.Address);
                        document.ReplaceText("[AcNo]", bank.City);

                        document.ReplaceText("[IFC]", bank.Mob);
                    }


                    ////Payment Info
                    document.ReplaceText("[VNo]", txt_brno.Text.ToString());
                    document.ReplaceText("[Date]", dtp_brdate.SelectedDate.Value.ToShortDateString());
                    string jinv = null;
                    if (cmb_jinv.Text != null && cmb_jinv.Text.Length > 0) { jinv = cmb_jinv.Text.ToString(); } else { jinv = ""; }
                    document.ReplaceText("[InvNo]", jinv);
                    document.ReplaceText("[Discount]", txt_discamount.Text.ToString());
                    document.ReplaceText("[Amount]", txt_amount.Text.ToString());
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

                    wordFname = CreateWordDoc();
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

                    wordFname = CreateWordDoc();
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

                    wordFname = CreateWordDoc();
                }
                if (wordFname != null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }

                if (xpsFname != null) Process.Start("explorer.exe", xpsFname);



            }
            catch (Exception e1)

            {
                MessageBox.Show(e1.Message.ToString());

            }
            finally
            {

            }
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

                //if (ViewModels_Variables.ModelViews.BankPayments.Count() <= 0)
                //{
                //    DB.BankPayment.Fetch();
                //    ViewModels_Variables.ModelViews.BPayments_To_List();
                //}
                //if (ViewModels_Variables.ModelViews.Journals.Count() <= 0)
                //{
                //    DB.Journal.Fetch();
                //    ViewModels_Variables.ModelViews.Journals_To_list();
                //}

                var view = ViewModels_Variables.ModelViews;
                DataContext = view;
                NewButtonState();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        void Dr_status()
        {
            int crid;
            try
            {
                if (cmb_draccount.SelectedItem != null)
                {


                    var row = (Model.AccountModel)cmb_draccount.SelectedItem;
                    if (row != null)
                    {
                        crid = row.ID;


                        creditLinit = row.CrLimit;
                        discLimit = row.MaxDisc;
                        var p = ViewModels_Variables.ModelViews.AccountGroups.Where((pp) => pp.ID == row.ParentGroup).FirstOrDefault();
                        if (p != null)
                        {
                            gCrLimit = p.Cr_Loc;
                        }
                        string infotxt;
                        infotxt = row.Name;
                        if (creditLinit > 0)
                        {
                            infotxt += " Dr.Lock : " + creditLinit;
                        }
                        else if (gCrLimit > 0)
                        {
                            infotxt += " Dr.Lock : " + gCrLimit;
                        }

                        var ob = DB.Connection.GetActBalance(crid);

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

        private void cmb_draccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Dr_status();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmb_cashaccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Bank_status();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmb_draccount_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.Key == Key.Enter) { Dr_status(); }
                if (cmb_draccount.SelectedItem != null)
                {


                    var row = (Model.AccountModel)cmb_draccount.SelectedItem;
                    if (row != null)
                    {
                        crid = row.ID;
                        if (e.Key == Key.Enter)
                        {
                            var jlist = (from j in ViewModels_Variables.ModelViews.Journals where j.CrAccount.ID == crid && DB.Connection.GetActBalance(crid, j.Invoice) > 0 select j.Invoice).ToList();
                            cmb_jinv.ItemsSource = jlist;
                        }
                    }
                }

                public_members._TabPress(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void dtp_chdate_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dtp_chdate_KeyUp(object sender, KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void cmb_cashaccount_GotFocus(object sender, RoutedEventArgs e)
        {

        }
    }
    public class Values
    {
        public string x { get; set; }
        public string y { get; set; }
    }
}
