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
using System.Data.SqlClient;
using System.Data;

using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using accounts.Model;

namespace accounts
{
    /// <summary>
    /// Interaction logic for Income_Expense_Statement.xaml
    /// </summary>
    public partial class Income_Expense_Statement : Window
    {
        ObservableCollection<CashBookModel> ie_list = new ObservableCollection<CashBookModel>();
        ListCollectionView reportview = null;
        ListCollectionView colletionview;


        SqlConnection con = new SqlConnection();
        DataTable source = new DataTable();
        string query1 = null;
        public Income_Expense_Statement()
        {
            InitializeComponent();
            dtp_from.SelectedDate = public_members._sysDate[0];
            dtp_to.SelectedDate = public_members._sysDate[0];
            FetchIEAccounts();

        }
        public void Search()
        {
            //try
            //{
            //    colletionview = new ListCollectionView(ie_list);
            //    colletionview.Filter = (e1) =>
            //    {
            //        CashBookModel emp1 = e1 as CashBookModel;
            //        if (Convert.ToDateTime(emp1.Date) >= dtp_from.SelectedDate && Convert.ToDateTime(emp1.Date) <= dtp_to.SelectedDate)
            //            return true;
            //        return false;
            //    };

            //    //var x = (from cash in ie_list where (cash=>cash.Date >= dtp_from.SelectedDate)   select cash).Sum<CashBookModel>(x2 => Convert.ToDouble(x2.Dr_Amount));
  
            //    var di = (colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, b) => c.Type == "DI")).Aggregate<CashBookModel, double>(0, (t, c) => t += Convert.ToDouble(c.Dr_Amount));
            //    var ii = (colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, b) => c.Type == "II")).Aggregate<CashBookModel, double>(0, (t, c) => t += Convert.ToDouble(c.Dr_Amount));
            //    var de = (colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, b) => c.Type == "DE")).Aggregate<CashBookModel, double>(0, (t, c) => t += Convert.ToDouble(c.Cr_Amount));
            //    var ie = (colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, b) => c.Type == "IE")).Aggregate<CashBookModel, double>(0, (t, c) => t += Convert.ToDouble(c.Cr_Amount));
            //    ditotal.DataContext = di;
            //    iitotal.DataContext = ii;
            //    detotal.DataContext = de;
            //    ietotal.DataContext = ie;
            //    subtotal.DataContext = (di + ii) - (de + ie);
            //    var filtered = colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, b) => c.Type != "II" && c.Type != "IE");
            //    var x = filtered.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Dr_Amount));
            //    var y = filtered.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Cr_Amount));
            //    ////Console.WriteLine("Total=" + x);

            //    string z;
            //    if (x > y)
            //    {
            //        z = (x - y).ToString() + " Dr";

            //    }
            //    else if (x < y)
            //    {

            //        z = y - x + " Cr";
            //    }
            //    else
            //    {

            //        z = "0.00";
            //    }
            //    lblbalance.Text = z;
            //    lblcr.Text = string.Format("{0:0.00}", Convert.ToDouble(y)) + " Cr.";
            //    lbldr.Text = string.Format("{0:0.00}", Convert.ToDouble(x)) + " Dr."; ;
            //    mylist.ItemsSource = filtered;
            //    //rep_grid.DataContext = colletionview;

            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message.ToString());
            //}
        }
        public void FetchIEAccounts()
        {
            //string query2 = null;
            //try
            //{
            //    con = public_members._OpenConnection();
            //    if (con != null)
            //    {
            //        int directIncome_id = public_members.Getgroupid("Direct Income");
            //        int indirectIncome_id = public_members.Getgroupid("Indirect Income");
            //        int directexpense_id = public_members.Getgroupid("Direct Expense");
            //        int indirectexpense_id = public_members.Getgroupid("Indirect expense");
            //        if (directIncome_id > 0)
            //        {
                         
                      
            //                query1 = "SELECT t.t_date, t.cr,t.dr ,t.led_id as id, ledgers.l_name as l_name  " +
            //                "FROM transactions t INNER JOIN  ledgers ON t.led_id = ledgers.id INNER JOIN    groups ON ledgers.l_parent = groups.id where groups.id =" + directIncome_id;
                      
            //            source = public_members._Fetch(query1, con);
            //            foreach (DataRow rows in source.Rows)
            //            {
            //                ie_list.Add(new CashBookModel()
            //                {
            //                    Date = Convert.ToDateTime(rows["t_date"].ToString()),
            //                    ACID = rows["id"].ToString(),
            //                    Account = rows["l_name"].ToString(),
            //                    Dr_Amount = rows["dr"].ToString(),
            //                    Type = "DI",

            //                });
            //            }
            //        }
            //        if (indirectIncome_id > 0)
            //        {
            //            query1 = "SELECT t.t_date, t.cr,t.dr ,t.led_id as id, ledgers.l_name as l_name  " +
            //            "FROM transactions t INNER JOIN  ledgers ON t.led_id = ledgers.id INNER JOIN    groups ON ledgers.l_parent = groups.id where groups.id =" + indirectIncome_id;

            //            source = null;
            //            source = public_members._Fetch(query1, con);
            //            foreach (DataRow rows in source.Rows)
            //            {
            //                ie_list.Add(new CashBookModel()
            //                {
            //                    Date = Convert.ToDateTime(rows["t_date"].ToString()),
            //                    ACID = rows["id"].ToString(),
            //                    Account = rows["l_name"].ToString(),
            //                    Dr_Amount = rows["dr"].ToString(),
            //                    Type = "II",

            //                });
            //            }
            //        }

            //        if (directexpense_id > 0)
            //        {
            //            query1 = "SELECT t.t_date, t.cr,t.dr ,t.led_id as id, ledgers.l_name as l_name  " +
            //            "FROM transactions t INNER JOIN  ledgers ON t.led_id = ledgers.id INNER JOIN    groups ON ledgers.l_parent = groups.id where groups.id =" + directexpense_id;

            //            source = null;
            //            source = public_members._Fetch(query1, con);

            //            foreach (DataRow rows in source.Rows)
            //            {
            //                ie_list.Add(new CashBookModel()
            //                {
            //                    Date = Convert.ToDateTime(rows["t_date"].ToString()),
            //                    ACID = rows["id"].ToString(),
            //                    Account = rows["l_name"].ToString(),
            //                    Cr_Amount = rows["cr"].ToString(),
            //                    Type = "DE",

            //                });
            //            }
            //        }

            //        if (indirectexpense_id > 0)
            //        {
            //            query1 = "SELECT t.t_date, t.cr,t.dr ,t.led_id as id, ledgers.l_name as l_name  " +
            //            "FROM transactions t INNER JOIN  ledgers ON t.led_id = ledgers.id INNER JOIN    groups ON ledgers.l_parent = groups.id where groups.id =" + indirectexpense_id;

            //            source = null;
            //            source = public_members._Fetch(query1, con);

            //            foreach (DataRow rows in source.Rows)
            //            {
            //                ie_list.Add(new CashBookModel()
            //                {
            //                    Date = Convert.ToDateTime(rows["t_date"].ToString()),
            //                    ACID = rows["id"].ToString(),
            //                    Account = rows["l_name"].ToString(),
            //                    Cr_Amount = rows["cr"].ToString(),
            //                    Type = "IE",
            //                });
            //            }
            //        }
                    
            //        colletionview = new ListCollectionView(ie_list);
            //        colletionview.Filter = (e1) =>
            //        {
            //            CashBookModel emp1 = e1 as CashBookModel;
            //            if (Convert.ToDateTime(emp1.Date) >= dtp_from.SelectedDate && Convert.ToDateTime(emp1.Date) <= dtp_to.SelectedDate)
            //                return true;
            //            return false;
            //        };
                 
            //        var di = (colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, b) => c.Type == "DI")).Aggregate<CashBookModel, double>(0, (t, c) => t += Convert.ToDouble(c.Dr_Amount));
            //        var ii = (colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, b) => c.Type == "II")).Aggregate<CashBookModel, double>(0, (t, c) => t += Convert.ToDouble(c.Dr_Amount));
            //        var de = (colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, b) => c.Type == "DE")).Aggregate<CashBookModel, double>(0, (t, c) => t += Convert.ToDouble(c.Cr_Amount));
            //        var ie = (colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, b) => c.Type == "IE")).Aggregate<CashBookModel, double>(0, (t, c) => t += Convert.ToDouble(c.Cr_Amount));

            //        ditotal.DataContext = di;
            //        iitotal.DataContext = ii;
            //        detotal.DataContext = de;
            //        ietotal.DataContext = ie;
            //        subtotal.DataContext = (di + ii) - (de + ie);
            //        var filtered = colletionview.Cast<CashBookModel>().Where<CashBookModel>((c, b) => c.Type != "II" && c.Type != "IE");
            //        var x = filtered.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Dr_Amount));
            //        var y = filtered.Cast<CashBookModel>().Aggregate<CashBookModel, double>(0, (total, s) => total += Convert.ToDouble(s.Cr_Amount)); 

            //        string z;
            //        if (x > y)
            //        {
            //            z = (x - y).ToString() + " Dr";

            //        }
            //        else if (x < y)
            //        {
            //            z = y - x + " Cr";
            //        }
            //        else
            //        {

            //            z = "0.00";
            //        }
            //        lblbalance.Text = z;
            //        lblcr.Text = string.Format("{0:0.00}", Convert.ToDouble(y)) + " Cr.";
            //        lbldr.Text = string.Format("{0:0.00}", Convert.ToDouble(x)) + " Dr."; ;
            //        mylist.ItemsSource = filtered;

            //        dtp_from.Focus();
            //    }
            //    else
            //    {
            //        MessageBox.Show("DataBase connection broken,avail technical support");
            //    }
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message.ToString());
            //}
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

        private void rep_grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

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
            this.FetchIEAccounts();
        }


    }
}
