using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace accounts
{
    /// <summary>
    /// Interaction logic for EmployeeRegistration.xaml
    /// </summary>
    public partial class EmployeeRegistration : Window
    {

        public EmployeeRegistration()
        {
            InitializeComponent();
        }
        void FindButtonState()
        {
            btn_save.IsEnabled = false;
            btn_update.IsEnabled = true;
            btn_del.IsEnabled = true;
            dtp_jdate.Focus();
        }


        void NewButtonState()
        {
            txt_eid.Text = "";
            txt_basicpay.Text = "";
            txt_city.Text = "";
            txt_compercentage.Text = "";
            txt_houseno.Text = "";
            txt_mobile.Text = "";
            txt_place.Text = "";
            txt_post.Text = "";
            txt_resino.Text = "";
            cmb_edepartment.Text = "";
            cmb_edesignation.Text = "";
            cmb_ename.Text = "";
            chk_daily.IsChecked = false;
            dtp_jdate.SelectedDate = ViewModels_Variables._sysDate[0];
            txt_eid.Text = DB.Connection.NewEntryno("ledgers", "id").ToString();


            btn_save.IsEnabled = true;
            btn_update.IsEnabled = false;
            btn_del.IsEnabled = false;
            dtp_jdate.Focus();
        }
        public void Find(int n)
        {

            try
            {

                var employees = ViewModels_Variables.ModelViews.Employees.Where((em) => em.Id == n).FirstOrDefault();

                if (employees != null)
                {
                    //txt_eid.Text  = employees.Rows[0]["eid"].ToString();
                    dtp_jdate.SelectedDate = employees.DOJ;
                    var dep = ViewModels_Variables.ModelViews.Departments.Where((dp) => dp.Dep_id == employees.Department.Dep_id).FirstOrDefault();
                    if (dep != null)
                    {
                        cmb_edepartment.SelectedItem = dep;
                    }

                    cmb_edesignation.Text = employees.Desig;
                    txt_basicpay.Text = employees.Basic.ToString("0.00");
                    txt_compercentage.Text = employees.Comm.ToString("0.00");
                    chk_daily.IsChecked = employees.IsDailyWager;

                    var ledger = ViewModels_Variables.ModelViews.Accounts.Where((acc) => acc.ID == employees.Account.ID).FirstOrDefault();
                    if (ledger != null)
                    {
                        cmb_ename.Text = ledger.Name;
                        txt_houseno.Text = ledger.Address;
                        txt_city.Text = ledger.City;
                        txt_mobile.Text = ledger.Mob;
                        txt_resino.Text = ledger.PhoneNo;
                    }
                    FindButtonState();
                }


                else
                {
                    MessageBox.Show("Employye Not Registered");
                }

            }

            catch (SqlException ee)
            {
                MessageBox.Show(ee.Message.ToString());
            }
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                NewButtonState();
            }
            catch
            {
                MessageBox.Show("Sysytem Date Error");
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (cmb_edepartment.Text.Length > 0 && cmb_edesignation.Text.Length > 0 && cmb_ename.Text.Length > 0)
                {

                    Model.AccountModel account = new Model.AccountModel();
                    Model.DepartmentModel department = new Model.DepartmentModel();
                    Model.EmployeeModel employee = new Model.EmployeeModel();

                    account.Name = cmb_ename.Text.ToUpper().Trim().ToUpper();
                    account.Short_Name = cmb_ename.Text.ToString().Trim().ToUpper();
                    account.Address = txt_houseno.Text.ToString().ToUpper().Trim();
                    account.City = txt_city.Text.ToString().ToUpper().Trim();
                    account.Mob = txt_mobile.Text.ToString().Trim();
                    account.PhoneNo = txt_resino.Text.ToString();
                    account.Catagory = "None";

                    var slctd_dep = (Model.DepartmentModel)cmb_edepartment.SelectedItem;
                    if (slctd_dep != null && cmb_edepartment.Text == slctd_dep.Name)
                    {
                        department = slctd_dep;
                    }
                    else
                    {
                        department.Name = cmb_edepartment.Text.ToString().ToUpper();
                        department.Narration = "";
                        department.Dep_Head = "";
                        department.Dep_id = 0;
                    }
                    employee.Account = account;
                    employee.Department = department;
                    employee.Emp_Id = txt_eid.Text.ToString();
                    employee.IsDailyWager = (bool)chk_daily.IsChecked;
                    double.TryParse(txt_basicpay.Text.ToString(), out double bp);
                    employee.Basic = bp;
                    double.TryParse(txt_compercentage.Text.ToString(), out double com);
                    employee.Comm = com;
                    employee.Desig = cmb_edesignation.Text.ToString().ToUpper();
                    employee.DOJ = dtp_jdate.SelectedDate.Value;

                    //Employee Reg
                    var r = DB.Employee.Save(employee: employee);

                    if (r > 0)
                    {
                        MessageBox.Show("Employee Registered");
                        btn_Reset_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Somethinh went wrong");
                    }
                }
                else
                {
                    MessageBox.Show("Enter data correctly");
                }
            }
            catch (SqlException e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }

        }

        private void txt_eid_KeyDown(object sender, KeyEventArgs e)
        {
            int n;
            int.TryParse(txt_eid.Text.ToString(), out n);
            if (e.Key == Key.Enter && txt_eid.Text.Trim().Length > 0)
            {
                Find(n);
            }
        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var slctd_emp = (Model.EmployeeModel)cmb_ename.SelectedItem;

                if (cmb_edepartment.Text.Length > 0 && cmb_edesignation.Text.Length > 0 && cmb_ename.Text.Length > 0 && slctd_emp != null)
                {

                    Model.AccountModel account = new Model.AccountModel();
                    
                    Model.EmployeeModel employee = slctd_emp;


                    var acct = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.ID == employee.Account.ID).FirstOrDefault();
                    if (acct != null) account = acct;
                    account.Name = cmb_ename.Text.ToUpper().Trim().ToUpper();
                    account.Short_Name = cmb_ename.Text.ToString().Trim().ToUpper();
                    account.Address = txt_houseno.Text.ToString().ToUpper().Trim();
                    account.City = txt_city.Text.ToString().ToUpper().Trim();
                    account.Mob = txt_mobile.Text.ToString().Trim();
                    account.PhoneNo = txt_resino.Text.ToString();
                    account.Catagory = "None";
                    if (cmb_edepartment.SelectedItem != null)
                    {
                        employee.Department = (Model.DepartmentModel)cmb_edepartment.SelectedItem;
                    }
                    else
                    {

                        employee.Department.Name = cmb_edepartment.Text.ToString().ToUpper();
                        employee.Department.Narration = "";
                        employee.Department.Dep_Head = "";
                        employee.Department.Dep_id = 0;
                    }

                    employee.Account = account;
                    employee.Emp_Id = txt_eid.Text.ToString();
                    employee.IsDailyWager = (bool)chk_daily.IsChecked;
                    double.TryParse(txt_basicpay.Text.ToString(), out double bp);
                    employee.Basic = bp;
                    double.TryParse(txt_compercentage.Text.ToString(), out double com);
                    employee.Comm = com;
                    employee.Desig = cmb_edesignation.Text.ToString().ToUpper();
                    employee.DOJ = dtp_jdate.SelectedDate.Value;

                    //Employee Reg
                    var r = DB.Employee.Update(employee: employee);

                    if (r ==true)
                    {
                        MessageBox.Show("Employee Information Updated");
                        btn_Reset_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Somethinh went wrong");
                    }
                }
                else
                {
                    MessageBox.Show("Enter data correctly");
                }
            }
            catch (SqlException e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }

        }

        private void btn_del_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmb_ename.SelectedItem!=null)
                {

                    var slctd = (Model.EmployeeModel)cmb_ename.SelectedItem;
                    if (slctd != null)
                    {
                        var rr = DB.Employee.Remove(slctd.Id);
                        if (rr == true)
                        {
                            MessageBox.Show("Employee Removed");
                            btn_Reset_Click(sender, e);

                        }
                        else
                        {
                            MessageBox.Show("Something went wrong");
                        }
                    }


                }
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

        private void btn_report_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        public void TabToSave()
        {

            if (btn_save.IsEnabled == true)
            {
                btn_save.Focus();
            }
            else
            {
                btn_update.Focus();
            }
        }

        private void txt_resino_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    TabToSave();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void txt_place_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
             
                if (ViewModels_Variables.ModelViews.Employees.Count <= 0)
                {
                    DB.Employee.Fetch();
                    ViewModels_Variables.ModelViews.Employees_To_List();
                }

                var v = ViewModels_Variables.ModelViews;
                DataContext = v;
                NewButtonState();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void cmb_ename_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void cmb_ename_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var slctd = (Model.EmployeeModel)cmb_ename.SelectedItem;
                if (slctd != null)
                {


                    var employees = ViewModels_Variables.ModelViews.Employees.Where((em) => em.Id == slctd.Id).FirstOrDefault();

                    if (employees != null)
                    {
                        NewButtonState();
                        txt_eid.Text = employees.Emp_Id;
                        dtp_jdate.SelectedDate = employees.DOJ;
                        var dep = ViewModels_Variables.ModelViews.Departments.Where((dp) => dp.Dep_id == employees.Department.Dep_id).FirstOrDefault();
                        if (dep != null)
                        {
                            cmb_edepartment.SelectedItem = dep;
                        }

                        cmb_edesignation.Text = employees.Desig;
                        txt_basicpay.Text = employees.Basic.ToString("0.00");
                        txt_compercentage.Text = employees.Comm.ToString("0.00");
                        chk_daily.IsChecked = employees.IsDailyWager;

                        var ledger = ViewModels_Variables.ModelViews.Accounts.Where((acc) => acc.ID == employees.Account.ID).FirstOrDefault();
                        if (ledger != null)
                        {
                            cmb_ename.Text = ledger.Name;
                            txt_houseno.Text = ledger.Address;
                            txt_city.Text = ledger.City;
                            txt_mobile.Text = ledger.Mob;
                            txt_resino.Text = ledger.PhoneNo;
                        }
                        FindButtonState();
                    }


                    else
                    {
                        MessageBox.Show("Employye Not Registered");
                    }

                }
            }

            catch (SqlException ee)
            {
                MessageBox.Show(ee.Message.ToString());
            }
        }
    }
}
