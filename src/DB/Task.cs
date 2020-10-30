using System;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Windows;

namespace accounts.DB
{
    static class Task
    {
        public static DataTable TaskTable;
        public static void Fetch()
        {
            TaskTable = Connection._FetchTable("select * from recurrings order by id");
        }
        public static int AddTask(Model.Task t)
        {
            int tid = 0;
            try
            {
                var con = DB.Connection.OpenConnection();
                if (con != null)
                {
                    OleDbCommand cmd = new OleDbCommand("DELETE FROM RECURRINGS WHERE ENO=" + t.ENO + " and entry='" + t.ENTRY + "'", con);
                    cmd.ExecuteNonQuery();
                    cmd = new OleDbCommand("insert into recurrings (entry,eno,task_label,task_amount)" + " values(@t_entry,@t_eno,@t_label,@t_amount)", con);
                    OleDbParameter tentry = new OleDbParameter("@t_entry", OleDbType.VarChar);
                    tentry.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter teno = new OleDbParameter("@t_eno", OleDbType.Integer);
                    teno.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter tlable = new OleDbParameter("@t_label", OleDbType.VarChar);
                    tlable.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter tamount = new OleDbParameter("@t_amount", OleDbType.Double);
                    tlable.Direction = System.Data.ParameterDirection.Input;
                    tentry.Value = t.ENTRY;
                    teno.Value = t.ENO;
                    tlable.Value = t.T_LABEL;
                    tamount.Value = t.T_AMOUNT;

                    cmd.Parameters.Add(tentry);
                    cmd.Parameters.Add(teno);
                    cmd.Parameters.Add(tlable);
                    cmd.Parameters.Add(tamount);

                    int r = cmd.ExecuteNonQuery();
                    if (r > 0)
                    {
                        tid = DB.Connection.NewEntryno("recurrings", "id", conn: con) - 1;
                        t.ID = tid;
                        ViewModels_Variables.ModelViews.Add_Update(t);

                    }
                    con.Close();

                }
            }
            catch (Exception er)
            {

                throw;
            }

            return tid;
        }
        public static Model.Task Find(int eno, string entry)
        {
            Model.Task task = new Model.Task();

            return task;
        }

        public static bool DeleteTask(int tid = 0, int eno = 0, string entry = null)
        {
            bool res = false;
            try
            {
                var con = DB.Connection.OpenConnection();
                if (con != null)
                {
                    OleDbCommand cmd = new OleDbCommand();
                    if (tid > 0 && eno == 0 && entry == null)
                    {
                        var t = ViewModels_Variables.ModelViews.Tasks.Where((tt) => tt.ID == tid).FirstOrDefault();
                        if (t != null) { ViewModels_Variables.ModelViews.Remove(t); }
                        cmd = new OleDbCommand("delete from recurrings where id=" + tid, con);
                    }
                    else if (tid == 0 && eno > 0 && entry != null)
                    {
                        var t = ViewModels_Variables.ModelViews.Tasks.Where((tt) => tt.ENO == eno && tt.ENTRY == entry).FirstOrDefault();
                        if (t != null) { ViewModels_Variables.ModelViews.Remove(t); }
                        cmd = new OleDbCommand("delete from recurrings where eno=" + eno + " and entry='" + entry + "'", con);
                    }

                    int r = cmd.ExecuteNonQuery();
                    if (r > 0)
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
    }
}
