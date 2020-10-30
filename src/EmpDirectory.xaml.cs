using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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
    /// Interaction logic for EmpDirectory.xaml
    /// </summary>
    public partial class EmpDirectory : Window
    {
        SqlConnection con = new SqlConnection();
        DataTable employees = new DataTable();
        DataTable departs = new DataTable();
        ObservableCollection<Employee> emp_observer = new ObservableCollection<Employee>();

        public EmpDirectory()
        {
            InitializeComponent();
            con = public_members._OpenConnection();
            if (con != null)
            {
                try
                {
                    employees = public_members._Fetch("select emp.isdailywager,dep.department,emp.doj,emp.designation,emp.basicpay,led.l_name,l_city,l_mob from " +
                      "emp_registration emp inner join ledgers led  on emp.lid = led.id inner join department_registration dep  on  emp.dep_id = dep.id", con);
                    departs = public_members._Fetch("select department from department_registration", con);
                    var cities = from em in employees.AsEnumerable() select em.Field<string>("l_city");
                    var jobs = (from em in employees.AsEnumerable() select em.Field<string>("designation")).Distinct();
                    //var cols = (from t in employees.AsEnumerable().GetFields() select t.Name);
                    //string[] columnNames = dt.Columns.Cast<DataColumn>()
                    //             .Select(x => x.ColumnName)
                    //             .ToArray();
                    //var cols = (from dc in employees.Columns.Cast<DataColumn>()
                    //            select dc.ColumnName).ToList();
                    //cmb_orderby.DataContext = cols.ToList();
                    var departments = from em in departs.AsEnumerable() select em.Field<string>("department");


                    cmb_departments.DataContext = departments.ToList();
                    cmb_cities.DataContext = cities.ToList();
                    cmb_jobs.DataContext = jobs.ToList();

                    //IOrderedEnumerable<DataRow> rows1=  employees.Select().OrderBy(x  =>x.Field<string>("department"));
                    foreach (DataRow rows in employees.Rows)
                    {
                        emp_observer.Add(new Employee()
                        {
                            Name = rows["l_name"].ToString(),
                            Designation = rows["designation"].ToString(),
                            Department = rows["department"].ToString(),
                            City = rows["l_city"].ToString(),
                            BasicPay = rows["basicpay"].ToString(),
                            IsDaily = (bool)rows["isdailywager"],
                            Contact = rows["l_mob"].ToString(),
                            Doj = Convert.ToDateTime(rows["doj"].ToString()),
                        });
                    }
                    var cols = from d in emp_observer.AsEnumerable() select d;

                    List<string> cols1 = new List<string>();

                    cols1.Add("Name");
                    cols1.Add("Designation");
                    cols1.Add("Department");
                    cols1.Add("City");
                    cols1.Add("BasicPay");
                    cols1.Add("Contact");
                    cols1.Add("Doj");


                    cmb_orderby.DataContext = cols1;
                    cmb_orderby.Text = "Name";
                    //IOrderedEnumerable<Employee> sorted =   emp_observer.OrderBy(z => z.Name);
                    var sorted = FuncOrderBy(emp_observer, cmb_orderby.Text.ToString());
                    ListCollectionView emp_colletionview = new ListCollectionView(sorted);
                    //ListCollectionView emp_colletionview = new ListCollectionView(emp_observer);
                    //emp_colletionview.Filter = (e) =>
                    //{
                    //    Employees emp1 = e as Employees;
                    //    if (emp1.Depart=="IT")
                    //        return true;
                    //    return false;

                    //};             
                    emp_grid.ItemsSource = emp_colletionview;
                }
                catch { }
            }
        }

        private void cmb_cities_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ListCollectionView emp_colletionview = new ListCollectionView(emp_observer);
                if (cmb_cities.Text.Trim().Length > 0)
                {

                    emp_colletionview.Filter = (e1) =>
                    {
                        Employee emp1 = e1 as Employee;
                        if (emp1.City == cmb_cities.Text.ToString())
                            return true;
                        return false;

                    };
                }
                emp_grid.ItemsSource = emp_colletionview;
            }
        }

        private void cmb_departments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ListCollectionView emp_colletionview = new ListCollectionView(emp_observer);
                if (cmb_departments.Text.Trim().Length > 0)
                {

                    emp_colletionview.Filter = (e1) =>
                    {
                        Employee emp1 = e1 as Employee;
                        if (emp1.Department == cmb_departments.Text.ToString())
                            return true;
                        return false;

                    };
                }
                emp_grid.ItemsSource = emp_colletionview;
            }
        }

        private void cmb_jobs_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ListCollectionView emp_colletionview = new ListCollectionView(emp_observer);
                if (cmb_jobs.Text.Trim().Length > 0)
                {

                    emp_colletionview.Filter = (e1) =>
                    {
                        Employee emp1 = e1 as Employee;
                        if (emp1.Designation == cmb_jobs.Text.ToString())
                            return true;
                        return false;

                    };
                }
                emp_grid.ItemsSource = emp_colletionview;
            }
        }

        private void cmb_orderby_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (cmb_orderby.Text.ToString().Length > 0)
                {


                    List<Employee> sorted = FuncOrderBy(emp_observer, cmb_orderby.Text.ToString());
                    ListCollectionView emp_colletionview = new ListCollectionView(sorted);

                    emp_grid.ItemsSource = emp_colletionview;
                }
                else
                {

                }
            }
            catch (SqlException rr)
            {
                MessageBox.Show(rr.Message.ToString());
            }
        }
        private List<Employee> FuncOrderBy(ObservableCollection<Employee> em, string OrderString)
        {
            IOrderedEnumerable<Employee> sorted = null;
            switch (OrderString)
            {
                case "Name":
                    sorted = emp_observer.OrderBy(z => z.Name);
                    break;
                case "Designation":
                    sorted = emp_observer.OrderBy(z => z.Designation);
                    break;
                case "Department":
                    sorted = emp_observer.OrderBy(z => z.Department);
                    break;
                case "City":
                    sorted = emp_observer.OrderBy(z => z.City);
                    break;
                case "BasicPay":
                    sorted = emp_observer.OrderBy(z => z.BasicPay);
                    break;
                case "Doj":
                    sorted = emp_observer.OrderBy(z => z.Doj);
                    break;
                case "Contact":
                    break;
            }
            return sorted.ToList();
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

    class Employee
    {
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string BasicPay { get; set; }
        public bool IsDaily { get; set; }
        public string Contact { get; set; }
        public DateTime Doj { get; set; }



    }
}
