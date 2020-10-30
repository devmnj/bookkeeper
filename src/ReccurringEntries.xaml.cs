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
using System.Data.SqlClient;
using System.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace accounts
{
    /// <summary>
    /// Interaction logic for ReccurringEntries.xaml
    /// </summary>
    public partial class ReccurringEntries : Window
    {
        CollectionView source;
        Model.Task curtask = new  Model.Task();
        System.Data.SqlClient.SqlConnection con = new SqlConnection();
        DataTable tasks = new DataTable();
        List<Model.Task> taskList = new  List<Model.Task>();
        public ReccurringEntries()
        {
            InitializeComponent();

            FetchTasks();
        }

        void ClearState()
        {
            lst_holidays.Items.Clear();
            lst_mode_config.Items.Clear();
            opt_daily.IsChecked = true;
            chk_active.IsChecked = false;
        }
        void FindButtonState()
        {
            btn_deleteTask.IsEnabled = true;
            btn_playTask.IsEnabled = true;
            btn_updateTask.IsEnabled = true;

        }

        void NewButtonState()
        {
            btn_deleteTask.IsEnabled = false;
            btn_playTask.IsEnabled = false;
            btn_updateTask.IsEnabled = false;
            if (lst_mode_config.DataContext == null)
            { lst_mode_config.DataContext = new List<string>(); }
            else
            {
                lst_mode_config.DataContext = null;
                lst_mode_config.DataContext = new List<string>();
            }

            lst_holidays.DataContext = null;
            opt_daily.IsChecked = true;
            chk_active.IsChecked = false;
            txt_tags.Text = "";
        }
        private void FetchTasks()
        {
            try
            {
                NewButtonState();
                con = public_members._OpenConnection();
                if (con != null)
                {
                    tasks = public_members._Fetch("select * from  recurrings order by id", con);
                    con.Close();
                    DateTime t;
                    taskList.Clear();
                    foreach (DataRow row in tasks.Rows)
                    {
                        taskList.Add(new Model.Task()
                        {
                            ID = Convert.ToInt32(row["id"].ToString()),
                            ENO = Convert.ToInt32(row["eno"].ToString()),
                            ENTRY = row["entry"].ToString(),
                            //MODE = Convert.ToInt32(row["mode"].ToString()),
                            //MODE_CONFIGURATIONS = row["mode_configuration"].ToString().Split(',').ToList<string>()
                            //,
                            ////ACTIVE = row["active"].ToString(),
                            //HOLLY_DAYS = row["holidays"].ToString().Split(',').ToList<string>()
                            //,
                            //LAST_TASK_DATE = row["lt_date"].ToString(),
                            //TAGS = row["tag"].ToString(),

                        });
                    }

                    //CollectionViewSource gridItems;
                    //gridItems = (CollectionViewSource)FindResource("gridViewsource");
                    //gridItems.Source = taskList;
                    source = new CollectionView(taskList);
                    taskGrid.ItemsSource = source;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString());
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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void taskGrid_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void taskGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            try
            {
                NewButtonState();
                curtask = new Model.Task();
                curtask = ((Model.Task)taskGrid.CurrentItem);
                if (curtask != null && curtask.ID > 0)
                {

                    //switch (curtask.MODE)
                    //{
                    //    case 1:
                    //        opt_daily.IsChecked = true;
                    //        break;
                    //    case 2:
                    //        opt_weekly.IsChecked = true;
                    //        break;
                    //    case 3:
                    //        opt_monthly.IsChecked = true;
                    //        break;
                    //}
                    Boolean a;
                    //Boolean.TryParse(curtask.ACTIVE, out a);
                    //chk_active.IsChecked = a;
                    //var t = ((List<string>)lst_mode_config.DataContext);t.Clear();
                    //var t = curtask.MODE_CONFIGURATIONS;
                    //lst_mode_config.Items.Refresh();
                    //lst_mode_config.DataContext = t;
                    //lst_holidays.DataContext = curtask.HOLLY_DAYS;
                    //txt_tags.Text = curtask.TAGS;
                    FindButtonState();
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }

        }

        private void btn_clearAll_Click(object sender, RoutedEventArgs e)
        {

            lst_mode_config.DataContext = null;


        }

        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lst_mode_config.Items.Count > 0)
                {
                    var i = lst_mode_config.SelectedIndex;
                    var s = (List<string>)lst_mode_config.DataContext;
                    if (s.Count > i && i >= 0) s.RemoveAt(i);
                    lst_mode_config.Items.Refresh();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message.ToString());
            }
        }

        private void cal_modedates_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cal_modedates_TouchDown(object sender, TouchEventArgs e)
        {


        }

        private void cal_modedates_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                if (opt_daily.IsChecked == true)
                {
                    var s = (List<string>)lst_mode_config.DataContext;
                    s.Add(cal_modedates.SelectedDate.Value.DayOfWeek.ToString());
                    lst_mode_config.Items.Refresh();

                }
                else if (opt_weekly.IsChecked == true)
                {
                    var s = (List<string>)lst_mode_config.DataContext;
                    s.Add(cal_modedates.SelectedDate.Value.DayOfWeek.ToString());
                    lst_mode_config.Items.Refresh();
                }
                else if (opt_monthly.IsChecked == true)
                {
                    var s = (List<string>)lst_mode_config.DataContext;
                    s.Add(cal_modedates.SelectedDate.Value.DayOfWeek.ToString());
                    lst_mode_config.Items.Refresh();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message.ToString());
            }
        }

        private void btn_playTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (curtask != null)
                {
                    con = public_members._OpenConnection();
                    if (con != null)
                    {
                        SqlCommand cmd = new SqlCommand("");
                        String sql = null;
                        bool res = false;
                        switch (curtask.ENTRY)
                        {
                            case "PAYMENT":
                                //payment p = new payment();
                                //res = p.Tasker(curtask.ID, curtask.ENO);
                                break;
                            case "RECEIPT":
                                Receipt r = new Receipt();
                                //res = r.Tasker(curtask.ENO, curtask.ID);
                                break;
                            case "JOURNAL":
                                journalposting j = new journalposting();
                                //res = j.Tasker(curtask.ENO, curtask.ID);
                                break;
                            case "PAYROLL VOUCHER":
                                payroll_payments_deductions pp = new payroll_payments_deductions();
                                //res = pp.Tasker(curtask.ENO, curtask.ID, public_members._sysDate[0]);
                                break;
                        }

                        if (res == true)
                        {
                            MessageBox.Show("Your Task has been executed");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Data Base connection Lost");
                    }
                    con.Close();
                }
                else
                {
                    MessageBox.Show("Task may be Deleted");
                }
            }
            catch (Exception ee)
            {

                MessageBox.Show(ee.Message.ToString());
            }
        }
        private void btn_updateTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (curtask != null)
                {

                    MessageBoxResult res1 = new MessageBoxResult();
                    res1 = MessageBox.Show("Do you want Update this Task [tag only]", "Task Master", MessageBoxButton.YesNo);
                    if (res1 == MessageBoxResult.Yes)
                    {

                        con = public_members._OpenConnection();
                        if (con != null)
                        {
                            SqlCommand cmd = new SqlCommand("");
                            String sql = null;
                            bool res = false;
                            string tag = "";
                            int act = 0;
                            if (chk_active.IsChecked == true) { act = 1; }
                            tag = txt_tags.Text.ToString();
                            cmd = new SqlCommand("update recurrings set tag='" + tag + "' where id=" + curtask.ID, con);
                            cmd.ExecuteNonQuery();

                            //switch (curtask.ENTRY)
                            //{
                            //    case "PAYMENT":
                            //        cmd = new SqlCommand("update payments set p_isrecurring="+act+" where p_no=" + curtask.ENO, con);
                            //        cmd.ExecuteNonQuery();

                            //        public_members.Refresh_Payments();
                            //        break;
                            //    case "RECEIPT":
                            //        cmd = new SqlCommand("update receipts set r_isrecurring="+act+" where r_no=" + curtask.ENO, con);
                            //        cmd.ExecuteNonQuery();

                            //        public_members.Refresh_Receipts();
                            //        break;
                            //    case "JOURNAL":
                            //        cmd = new SqlCommand("update journal_entry set j_isrecurring="+act+" where j_no=" + curtask.ENO, con);
                            //        cmd.ExecuteNonQuery();

                            //        public_members.Refresh_Journals();
                            //        break;
                            //}


                            con.Close();
                            MessageBox.Show("Task Updated");
                            NewButtonState();
                            FetchTasks();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please double click task to load");
                }


            }
            catch (Exception ee)
            {

                MessageBox.Show(ee.Message.ToString());
            }
        }

        private void btn_deleteTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (curtask != null)
                {

                    MessageBoxResult res1 = new MessageBoxResult();
                    res1 = MessageBox.Show("Do you want Delete this Task", "Task Master", MessageBoxButton.YesNo);
                    if (res1 == MessageBoxResult.Yes)
                    {

                        con = public_members._OpenConnection();
                        if (con != null)
                        {
                            SqlCommand cmd = new SqlCommand("");
                            String sql = null;
                            bool res = false;
                            switch (curtask.ENTRY)
                            {
                                case "PAYMENT":
                                    cmd = new SqlCommand("update payments set p_isrecurring=0 where p_no=" + curtask.ENO, con);
                                    cmd.ExecuteNonQuery();
                                    cmd = new SqlCommand("update payments set p_taskid=0 where p_taskid=" + curtask.ID, con);
                                    cmd.ExecuteNonQuery();
                                    res = true;
                                    public_members.Refresh_Payments();
                                    break;
                                case "RECEIPT":
                                    cmd = new SqlCommand("update receipts set r_isrecurring=0 where r_no=" + curtask.ENO, con);
                                    cmd.ExecuteNonQuery();
                                    cmd = new SqlCommand("update receipts set r_taskid=0 where r_taskid=" + curtask.ID, con);
                                    cmd.ExecuteNonQuery();
                                    res = true;
                                    public_members.Refresh_Receipts();
                                    break;
                                case "JOURNAL":
                                    //cmd = new SqlCommand("update journal_entry set j_isrecurring=0 where j_no=" + curtask.ENO, con);
                                    //cmd.ExecuteNonQuery();
                                    //cmd = new SqlCommand("update journal_entry set j_taskid=0 where j_taskid=" + curtask.ID, con);
                                    cmd.ExecuteNonQuery();
                                    res = true;
                                    public_members.Refresh_Journals();
                                    break;
                            }
                            cmd = new SqlCommand("delete from recurrings where id=" + curtask.ID, con);
                            cmd.ExecuteNonQuery();

                            if (res == true)
                            {

                                MessageBox.Show("Your Task has been Deleted");
                                FetchTasks();
                                NewButtonState();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Data Base connection Lost");
                        }
                        con.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Please double click task to load");
                }

            }
            catch (Exception ee)
            {

                MessageBox.Show(ee.Message.ToString());
            }
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            NewButtonState();
            source = null;
            taskGrid.ItemsSource = source;
            FetchTasks();
        }

        private void lst_mode_config_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
