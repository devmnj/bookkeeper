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
    /// Interaction logic for AccountManager.xaml
    /// </summary>
    public partial class AccountManager : Window
    {
        public AccountManager()
        {
            InitializeComponent();
            DataContext = ViewModels_Variables.ModelViews;
            ViewModels_Variables.ModelViews.RefreshCashBook();

        }

        void CheckAll()
        {
            try
            {
                foreach (var i in lst_ac.Items)
                {
                    ((Model.AccountModel)i).IsChecked = true;
                }
            }
            catch (Exception er)
            {

                throw;
            }
        }
        void UnCheckAll()
        {
            try
            {
                foreach (var i in lst_ac.Items)
                {
                    ((Model.AccountModel)i).IsChecked = false;
                }
            }
            catch (Exception er)
            {

                throw;
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var acc = ViewModels_Variables.ModelViews.Accounts.Where((a) => a.Short_Name == ((CheckBox)sender).Content.ToString());
                if (acc != null)
                {
                    var filtered = ViewModels_Variables.ModelViews.CashBook.Where((ac) => ac.DrAccount.ID == acc.FirstOrDefault().ID).ToList();
                    detailGrid.ItemsSource = filtered;
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }
        }


        private void tbtn_unselectall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UnCheckAll();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message.ToString());
            }
        }

        private void tbtn__Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckAll();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message.ToString());
            }
        }

        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Get selected
                int delcount = 0;
                var selAccounts = lst_ac.Items.Cast<Model.AccountModel>().Where((ac) => ac.IsChecked == true).ToList<Model.AccountModel>();
                if (selAccounts.Count > 0)
                {
                    foreach (var ac in selAccounts)
                    {
                        var tr = ViewModels_Variables.ModelViews.AccountTransactions.Where((at) => at.Op_Ac_Id == ac.ID || at.Ac_Id == ac.ID);
                        if (tr != null && tr.Count() > 0)
                        {
                            if (chk_clear_entries.IsChecked == false)
                            {
                                if (DB.Transactions.DeleteAll(ac.ID) == true) delcount++;
                            }
                            else
                            {
                                if (DB.Transactions.DeleteAll(ac.ID, true) == true) delcount++;
                            }
                        }
                    }
                    if (delcount > 0)
                    {
                        MessageBox.Show(delcount + " Accounts were cleared");
                        chk_clear_entries.IsChecked = false;
                        UnCheckAll();
                    }
                    else { MessageBox.Show("No Transaction found"); }

                }
                else
                {
                    MessageBox.Show("Please choose a account");
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message.ToString());
            }
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.UnCheckAll();
                chk_clear_entries.IsChecked = false;
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
    }
}
