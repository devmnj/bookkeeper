using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Windows;

namespace accounts.DB
{
    public static class Connection
    {
        public static bool Backup(string backup_path,string bckup_name="backend")
        {
            bool res=false;
            bool flag=true;

            try
            {
                if (backup_path.Length <= 1 && File.Exists(backup_path) == false)
                {
                    flag = false;
                }
                if (flag == true)
                {
                    Random random = new Random();
                    var backup = public_members.GnenerateFileName(path: backup_path, prefix:bckup_name, ext: ".accdb");

                    File.Copy(public_members.backend_path, backup);
                    if (File.Exists(backup))
                    {
                        res = true;
                    }
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public static OleDbConnection OpenConnection(string db = null)
        {
            var con = new OleDbConnection();
            try
            {


                con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["bookkeeper.Properties.Settings.ConnectionString"].ConnectionString;

                con.Open();
                if (con.State == System.Data.ConnectionState.Closed || con.State == System.Data.ConnectionState.Broken)
                {
                    con = null;
                }


            }
            catch (Exception ex)
            {
                con = null;
                MessageBox.Show(ex.Message.ToString());

            }

            return con;
        }
        public static DataTable _FetchTable(string qry, OleDbConnection conn = null)
        {
            DataTable output = new DataTable();
            OleDbConnection con;
            try
            {

                if (conn == null)
                {
                    con = Connection.OpenConnection();
                }
                else
                {
                    con = conn;
                }
                if (con.State == ConnectionState.Open)
                {
                    OleDbDataAdapter adapt = new OleDbDataAdapter(qry, con);

                    adapt.Fill(output);

                    if (conn == null) { con.Close(); }


                }
                else
                {
                    MessageBox.Show("Connection is broken");
                }
            }
            catch (OleDbException exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
            return output;
        }
        public static int NewEntryno(string table, string field, string ordrby = null, OleDbConnection conn = null)
        {
            int newNo = 0;
            DataTable rtable;
            try
            {
                if (ordrby == null) ordrby = field;
                if (conn != null)
                {
                    rtable = _FetchTable("select   " + field + " from " + table + " order by " + ordrby, conn: conn);
                }
                else
                {
                    rtable = _FetchTable("select   " + field + " from " + table + " order by " + ordrby);
                }

                if (rtable.Rows.Count <= 0)
                {
                    newNo = 1;
                }
                else
                {
                    newNo = int.Parse(rtable.Rows[rtable.Rows.Count - 1][0].ToString()) + 1;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
            return newNo;
        }
        public static int CreateLedger(string gname, int parent)
        {
            int newgid = 0;
            //try
            //{
            //    int gid = (gname);
            //    if (gid == 0)
            //    {
            //        _sql_con = _OpenConnection();
            //        if (_sql_con != null)
            //        {
            //            SqlCommand cmd = new SqlCommand("insert into ledgers (l_name,l_short_name,l_parent) values(@g_name,@l_short_name,@g_parent)", _sql_con);
            //            SqlParameter g_name = new SqlParameter("@g_name", SqlDbType.VarChar); g_name.Direction = ParameterDirection.Input;
            //            SqlParameter s_name = new SqlParameter("@l_short_name", SqlDbType.VarChar); g_name.Direction = ParameterDirection.Input;
            //            SqlParameter g_parent = new SqlParameter("@g_parent", SqlDbType.Int); g_parent.Direction = ParameterDirection.Input;
            //            g_name.Value = gname;
            //            s_name.Value = gname;
            //            g_parent.Value = parent;

            //            cmd.Parameters.Add(g_name);
            //            cmd.Parameters.Add(s_name);
            //            cmd.Parameters.Add(g_parent);
            //            cmd.ExecuteNonQuery();
            //            newgid = Ledgerid(gname);
            //            UpdateLedgerItemSources();
            //        }
            //        _sql_con.Close();
            //    }
            //    else
            //    {
            //        newgid = gid;
            //    }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            return newgid;
        }
        public static int CreateGroup(string gname, long parent, string cat = "None")
        {
            int newgid = 0;
            //try
            //{
            //    int gid = Getgroupid(gname);
            //    if (gid == 0)
            //    {
            //        _sql_con = _OpenConnection();
            //        if (_sql_con != null)
            //        {
            //            SqlCommand cmd = new SqlCommand("insert into groups (g_name,g_parent,g_catagory) values(@g_name,@g_parent,@g_catagory)", _sql_con);
            //            SqlParameter g_name = new SqlParameter("@g_name", SqlDbType.VarChar); g_name.Direction = ParameterDirection.Input;
            //            SqlParameter g_parent = new SqlParameter("@g_parent", SqlDbType.Int); g_parent.Direction = ParameterDirection.Input;
            //            SqlParameter g_catagory = new SqlParameter("@g_catagory", SqlDbType.VarChar); g_catagory.Direction = ParameterDirection.Input;
            //            g_name.Value = gname;
            //            g_catagory.Value = cat;
            //            g_parent.Value = parent;
            //            cmd.Parameters.Add(g_name);
            //            cmd.Parameters.Add(g_parent);
            //            cmd.Parameters.Add(g_catagory);
            //            cmd.ExecuteNonQuery();
            //            newgid = Getgroupid(gname);
            //            Refresh_Groups();
            //        }
            //        _sql_con.Close();
            //    }
            //    else
            //    {
            //        newgid = gid;
            //    }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            return newgid;
        }
        //public static double GetActBalance(int id)
        //{
        //    double bal = 0;

        //    try
        //    {

        //            string sql = "select distinct(l.id) , (select  case when sum(cr)> sum(dr) then sum(cr)-sum(dr) when sum(dr)> sum(cr) then sum(dr)-sum(cr) else 0  end from transactions where op_led_id = l.id) as balance " +
        //                          " from ledgers l inner join transactions t on l.id = t.led_id  where l.id = " + id + " group by t.led_id,l.id  ";
        //            var t = _FetchTable(sql);

        //            if (t.Rows.Count > 0)
        //            {
        //                double.TryParse(t.Rows[0]["balance"].ToString(), out bal);
        //            }


        //    }
        //    catch (Exception er)
        //    {

        //        MessageBox.Show(er.Message.ToString());
        //    }
        //    return bal;
        //} 
        public static System.Collections.Generic.Dictionary<string, double> GetOBDrCr(DateTime dt, long id)
        {
            Dictionary<string, double> account = new Dictionary<string, double>();
            try
            {

                var cr = (from c in ViewModels_Variables.ModelViews.AccountTransactions where c.Op_Ac_Id == id && c.Tr_date < dt select c.Cr_Amount).Sum();
                var dr = (from c in ViewModels_Variables.ModelViews.AccountTransactions.AsEnumerable() where c.Op_Ac_Id == id && c.Tr_date < dt select c.Dr_Amount).Sum();
                double b=0,b1 = 0;
                if (cr > dr) { b = cr - dr; }
                if (dr > cr) { b1 = dr - cr; }
                account.Add("Cr", Convert.ToDouble(b));
                account.Add("Dr", Convert.ToDouble(b1));

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message.ToString());
            }
            return account;
        }
        public static System.Collections.Generic.Dictionary<string, double> GetDrCr(long id)
        {
            Dictionary<string, double> account = new Dictionary<string, double>();
            try
            {

                var cr = (from c in ViewModels_Variables.ModelViews.AccountTransactions where c.Op_Ac_Id == id select c.Cr_Amount).Sum();
                var dr = (from c in ViewModels_Variables.ModelViews.AccountTransactions.AsEnumerable() where c.Op_Ac_Id == id select c.Dr_Amount).Sum();
                account.Add("Cr", Convert.ToDouble(cr));
                account.Add("Dr", Convert.ToDouble(dr));

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message.ToString());
            }
            return account;
        }

        public static double GetActBalance(int id, string byInvoice = null)
        {
            double b1 = 0;

            try
            {
                if (ViewModels_Variables.ModelViews.AccountTransactions == null || ViewModels_Variables.ModelViews.AccountTransactions.Count <= 0)
                {
                    ViewModels_Variables.ModelViews.Trans_To_list();
                }

                if (ViewModels_Variables.ModelViews.AccountTransactions != null)
                {
                    if (byInvoice != null)
                    {
                        var cr = (from c in ViewModels_Variables.ModelViews.AccountTransactions where c.Op_Ac_Id == id && c.Cinv_no == byInvoice select c.Cr_Amount).Sum();
                        var dr = (from c in ViewModels_Variables.ModelViews.AccountTransactions where c.Op_Ac_Id == id && c.Cinv_no == byInvoice select c.Dr_Amount).Sum();

                        if (cr > dr) { b1 = Convert.ToDouble(cr) - Convert.ToDouble(dr); }
                        else if (dr > cr) { b1 = Convert.ToDouble(dr) - Convert.ToDouble(cr); }
                    }
                    else
                    {
                        var cr = (from c in ViewModels_Variables.ModelViews.AccountTransactions where c.Op_Ac_Id == id select c.Cr_Amount).Sum();
                        var dr = (from c in ViewModels_Variables.ModelViews.AccountTransactions where c.Op_Ac_Id == id select c.Dr_Amount).Sum();

                        if (cr > dr) { b1 = Convert.ToDouble(cr) - Convert.ToDouble(dr); }
                        else if (dr > cr) { b1 = Convert.ToDouble(dr) - Convert.ToDouble(cr); }
                    }
                }



            }
            catch (Exception er)
            {
                b1 = 0;
                //MessageBox.Show(er.Message.ToString());
            }


            return b1;
        }


        public static double GetActBalance(DateTime dt, int id, string byInvoice)
        {
            double b1 = 0;

            try
            {
                if (ViewModels_Variables.ModelViews.AccountTransactions == null || ViewModels_Variables.ModelViews.AccountTransactions.Count <= 0)
                {
                    ViewModels_Variables.ModelViews.Trans_To_list();
                }

                var cr = (from c in ViewModels_Variables.ModelViews.AccountTransactions where c.Op_Ac_Id == id && c.Cinv_no == byInvoice && c.Tr_date <= dt select c.Cr_Amount).Sum();
                var dr = (from c in ViewModels_Variables.ModelViews.AccountTransactions where c.Op_Ac_Id == id && c.Cinv_no == byInvoice && c.Tr_date <= dt select c.Dr_Amount).Sum();

                if (cr > dr) { b1 = Convert.ToDouble(cr) - Convert.ToDouble(dr); }
                else if (dr > cr) { b1 = Convert.ToDouble(dr) - Convert.ToDouble(cr); }




            }
            catch (Exception)
            {

                throw;
            }


            return b1;
        }
        public static double GetOB(DateTime dt, int id, string byInvoice)
        {
            double b1 = 0;

            try
            {
                if (ViewModels_Variables.ModelViews.AccountTransactions == null || ViewModels_Variables.ModelViews.AccountTransactions.Count <= 0)
                {
                    ViewModels_Variables.ModelViews.Trans_To_list();
                }

                var cr = (from c in ViewModels_Variables.ModelViews.AccountTransactions where c.Op_Ac_Id == id && c.Cinv_no == byInvoice && c.Tr_date < dt select c.Cr_Amount).Sum();
                var dr = (from c in ViewModels_Variables.ModelViews.AccountTransactions where c.Op_Ac_Id == id && c.Cinv_no == byInvoice && c.Tr_date < dt select c.Dr_Amount).Sum();

                if (cr > dr) { b1 = Convert.ToDouble(cr) - Convert.ToDouble(dr); }
                else if (dr > cr) { b1 = Convert.ToDouble(dr) - Convert.ToDouble(cr); }




            }
            catch (Exception)
            {

                throw;
            }


            return b1;
        }
        public static Dictionary<string, double> GetActBalance(DateTime fdate, int id)
        {
            Dictionary<string, double> bal = new Dictionary<string, double>();

            string letter = null; ;
            try
            {
                if (ViewModels_Variables.ModelViews.AccountTransactions == null || ViewModels_Variables.ModelViews.AccountTransactions.Count <= 0)
                {
                    ViewModels_Variables.ModelViews.Trans_To_list();
                }



                var cr = (from c in ViewModels_Variables.ModelViews.AccountTransactions where c.Op_Ac_Id == id && c.Tr_date <= fdate select c.Cr_Amount).Sum();
                var dr = (from c in ViewModels_Variables.ModelViews.AccountTransactions where c.Op_Ac_Id == id && c.Tr_date <= fdate select c.Dr_Amount).Sum();
                double b1;
                if (cr > dr) { b1 = Convert.ToDouble(cr) - Convert.ToDouble(dr); letter = "Cr"; }
                else if (dr > cr) { b1 = Convert.ToDouble(dr) - Convert.ToDouble(cr); letter = "Dr"; }
                else
                { b1 = 0; }
                if (letter != null)
                {
                    if (letter == "Dr")
                    {
                        bal.Add("Cr", 0);
                    }
                    else if (letter == "Cr")
                    {
                        bal.Add("Dr", 0);
                    }
                    else if (letter == "")
                    {
                        bal.Add("Cr", 0);
                        bal.Add("Dr", 0);
                    }

                    if (letter != "") bal.Add(letter, b1);
                }
                else
                {
                    bal.Add("Cr", 0);
                    bal.Add("Dr", 0);
                }


            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }


            return bal;
        }
        public static bool CheckCreditLocks(double currentAmount, double drL, double gDrl, int id)
        {

            bool res = true;
            currentAmount += GetActBalance(id, "");
            try
            {

                if (drL == 0 && gDrl != 0)
                {
                    if (currentAmount > gDrl && gDrl != 0) { res = false; }
                }


                if (currentAmount > drL && drL != 0)
                {
                    res = false;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return res;
        }
    }
}
