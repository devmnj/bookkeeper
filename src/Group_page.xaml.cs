using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace accounts
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class groupreg : Page
    {
        DataTable groups;
        SqlConnection con = new SqlConnection();
        public groupreg()
        {
            InitializeComponent();
          
            loadparents();
            NewButtonState();
        }
        void loadparents()
        {
            
            con = public_members._OpenConnection();
            if (con != null)
            {
                groups = public_members._Fetch("select * from groups", con);
                List<string> str = new List<string>();
                var auttext = from g in groups.AsEnumerable() select g.Field<string>("g_name");
                sub_list.DataContext =auttext.ToList<string>();     
                con.Close();               
                
                
               
            }
            else
            {
                MessageBox.Show("Server Not found");
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        void FindButtonState()
        {
            btn_save.IsEnabled= false;
           // btn_edit.IsEnabled = true;
            btn_del.IsEnabled = true;

        }
        void NewButtonState()
        {
            btn_save.IsEnabled = true;
           // btn_edit.IsEnabled = false;
            btn_del.IsEnabled = false;

        }

        private void txt_gname_KeyDown(object sender, KeyEventArgs e)
        {
            DataTable g = new DataTable();
            if (e.Key == Key.Enter && txt_gname.Text.ToString().Trim().Length >= 1 && btn_save.IsEnabled == true)
            {
                con = public_members._OpenConnection();
                if (con != null)
                {
                    g = public_members._Fetch("select * from groups", con);                    
                    con.Close();                   
                }
                else
                {
                    MessageBox.Show("Server Not found");
                }

                var dv = g.AsDataView();
                var parentdv = g.AsDataView();
                dv.RowFilter = "g_name='" + txt_gname.Text.ToString().Trim() + "'";
                //DataRow[] rows = g.Select("g_name='" + txt_gname.Text.ToString().Trim() + "'");

                if (dv.Count>= 1)
                {
                    long id = Convert.ToInt64(dv[0]["g_parent"]);

                    parentdv.RowFilter = "ID=" + id  ;
                    if (parentdv.Count >= 1)
                    {
                        string p = parentdv[0]["g_name"].ToString();
                        var loc = sub_list.Items.IndexOf(p);
                        sub_list.SelectedIndex = loc;
                    }
                    else if (id == 0)
                    {
                        var loc = sub_list.Items.IndexOf(txt_gname.Text.ToUpper());
                        sub_list.SelectedIndex = loc;
                      
                    }
                    else
                    { 
                        MessageBox.Show("Parent Missing");
                    }
                    txt_crlock.Text = Convert.ToString(dv[0]["g_cr_lock"]);
                    txt_drlock.Text = Convert.ToString(dv[0]["g_dr_lock"]);
                    txt_disc.Text = Convert.ToString(dv[0]["g_maxdisc"]);
                    txt_code.Text = Convert.ToInt64(dv[0]["id"]).ToString();
                    FindButtonState();
                }
                else
                {
                    MessageBox.Show("No such Group Registered");
                    btn_reset_Click(sender, e);
                }
            }
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            txt_code.Text = "";
            txt_crlock.Text = "";
            txt_disc.Text = "";
            txt_drlock.Text = "";
            txt_gname.Text = "";
            txt_gname.Focus();
            NewButtonState();
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
