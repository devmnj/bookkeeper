using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Windows.Xps.Packaging;
using accounts.Model;
using Xceed.Document.NET;

namespace accounts
{
    /// <summary>
    /// Interaction logic for CashBook1.xaml
    /// </summary>
    /// 


    public partial class CashBook1 : System.Windows.Window
    {
        Random random;
        bool loaded = false;
        string wordFname = null;
        string xpsFname = null;
        string pdfFname = null; List<CashBookModel> report = new List<CashBookModel>();
        public CashBook1()
        {
            InitializeComponent();
            dtp_from.SelectedDate = ViewModels_Variables._sysDate[0];
            dtp_to.SelectedDate = DateTime.Now;
            dtp_from.Focus();

        }
        public void Find(int ac)
        {
            try
            {

                var cash = ViewModels_Variables.ModelViews.Accounts.Where((acc) => acc.ID == ac).FirstOrDefault();
                cmb_ACCOUNTS.SelectedItem = cash;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public CashBook1(int ac)
        {
            InitializeComponent();

            loaded = false;




        }


        private void Cmb_ACCOUNTS_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Enter)
            //{


            //}
        }


        private void dtp_from_keyup(object sender, KeyEventArgs e)
        {

            public_members._TabPress(e);
        }


        private void refresh_data_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cmb_ACCOUNTS.SelectedItem = null;
                Window_Loaded(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }

        }



        private void dtp_to_KeyUp(object sender, KeyEventArgs e)
        {

            public_members._TabPress(e);
        }

        private void rep_grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (rep_grid.CurrentColumn == rep_grid.Columns[1] || rep_grid.CurrentColumn == rep_grid.Columns[2])
                {
                    var cur = ((CashBookModel)rep_grid.CurrentItem);


                    if (cur != null)
                    {
                        if (cur.Voucher != null)
                        {
                            switch (cur.Voucher)
                            {
                                case "RECEIPT":
                                    Receipt rform = new Receipt();
                                    rform.Owner = this;
                                    rform.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                    rform.Show();
                                    rform.Find(cur.VNo);
                                    break;
                                case "PAYMENT":
                                    payment pform = new payment();
                                    pform.Owner = this;
                                    pform.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                    pform.Show();
                                    pform.Find(cur.VNo);
                                    break;
                                case "JOURNAL":
                                    journalposting jform = new journalposting();
                                    jform.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                    jform.Show();
                                    jform.Find(cur.VNo);
                                    break;
                                case "BANK RECEIPT":
                                    bankReceipt br = new bankReceipt();
                                    br.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                    br.Show(); br.Find(cur.VNo);
                                    break;
                                case "BANK PAYMENT":
                                    BankPayment bP = new BankPayment();
                                    bP.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                    bP.Show(); bP.Find(cur.VNo);
                                    break;
                                case "PAYROLL VOUCHER":
                                    payroll_payments_deductions Pv = new payroll_payments_deductions();
                                    Pv.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                    Pv.Show(); Pv.Find(cur.VNo);
                                    break;
                            }
                        }
                    }
                }

                else if (rep_grid.CurrentColumn == rep_grid.Columns[3])
                {

                    var accc = ((CashBookModel)rep_grid.CurrentItem);

                    if (accc != null && accc.DrAccount != null)
                    {
                        accountRegistration acc = new accountRegistration();
                        acc.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        acc.Show();
                        acc.Find(accc.DrAccount.ID);
                    }
                }

                else if (rep_grid.CurrentColumn == rep_grid.Columns[4])
                {
                    var accn = ((CashBookModel)rep_grid.CurrentItem);

                    if (accn.DrAccount != null)
                    {
                        CashBook1 r = new CashBook1();
                        r.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        r.Show();
                        r.Find(accn.DrAccount.ID);
                    }
                }


            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message.ToString());
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

        private void rep_grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmb_groups_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {


            }
        }

        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            XpsDocument xps = null;

            try
            {
                if (wordFname == null)
                {
                    wordFname = GenerateWordDoc();
                }
                if (xpsFname == null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }
                if (xpsFname!=null) {
                    xps = new XpsDocument(xpsFname, FileAccess.Read);
                    accounts.PrintDialogue print = new PrintDialogue(xps, this.Title + " : " + cmb_ACCOUNTS.Text.ToString());
                    print.Show();
                    xps.Close();
                }
            }
            catch (Exception w1)
            {
                MessageBox.Show(w1.Message.ToString());
            }
        }

        string GenerateWordDoc()
        {
            string fname = null;
            try
            {
                if (cmb_ACCOUNTS.SelectedItem != null)
                {

                    fname = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\Doc");

                    using (var document = Xceed.Words.NET.DocX.Create(fname))
                    {
                        document.ApplyTemplate(@"doctemplates /CashLedger.dotx");
                        document.ReplaceText("[MyCompany]", ViewModels_Variables.ModelViews.CompanyProfile[0].company);
                        document.ReplaceText("[Clandmark]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].lmark);
                        document.ReplaceText("[CCity]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].place);
                        document.ReplaceText("[CPhone]", "Phone :" + ViewModels_Variables.ModelViews.CompanyProfile[0].officeno);
                        document.ReplaceText("[Account]", ((Model.AccountModel)cmb_ACCOUNTS.SelectedItem).Name);
                        document.ReplaceText("[AccountID]", ((Model.AccountModel)cmb_ACCOUNTS.SelectedItem).ID.ToString());
                        document.ReplaceText("[SDate]", dtp_from.SelectedDate.Value.ToShortDateString());
                        document.ReplaceText("[EDate]", dtp_to.SelectedDate.Value.ToShortDateString());
                        document.ReplaceText("[ReportDate]", public_members._sysDate[0].ToShortDateString());

                        var t = document.Tables;
                        var sorted = rep_grid.Items.Cast<CashBookModel>().ToList();
                        if (sorted != null)
                        {
                            Row newrow;
                            int i = 1;
                            foreach (var v in sorted)
                            {
                                ++i;
                                newrow = t[1].InsertRow();
                                newrow.Cells[0].Paragraphs[0].Append(v.Date.ToShortDateString());
                                if (v.Voucher != null)
                                {
                                    string L = "";
                                    if (v.Voucher == "Bank Payment") { L = "BAP"; }
                                    else if (v.Voucher == "OB")
                                    {
                                        L = "OB";
                                    }
                                    else if (v.Voucher == "PayRoll Voucher") { L = "PAV"; }
                                    else if (v.Voucher == "PayRoll") { L = "PAR"; }
                                    else if (v.Voucher == "Bank Receipt") { L = "BAR"; }
                                    else
                                    {
                                        L = v.Voucher.Substring(0, 3);
                                    }
                                    if (v.VNo == 0) { newrow.Cells[1].Paragraphs[0].Append(" " + " " + L); }
                                    else
                                        newrow.Cells[1].Paragraphs[0].Append(v.VNo + " " + L);
                                }
                                else
                                {
                                    newrow.Cells[1].Paragraphs[0].Append(v.VNo.ToString());
                                }

                                if (v.Voucher != null) { newrow.Cells[2].Paragraphs[0].Append(v.Voucher); } else { newrow.Cells[2].Paragraphs[0].Append(v.DrAccount.Name); }
                                newrow.Cells[3].Paragraphs[0].Append(v.Dr_Amount.ToString("0.00")).Alignment = Alignment.right;
                                newrow.Cells[4].Paragraphs[0].Append(v.Cr_Amount.ToString("0.00")).Alignment = Alignment.right;
                                t[1].Rows.Add(newrow);

                                if (v.Invno != null && v.Invno.Length > 0)
                                {
                                    newrow = t[1].InsertRow();
                                    newrow.Cells[2].Paragraphs[0].Append("Invoice No: " + v.Invno).Alignment = Alignment.right;
                                    t[1].Rows.Add(newrow);
                                    newrow = t[1].InsertRow();
                                    newrow.Cells[2].Paragraphs[0].Append("Balance: " + v.Balance.ToString("0.00")).Color(System.Drawing.Color.DarkGreen).Alignment = Alignment.right;
                                    t[1].Rows.Add(newrow);

                                }
                                if (v.Narration != null && v.Narration.Length > 0)
                                {
                                    newrow = t[1].InsertRow();
                                    newrow.Cells[2].Paragraphs[0].Append(v.Narration);
                                    t[1].Rows.Add(newrow);
                                }
                            }
                            newrow = t[1].InsertRow();
                            t[1].Rows.Add(newrow);

                            var x = sorted.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Dr_Amount));
                            var y = sorted.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Cr_Amount));
                            newrow = t[1].InsertRow();
                            newrow.Cells[1].Paragraphs[0].Append("Total").Bold(true);
                            newrow.Cells[3].Paragraphs[0].Append(string.Format("{0:0.00}", y)).Bold(true).Alignment = Alignment.right;
                            newrow.Cells[4].Paragraphs[0].Append(string.Format("{0:0.00}", x)).Bold(true).Alignment = Alignment.right; ;
                            t[1].Rows.Add(newrow);
                            newrow = t[1].InsertRow();
                            newrow.Cells[1].Paragraphs[0].Append("");

                            newrow.Cells[3].Paragraphs[0].Append("").Bold(true).Alignment = Alignment.right;
                            newrow.Cells[4].Paragraphs[0].Append(lblbalance.Text).Bold(true).FontSize(14).Alignment = Alignment.right;
                            t[1].Rows.Add(newrow);
                        }
                        document.Save();
                        document.Dispose();

                    }
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }
            return fname;
        }
        private void btn_doc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wordFname == null)
                {
                    wordFname = GenerateWordDoc();
                }
               if (wordFname != null)  Process.Start("WINWORD.exe", wordFname);
            }
            catch (Exception e1)

            {
                MessageBox.Show(e1.Message.ToString());

            }
        }

        private void btn_pdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (wordFname == null)
                {
                    wordFname = GenerateWordDoc();
                }
                if (pdfFname == null)
                {
                    pdfFname = ReportExport.WordExport.CreatePDF(wordFname);
                }

               if (pdfFname != null)  Process.Start("AcroRd32.exe", pdfFname);

            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void btn_excel_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var fname = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\workbook", ".xlsx");
                var app = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook workbook;
                workbook = app.Workbooks.Add(Type.Missing);
                var worKsheeT = (Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet;
                worKsheeT.Range[worKsheeT.Cells[1, 1], worKsheeT.Cells[1, 8]].Merge();
                worKsheeT.Cells[1, 1] = "Cash Book";
                var range = (Range)worKsheeT.Cells[1, 1];
                range.Font.Bold = true;
                range.Font.Size = 25;
                range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                worKsheeT.Name = "CashBook";

                worKsheeT.Range[worKsheeT.Cells[3, 1], worKsheeT.Cells[3, 8]].Merge();
                if (cmb_ACCOUNTS.SelectedItem != null)
                {
                    worKsheeT.Cells[3, 1] = "Cash Report  of " + ((Model.AccountModel)cmb_ACCOUNTS.SelectedItem).Name + " on " + dtp_from.SelectedDate.Value.ToShortDateString() + " - " + dtp_to.SelectedDate.Value.ToShortDateString();
                }else
                { worKsheeT.Cells[3, 1] = "Cash Report  of " + " " + " on " + dtp_from.SelectedDate.Value.ToShortDateString() + " - " + dtp_to.SelectedDate.Value.ToShortDateString(); }
                worKsheeT.Cells.Font.Size = 12;

                Table Heading;
                worKsheeT.Cells[5, 1] = "SLNO";
                worKsheeT.Cells[5, 1].Font.Size = 13;

                worKsheeT.Cells[5, 2] = "Date";
                worKsheeT.Cells[5, 2].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 2]).EntireColumn.ColumnWidth = 15;
                worKsheeT.Cells[5, 3] = "Voucher";
                worKsheeT.Cells[5, 3].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 3]).EntireColumn.ColumnWidth = 20;

                worKsheeT.Cells[5, 4] = "V.No";
                worKsheeT.Cells[5, 4].Font.Size = 13;

                worKsheeT.Cells[5, 5] = "Account";
                worKsheeT.Cells[5, 5].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 5]).EntireColumn.ColumnWidth = 40;

                worKsheeT.Cells[5, 6] = "Dr Amount";
                ((Range)worKsheeT.Cells[5, 6]).EntireColumn.ColumnWidth = 16;
                worKsheeT.Cells[5, 6].Font.Size = 13;

                worKsheeT.Cells[5, 7] = "Credit Amount";
                ((Range)worKsheeT.Cells[5, 7]).EntireColumn.ColumnWidth = 16;
                worKsheeT.Cells[5, 7].Font.Size = 13;

                var sorted = report;
               if(sorted!=null) {
                    int i = 5;
                    int j = 0;
                    foreach (var v in sorted)
                    {
                        i++;
                        j++;
                        worKsheeT.Cells[i, 1] = j.ToString();
                        worKsheeT.Cells[i, 2] = v.Date;
                        worKsheeT.Cells[i, 3] = v.Voucher;
                        worKsheeT.Cells[i, 4] = v.VNo;
                        if (v.DrAccount != null) worKsheeT.Cells[i, 5] = v.DrAccount.Name;
                        worKsheeT.Cells[i, 6] = v.Dr_Amount;
                        worKsheeT.Cells[i, 7] = v.Cr_Amount;
                    }
                    Microsoft.Office.Interop.Excel.Range xLSRanges = worKsheeT.Range["A5", "G" + i];
                    Microsoft.Office.Interop.Excel.Borders border = xLSRanges.Borders;
                    border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    border.Weight = 2D;

                    xLSRanges = worKsheeT.Range["E6", "G" + i];
                    xLSRanges.NumberFormat = "0.00";
                    xLSRanges = worKsheeT.Range["A5", "G5"];
                    xLSRanges.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbLightBlue;

                    i++;
                    int frange_start = i;
                    worKsheeT.Cells[i, 2] = "Total";
                    var crt = (from cr in sorted.AsEnumerable() select Convert.ToDouble(cr.Cr_Amount)).Sum();
                    worKsheeT.Cells[i, 7] = crt;
                    var drt = (from cr in sorted.AsEnumerable() select Convert.ToDouble(cr.Dr_Amount)).Sum();
                    worKsheeT.Cells[i, 6] = drt;
                    xLSRanges = worKsheeT.Range["E" + frange_start, "g" + frange_start];
                    xLSRanges.NumberFormat = "0.00";
                    i++;
                    worKsheeT.Cells[i, 2] = "Balance";
                    double b = 0;
                    if (crt > drt)
                    {
                        b = crt - drt;
                        worKsheeT.Cells[i, 7] = b;
                    }
                    else if (drt > crt)
                    {
                        b = drt - crt;
                        worKsheeT.Cells[i, 6] = b;
                    }
                    else
                    {
                        worKsheeT.Cells[i, 6] = 0.00;
                    }

                    xLSRanges = worKsheeT.Range["E" + frange_start, "G" + i];
                    xLSRanges.NumberFormat = "0.00";
                    xLSRanges = worKsheeT.Range["A" + frange_start, "G" + i];
                    xLSRanges.Font.Bold = true;
                    //xLSRanges = worKsheeT.Range["C" + i, "C" + i];
                    //xLSRanges.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                }
                workbook.SaveAs(fname);
                workbook.Close();
                Process.Start("EXCEL", fname);
            }
            catch (Exception e3)
            {

                MessageBox.Show(e3.Message.ToString());
            }
        }

        private void btn_xps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wordFname == null)
                {
                    wordFname = GenerateWordDoc();
                }
                if (xpsFname == null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }
                if (xpsFname != null) Process.Start("explorer.exe", xpsFname);
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }
        }
        void ClearReportcache()
        {
            try
            {
                xpsFname = null;
                pdfFname = null;
                wordFname = null;
            }
            catch (Exception)
            {

                throw;
            }
        }
        void DrawCashBook()
        {
            try
            {
                lblcr.Text = "";
                lbldr.Text = "";

                lblbalance.Text = "";

                report = ViewModels_Variables.ModelViews.CashBook.ToList();
                var cash = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.Parent.Name == "CASH").FirstOrDefault();

                if (report != null && report.Count > 0 && ViewModels_Variables.ModelViews.Accounts != null && ViewModels_Variables.ModelViews.Accounts.Count > 0)
                {
                    if (cmb_ACCOUNTS.SelectedItem != null)
                    {
                        var slctd = (Model.AccountModel)cmb_ACCOUNTS.SelectedItem;
                        report = (report.Where((x) => x.Date >= dtp_from.SelectedDate && x.Date <= dtp_to.SelectedDate && x.CrAccount.ID == slctd.ID)).ToList();
                        var b1 = DB.Connection.GetDrCr(slctd.ID);
                        double b = 0;
                        if (b1["Cr"] > b1["Dr"]) { b = b1["Cr"] - b1["Dr"]; lblbalance.Text = "Cr " + b.ToString("0.00"); }
                        else if (b1["Dr"] > b1["Cr"]) { b = b1["Dr"] - b1["Cr"]; lblbalance.Text = "Dr " + b.ToString("0.00"); }
                        lblcr.Text = "Cr " + b1["Cr"].ToString("0.00"); lbldr.Text = "Dr " + b1["Dr"].ToString("0.00");

                        //OB
                        var ob = DB.Connection.GetOBDrCr(dtp_from.SelectedDate.Value, slctd.ID);
                        Model.CashBookModel obmodel = new Model.CashBookModel();
                        if (ob["Cr"] > 0 || ob["Dr"] > 0)
                        {
                            obmodel.Voucher = "OB";
                            if (ob["Dr"] > 0) obmodel.Cr_Amount = ob["Dr"];
                            else if (ob["Cr"] > 0) obmodel.Dr_Amount = ob["Cr"];
                            obmodel.Date = dtp_from.SelectedDate.Value.AddDays(-1);
                            report.Insert(0, obmodel);
                        }


                    }
                    else
                    {
                        report = (report.Where((x) => x.Date >= dtp_from.SelectedDate && x.Date <= dtp_to.SelectedDate)).ToList();


                    }
                    //var cr = (from c in report select c.Cr_Amount).Sum();
                    //var dr = (from c in report select c.Dr_Amount).Sum();
                    //var b = (from c in report select c.Opening).Sum();
                    //lblcr.Text = cr.ToString("0.00");
                    //lbldr.Text = dr.ToString("0.00");
                    //var g = (from x in report select x.CrAccount.Parent).Distinct();
                    //if (g!=null)cmb_groups.ItemsSource = g;

                    rep_grid.ItemsSource = report;
                    //lblbalance.Text = report.Sum((s) => s.Dr_Amount);

                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dtp_from.SelectedDate = ViewModels_Variables._sysDate[0];
                dtp_to.SelectedDate = DateTime.Now;
                if (ViewModels_Variables.ModelViews.AccountTransactions == null && ViewModels_Variables.ModelViews.AccountTransactions.Count <= 0)
                {
                    ViewModels_Variables.ModelViews.Trans_To_list();
                }
                ViewModels_Variables.ModelViews.RefreshCashBook();
                var cash = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.Parent.Name == "CASH IN HAND").FirstOrDefault();
                if (cash == null) cash = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.Parent.Name == "CASH").FirstOrDefault();
                if (cash != null && cmb_ACCOUNTS.SelectedItem == null) cmb_ACCOUNTS.SelectedItem = cash;
                DataContext = ViewModels_Variables.ModelViews;
                loaded = true;
                DrawCashBook();
                ClearReportcache();

            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void dtp_from_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ClearReportcache();
                if (loaded == true) DrawCashBook();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void dtp_to_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ClearReportcache();
                if (loaded == true) DrawCashBook();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void cmb_ACCOUNTS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ClearReportcache();
                if (loaded == true) DrawCashBook();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void rep_grid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {

        }
    }

}
