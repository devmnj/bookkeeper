using accounts.Model;
using Microsoft.Office.Interop.Excel;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Xps.Packaging;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace accounts
{
    /// <summary>
    /// Interaction logic for trialBalance.xaml
    /// </summary>
    public partial class trialBalance : System.Windows.Window
    {

        string wordFname = null;
        string xpsFname = null;
        string pdfFname = null;
        string exccelFname = null;
        void ClearPrintCahe()
        {
            try
            {
                wordFname = null;
                xpsFname = null;
                pdfFname = null;
                exccelFname = null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public trialBalance()
        {
            InitializeComponent();
            FetchTrial();
        }
        public void FetchTrial()
        {
            try
            {
                ViewModels_Variables.ModelViews.RefreshGroupBook();
                var dr = ViewModels_Variables.ModelViews.GroupBook.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Dr_Amount));
                var cr = ViewModels_Variables.ModelViews.GroupBook.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Cr_Amount));

                lblcr.Text = string.Format("{0:0.00}", Convert.ToDouble(cr)) + " Cr.";
                lbldr.Text = string.Format("{0:0.00}", Convert.ToDouble(dr)) + " Dr."; ;

                group_acc_grid.ItemsSource = ViewModels_Variables.ModelViews.GroupBook;
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
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

        private void refresh_data_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                ClearPrintCahe();
                FetchTrial();
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
                    File.Delete(wordFname);
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
        string CreateWordDoc()
        {
            String fileName = null;
            try
            {
                fileName = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\Doc");

                using (var document = DocX.Create(fileName))
                {
                    document.ApplyTemplate(@"doctemplates/trialbalance.dotx");
                    //Footer
                    document.ReplaceText("[MyCompany]", ViewModels_Variables.ModelViews.CompanyProfile[0].company);
                    document.ReplaceText("[Clandmark]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].lmark);
                    document.ReplaceText("[CCity]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].place);
                    document.ReplaceText("[CPhone]", "Phone :" + ViewModels_Variables.ModelViews.CompanyProfile[0].officeno);
                    document.ReplaceText("[Year]", ViewModels_Variables.ModelViews.CompanyProfile[0].f_date1.Year.ToString());
                    var t = document.Tables;
                    var sorted = group_acc_grid.Items.Cast<CashBookModel>().ToList();
                    {
                        Row newrow;
                        int i = 1;
                        foreach (var v in sorted)
                        {
                            ++i;
                            newrow = t[1].InsertRow();
                            newrow.Cells[0].Paragraphs[0].Append(v.CrAccount.ID.ToString()).Alignment = Alignment.left;
                            newrow.Cells[1].Paragraphs[0].Append(v.CrAccount.Name).Alignment = Alignment.left;

                            newrow.Cells[2].Paragraphs[0].Append(v.Dr_Amount.ToString()).Alignment = Alignment.right;
                            newrow.Cells[3].Paragraphs[0].Append(v.Cr_Amount.ToString()).Alignment = Alignment.right;


                            t[1].Rows.Add(newrow);
                        }

                        newrow = t[1].InsertRow();
                        t[1].Rows.Add(newrow);

                        var x = sorted.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Dr_Amount));
                        var y = sorted.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Cr_Amount));



                        newrow = t[1].InsertRow();
                        newrow.Cells[1].Paragraphs[0].Append("Total").Bold(true);
                        newrow.Cells[3].Paragraphs[0].Append(string.Format("{0:0.00}", y)).Bold(true).Alignment = Alignment.right;
                        newrow.Cells[2].Paragraphs[0].Append(string.Format("{0:0.00}", x)).Bold(true).Alignment = Alignment.right;

                        t[1].Rows.Add(newrow);


                        document.Save();
                        document.Dispose();
                    }
                }


            }
            catch (Exception e1)

            {
                MessageBox.Show(e1.Message.ToString());

            }
            return fileName;
        }
        private void btn_doc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wordFname == null)
                {
                  wordFname=  CreateWordDoc();
                }
                if (wordFname != null) Process.Start("WINWORD.exe", wordFname);
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


                string fileName = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\WorkBook", ".xlsx");

                var app = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook workbook;
                workbook = app.Workbooks.Add(Type.Missing);
                var worKsheeT = (Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet;
                worKsheeT.Range[worKsheeT.Cells[1, 1], worKsheeT.Cells[1, 8]].Merge();
                worKsheeT.Cells[1, 1] = "Trial Balance Report";

                var range = (Range)worKsheeT.Cells[1, 1];
                range.Font.Bold = true;
                range.Font.Size = 25;
                range.Font.Size = 25;
                range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;



                worKsheeT.Range[worKsheeT.Cells[3, 1], worKsheeT.Cells[3, 8]].Merge();
                worKsheeT.Cells[3, 1] = ("Trial Balance Report" + " As on " + public_members._sysDate[0].ToLongDateString());

                worKsheeT.Cells.Font.Size = 12;

                //Table Heading 
                worKsheeT.Cells[5, 1] = "SLNO";
                worKsheeT.Cells[5, 1].Font.Size = 13;

                worKsheeT.Cells[5, 2] = "ID";
                worKsheeT.Cells[5, 2].Font.Size = 13;

                worKsheeT.Cells[5, 3] = "Account";
                worKsheeT.Cells[5, 3].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 3]).EntireColumn.ColumnWidth = 40;

                worKsheeT.Cells[5, 4] = "Dr Amount";
                ((Range)worKsheeT.Cells[5, 4]).EntireColumn.ColumnWidth = 16;
                worKsheeT.Cells[5, 4].Font.Size = 13;

                worKsheeT.Cells[5, 5] = "Credit Amount";
                ((Range)worKsheeT.Cells[5, 5]).EntireColumn.ColumnWidth = 16;
                worKsheeT.Cells[5, 5].Font.Size = 13;


                var sorted = group_acc_grid.Items.Cast<CashBookModel>().ToList();
                {
                    int i = 5;
                    int j = 0;
                    foreach (var v in sorted)
                    {
                        i++;
                        j++;
                        worKsheeT.Cells[i, 1] = j.ToString();
                        worKsheeT.Cells[i, 2] = v.CrAccount.ID;
                        worKsheeT.Cells[i, 3] = v.CrAccount.Name;
                        worKsheeT.Cells[i, 4] = v.Dr_Amount;
                        worKsheeT.Cells[i, 5] = v.Dr_Amount;

                    }
                    Microsoft.Office.Interop.Excel.Range xLSRanges = worKsheeT.Range["A5", "e" + i];
                    Microsoft.Office.Interop.Excel.Borders border = xLSRanges.Borders;
                    border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    border.Weight = 2D;

                    xLSRanges = worKsheeT.Range["d6", "e" + i];
                    xLSRanges.NumberFormat = "0.00";
                    xLSRanges = worKsheeT.Range["A5", "e5"];
                    xLSRanges.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbLightBlue;

                    i++;
                    int frange_start = i;
                    worKsheeT.Cells[i, 2] = "Total";
                    var crt = (from cr in sorted.AsEnumerable() select Convert.ToDouble(cr.Cr_Amount)).Sum();
                    worKsheeT.Cells[i, 5] = crt;
                    var drt = (from cr in sorted.AsEnumerable() select Convert.ToDouble(cr.Dr_Amount)).Sum();
                    worKsheeT.Cells[i, 4] = drt;


                    xLSRanges = worKsheeT.Range["E" + frange_start, "e" + frange_start];
                    xLSRanges.NumberFormat = "0.00";


                    xLSRanges = worKsheeT.Range["E" + frange_start, "e" + i];
                    xLSRanges.NumberFormat = "0.00";
                    xLSRanges = worKsheeT.Range["A" + frange_start, "e" + i];
                    xLSRanges.Font.Bold = true;
                    //xLSRanges = worKsheeT.Range["C" + i, "C" + i];
                    //xLSRanges.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                }
                File.Delete(fileName);
                workbook.SaveAs(fileName);

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
                if (wordFname != null)
                {
                    pdfFname = ReportExport.WordExport.CreatePDF(wordFname);
                }
                if (pdfFname != null)
                {
                    Process.Start("AcroRd32.exe", pdfFname);
                }


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
                if(xpsFname != null)
                {
                    Process.Start("explorer.exe", xpsFname);
                }

                 
            }
            catch (Exception e1)

            {
                MessageBox.Show(e1.Message.ToString());

            }
        }
    }
}
