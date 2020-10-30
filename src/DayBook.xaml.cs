
using accounts.Model;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Xps.Packaging;
using Xceed.Document.NET;
using Xceed.Words.NET;
using DataTable = System.Data.DataTable;

namespace accounts
{
    /// <summary>
    /// Interaction logic for DayBook.xaml
    /// </summary>
    public partial class DayBook : System.Windows.Window
    {
        List<Model.CashBookModel> dbook;
        ObservableCollection<CashBookModel> ob_cash = new ObservableCollection<CashBookModel>();
        ListCollectionView rview = null;
        ListCollectionView colletionview;
    
        DataTable rsource = new DataTable();
        DataTable pvtble = new DataTable();
        DataTable rvtble = new DataTable();
        List<double> oblist = new List<double>();
        string wordFname = null;
        string xpsFname = null;
        string pdfFname = null;
        void EntryWiseDayBook()
        {
            try
            {
                var receipt = (from r in ViewModels_Variables.ModelViews.DayBook where r.Voucher == "RECEIPT" && r.Date >= dtp_from.SelectedDate && r.Date <= dtp_to.SelectedDate select r.Dr_Amount).Sum();
                var payment = (from r in ViewModels_Variables.ModelViews.DayBook where r.Voucher == "PAYMENT" && r.Date >= dtp_from.SelectedDate && r.Date <= dtp_to.SelectedDate select r.Cr_Amount).Sum();
                var journal = (from r in ViewModels_Variables.ModelViews.DayBook where r.Voucher == "JOURNAL" && r.Date >= dtp_from.SelectedDate && r.Date <= dtp_to.SelectedDate select r.Dr_Amount).Sum();
                var journal1 = (from r in ViewModels_Variables.ModelViews.DayBook where r.Voucher == "JOURNAL" && r.Date >= dtp_from.SelectedDate && r.Date <= dtp_to.SelectedDate select r.Cr_Amount).Sum();
                var bpayment = (from r in ViewModels_Variables.ModelViews.DayBook where r.Voucher == "BANK PAYMENT" && r.Date >= dtp_from.SelectedDate && r.Date <= dtp_to.SelectedDate select r.Cr_Amount).Sum();
                var breciept = (from r in ViewModels_Variables.ModelViews.DayBook where r.Voucher == "BANK RECEIPT" && r.Date >= dtp_from.SelectedDate && r.Date <= dtp_to.SelectedDate select r.Dr_Amount).Sum();

                var receipt1 = (from r in ViewModels_Variables.ModelViews.DayBook where r.Voucher == "RECEIPT" && r.Date < dtp_from.SelectedDate select r.Dr_Amount).Sum();
                var payment1 = (from r in ViewModels_Variables.ModelViews.DayBook where r.Voucher == "PAYMENT" && r.Date < dtp_from.SelectedDate select r.Cr_Amount).Sum();
                var journal2 = (from r in ViewModels_Variables.ModelViews.DayBook where r.Voucher == "JOURNAL" && r.Date < dtp_from.SelectedDate select r.Dr_Amount).Sum();
                var journal3 = (from r in ViewModels_Variables.ModelViews.DayBook where r.Voucher == "JOURNAL" && r.Date < dtp_from.SelectedDate select r.Cr_Amount).Sum();
                var bpayment1 = (from r in ViewModels_Variables.ModelViews.DayBook where r.Voucher == "BANK PAYMENT" && r.Date < dtp_from.SelectedDate select r.Cr_Amount).Sum();
                var breciept1 = (from r in ViewModels_Variables.ModelViews.DayBook where r.Voucher == "BANK RECEIPT" && r.Date < dtp_from.SelectedDate select r.Dr_Amount).Sum();

                colletionview = new ListCollectionView(ViewModels_Variables.ModelViews.CashBook);
                var tcr_ob = colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, r1) => c.Date < dtp_from.SelectedDate.Value).Aggregate<CashBookModel, double>(0, (total, s) => total += s.Cr_Amount);
                var tdr_ob = colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, r1) => c.Date < dtp_from.SelectedDate.Value).Aggregate<CashBookModel, double>(0, (total, s) => total +=  s.Dr_Amount);
                 

                double ob;
                IEnumerable<CashBookModel> coll = null;
                CashBookModel obmodel = new CashBookModel();

                if (tcr_ob > tdr_ob)
                {
                    ob = (tcr_ob - tdr_ob);
                    obmodel = new CashBookModel();
                    obmodel.Cr_Amount = ob;
                    obmodel.Voucher = "OB";
                    obmodel.Date = dtp_from.SelectedDate.Value;


                }
                else if (tcr_ob < tdr_ob)
                {
                    ob = (tdr_ob - tcr_ob);
                    obmodel = new CashBookModel();
                    obmodel.Dr_Amount = ob;
                    obmodel.Voucher = "OB";
                    obmodel.Date = dtp_from.SelectedDate.Value;
                }
                else
                {
                    ob = 0;
                }

                List<CashBookModel> items = new List<CashBookModel>();
                if (obmodel != null) items.Add(obmodel);
                CashBookModel dbitems = new CashBookModel()
                {
                    Voucher = "RECEIPTS",
                    Dr_Amount = receipt,
                };
                items.Add(dbitems);
                dbitems = new CashBookModel()
                {
                    Voucher = "PAYMENTS",
                    Cr_Amount = payment,
                };
                items.Add(dbitems);

                dbitems = new CashBookModel()
                {
                    Voucher = "JOURNALS",
                    Dr_Amount = journal,
                    Cr_Amount = journal1,
                };
                items.Add(dbitems);
                dbitems = new CashBookModel()
                {
                    Voucher = "BANK RECEIPTS",
                    Dr_Amount = breciept,
                };
                items.Add(dbitems);
                dbitems = new CashBookModel()
                {
                    Voucher = "BANK PAYMETS",
                    Cr_Amount = bpayment,
                };
                items.Add(dbitems);
                double b = (receipt + breciept + journal + tdr_ob) - (payment + bpayment + journal1 + ob);
                ListCollectionView dview = new ListCollectionView(items);

                if (colletionview.Count > 0)
                {


                    var x = colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, r) => c.Date >= dtp_from.SelectedDate.Value).Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Dr_Amount));
                    var y = colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, r) => c.Date >= dtp_from.SelectedDate.Value).Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Cr_Amount));

                    string z;
                    double am = 0;
                    if (x > y)
                    {

                        z = " Dr";
                        am = (x - y);

                    }
                    else if (x < y)
                    {

                        z = " Cr";
                        am = y - x;
                    }
                    else
                    {

                        z = "0.00";
                        am = 0;
                    }
                    lblbalance.Text = string.Format("{0:0.00}", Convert.ToDouble(am)) + z;
                    lblcr.Text = string.Format("{0:0.00}", Convert.ToDouble(y)) + " Cr.";
                    lbldr.Text = string.Format("{0:0.00}", Convert.ToDouble(x)) + " Dr.";

                }


                rep_grid.ItemsSource = dview;
            }
            catch (Exception)
            {

                throw;
            }
        }
        bool loaded = false;
        public DayBook()
        {
            InitializeComponent();
            dtp_from.SelectedDate = public_members._sysDate[0];
            dtp_to.SelectedDate = DateTime.Now.Date; ;
            loaded = false;
        }
        
       
         
        private void rep_grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //try
            //{
            //    var cur = ((CashBookModel)rep_grid.CurrentItem);

            //    if (cur != null && cur.VNo.Length > 0)
            //    {
            //        if (cur.Voucher != null && cur.Voucher.Length > 0)
            //        {
            //            switch (cur.Voucher)
            //            {
            //                case "RECEIPT":
            //                    Receipt rform = new Receipt();
            //                    rform.Owner = this;
            //                    rform.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //                    rform.Show();
            //                    rform.Find(int.Parse(cur.VNo.ToString()));
            //                    break;
            //                case "PAYMENT":
            //                    payment pform = new payment();
            //                    pform.Owner = this;
            //                    pform.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //                    pform.Show();
            //                    pform.Find(int.Parse(cur.VNo.ToString()));
            //                    break;
            //                case "JOURNAL":
            //                    journalposting jform = new journalposting();
            //                    jform.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //                    jform.Show();
            //                    jform.Find(int.Parse(cur.VNo.ToString()));
            //                    break;
            //                case "BANK RECEIPT":
            //                    bankReceipt br = new bankReceipt();
            //                    br.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //                    br.Show();
            //                    br.Find(int.Parse(cur.VNo.ToString()));
            //                    break;
            //                case "BANK PAYMENT":
            //                    BankPayment bp = new BankPayment();
            //                    bp.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //                    bp.Show();
            //                    bp.Find(int.Parse(cur.VNo.ToString()));
            //                    break;
            //            }
            //        }
            //    }
            //}
            //catch (Exception ee)
            //{
            //    MessageBox.Show(ee.Message.ToString());
            //}
        }

        private void cmb_ACCOUNTS_KeyDown(object sender, KeyEventArgs e)
        {
            

        }

         

        private void refresh_data_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Window_Loaded(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
            
        }

        private void dtp_from_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (cmb_ACCOUNTS.SelectedItem == null     )
                {
                    FetchDayBook();
                }
                else
                {
                    FetchDayBook(((Model.AccountModel)cmb_ACCOUNTS.SelectedItem).ID);
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void dtp_to_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (cmb_ACCOUNTS.SelectedItem == null)
                {
                    FetchDayBook();
                }
                else
                {
                    FetchDayBook(((Model.AccountModel)cmb_ACCOUNTS.SelectedItem).ID);
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

        private void cmb_groups_KeyDown(object sender, KeyEventArgs e)
        {
            
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
                xps = new XpsDocument(xpsFname, FileAccess.Read);
                accounts.PrintDialogue print = new PrintDialogue(xps, this.Title + " : " + cmb_ACCOUNTS.Text.ToString());
                print.Show();
                xps.Close();
            }
            catch (Exception w1)
            {
                MessageBox.Show(w1.Message.ToString());
            }
        }
        string GenerateWordDoc( )
        {
            string fname = null;
            try
            {

                fname = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\Doc");
                using (var document = DocX.Create(fname))
                {
                    document.ApplyTemplate(@"doctemplates/DayBook.dotx");
                    //Footer
                    //document.ReplaceText("[MyCompany]", public_members._company[0].company);
                    //document.ReplaceText("[Clandmark]", " | " + public_members._company[0].lmark);
                    //document.ReplaceText("[CCity]", " | " + public_members._company[0].place);
                    //document.ReplaceText("[CPhone]", "Phone :" + public_members._company[0].officeno);

                    document.ReplaceText("[Sdate]", dtp_from.SelectedDate.Value.ToShortDateString());
                    document.ReplaceText("[Edate]", dtp_to.SelectedDate.Value.ToShortDateString());
                    document.ReplaceText("[ReportDate]", public_members._sysDate[0].ToShortDateString());

                    var t = document.Tables;

                    var sorted = colletionview.SourceCollection.Cast<CashBookModel>().ToList();
                    { 

                        Row newrow;
                        int i = 1;
                        foreach (var v in sorted)
                        {
                            ++i;
                            newrow = t[2].InsertRow();
                            newrow.Cells[0].Paragraphs[0].Append(v.Date.ToShortDateString());
                            if (v.Voucher != null) { newrow.Cells[1].Paragraphs[0].Append(v.Voucher); } else { newrow.Cells[1].Paragraphs[0].Append(v.DrAccount.Name); }
                            newrow.Cells[2].Paragraphs[0].Append(v.Dr_Amount.ToString()).Alignment = Alignment.right;
                            newrow.Cells[3].Paragraphs[0].Append(v.Cr_Amount.ToString()).Alignment = Alignment.right;
                            t[2].Rows.Add(newrow);
                        }

                        newrow = t[2].InsertRow();
                        //newrow.Cells[0].Paragraphs[0].Append("");
                        //newrow.Cells[1].Paragraphs[0].Append("");
                        //newrow.Cells[2].Paragraphs[0].Append("");
                        //newrow.Cells[3].Paragraphs[0].Append("");
                        t[2].Rows.Add(newrow);

                        //var b = public_members.GetActBalance(dtp_to.SelectedDate.Value , public_members.Ledgerid(cmb_ACCOUNTS.Text.ToString()));

                        var x = sorted.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Dr_Amount));
                        var y = sorted.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Cr_Amount));

                        newrow = t[2].InsertRow();
                        newrow.Cells[1].Paragraphs[0].Append("Total").Bold(true);
                        newrow.Cells[2].Paragraphs[0].Append(string.Format("{0:0.00}", x)).Bold(true).Alignment=Alignment.right;
                        newrow.Cells[3].Paragraphs[0].Append(string.Format("{0:0.00}", y)).Bold(true).Alignment = Alignment.right;
                        t[1].Rows.Add(newrow);

                        newrow = t[2].InsertRow();
                        newrow.Cells[1].Paragraphs[0].Append("");
                        
                        newrow.Cells[3].Paragraphs[0].Append(lblbalance.Text.ToString()).Bold(true).Alignment=Alignment.right;
                        t[2].Rows.Add(newrow);
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

                Process.Start("AcroRd32.exe", pdfFname);

            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void btn_doc_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (wordFname == null)
                {
                    wordFname = GenerateWordDoc();
                }
                Process.Start("WINWORD.exe", wordFname);



            }
            catch (Exception r)
            {

                MessageBox.Show(r.Message.ToString());
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
                Process.Start("explorer.exe", xpsFname);
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

             
                string fileName = public_members.GnenerateFileName( public_members.reportPath + @"\BookKeeper\workbook" , ".xlsx");

                var app = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook workbook;
                workbook = app.Workbooks.Add(Type.Missing);
                var worKsheeT = (Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet;
                worKsheeT.Range[worKsheeT.Cells[1, 1], worKsheeT.Cells[1, 8]].Merge();
                worKsheeT.Cells[1, 1] = "Day Book";

                var range = (Range)worKsheeT.Cells[1, 1];
                range.Font.Bold = true;
                range.Font.Size = 25;
                range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                worKsheeT.Name = "CashBook";

                worKsheeT.Range[worKsheeT.Cells[3, 1], worKsheeT.Cells[3, 8]].Merge();
                worKsheeT.Cells[3, 1] = "Daybook " +  " on " + dtp_from.SelectedDate.Value.ToShortDateString() + " - " + dtp_to.SelectedDate.Value.ToShortDateString();
                worKsheeT.Cells.Font.Size = 12;

                //Table Heading 
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

                var sorted = colletionview.Cast<CashBookModel>();
                {
                    int i = 5;
                    int j = 0;
                    foreach (var v in sorted)
                    {
                        i++;
                        j++;
                        worKsheeT.Cells[i, 1] = j.ToString();
                        worKsheeT.Cells[i, 2] = v.Date;
                        worKsheeT.Cells[i, 3] = v.Voucher;
                       if(v.VNo>0) worKsheeT.Cells[i, 4] = v.VNo;
                        if(v.DrAccount!=null)worKsheeT.Cells[i, 5] = v.DrAccount.Name;
                        worKsheeT.Cells[i, 6] = v.Dr_Amount.ToString("0.00");
                        worKsheeT.Cells[i, 7] = v.Cr_Amount.ToString("0.00");
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
                File.Delete(fileName);
                workbook.SaveAs(  fileName);

                workbook.Close();

                Process.Start("EXCEL", fileName);


            }
            catch (Exception e3)
            {

                MessageBox.Show(e3.Message.ToString());
            }
        }

        private void dtp_from_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ClearReportcache();
            if (cmb_ACCOUNTS.SelectedItem == null)
            {
                if (loaded == true) FetchDayBook();
            }
            else
            {
                if (loaded == true) FetchDayBook(((Model.AccountModel)cmb_ACCOUNTS.SelectedItem).ID);
            }
        }

        private void dtp_to_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ClearReportcache();
            if (cmb_ACCOUNTS.SelectedItem == null)
            {
                if (loaded == true) FetchDayBook();
            }
            else
            {
                if (loaded == true) FetchDayBook(((Model.AccountModel)cmb_ACCOUNTS.SelectedItem).ID);
            }
        }

        private void cmb_ACCOUNTS_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                ClearReportcache();
                if (cmb_ACCOUNTS.SelectedItem == null)
                {
                    if (loaded == true) FetchDayBook();
                }
                else
                {
                    if (loaded == true) FetchDayBook(((Model.AccountModel)cmb_ACCOUNTS.SelectedItem).ID);
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }

        }

        private void cmb_groups_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
           
             
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModels_Variables.ModelViews.RefreshDaybook();
                
                var cash = ViewModels_Variables.ModelViews.Accounts.Where((w) => w.Parent.Name == "CASH");
                if(cash.Count()<=0) cash = ViewModels_Variables.ModelViews.Accounts.Where((w) => w.Parent.Name == "CASH IN HAND");
                
                cmb_ACCOUNTS.ItemsSource = cash;
                if (cmb_ACCOUNTS.Items.Count > 0) cmb_ACCOUNTS.SelectedIndex = 0;
                loaded = true;
                FetchDayBook(cash.FirstOrDefault().ID);
                ClearReportcache();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        private void FetchDayBook(int cid = 0)
        {
            try
            {
                
                if (cid != 0) { dbook = (from g1 in ViewModels_Variables.ModelViews.DayBook where g1.DrAccount.ID == cid select g1).ToList(); }
                else { dbook = ViewModels_Variables.ModelViews.DayBook.ToList(); }
                var groups = (from g1 in dbook select g1.CrAccount).Distinct();
                cmb_groups.ItemsSource = groups;

                colletionview = new ListCollectionView(dbook);
                var tcr_ob = colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, r) => c.Date < dtp_from.SelectedDate.Value).Aggregate<CashBookModel, double>(0, (total, s) => total +=  s.Cr_Amount);
                var tdr_ob = colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, r) => c.Date < dtp_from.SelectedDate.Value).Aggregate<CashBookModel, double>(0, (total, s) => total +=  s.Dr_Amount);

                colletionview.Filter = (e1) =>
                {
                    CashBookModel emp1 = e1 as CashBookModel;
                    if (((Convert.ToDateTime(emp1.Date) >= dtp_from.SelectedDate && Convert.ToDateTime(emp1.Date) <= dtp_to.SelectedDate) || emp1.Voucher == "OB CASH"))
                        return true;
                    return false;
                };

                var sorted = colletionview.Cast<CashBookModel>().ToList<CashBookModel>();
                // var sorted =FuncOrderBy(colletionview, "Date");

                double ob;
                IEnumerable<CashBookModel> coll = null;
                CashBookModel obmodel = new CashBookModel();

                if (tcr_ob > tdr_ob)
                {
                    ob = (tcr_ob - tdr_ob);
                    obmodel = new CashBookModel();
                    obmodel.Cr_Amount =  ob;
                    obmodel.Voucher = "OB";
                    obmodel.Date = dtp_from.SelectedDate.Value;
                    List<CashBookModel> oblist = new List<CashBookModel>();
                    oblist.Add(obmodel);
                    coll = oblist;
                    sorted.InsertRange(0, coll);
                }
                else if (tcr_ob < tdr_ob)
                {
                    ob = (tdr_ob - tcr_ob);
                    obmodel = new CashBookModel();
                    obmodel.Dr_Amount =ob;
                    obmodel.Voucher = "OB";
                    obmodel.Date = dtp_from.SelectedDate.Value;
                    List<CashBookModel> oblist = new List<CashBookModel>();
                    oblist.Add(obmodel);
                    coll = oblist;
                    sorted.InsertRange(0, coll);
                }
                else
                {
                    ob = 0;
                }

                colletionview = new ListCollectionView(sorted);
               
                if (colletionview.Count > 0)
                {


                    var x = colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, r) => c.Date >= dtp_from.SelectedDate.Value).Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Dr_Amount));
                    var y = colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, r) => c.Date >= dtp_from.SelectedDate.Value).Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Cr_Amount));

                    string z;
                    double am = 0;
                    if (x > y)
                    {

                        z = " Dr";
                        am = (x - y);

                    }
                    else if (x < y)
                    {

                        z = " Cr";
                        am = y - x;
                    }
                    else
                    {

                        z = "0.00";
                        am = 0;
                    }
                    if (am > 0) { lblbalance.Text = am + z; }
                    else { lblbalance.Text = ""; }
                    lblcr.Text =  y.ToString("0.00")+ " Cr.";
                    lbldr.Text = x.ToString("0.00") + " Dr.";

                }

                rep_grid.ItemsSource = colletionview;
            }
            catch (SqlException ee)
            {
                MessageBox.Show(ee.Message, ToString());
            }

        }

    }
}
