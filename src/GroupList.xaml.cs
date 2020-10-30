using accounts.Model;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace accounts
{
    /// <summary>
    /// Interaction logic for GroupList.xaml
    /// </summary>
    public partial class GroupList : System.Windows.Window
    {
        IEnumerable<CashBookModel> gbook;
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
        public GroupList(string title)
        {
            InitializeComponent();
            cmb_groups.Visibility = Visibility.Collapsed;


        }
        public GroupList()
        {
            InitializeComponent();
            FetchIEAccounts();

        }
        public GroupList(int gp)
        {
            InitializeComponent();
            var group = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.ID == gp).FirstOrDefault();
            if (group != null) { cmb_groups.SelectedItem = group; }
            else
            {
                cmb_groups.SelectedItem = null;
            }

        }
        public void Search()
        {
            try
            {

            }
            catch (Exception eee)
            {
                MessageBox.Show(eee.Message.ToString());
            }
        }
        public void CatogorySearch()
        {
            try
            {
                ClearReportcache();
               
                gbook = ViewModels_Variables.ModelViews.GroupBook;

                if (cmb_catagory.Text != null && cmb_catagory.SelectedItem.ToString().Length > 0)
                {
                    gbook = ViewModels_Variables.ModelViews.GroupBook.Where((a) => a.CrAccount.Parent.Catagory == cmb_catagory.SelectedItem.ToString());
                }

                var cr = gbook.Sum((a) => a.Cr_Amount);
                var dr = gbook.Sum((a) => a.Dr_Amount);
                var bal = gbook.Sum((a) => a.Balance);
                lblcr.Text = "Cr " + cr.ToString("0.00");
                lbldr.Text = "Dr " + dr.ToString("0.00");
                lblbalance.Text = " " + bal.ToString("0.00");
                group_acc_grid.ItemsSource = gbook;

            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }
        public void FetchIEAccounts()
        {
            try
            {
                ClearReportcache();
                var parent = (Model.GroupModel)cmb_groups.SelectedItem;
               
                if (parent != null)
                {
                    gbook = ViewModels_Variables.ModelViews.GroupBook.Where((a) => a.CrAccount.Parent.ID == parent.ID);
                }
                else { gbook = ViewModels_Variables.ModelViews.GroupBook; }
                var cr = gbook.Sum((a) => a.Cr_Amount);
                var dr = gbook.Sum((a) => a.Dr_Amount);
                var bal = gbook.Sum((a) => a.Balance);
                lblcr.Text = "Cr " + cr.ToString("0.00");
                lbldr.Text = "Dr " + dr.ToString("0.00");
                lblbalance.Text = " " + bal.ToString("0.00");
                group_acc_grid.ItemsSource = gbook;

            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }

        private void cmb_groups_KeyDown(object sender, KeyEventArgs e)
        {
            ClearReportcache();
            if (e.Key == Key.Enter) { Search(); }
        }

        private void dtp_from_KeyUp(object sender, KeyEventArgs e)
        {
            Search(); 
        }

        private void dtp_to_KeyUp(object sender, KeyEventArgs e)
        {
            Search();
        }



        private void refresh_data_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cmb_groups.SelectedItem = null;
                Window_Loaded(sender, e);
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
                if (xpsFname != null)
                {
                    xps = new XpsDocument(xpsFname, FileAccess.Read);
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
          string GenerateWordDoc(  )
        {
            string fname = null;
            try
            {

                fname = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\Doc");

                using (var document = DocX.Create(fname))
                {
                    document.ApplyTemplate(@"doctemplates/GroupReport.dotx");
                    //Footer
                    document.ReplaceText("[MyCompany]", ViewModels_Variables.ModelViews.CompanyProfile[0].company);
                    document.ReplaceText("[Clandmark]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].lmark);
                    document.ReplaceText("[CCity]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].place);
                    document.ReplaceText("[CPhone]", "Phone :" + ViewModels_Variables.ModelViews.CompanyProfile[0].officeno);

                    if (cmb_groups.Text != null && cmb_groups.Text.Length > 0) { document.ReplaceText("[Group]", cmb_groups.Text.ToString()); } else { document.ReplaceText("[Group]", ""); }

                    document.ReplaceText("[EDate]", public_members._sysDate[0].ToShortDateString());


                    var t = document.Tables;

                    var sorted =gbook;
                   if(sorted!=null) {

                        Row newrow;
                        int i = 1;
                        foreach (var v in sorted)
                        {
                            ++i;
                            newrow = t[1].InsertRow();
                            newrow.Cells[0].Paragraphs[0].Append(v.CrAccount.ID.ToString()).Alignment = Alignment.left;
                            newrow.Cells[1].Paragraphs[0].Append(v.CrAccount.Name).Alignment = Alignment.left;

                            newrow.Cells[2].Paragraphs[0].Append(v.Dr_Amount.ToString("0.00")).Alignment = Alignment.right;
                            newrow.Cells[3].Paragraphs[0].Append(v.Cr_Amount.ToString("0.00")).Alignment = Alignment.right;
                            newrow.Cells[4].Paragraphs[0].Append(v.Balance.ToString()).Alignment = Alignment.right;

                            t[1].Rows.Add(newrow);
                        }

                        newrow = t[1].InsertRow();
                    t[1].Rows.Add(newrow);

                    var x = sorted.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Dr_Amount));
                    var y = sorted.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Cr_Amount));
                    var z = sorted.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Balance));


                    newrow = t[1].InsertRow();
                    newrow.Cells[1].Paragraphs[0].Append("Total").Bold(true);
                    newrow.Cells[3].Paragraphs[0].Append(string.Format("{0:0.00}", y)).Bold(true).Alignment = Alignment.right;
                    newrow.Cells[2].Paragraphs[0].Append(string.Format("{0:0.00}", x)).Bold(true).Alignment = Alignment.right;
                    newrow.Cells[4].Paragraphs[0].Append(string.Format("{0:0.00}", z)).Bold(true).Alignment = Alignment.right;
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
                              if (wordFname!= null)  Process.Start("WINWORD.exe", wordFname);
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


                string fileName = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\workbook" ,".xlsx");

                var app = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook workbook;
                workbook = app.Workbooks.Add(Type.Missing);
                var worKsheeT = (Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet;
                worKsheeT.Range[worKsheeT.Cells[1, 1], worKsheeT.Cells[1, 8]].Merge();
                worKsheeT.Cells[1, 1] = "Account Group Report";

                var range = (Range)worKsheeT.Cells[1, 1];
                range.Font.Bold = true;
                range.Font.Size = 25;
                range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                if (cmb_groups.Text.Length > 0)
                {
                    worKsheeT.Name = cmb_groups.Text.ToString();
                }


                worKsheeT.Range[worKsheeT.Cells[3, 1], worKsheeT.Cells[3, 8]].Merge();

                if (cmb_groups.Text.Length > 0)
                {
                    worKsheeT.Cells[3, 1] = ("Group Report of " + cmb_groups.Text.ToString() + " As on " + public_members._sysDate[0].ToLongDateString());
                }
                else
                {
                    worKsheeT.Cells[3, 1] = ("Group Report" + " As on " + public_members._sysDate[0].ToLongDateString());
                }



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

                worKsheeT.Cells[5, 6] = "Balance";
                ((Range)worKsheeT.Cells[5, 6]).EntireColumn.ColumnWidth = 16;
                worKsheeT.Cells[5, 6].Font.Size = 13;

                var sorted =gbook;
                if(sorted!=null){
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
                        worKsheeT.Cells[i, 5] = v.Cr_Amount;
                        worKsheeT.Cells[i, 6] = v.Balance;
                    }
                    Microsoft.Office.Interop.Excel.Range xLSRanges = worKsheeT.Range["A5", "f" + i];
                    Microsoft.Office.Interop.Excel.Borders border = xLSRanges.Borders;
                    border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    border.Weight = 2D;

                    xLSRanges = worKsheeT.Range["E6", "f" + i];
                    xLSRanges.NumberFormat = "0.00";
                    xLSRanges = worKsheeT.Range["A5", "f5"];
                    xLSRanges.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbLightBlue;

                    i++;
                    int frange_start = i;
                    worKsheeT.Cells[i, 2] = "Total";
                    var crt = (from cr in sorted.AsEnumerable() select Convert.ToDouble(cr.Cr_Amount)).Sum();
                    worKsheeT.Cells[i, 5] = crt;
                    var drt = (from cr in sorted.AsEnumerable() select Convert.ToDouble(cr.Dr_Amount)).Sum();
                    worKsheeT.Cells[i, 4] = drt;
                    var bt = (from cr in sorted.AsEnumerable() select Convert.ToDouble(cr.Balance)).Sum();
                    worKsheeT.Cells[i, 6] = bt;

                    xLSRanges = worKsheeT.Range["E" + frange_start, "f" + frange_start];
                    xLSRanges.NumberFormat = "0.00";


                    xLSRanges = worKsheeT.Range["E" + frange_start, "f" + i];
                    xLSRanges.NumberFormat = "0.00";
                    xLSRanges = worKsheeT.Range["A" + frange_start, "f" + i];
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
                    wordFname = GenerateWordDoc();
                }
                if (pdfFname == null)
                {
                    pdfFname = ReportExport.WordExport.CreatePDF(wordFname);
                }

                  if (pdfFname != null) Process.Start("AcroRd32.exe", pdfFname);

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
                    wordFname = GenerateWordDoc();
                }
                if (xpsFname == null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }
                 if (xpsFname != null)  Process.Start("explorer.exe", xpsFname);
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void cmb_groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModels_Variables.ModelViews.RefreshGroupBook();
                DataContext = ViewModels_Variables.ModelViews;
                var catagories = (from c in ViewModels_Variables.ModelViews.AccountGroups where c.Catagory != "NONE" || c.Catagory != "" select c.Catagory).Distinct().ToList();

                cmb_catagory.ItemsSource = catagories;
                ClearReportcache();
                
                FetchIEAccounts();

            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void cmb_groups_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                
                FetchIEAccounts();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void group_acc_grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void group_acc_grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (group_acc_grid.CurrentColumn == group_acc_grid.Columns[1])
                {
                    var cur = ((CashBookModel)group_acc_grid.CurrentItem);
                    if (cur != null)
                    {
                        CashBook1 rform = new CashBook1();
                        rform.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        rform.Show();rform.Find(cur.CrAccount.ID);

                        //MessageBox.Show(cur.Text.ToString());
                    }

                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString());

            }
        }

        private void cmb_catagory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CatogorySearch();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
    }
}
