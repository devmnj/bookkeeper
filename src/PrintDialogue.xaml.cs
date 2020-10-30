using System;
using System.Collections.Generic;
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

namespace accounts
{
    /// <summary>
    /// Interaction logic for PrintDialogue.xaml
    /// </summary>
    public partial class PrintDialogue : Window
    {
        public PrintDialogue()
        {
            InitializeComponent();
        }
        public PrintDialogue(XpsDocument xpsDocument,string title=null )
        {
            InitializeComponent();
            this.Title = this.Title + "-" + title;
            printDocumentViewer.Document = xpsDocument.GetFixedDocumentSequence();
            this.WindowState = WindowState.Maximized;
        }
    }
}
