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
using System.Windows.Shapes;

namespace accounts
{
    /// <summary>
    /// Interaction logic for ServiceRegistration.xaml
    /// </summary>
    public partial class ServiceRegistration : Window
    {
        List<int> p_nos = new List<int>();
        int cindex = 0;
        public ServiceRegistration()
        {
            InitializeComponent();
            NewButtonState();
            cmb_servicename.ItemsSource = public_members.servicename_itemsource;
            cmb_catagory.ItemsSource = public_members.servicecatagory_itemsource;
            cmb_scode.ItemsSource = public_members.servicecode_itemsource;
            cmb_brand.ItemsSource = public_members.servicebrand_itemsource;
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
        public void Find(int slno)
        {
            try
            {
                var sr = public_members.service_registration.Select("id=" + slno);
                if (sr.Length > 0)
                {
                    NewButtonState();
                    txt_slno.Text = sr[0]["id"].ToString();
                    txt_unitp1.Text = string.Format("{0:0.00}", sr[0]["unitprice1"]);
                    cmb_brand.Text = string.Format("{0:0.00}", sr[0]["brand"]);
                    cmb_catagory.Text = sr[0]["catagory"].ToString();
                    cmb_scode.Text = sr[0]["code"].ToString();
                    cmb_servicename.Text = sr[0]["service"].ToString();
                    FindButtonState();

                }
                else
                {
                    MessageBox.Show("Service Registration Not Found");
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }
        public void Find(string code)
        {
            try
            {
                if (code.Length > 0)
                {
                    var sr = public_members.service_registration.Select("code='" + code + "'");
                    if (sr.Length > 0)
                    {
                        NewButtonState();
                        txt_slno.Text = sr[0]["id"].ToString();
                        txt_unitp1.Text = string.Format("{0:0.00}", sr[0]["unitprice1"]);
                        cmb_brand.Text = string.Format("{0:0.00}", sr[0]["brand"]);
                        cmb_catagory.Text = sr[0]["catagory"].ToString();
                        cmb_scode.Text = sr[0]["code"].ToString();
                        cmb_servicename.Text = sr[0]["service"].ToString();
                        FindButtonState();
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }public void Find2(string service)
        {
            try
            {
                if (service.Length > 0)
                {
                    var sr = public_members.service_registration.Select("service='" + service + "'");
                    if (sr.Length > 0)
                    {
                        NewButtonState();
                        txt_slno.Text = sr[0]["id"].ToString();
                        txt_unitp1.Text = string.Format("{0:0.00}", sr[0]["unitprice1"]);
                        cmb_brand.Text = string.Format("{0:0.00}", sr[0]["brand"]);
                        cmb_catagory.Text = sr[0]["catagory"].ToString();
                        cmb_scode.Text = sr[0]["code"].ToString();
                        cmb_servicename.Text = sr[0]["service"].ToString();
                        FindButtonState();
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }
        void NewButtonState()
        {
            btn_del.IsEnabled = false;
            btn_save.IsEnabled = true;
            btn_update.IsEnabled = false;
            txt_unitp1.Text = "0.00";

            cmb_brand.Text = "";
            cmb_catagory.Text = "";
            txt_id.Text = "";
            cmb_servicename.Text = "";
            cmb_scode.Text = "";
            txt_slno.Text = public_members.GetSerialNo("service_registration", "id").ToString();
            p_nos = (from p in public_members.service_registration.AsEnumerable() select p.Field<int>("id")).ToList();
            cmb_scode.Focus();
        }
        void FindButtonState()
        {
            btn_del.IsEnabled = true;
            btn_save.IsEnabled = false;
            btn_update.IsEnabled = true;
            cmb_scode.Focus();
        }

        private void btn_moveprevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (p_nos.Count > 0 && cindex >= 0 && cindex < p_nos.Count)
                {
                    if (cindex != 0) { cindex--; }

                    Find(p_nos[cindex]);
                }


                else
                {
                    MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                MessageBox.Show(perror.Message.ToString());
            }
        }

        private void btn_movenext_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (p_nos.Count > 0 && cindex < p_nos.Count - 1)
                {

                    cindex++;

                    Find(p_nos[cindex]);
                }


                else
                {
                    MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                MessageBox.Show(perror.Message.ToString());
            }
        }

        private void btn_movelast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (p_nos.Count > 0)
                {
                    int pno = p_nos[p_nos.Count - 1];
                    cindex = p_nos.Count - 1;
                    Find(pno);
                }


                else
                {
                    MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                MessageBox.Show(perror.Message.ToString());
            }
        }

        private void btn_movefirst_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (p_nos.Count > 0)
                {
                    int pno = p_nos[0];
                    cindex = 0;
                    Find(pno);
                }


                else
                {
                    MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                MessageBox.Show(perror.Message.ToString());
            }
        }

        private void txt_rnofind_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if(txt_rnofind.Text!=null && txt_rnofind.Text.Length>0 && e.Key == Key.Enter) { Find(Convert.ToInt32(txt_rnofind.Text.ToString())); }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewButtonState();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmb_servicename.Text != null && cmb_servicename.Text.Length > 0)
                {
                    SqlConnection con = new SqlConnection();
                    SqlParameter service = new SqlParameter("@service", System.Data.SqlDbType.VarChar); service.Direction = System.Data.ParameterDirection.Input;
                    SqlParameter catagory = new SqlParameter("@catagory", System.Data.SqlDbType.VarChar); catagory.Direction = System.Data.ParameterDirection.Input;
                    SqlParameter scode = new SqlParameter("@scode", System.Data.SqlDbType.VarChar); scode.Direction = System.Data.ParameterDirection.Input;
                    SqlParameter price1 = new SqlParameter("@unitprice1", System.Data.SqlDbType.Float); price1.Direction = System.Data.ParameterDirection.Input;
                    SqlParameter brand = new SqlParameter("@brand", System.Data.SqlDbType.VarChar); brand.Direction = System.Data.ParameterDirection.Input;
                    con = public_members._OpenConnection();
                    if (con != null)
                    {
                        SqlCommand cmd = new SqlCommand("insert into service_registration ([service],[catagory],[unitprice1],[brand],[code])  values(@service,@catagory,@unitprice1,@brand,@scode)", con);
                        service.Value = cmb_servicename.Text.ToString().ToUpper();
                        catagory.Value = cmb_catagory.Text.ToString().ToUpper();
                        scode.Value = cmb_scode.Text.ToString().ToUpper();
                        price1.Value = Convert.ToDouble(txt_unitp1.Text.ToString());
                        brand.Value = cmb_brand.Text.ToUpper().ToString();
                        cmd.Parameters.Add(service);
                        cmd.Parameters.Add(catagory);
                        cmd.Parameters.Add(scode);
                        cmd.Parameters.Add(price1);
                        cmd.Parameters.Add(brand);

                        int r = cmd.ExecuteNonQuery();
                        con.Close();
                        if (r != 0)
                        {
                            MessageBox.Show("Service Created");
                            btn_Reset_Click(sender, e);
                            public_members.Refresh_ServiceReigistration();
                        }
                    }


                }
                else
                {
                    MessageBox.Show("Enter data correctly");
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var services = public_members.service_registration.Select("id=" + txt_slno.Text.ToString());
                if (services.Length > 0)
                {
                    MessageBoxResult res = new MessageBoxResult();
                    res = MessageBox.Show("Do you want Edit this Service", "Update Service", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        if (cmb_servicename.Text != null && cmb_servicename.Text.Length > 0)
                        {
                            SqlConnection con = new SqlConnection();
                            SqlParameter service = new SqlParameter("@service", System.Data.SqlDbType.VarChar); service.Direction = System.Data.ParameterDirection.Input;
                            SqlParameter catagory = new SqlParameter("@catagory", System.Data.SqlDbType.VarChar); catagory.Direction = System.Data.ParameterDirection.Input;
                            SqlParameter scode = new SqlParameter("@scode", System.Data.SqlDbType.VarChar); scode.Direction = System.Data.ParameterDirection.Input;
                            SqlParameter price1 = new SqlParameter("@unitprice1", System.Data.SqlDbType.Float); price1.Direction = System.Data.ParameterDirection.Input;
                            SqlParameter brand = new SqlParameter("@brand", System.Data.SqlDbType.VarChar); brand.Direction = System.Data.ParameterDirection.Input;
                            con = public_members._OpenConnection();
                            if (con != null)
                            {
                                SqlCommand cmd = new SqlCommand("update service_registration set service=@service,catagory=@catagory,unitprice1=@unitprice1,barand=@brand,code=@scode  where id=" + txt_slno.Text.ToString(), con);

                                service.Value = cmb_servicename.Text.ToString().ToUpper();
                                catagory.Value = cmb_catagory.Text.ToString().ToUpper();
                                scode.Value = cmb_scode.Text.ToString().ToUpper();
                                price1.Value = Convert.ToDouble(txt_unitp1.Text.ToString());
                                brand.Value = cmb_brand.Text.ToUpper().ToString();
                                cmd.Parameters.Add(service);
                                cmd.Parameters.Add(catagory);
                                cmd.Parameters.Add(scode);
                                cmd.Parameters.Add(price1);
                                cmd.Parameters.Add(brand);

                                int r = cmd.ExecuteNonQuery();
                                con.Close();
                                if (r != 0)
                                {
                                    MessageBox.Show("Service Updated");
                                    btn_Reset_Click(sender, e);
                                    public_members.Refresh_ServiceReigistration();
                                }
                            }


                        }
                        else
                        {
                            MessageBox.Show("Enter data correctly");
                        }
                    }
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void btn_del_Click(object sender, RoutedEventArgs e)
        {
            //delete
            SqlCommand cmd;
            MessageBoxResult res = new MessageBoxResult();
            res = MessageBox.Show("Do you want delete this Service Registration", "Delete Service", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                SqlConnection con = new SqlConnection();
                con = public_members._OpenConnection();
                if (con != null)
                {
                    cmd = new SqlCommand("delete from service_registration where id=" + txt_slno.Text.ToString(), con);
                    cmd.ExecuteNonQuery();

                    public_members.Refresh_ServiceReigistration();


                    MessageBox.Show("Service entry Deleted");
                    btn_Reset_Click(sender, e);

                }
                else
                {
                    MessageBox.Show("DataBase connection lost");
                }

            }
        }

        private void btn_find_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txt_rnofind.Text != null && txt_rnofind.Text.Length > 0  ) { Find(Convert.ToInt32(txt_rnofind.Text.ToString())); }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmb_scode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if(cmb_scode.Text!=null && cmb_scode.Text.Length > 0 && e.Key==Key.Enter ) { Find(cmb_scode.Text.ToString()); }
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmb_servicename_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if(cmb_servicename.Text!=null && cmb_servicename.Text.Length>0 && e.Key == Key.Enter) { Find2(cmb_servicename.Text.ToString()); }
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmb_catagory_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_unitp1_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_unitp2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    public_members._TabPress(e);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmb_brand_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cmb_brand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmb_servicename_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
