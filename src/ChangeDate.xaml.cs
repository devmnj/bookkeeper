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
    /// Interaction logic for ChangeDate.xaml
    /// </summary>
    public partial class ChangeDate : Window
    {
        public ChangeDate()
        {
            InitializeComponent();
            dtp_sysdate.SelectedDate = public_members._sysDate[0];
            dtp_sysdate.Focus();
        }

        private void txt_sys_date_Click(object sender, RoutedEventArgs e)
        {
            public_members._sysDate.Clear();
            public_members._sysDate.Add( dtp_sysdate.SelectedDate.Value);
           //public_members.Refresh_FrontPanelItems();
            this.Close();
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

        private void dtp_sysdate_KeyUp(object sender, KeyEventArgs e)
        {
            public_members._TabPress(e);
        }
    }
}
