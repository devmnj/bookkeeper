using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using CodeReason.Reports;


namespace accounts
{
    /// <summary>
    /// Interaction logic for DayBookReportViewer.xaml
    /// </summary>
    public partial class DayBookReportViewer : Window
    {
        DataTable table;
        List<DataTable> tableList;
        string _title="", _subtittle="",_gt;
        DataTable tableHeader;
        string _footer = "";
        DataTable tableData;
        string reportType = "Dynamic";
        string _template = "DBReport.xaml";
        private bool _firstActivated = true;
        public DayBookReportViewer()
        {
            InitializeComponent();
        }
        public DayBookReportViewer(List<DataTable> tables, string template, string tt = null, string subtt = null, string gt = null, string footer= null)
        {
            InitializeComponent();
             tableList = tables;
            _title = tt;
            _footer = footer;
            _subtittle = subtt;
            _gt = gt;
          
            if (template != null) _template = template;
            reportType = "Customized";
        }
      public System.Windows.Input.ICommand SaveToDoc()
        {
            
            MessageBox.Show("I am a command");
            return null;
        }
        public DayBookReportViewer(DataTable headers, DataTable datas,string title,string gsubtt, string footer = null)
        {
            InitializeComponent(); 
            tableHeader = headers;
            tableData = datas;
            _title = title;
            _footer = footer;
            _subtittle = _subtittle;
            reportType = "Dynamic";
        }
        public DayBookReportViewer(DataTable reporttable)
        {
            InitializeComponent();
            table = reporttable;

        }
        public DayBookReportViewer(DataTable reporttable, string template, string tt = null, string subtt = null, string gt = null, string footer = null)
        {
            InitializeComponent();
            table = reporttable;
            _title = tt;
            _footer = footer;
            _subtittle = subtt;
            _gt = gt;
            reportType = "Simple";
           if(template!=null) _template = template;

        }
        void Customized()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(delegate
            {
                try
                {
                    var header = tableList[0];
                    var headsection = tableList[1];
                    var body = tableList[2];
                    var foo = tableList[3];



                    ReportDocument reportDocument = new ReportDocument();

                    StreamReader reader = new StreamReader(new FileStream(@"Templates\" + _template, FileMode.Open, FileAccess.Read));
                    reportDocument.XamlData = reader.ReadToEnd();
                    reportDocument.ReportTitle = _title;
                    reportDocument.SubTittle = _subtittle;
                    reportDocument.ReportFooter = _footer;

                    reportDocument.GrndTotal = _gt;
                    reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Templates\");
                    reader.Close();

                    ReportData data = new ReportData();

                    // set constant document values
                    data.ReportDocumentValues.Add("PrintDate", DateTime.Now); // print date is now

                    //table = new DataTable("Ean");
                    //table = new DataTable("Ean");
                    //table.Columns.Add("Position", typeof(string));
                    //table.Columns.Add("Item", typeof(string));
                    //table.Columns.Add("EAN", typeof(string));
                    //table.Columns.Add("Count", typeof(int));
                    //Random rnd = new Random(1234);
                    //for (int i = 1; i <= 100; i++)
                    //{
                    //    // randomly create some items
                    //    table.Rows.Add(new object[] { i, "Item " + i.ToString("0000"), "123456790123", rnd.Next(9) + 1 });
                    //}
                    data.DataTables.Add(header);
                    data.DataTables.Add(headsection);
                    data.DataTables.Add(body);
                    data.DataTables.Add(foo);

                    DateTime dateTimeStart = DateTime.Now; // start time measure here

                    XpsDocument xps = reportDocument.CreateXpsDocument(data);
                    
                    documentViewer.Document = xps.GetFixedDocumentSequence();

                    // show the elapsed time in window title
                    Title += " - generated in " + (DateTime.Now - dateTimeStart).TotalMilliseconds + "ms";
                }
                catch (Exception ex)
                {
                    // show exception
                    MessageBox.Show(ex.Message + "\r\n\r\n" + ex.GetType() + "\r\n" + ex.StackTrace, ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                finally
                {
                    busyDecorator.IsBusyIndicatorHidden = true;
                }
            }));
        }
        void SimpleReport()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(delegate
            {
                try
                {
                    ReportDocument reportDocument = new ReportDocument();

                    StreamReader reader = new StreamReader(new FileStream(@"Templates\"+_template, FileMode.Open, FileAccess.Read));
                    reportDocument.XamlData = reader.ReadToEnd();
                    reportDocument.ReportTitle = _title;
                    reportDocument.SubTittle = _subtittle;
                    reportDocument.ReportFooter = _footer;

                    reportDocument.GrndTotal = _gt;
                    reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Templates\");
                    reader.Close();

                    ReportData data = new ReportData();
                   
                    // set constant document values
                    data.ReportDocumentValues.Add("PrintDate", DateTime.Now); // print date is now

                    //table = new DataTable("Ean");
                    //table = new DataTable("Ean");
                    //table.Columns.Add("Position", typeof(string));
                    //table.Columns.Add("Item", typeof(string));
                    //table.Columns.Add("EAN", typeof(string));
                    //table.Columns.Add("Count", typeof(int));
                    //Random rnd = new Random(1234);
                    //for (int i = 1; i <= 100; i++)
                    //{
                    //    // randomly create some items
                    //    table.Rows.Add(new object[] { i, "Item " + i.ToString("0000"), "123456790123", rnd.Next(9) + 1 });
                    //}
                    data.DataTables.Add(table);
                    
                    DateTime dateTimeStart = DateTime.Now; // start time measure here

                    XpsDocument xps = reportDocument.CreateXpsDocument(data);
                    documentViewer.Document = xps.GetFixedDocumentSequence();

                    // show the elapsed time in window title
                    Title += " - generated in " + (DateTime.Now - dateTimeStart).TotalMilliseconds + "ms";
                }
                catch (Exception ex)
                {
                    // show exception
                    MessageBox.Show(ex.Message + "\r\n\r\n" + ex.GetType() + "\r\n" + ex.StackTrace, ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                finally
                {
                    busyDecorator.IsBusyIndicatorHidden = true;
                }
            }));
        }
        //public XpsDocument CreateXpsDocument(ReportData data, string fileName)
        //{
        //    Package pkg = Package.Open(fileName, FileMode.Create, FileAccess.ReadWrite);
        //    string pack = "pack://report.xps";
        //    PackageStore.RemovePackage(new Uri(pack));
        //    PackageStore.AddPackage(new Uri(pack), pkg);

        //    PdfSharp.Xps.XpsModel.XpsDocument doc = 
        //    XpsSerializationManager rsm = new XpsSerializationManager(new XpsPackagingPolicy(doc), false);
        //    ReportPaginator rp = new ReportPaginator(this, data);
        //    rsm.SaveAsXaml(rp);

        //    rsm.Commit();
        //    pkg.Close();
        //    return new XpsDocument(fileName, FileAccess.ReadWrite);
        //}
        void DynamicReport()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(delegate
            {
                try
                {
                    var dateTimeStart = DateTime.Now;
                    ReportDocument reportDocument = new ReportDocument();
                    StreamReader reader = new StreamReader(new FileStream(@"Templates\DynamicReport.xaml", FileMode.Open, FileAccess.Read));
                    reportDocument.XamlData = reader.ReadToEnd();
                    reportDocument.ReportTitle = _title;
                    reportDocument.SubTittle = _subtittle;
                    reportDocument.ReportFooter=_footer;
                    reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Templates\");
                    reader.Close();

                    object[] obj;
                    ReportData data = new ReportData();



                    //tableHeader.Columns.Add();
                    //tableHeader.Rows.Add(new object[] { "Service" });
                    //tableHeader.Rows.Add(new object[] { "Amount" });
                    //tableHeader.Rows.Add(new object[] { "Price" });
                    //tableData.Columns.Add();
                    //tableData.Columns.Add();
                    //tableData.Columns.Add();
                    //obj = new object[3];
                    //for (int i = 0; i < 15; i++)
                    //{
                    //    obj[0] = String.Format("Service offered. Nº{0}", i);
                    //    obj[1] = i * 2;
                    //    obj[2] = String.Format("{0} €", i);
                    //    tableData.Rows.Add(obj);
                    //}

                    data.DataTables.Add(tableData);
                    data.DataTables.Add(tableHeader);

                    Title += " - generated in " + (DateTime.Now - dateTimeStart).TotalMilliseconds + "ms";

                    XpsDocument xps = reportDocument.CreateXpsDocument(data,"TEST");
                    PdfSharp.Xps.XpsConverter.Convert(xps.Uri.ToString());
                    
                }
                catch (Exception ex)
                {
                    // show exception
                    MessageBox.Show(ex.Message + "\r\n\r\n" + ex.GetType() + "\r\n" + ex.StackTrace, ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                finally
                {
                    busyDecorator.IsBusyIndicatorHidden = true;
                }
            }));
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            try
            {
                if (e.Key == System.Windows.Input.Key.Enter)
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

        private void Window_Activated(object sender, EventArgs e)
        {

            if (!_firstActivated) return;

            _firstActivated = false;
            try
            {
                if (reportType == "Simple")
                {
                    SimpleReport();
                }
                else if (reportType == "Customized")
                {
                    Customized();
                }
                else
                {
                    DynamicReport();
                }
            }
            catch (Exception)
            {

                throw;
            }
          
        }



    }
}
