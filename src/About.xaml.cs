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

namespace accounts
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            this.DataContext = ViewModels_Variables.ModelViews;
            if (ViewModels_Variables.ModelViews.FrontPanel["IS COMMERCIAL ACCOUNT"] == "TRUE")
            {
                TBL_EDITION.Text = "COMMERCIAL EDITION";

            }

            else if (ViewModels_Variables.ModelViews.FrontPanel["IS INDIVIDUAL ACCOUNT"] == "TRUE")
            {
                TBL_EDITION.Text = "INDIVIDUAL EDITION";

            }

        }
    }
}
