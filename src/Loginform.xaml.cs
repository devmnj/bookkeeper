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
    /// Interaction logic for Loginform.xaml
    /// </summary>
    public partial class Loginform : Window
    {
        public Loginform()
        {
            try
            {
                InitializeComponent();
                this.frame1.Content = new loginPage();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    MessageBoxResult res = new MessageBoxResult();
                    res = MessageBox.Show("Do you want close this Application", "Close the Application", MessageBoxButton.YesNo);
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
    }
}
