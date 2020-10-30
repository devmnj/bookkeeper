using accounts.Model;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Xps.Packaging;
using Xceed.Document.NET;

namespace accounts
{
    /// <summary>
    /// Interaction logic for bpReport.xaml
    /// </summary>
    public partial class bpReport : System.Windows.Window
    {

        IEnumerable<Model.BankPaymentModel> report;
        string wordFname = null;
        string xpsFname = null;
        string pdfFname = null;
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
        public bpReport()
        {
            InitializeComponent();
            dtp_from.SelectedDate = public_members._sysDate[0];
            dtp_to.SelectedDate = public_members._sysDate[0];

            
        }
 
        private void cmb_ACCOUNTS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                 
            }
        }


        private void cmb_orderby_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                
            }
            catch (SqlException rr)
            {
                MessageBox.Show(rr.Message.ToString());
            }
        }

        private void refresh_data_Click(object sender, RoutedEventArgs e)
        {
             
        }

        private void rep_grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (rep_grid.CurrentColumn == rep_grid.Columns[1])
                {
                    var cur = (Model.BankPaymentModel)rep_grid.CurrentItem;

                    if (cur != null)
                    {

                        BankPayment rform = new  BankPayment();
                        rform.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        rform.Show();
                        rform.Find(cur.pno);
                        //    //MessageBox.Show(cur.Text.ToString());




                    }
                }
            }
            catch (Exception er)
            {

            }
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

        private void dtp_from_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    
                }
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void dtp_to_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    
                }
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmb_status_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    
                }
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void rep_grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            XpsDocument xps = null;

            try
            {
                if (wordFname == null)
                {
                    wordFname = CreateWordDoc();
                }
                if (xpsFname == null && wordFname!=null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }
                if (xpsFname != null)
                {
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
        string CreateWordDoc()
        {
            string fname = null;
            try
            {
                if (report != null) { 
                fname = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\Doc");

                using (var document = Xceed.Words.NET.DocX.Create(fname))
                {
                    document.ApplyTemplate(@"doctemplates/bankpaymentReport.dotx");
                    //Footer
                    //document.ReplaceText("[MyCompany]", public_members._company[0].company);
                    //document.ReplaceText("[Clandmark]", " | " + public_members._company[0].lmark);
                    //document.ReplaceText("[CCity]", " | " + public_members._company[0].place);
                    //document.ReplaceText("[CPhone]", "Phone :" + public_members._company[0].officeno);

                    document.ReplaceText("[SDate]", dtp_from.SelectedDate.Value.ToShortDateString());
                    document.ReplaceText("[EDate]", dtp_to.SelectedDate.Value.ToShortDateString());
                    document.ReplaceText("[ReportDate]", public_members._sysDate[0].ToShortDateString());

                    if (cmb_ACCOUNTS.SelectedItem != null )
                    {
                        document.ReplaceText("[Account]", ((Model.AccountModel)cmb_ACCOUNTS.SelectedItem).Name);
                    }

                    else
                    {
                        document.ReplaceText("[Account]", "");
                    }
                    var t = document.Tables;

                    var sorted = report;
                    {

                        Row newrow;
                        int i = 1;
                        foreach (var v in sorted)
                        {
                            ++i;
                            newrow = t[1].InsertRow();
                            newrow.Cells[0].Paragraphs[0].Append(v.Date.ToShortDateString());
                            newrow.Cells[1].Paragraphs[0].Append(v.pno.ToString());
                            if (cmb_ACCOUNTS.SelectedItem != null  ) { newrow.Cells[2].Paragraphs[0].Append(v.DrAccount.Name); } else { newrow.Cells[2].Paragraphs[0].Append(v.DrAccount.Name); }
                                newrow.Cells[3].Paragraphs[0].Append(v.DiscAmount.ToString("0.00")).Alignment = Alignment.right;
                                newrow.Cells[5].Paragraphs[0].Append(v.Amount.ToString("0.00")).Alignment = Alignment.right;
                                newrow.Cells[4].Paragraphs[0].Append(v.BankCharge.ToString("0.00")).Alignment = Alignment.right;
                                t[1].Rows.Add(newrow);

                            if (v.Invno != null && v.Invno.Length > 0)
                            {
                                newrow = t[1].InsertRow();
                                newrow.Cells[1].Paragraphs[0].Append("InvNo :" + v.Invno);
                                t[1].Rows.Add(newrow);
                                newrow = t[1].InsertRow();
                                newrow.Cells[1].Paragraphs[0].Append("Inv balance :" + v.InvBalance).Color(System.Drawing.Color.DarkRed);
                                t[1].Rows.Add(newrow);
                                newrow = t[1].InsertRow();
                                newrow.Cells[1].Paragraphs[0].Append(v.CheqDate.ToShortDateString() + "," + v.Type + "," + v.CheqNo.ToUpper() + "," + v.status);
                                t[1].Rows.Add(newrow);
                            }
                        }

                        newrow = t[1].InsertRow();
                        t[1].Rows.Add(newrow);
                        
                        var x = sorted.Cast<BankPaymentModel>().Aggregate<BankPaymentModel, double>(0, (total, s) => total += Convert.ToDouble(s.Amount));
                        var y = sorted.Cast<BankPaymentModel>().Aggregate<BankPaymentModel, double>(0, (total, s) => total += Convert.ToDouble(s.DiscAmount));
                        var z = sorted.Cast<BankPaymentModel>().Aggregate<BankPaymentModel, double>(0, (total, s) => total += Convert.ToDouble(s.BankCharge));


                        newrow = t[1].InsertRow();
                        newrow.Cells[1].Paragraphs[0].Append("Total").Bold(true);
                        newrow.Cells[4].Paragraphs[0].Append(string.Format("{0:0.00}", y)).Bold(true).Alignment = Alignment.right;
                        newrow.Cells[5].Paragraphs[0].Append(string.Format("{0:0.00}", x)).Bold(true).Alignment = Alignment.right;
                        t[1].Rows.Add(newrow);
                        newrow.Cells[3].Paragraphs[0].Append(string.Format("{0:0.00}", z)).Bold(true).Alignment = Alignment.right;
                        t[1].Rows.Add(newrow);

                        document.Save();
                        document.Dispose();
                    }
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
                    wordFname = CreateWordDoc();
                }
               if (wordFname!=null) Process.Start("WINWORD.exe", wordFname);
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

               
                string fileName = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\workbook",".xlsx");

                var app = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook workbook;
                workbook = app.Workbooks.Add(Type.Missing);
                var worKsheeT = (Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet;
                worKsheeT.Range[worKsheeT.Cells[1, 1], worKsheeT.Cells[1, 3]].Merge();
                worKsheeT.Cells[1, 1] = "Bank Payment Report";

                var range = (Range)worKsheeT.Cells[1, 1];
                range.Font.Bold = true;
                range.Font.Size = 25;
                range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                worKsheeT.Name = "Bank Payment Report";

                worKsheeT.Range[worKsheeT.Cells[3, 1], worKsheeT.Cells[3, 8]].Merge();

                if (cmb_ACCOUNTS.Text.Length > 0) { worKsheeT.Cells[3, 1] = "Bank Payment Report of " + cmb_ACCOUNTS.Text.ToString() + "As on " + public_members._sysDate[0].ToShortDateString(); }
                else { worKsheeT.Cells[3, 1] = "Bank Payment Report " + "As on " + public_members._sysDate[0].ToShortDateString(); }


                worKsheeT.Cells.Font.Size = 12;

                //Table Heading 
                worKsheeT.Cells[5, 1] = "SLNO";
                worKsheeT.Cells[5, 1].Font.Size = 13;

                worKsheeT.Cells[5, 2] = "Date";
                worKsheeT.Cells[5, 2].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 2]).EntireColumn.ColumnWidth = 15;

                worKsheeT.Cells[5, 3] = "V.NO";
                worKsheeT.Cells[5, 3].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 3]).EntireColumn.ColumnWidth = 10;

                worKsheeT.Cells[5, 4] = "J.Inv";
                worKsheeT.Cells[5, 4].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 4]).EntireColumn.ColumnWidth = 10;

                worKsheeT.Cells[5, 5] = "Cheq/DD No";
                worKsheeT.Cells[5, 5].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 5]).EntireColumn.ColumnWidth = 20;

                worKsheeT.Cells[5, 6] = "Status";
                worKsheeT.Cells[5, 6].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 6]).EntireColumn.ColumnWidth = 10;

                worKsheeT.Cells[5, 7] = "Account";
                worKsheeT.Cells[5, 7].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 7]).EntireColumn.ColumnWidth = 40;

                worKsheeT.Cells[5, 8] = "Bank Charge";
                worKsheeT.Cells[5, 8].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 8]).EntireColumn.ColumnWidth = 15;

                worKsheeT.Cells[5, 9] = "Discount";
                worKsheeT.Cells[5, 9].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 9]).EntireColumn.ColumnWidth = 15;

                worKsheeT.Cells[5, 10] = "Amount";
                worKsheeT.Cells[5, 10].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 10]).EntireColumn.ColumnWidth = 15;



                var sorted = report;
                {
                    int i = 5;
                    int j = 0;
                    foreach (var v in sorted)
                    {
                        i++;
                        j++;
                        worKsheeT.Cells[i, 1] = j.ToString();
                        worKsheeT.Cells[i, 2] = v.Date.ToShortDateString();
                        worKsheeT.Cells[i, 3] = v.pno.ToString();
                        worKsheeT.Cells[i, 4] = v.Invno;
                        worKsheeT.Cells[i, 5] = v.CheqNo;
                        worKsheeT.Cells[i, 6] = v.status;
                        worKsheeT.Cells[i, 7] = v.CrAccount.Name;
                        worKsheeT.Cells[i, 8] = v.BankCharge;
                        worKsheeT.Cells[i, 9] = v.DiscAmount;
                        worKsheeT.Cells[i, 10] = v.Amount;

                    }
                    Microsoft.Office.Interop.Excel.Range xLSRanges = worKsheeT.Range["A5", "j" + i];
                    Microsoft.Office.Interop.Excel.Borders border = xLSRanges.Borders;
                    border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    border.Weight = 2D;

                    xLSRanges = worKsheeT.Range["g6", "h" + i];
                    xLSRanges.NumberFormat = "0.00";
                    xLSRanges = worKsheeT.Range["g6", "h" + i];
                    xLSRanges.NumberFormat = "0.00";
                    xLSRanges = worKsheeT.Range["A5", "j5"];
                    xLSRanges.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbLightBlue;

                    i++;
                    int frange_start = i;
                    worKsheeT.Cells[i, 2] = "Total";
                    var crt = (from cr in sorted.AsEnumerable() select Convert.ToDouble(cr.Amount)).Sum();
                    worKsheeT.Cells[i, 10] = crt;
                    crt = (from cr in sorted.AsEnumerable() select Convert.ToDouble(cr.DiscAmount)).Sum();
                    worKsheeT.Cells[i, 9] = crt;

                    xLSRanges = worKsheeT.Range["A" + frange_start, "j" + i];
                    xLSRanges.Font.Bold = true;
                    xLSRanges = worKsheeT.Range["E" + frange_start, "j" + i];
                    xLSRanges.NumberFormat = "0.00";

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

        private void btn_pdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (wordFname == null)
                {
                    wordFname = CreateWordDoc();
                }
                if (pdfFname == null && wordFname!=null)
                {
                    pdfFname = ReportExport.WordExport.CreatePDF(wordFname);
                }

              if(pdfFname!=null)  Process.Start("AcroRd32.exe", pdfFname);

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
                if (xpsFname == null && wordFname!=null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }
              if(xpsFname!=null)  Process.Start("explorer.exe", xpsFname);
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
                dtp_to.SelectedDate = DateTime.Now.Date;
                dtp_from.SelectedDate = public_members._sysDate[0];


                if (ViewModels_Variables.ModelViews.BankPayments.Count <= 0)
                {
                    DB.BankPayment.Fetch();
                    ViewModels_Variables.ModelViews.BPayments_To_List();
                }
                this.DataContext = ViewModels_Variables.ModelViews;
                var accs = (from x in ViewModels_Variables.ModelViews.BankPayments select x.DrAccount).Distinct();
                cmb_ACCOUNTS.ItemsSource = accs;
                rep_grid.ItemsSource = report;
                Refresh();

            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }
        void Refresh()
        {
            try
            {
                report = ViewModels_Variables.ModelViews.BankPayments;
                ClearReportcache();
                if (ViewModels_Variables.ModelViews.BankPayments != null && ViewModels_Variables.ModelViews.BankPayments.Count > 0 && ViewModels_Variables.ModelViews.Accounts != null && ViewModels_Variables.ModelViews.Accounts.Count > 0)
                {
                    if (cmb_ACCOUNTS.SelectedItem != null)
                    {
                        var slctd = (Model.AccountModel)cmb_ACCOUNTS.SelectedItem;
                        report = ViewModels_Variables.ModelViews.BankPayments.Where((x) => x.Date >= dtp_from.SelectedDate && x.Date <= dtp_to.SelectedDate && x.CrAccount.ID == slctd.ID);

                    }
                    else
                    {
                        report = ViewModels_Variables.ModelViews.BankPayments.Where((x) => x.Date >= dtp_from.SelectedDate && x.Date <= dtp_to.SelectedDate);

                    }
                    rep_grid.ItemsSource = report;

                    lblTOTAL.Text = report.Sum((s) => s.Amount).ToString("0.00");
                }
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
                Refresh();
            }
            catch (SqlException rr)
            {
                MessageBox.Show(rr.Message.ToString());
            }
        }

        private void dtp_to_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Refresh();
            }
            catch (SqlException rr)
            {
                MessageBox.Show(rr.Message.ToString());
            }
        }

        private void cmb_ACCOUNTS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Refresh();
            }
            catch (SqlException rr)
            {
                MessageBox.Show(rr.Message.ToString());
            }
        }

        private void cmb_status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Refresh();
            }
            catch (SqlException rr)
            {
                MessageBox.Show(rr.Message.ToString());
            }
        }

        private void refresh_data_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                cmb_ACCOUNTS.SelectedItem = null;
                Window_Loaded(sender, e);
            }
            catch (SqlException rr)
            {
                MessageBox.Show(rr.Message.ToString());
            }
        }
    }
}
