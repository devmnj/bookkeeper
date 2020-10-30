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
    /// Interaction logic for PayrollADAL.xaml
    /// </summary>
    public partial class PayrollADAL : Window
    {
        SqlConnection con = new SqlConnection();
        DataTable employees = new DataTable();
        DataTable ledgers = new DataTable();
        public PayrollADAL()
        {
            InitializeComponent();
           
            loadledgers();
            dtp_post_date.SelectedDate = DateTime.Now;
            txt_entryno.Text = public_members.GetSerialNo("payroll_posting", "id").ToString();
            NewButtonState();
        }
        void NewButtonState()
        {
            btn_save.IsEnabled = true;
            btn_update.IsEnabled = false;
            btn_del.IsEnabled = false;
        }
        void FindButtonState()
        {
            btn_save.IsEnabled = false;
            btn_update.IsEnabled = true;
            btn_del.IsEnabled = true;

        }
        void loadledgers(bool autfill = true)
        {
            con = public_members._OpenConnection();
            if (con != null)
            {
                ledgers = public_members._Fetch("select * from ledgers order by id", con);
                employees = public_members._Fetch("select emp.lid, led.l_name,emp.eid from emp_registration emp inner join ledgers led on led.id=emp.lid  order by lid", con);
                var emps = from em in employees.AsEnumerable() select em.Field<string>("l_name");
                cmb_emp_name.DataContext = emps.ToList();
                con.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Server Not found");
            }
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
           
            dtp_post_date.SelectedDate = DateTime.Now;
            txt_entryno.Text = public_members.GetSerialNo("payroll_posting", "id").ToString();
            txt_amount.Text = "";
            txt_narration.Text = "";
            cmb_emp_name.Text = "";
            cmb_type.Text = ""; 
            NewButtonState();
        }
        public void Find(int n)
        {
            try
            {
                DataTable payroll_posting = new DataTable();
                con = public_members._OpenConnection();
                if (con != null)
                {
                    payroll_posting = public_members._Fetch("select * from payroll_posting where id=" + n, con);
                    if (payroll_posting.Rows.Count > 0)
                    {
                        txt_entryno.Text = payroll_posting.Rows[0]["id"].ToString();
                        dtp_post_date.SelectedDate = Convert.ToDateTime(payroll_posting.Rows[0]["post_date"].ToString());
                        DataRow[] emp = employees.Select("eid=" + payroll_posting.Rows[0]["eid"]);
                        if (emp.Length > 0)
                        {
                            cmb_emp_name.Text = public_members.LedgeName(emp[0]["lid"].ToString(), true);
                        }
                        cmb_type.Text = payroll_posting.Rows[0]["type"].ToString();
                        txt_amount.Text = payroll_posting.Rows[0]["amount"].ToString();
                        txt_narration.Text = payroll_posting.Rows[0]["narration"].ToString();
                        con.Close();
                        FindButtonState();
                    }
                    else
                    {
                        MessageBox.Show("Entry not found");
                    }

                }


            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            int emp_id = 0, emp_ledid = 0;
            DataRow[] rows = employees.Select();
            if (rows.Length > 0)
            {
                emp_id = Convert.ToInt32(rows[0]["eid"].ToString());
            }
            if (cmb_emp_name.Text.Length > 0 && cmb_type.Text.Length > 0 && txt_amount.Text.Length > 0 && emp_id > 0)
            {
                try
                {
                    con = public_members._OpenConnection();
                    if (con != null)
                    {
                        var sql = "insert into payroll_posting(post_date,eid,type,amount,narration) values(@pdate,@eid,@ptype,@amount,@narration)";
                        cmd.CommandText = sql;
                        cmd.Connection = con;
                        SqlParameter pdate = new SqlParameter("@pdate", SqlDbType.DateTime);
                        pdate.Direction = ParameterDirection.Input;
                        pdate.Value = dtp_post_date.SelectedDate;

                        SqlParameter eid = new SqlParameter("@eid", SqlDbType.Int);
                        eid.Direction = ParameterDirection.Input;
                        eid.Value = emp_id;

                        SqlParameter ptype = new SqlParameter("@ptype", SqlDbType.VarChar);
                        ptype.Direction = ParameterDirection.Input;
                        ptype.Value = cmb_type.Text.ToString();

                        SqlParameter narration = new SqlParameter("@narration", SqlDbType.VarChar);
                        narration.Direction = ParameterDirection.Input;
                        narration.Value = txt_narration.Text.ToString();

                        SqlParameter amount = new SqlParameter("@amount", SqlDbType.Float);
                        amount.Direction = ParameterDirection.Input;
                        float am;
                        float.TryParse(txt_amount.Text.ToString(), out am);
                        amount.Value = am;


                        cmd.Parameters.Add(pdate);
                        cmd.Parameters.Add(eid);
                        cmd.Parameters.Add(ptype);
                        cmd.Parameters.Add(amount);
                        cmd.Parameters.Add(narration);
                        int t = cmd.ExecuteNonQuery();
                        if (t > 0)
                        {
                            MessageBox.Show("Posting Succeeded"); btn_Reset_Click(sender, e);
                            con.Close();
                        }
                        else
                        {
                            MessageBox.Show("Entery not saved");
                        }
                    }
                }
                catch (SqlException sqle)
                {
                    MessageBox.Show(sqle.Message.ToString());
                }
            }
            else
            {
                MessageBox.Show("Enter data Correctly");
            }
        }

        private void txt_find_eno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txt_find_eno.Text.Length > 0)
                {
                    int.TryParse(txt_find_eno.Text.ToString(), out int en);
                    Find(en);
                }
                else
                {
                    MessageBox.Show("Enter data correctly");
                }

            }

        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            int emp_id = 0, emp_ledid = 0;
            DataRow[] rows = employees.Select();
            if (rows.Length > 0)
            {
                emp_id = Convert.ToInt32(rows[0]["eid"].ToString());
            }
            if (cmb_emp_name.Text.Length > 0 && cmb_type.Text.Length > 0 && txt_amount.Text.Length > 0 && emp_id > 0)
            {

                MessageBoxResult res = new MessageBoxResult();
                res = System.Windows.MessageBox.Show("Do you want Edit this entry", "Update Payroll Posting", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {

                    try
                    {
                        con = public_members._OpenConnection();
                        if (con != null)
                        {
                            var sql = "update payroll_posting set post_date=@pdate,eid=@eid,type=@ptype,amount=@amount,narration=@narration where id="+ txt_entryno.Text.ToString();
                            cmd.CommandText = sql;
                            cmd.Connection = con;
                            SqlParameter pdate = new SqlParameter("@pdate", SqlDbType.DateTime);
                            pdate.Direction = ParameterDirection.Input;
                            pdate.Value = dtp_post_date.SelectedDate;

                            SqlParameter eid = new SqlParameter("@eid", SqlDbType.Int);
                            eid.Direction = ParameterDirection.Input;
                            eid.Value = emp_id;

                            SqlParameter ptype = new SqlParameter("@ptype", SqlDbType.VarChar);
                            ptype.Direction = ParameterDirection.Input;
                            ptype.Value = cmb_type.Text.ToString();

                            SqlParameter narration = new SqlParameter("@narration", SqlDbType.VarChar);
                            narration.Direction = ParameterDirection.Input;
                            narration.Value = txt_narration.Text.ToString();

                            SqlParameter amount = new SqlParameter("@amount", SqlDbType.Float);
                            amount.Direction = ParameterDirection.Input;
                            float am;
                            float.TryParse(txt_amount.Text.ToString(), out am);
                            amount.Value = am;


                            cmd.Parameters.Add(pdate);
                            cmd.Parameters.Add(eid);
                            cmd.Parameters.Add(ptype);
                            cmd.Parameters.Add(amount);
                            cmd.Parameters.Add(narration);
                            int t = cmd.ExecuteNonQuery();
                            if (t > 0)
                            {
                                con.Close();
                                MessageBox.Show("Posting Updated"); btn_Reset_Click(sender, e);

                            }
                            else
                            {
                                MessageBox.Show("Entery not saved");
                            }
                        }
                    }
                    catch (SqlException sqle)
                    {
                        MessageBox.Show(sqle.Message.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("Enter data Correctly");
            }
        }

        private void btn_del_Click(object sender, RoutedEventArgs e)

        {

            SqlCommand cmd = new SqlCommand();
            if (txt_entryno.Text.Length > 0 && btn_save.IsEnabled == false)
            {
                MessageBoxResult res = new MessageBoxResult();
                res = MessageBox.Show("Do you want delete this Payroll Posting", "Delete Payroll", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    long ljeno = public_members.GetSerialNo("payroll_posting", "id") - 1;
                    long cjeno = 0;
                    long.TryParse(txt_entryno.Text.ToString(), out cjeno);
                    con = public_members._OpenConnection();
                    if (con != null)
                    {
                        if (ljeno == cjeno)
                        {
                            public_members.Delete_reseed("payroll_posting", "id", cjeno, cjeno);
                        }
                        else
                        {
                            cmd = new SqlCommand("update payroll_posting set  eid=0,amount=0,NARRATION='DELETED' WHERE id=" + txt_entryno.Text.ToString(), con);
                            cmd.ExecuteNonQuery();
                        }

                        //public_members.transactions.delete(long.Parse(txt_jno.Text.ToString()), "JOURNAL", conn: public_members._sql_con);
                        MessageBox.Show("Entry Deleted");
                        btn_Reset_Click(sender, e);

                    }
                    else
                    {
                        MessageBox.Show("DataBase connection lost");
                    }
                    public_members._sql_con.Close();

                }
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
