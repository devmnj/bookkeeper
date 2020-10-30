using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Input;
using Xceed.Words.NET;
using Xceed.Document.NET;
using System.Diagnostics;
using System.IO;
using Microsoft.Office.Interop.Excel;
using accounts.Model;
using System.Windows.Xps.Packaging;

namespace accounts
{
    /// <summary>
    /// Interaction logic for account_List.xaml
    /// </summary>
    public partial class account_List : System.Windows.Window
    {
        string wordFname = null;
        string xpsFname = null;
        string pdfFname = null;
        List<CashBookModel> accs;
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

        public account_List()
        {
            InitializeComponent();

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

        public void Search()
        {
            //try
            //{

            //    collectionview = new ListCollectionView(public_members.accounts_obscoll);

            //    if (cmb_city.Text != null && cmb_city.Text.Trim().Length > 0)
            //    {
            //        collectionview.Filter = (e1) =>
            //        {
            //            AccountModel acc = e1 as AccountModel;
            //            if (acc.City == cmb_city.Text.ToString())
            //                return true;
            //            return false;
            //        };
            //    }

            //    if (cmb_catagory.Text != null && cmb_catagory.Text.Trim().Length > 0)
            //    {
            //        collectionview.Filter = (e1) =>
            //        {
            //            AccountModel acc = e1 as AccountModel;
            //            if (acc.Catagory == cmb_catagory.Text.ToString())
            //                return true;
            //            return false;
            //        };
            //    }


            //    if (cmb_group.Text != null && cmb_group.Text.Trim().Length > 0)
            //    {
            //        collectionview.Filter = (e1) =>
            //        {
            //            AccountModel acc = e1 as AccountModel;
            //            if (acc.Group == cmb_group.Text.ToString())
            //                return true;
            //            return false;
            //        };
            //    }
            //    var sorted = FuncOrderBy(collectionview, cmb_orderby.Text.ToString());
            //    collectionview = new ListCollectionView(sorted);

            //    acc_grid.ItemsSource = collectionview;
            //    int count = 0;
            //    if (collectionview.Count > 0) { count = collectionview.Count - 1; }
            //    lblTOTAL.Text = count.ToString() + " Nos found!";

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message.ToString());
            //}

        }

        private void cmb_orderby_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter) { Search(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void cmb_city_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter) { Search(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void cmb_catagory_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter) { Search(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void cmb_catagory_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void cmb_group_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter) { Search(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void acc_grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (acc_grid.CurrentColumn == acc_grid.Columns[0] || acc_grid.CurrentColumn == acc_grid.Columns[1] || acc_grid.CurrentColumn == acc_grid.Columns[2])
                {
                    var cur = (CashBookModel)acc_grid.CurrentItem;

                    if (cur.DrAccount != null)
                    {
                        accountRegistration acc = new accountRegistration();
                        acc.Find(cur.DrAccount.ID);
                        acc.Show();

                    }

                }
                else if (acc_grid.CurrentColumn == acc_grid.Columns[3])
                {
                    var cur = (CashBookModel)acc_grid.CurrentItem;

                    if (cur.DrAccount != null)
                    {
                        GroupRegistration acc = new  GroupRegistration();
                       
                        acc.Show(); acc.Find(cur.DrAccount.Parent.ID);

                    }
                }
            }
            catch (Exception ee)
            {

            }
        }

        private void acc_grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }



        private void refresh_data_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cmb_city.SelectedItem = null;
                cmb_group.SelectedItem = null;
                cmb_catagory.SelectedItem = null;
                Window_Loaded(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
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
                if (xpsFname == null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }
                xps = new XpsDocument(xpsFname, FileAccess.Read);
                accounts.PrintDialogue print = new PrintDialogue(xps, this.Title  );
                print.Show();
                xps.Close();
            }
            catch (Exception w1)
            {
                MessageBox.Show(w1.Message.ToString());
            }
        }
        string CreateWordDoc( )
        {
            string fname = null;
            try
            {

                fname = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\Doc");


                using (var document = DocX.Create(fname))
                {
                    document.ApplyTemplate(@"doctemplates/AccountList.dotx");
                    //Footer
                    document.ReplaceText("[MyCompany]", ViewModels_Variables.ModelViews.CompanyProfile[0].company);
                    document.ReplaceText("[Clandmark]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].lmark);
                    document.ReplaceText("[CCity]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].place);
                    document.ReplaceText("[CPhone]", "Phone :" + ViewModels_Variables.ModelViews.CompanyProfile[0].officeno);  

                    document.ReplaceText("[EDate]", public_members._sysDate[0].ToShortDateString());



                    var t = document.Tables;

                    var sorted = accs;
                    {

                        Row newrow;
                        int i = 1;
                        foreach (var v in sorted)
                        {
                            ++i;
                            newrow = t[1].InsertRow();
                            newrow.Cells[0].Paragraphs[0].Append(v.DrAccount.ID.ToString());
                            newrow.Cells[1].Paragraphs[0].Append(v.DrAccount.Name);
                            newrow.Cells[2].Paragraphs[0].Append(v.DrAccount.Address);
                            newrow.Cells[3].Paragraphs[0].Append(v.DrAccount.PhoneNo).Alignment = Alignment.right;
                            newrow.Cells[4].Paragraphs[0].Append(v.Balance.ToString("0.00")).Alignment = Alignment.right;
                            t[1].Rows.Add(newrow);
                        }

                        newrow = t[1].InsertRow();
                        t[1].Rows.Add(newrow);
                        var x = sorted.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Balance));


                        newrow = t[1].InsertRow();
                        newrow.Cells[1].Paragraphs[0].Append("Total").Bold(true);

                        newrow.Cells[4].Paragraphs[0].Append(string.Format("{0:0.00}", x)).Bold(true).Alignment = Alignment.right;
                        t[1].Rows.Add(newrow);


                        document.Save();
                        document.Dispose();
                    }
                }
                //}

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
                Process.Start("WINWORD.exe", wordFname);
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
                worKsheeT.Range[worKsheeT.Cells[1, 1], worKsheeT.Cells[1, 8]].Merge();
                worKsheeT.Cells[1, 1] = "Account List";

                var range = (Range)worKsheeT.Cells[1, 1];
                range.Font.Bold = true;
                range.Font.Size = 25;
                range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                worKsheeT.Name = "Account List";

                worKsheeT.Range[worKsheeT.Cells[3, 1], worKsheeT.Cells[3, 8]].Merge();
                worKsheeT.Cells[3, 1] = "Account List " + "As on " + public_members._sysDate[0].ToShortDateString();
                worKsheeT.Cells.Font.Size = 12;

                //Table Heading 
                worKsheeT.Cells[5, 1] = "SLNO";
                worKsheeT.Cells[5, 1].Font.Size = 13;

                worKsheeT.Cells[5, 2] = "Name";
                worKsheeT.Cells[5, 2].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 2]).EntireColumn.ColumnWidth = 60;

                worKsheeT.Cells[5, 3] = "Address";
                worKsheeT.Cells[5, 3].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 3]).EntireColumn.ColumnWidth = 60;

                worKsheeT.Cells[5, 4] = "Phone No";
                worKsheeT.Cells[5, 4].Font.Size = 13;
                ((Range)worKsheeT.Cells[5, 4]).EntireColumn.ColumnWidth = 30;

                worKsheeT.Cells[5, 5] = "Balance";
                ((Range)worKsheeT.Cells[5, 6]).EntireColumn.ColumnWidth = 40;
                worKsheeT.Cells[5, 5].Font.Size = 13;


                var sorted = accs;
                {
                    int i = 5;
                    int j = 0;
                    foreach (var v in sorted)
                    {
                        i++;
                        j++;
                        worKsheeT.Cells[i, 1] = j.ToString();
                        worKsheeT.Cells[i, 2] = v.DrAccount.Name;
                        worKsheeT.Cells[i, 3] = v.DrAccount.Address;
                        worKsheeT.Cells[i, 4] = v.DrAccount.PhoneNo;
                        worKsheeT.Cells[i, 5] = v.Balance;

                    }
                    Microsoft.Office.Interop.Excel.Range xLSRanges = worKsheeT.Range["A5", "E" + i];
                    Microsoft.Office.Interop.Excel.Borders border = xLSRanges.Borders;
                    border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    border.Weight = 2D;

                    xLSRanges = worKsheeT.Range["E6", "E" + i];
                    xLSRanges.NumberFormat = "0.00";
                    xLSRanges = worKsheeT.Range["A5", "E5"];
                    xLSRanges.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbLightBlue;

                    i++;
                    int frange_start = i;
                    worKsheeT.Cells[i, 2] = "Total";
                    var crt = (from cr in sorted.AsEnumerable() select Convert.ToDouble(cr.Balance)).Sum();
                    worKsheeT.Cells[i, 5] = crt;

                    xLSRanges = worKsheeT.Range["A" + frange_start, "E" + i];
                    xLSRanges.Font.Bold = true;
                    xLSRanges = worKsheeT.Range["E" + frange_start, "E" + i];
                    xLSRanges.NumberFormat = "0.00";

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

        private void btn_xps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wordFname == null)
                {
                    wordFname = CreateWordDoc();
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
        void ListAccount()
        {
            try
            {
                ClearReportcache();
                accs = ViewModels_Variables.ModelViews.AccountList.ToList();
                if(cmb_catagory.SelectedItem!=null  )
                {
                  accs = accs.Where((a1) => a1.Catagory == cmb_catagory.SelectedItem.ToString()).ToList<CashBookModel>();
                }
                var group = (Model.GroupModel)cmb_group.SelectedItem;
                if (group != null)
                {
                    accs = accs.Where((a1) => a1.DrAccount.Parent.ID==group.ID).ToList();
                }
                if(cmb_city.SelectedItem != null  )
                {
                    accs = accs.Where((a1) => a1.DrAccount.City==cmb_city.SelectedItem.ToString()).ToList();
                }
                //var dr = accs.Sum((a) => a.Dr_Amount);
                //var cr = accs.Sum((a) => a.Cr_Amount);
                //lblcr.Text = "Payable " + dr.ToString("0.00");
                //lblTOTAL.Text = "Receivable " +cr.ToString("0.00");
                acc_grid.ItemsSource = accs;
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
                ViewModels_Variables.ModelViews.RefreshAccountList();
                var view = ViewModels_Variables.ModelViews;
                DataContext = view;

                ListAccount();
                cmb_city.ItemsSource = (from c in view.Accounts where c.City !="" select c.City ).Distinct().ToList<string>();
                cmb_catagory.ItemsSource = (from c in view.AccountGroups where c.Catagory != "" select c.Catagory).Distinct().ToList<string>();

            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void cmb_city_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListAccount();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void cmb_group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListAccount();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void cmb_catagory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListAccount();
            }
            catch (Exception er)
            {

               
            }
        }

        
    }
}
