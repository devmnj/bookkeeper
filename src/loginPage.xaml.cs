using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
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
    /// Interaction logic for loginPage.xaml
    /// </summary>
    public partial class loginPage : Page
    {
        System.Data.SqlClient.SqlConnection Connection = new System.Data.SqlClient.SqlConnection();
        public loginPage()
        {
            InitializeComponent();
           
            cmb_eid.Focus();
        }

        private void btn_out_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    if (cmb_eid.Text != null && cmb_eid.Text.Length > 0)
            //    {
            //        var emp = public_members.employees.Select("eid='" + cmb_eid.Text.ToString() + "'");
            //        if (emp.Length > 0)
            //        {
            //            var att = public_members.payroll_att.Select("eid='" + emp[0]["eid"] + "' and indate='" + public_members._sysDate[0] + "'");
            //            if (att.Length > 0)
            //            {
            //                var lout = public_members.payroll_att.Select("eid='" + emp[0]["eid"] + "' and outdate='" + public_members._sysDate[0] + "'");

            //                if (lout.Length > 0)
            //                {
            //                    MessageBox.Show("You are already loged out");
            //                }
            //                else
            //                {
            //                    Connection = public_members._OpenConnection();
            //                    if (Connection != null)
            //                    {
            //                        SqlParameter outdate = new SqlParameter("@outdate", System.Data.SqlDbType.DateTime); outdate.Direction = System.Data.ParameterDirection.Input;
            //                        outdate.Value = public_members._sysDate[0];
            //                        SqlCommand cmd = new SqlCommand("update payroll_att set outdate=@outdate where eid='" + cmb_eid.Text.ToString() + "'", Connection);
            //                        cmd.Parameters.Add(outdate);
            //                        cmd.ExecuteNonQuery();
            //                        Connection.Close();
            //                        public_members.Refresh_payroll_att();
            //                        MessageBox.Show("You are loged Out");
            //                        cmb_eid.Text = "";
            //                        cmb_eid.Focus();
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                MessageBox.Show("You Not loged in");
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("You are not a Authorised Employee, contact superiors");
            //        }
            //    }
            //    else
            //    { MessageBox.Show("Enter Your Eid correctly"); cmb_eid.Focus(); }
            //}
            //catch (Exception r)
            //{

            //    MessageBox.Show(r.Message.ToString());
            //}
        }

        private void btn_in_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    if (cmb_eid.Text != null && cmb_eid.Text.Length > 0)
            //    {
            //        var emp = public_members.employees.Select("eid='" + cmb_eid.Text.ToString() + "'");
            //        if (emp.Length > 0)
            //        {
            //            var att = public_members.payroll_att.Select("eid='" + emp[0]["eid"] + "' and indate='" + public_members._sysDate[0] + "'");
            //            if (att.Length > 0)
            //            {
            //                MessageBox.Show("You already loged in");
            //            }
            //            else
            //            {
            //                Connection = public_members._OpenConnection();
            //                if (Connection != null)
            //                {
            //                    SqlParameter eid = new SqlParameter("@eid", System.Data.SqlDbType.VarChar); eid.Direction = System.Data.ParameterDirection.Input; eid.Value = cmb_eid.Text.ToString();
            //                    SqlParameter indate = new SqlParameter("@indate", System.Data.SqlDbType.DateTime); indate.Direction = System.Data.ParameterDirection.Input; indate.Value = public_members._sysDate[0];

            //                    SqlCommand cmd = new SqlCommand("insert into payroll_att (eid,indate) values(@eid,@indate)", Connection);
            //                    cmd.Parameters.Add(eid);
            //                    cmd.Parameters.Add(indate);
            //                    cmd.ExecuteNonQuery();
            //                    Connection.Close();
            //                    public_members.Refresh_payroll_att();
            //                    MessageBox.Show("You are loged In");
            //                    cmb_eid.Text = "";
            //                    cmb_eid.Focus();
            //                }
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("You are not a Authorised Employee, contact superiors");
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Enter Your Eid correctly");
            //        cmb_eid.Focus();
            //    }
            //}

            //catch (Exception e11)
            //{

            //    MessageBox.Show(e11.Message.ToString());
            //}
        }

        private void cmb_eid_KeyDown(object sender, KeyEventArgs e)
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
    }
}
