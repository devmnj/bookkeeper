using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Windows;
using System.Data;

namespace accounts.DB
{
    static class Transactions
    {
        public static DataTable TransactionsTable;
        public static void Fetch()
        {
            TransactionsTable = DB.Connection._FetchTable("select * from transactions order by t_date");
        }
        public static bool DeleteAll(int id, bool delentries = false)
        {

            bool res = false;
            try
            {
                var con = Connection.OpenConnection();
                if (con.State == System.Data.ConnectionState.Open)
                {

                    if (delentries == true)
                    {
                        var rec = ViewModels_Variables.ModelViews.AccountTransactions.Where((t) => t.Entry == "RECEIPT" && (t.Ac_Id == id || t.Op_Ac_Id == id)).ToList();
                        var pay = ViewModels_Variables.ModelViews.AccountTransactions.Where((t) => t.Entry == "PAYMENT" && (t.Ac_Id == id || t.Op_Ac_Id == id)).ToList();
                        var brec = ViewModels_Variables.ModelViews.AccountTransactions.Where((t) => t.Entry == "BANK RECEIPT" && (t.Ac_Id == id || t.Op_Ac_Id == id)).ToList();
                        var bpay = ViewModels_Variables.ModelViews.AccountTransactions.Where((t) => t.Entry == "BANK PAYMENT" && (t.Ac_Id == id || t.Op_Ac_Id == id)).ToList();
                        var jp = ViewModels_Variables.ModelViews.AccountTransactions.Where((t) => t.Entry == "JOURNAL" && (t.Ac_Id == id || t.Op_Ac_Id == id)).ToList();


                        OleDbCommand cmd = new OleDbCommand();
                        //RECEIPT
                        if (rec != null && rec.Count() > 0)
                        {
                            foreach (var t in rec)
                            {
                                cmd = new OleDbCommand("delete from receipts  where r_no=" + t.Eno, con);
                                cmd.ExecuteNonQuery();
                                var curr = ViewModels_Variables.ModelViews.Receipts.Where((rt) => rt.rno == t.Eno).FirstOrDefault();
                                if (curr != null) ViewModels_Variables.ModelViews.Remove(curr);
                            }
                        }
                        //PAYMENT
                        if (pay != null && pay.Count() > 0)
                        {
                            foreach (var t in pay)
                            {
                                cmd = new OleDbCommand("delete from payments where p_no=" + t.Eno, con);
                                cmd.ExecuteNonQuery();
                                var curr = ViewModels_Variables.ModelViews.Payments.Where((rt) => rt.pno == t.Eno).FirstOrDefault();
                                if (curr != null) ViewModels_Variables.ModelViews.Remove(curr);
                            }
                        }
                        //BP
                        if (bpay != null && bpay.Count() > 0)
                        {
                            foreach (var t in bpay)
                            {
                                cmd = new OleDbCommand("delete from bank_payments where bp_no=" + t.Eno, con);
                                cmd.ExecuteNonQuery();
                                var curr = ViewModels_Variables.ModelViews.BankPayments.Where((rt) => rt.pno == t.Eno).FirstOrDefault();
                                if (curr != null) ViewModels_Variables.ModelViews.Remove(curr);
                            }
                        }
                        //BR
                        if (brec != null && brec.Count() > 0)
                        {
                            foreach (var t in brec)
                            {
                                cmd = new OleDbCommand("delete from bank_receipts where br_no=" + t.Eno, con);
                                cmd.ExecuteNonQuery();
                                var curr = ViewModels_Variables.ModelViews.BankReceipts.Where((rt) => rt.rno == t.Eno).FirstOrDefault();
                                if (curr != null) ViewModels_Variables.ModelViews.Remove(curr);
                            }
                        }
                        //JP
                        if (jp != null && jp.Count() > 0)
                        {
                            foreach (var t in bpay)
                            {
                                cmd = new OleDbCommand("delete from bank_payments where bp_no=" + t.Eno, con);
                                cmd.ExecuteNonQuery();
                                var curr = ViewModels_Variables.ModelViews.Journals.Where((rt) => rt.jno == t.Eno).FirstOrDefault();
                                if (curr != null) ViewModels_Variables.ModelViews.Remove(curr);
                            }
                        }
                    }

                    OleDbCommand command = new OleDbCommand("delete from transactions where led_id=" + id + " or op_led_id=" + id, con);
                    int r = command.ExecuteNonQuery();
                    if (r > 0)
                    {
                        res = true;

                        var trsansaction = ViewModels_Variables.ModelViews.AccountTransactions.Where((t) => t.Ac_Id == id || t.Op_Ac_Id == id).ToList();
                        if (trsansaction != null && trsansaction.Count() > 0)

                        {

                            foreach (var t in trsansaction)
                            {
                                ViewModels_Variables.ModelViews.Remove(t);
                            }
                        }
                        ViewModels_Variables.ModelViews.Refresh_FrontPanelItems();
                        ViewModels_Variables.ModelViews.RefreshCashBook();
                        ViewModels_Variables.ModelViews.RefreshDaybook();
                        ViewModels_Variables.ModelViews.RefreshGroupBook();
                    }
                }
                con.Close();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
            return res;
        }
        public static int post(Model.Trsansactions trsansaction)
        {

            int result = 0;
            try
            {
                var con = Connection.OpenConnection();
                if (con.State != System.Data.ConnectionState.Closed)
                {
                    OleDbCommand cmd = new OleDbCommand(); cmd.Connection = con;
                    cmd.CommandText = "insert into transactions ([cinvno],led_id,op_led_id,dr,cr,eno,entry,t_date) values (@cinvno,@led_id,@op_led_id,@dr,@cr,@eno,@entry,@t_date)";

                    OleDbParameter c_inv = new OleDbParameter("@cinvno", OleDbType.VarChar); c_inv.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter lid = new OleDbParameter("@led_id", OleDbType.Integer); lid.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter oplid = new OleDbParameter("@op_led_id", OleDbType.Integer); oplid.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter dr1 = new OleDbParameter("@dr", OleDbType.Decimal); dr1.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter cr1 = new OleDbParameter("@cr", OleDbType.Decimal); cr1.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter eno = new OleDbParameter("@eno", OleDbType.Integer); eno.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter entry_n = new OleDbParameter("@entry", OleDbType.VarChar); entry_n.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter tdate = new OleDbParameter("@t_date", OleDbType.Date); tdate.Direction = System.Data.ParameterDirection.Input;

                    lid.Value = trsansaction.Ac_Id;
                    oplid.Value = trsansaction.Op_Ac_Id;
                    dr1.Value = trsansaction.Dr_Amount;
                    cr1.Value = trsansaction.Cr_Amount;
                    eno.Value = trsansaction.Eno;
                    entry_n.Value = trsansaction.Entry;
                    c_inv.Value = trsansaction.Cinv_no;
                    tdate.Value = trsansaction.Tr_date;

                    cmd.Parameters.Add(c_inv);
                    cmd.Parameters.Add(lid);
                    cmd.Parameters.Add(oplid);
                    cmd.Parameters.Add(dr1);
                    cmd.Parameters.Add(cr1);
                    cmd.Parameters.Add(eno);
                    cmd.Parameters.Add(entry_n);

                    cmd.Parameters.Add(tdate);

                    int r = cmd.ExecuteNonQuery();
                    if (r > 0)
                    {
                        result = Connection.NewEntryno("transactions", "id", "id") - 1;
                        trsansaction.Id = result;

                        ViewModels_Variables.ModelViews.Add_Update(trsansaction);
                        ViewModels_Variables.ModelViews.Refresh_FrontPanelItems();
                        ViewModels_Variables.ModelViews.RefreshCashBook();
                        ViewModels_Variables.ModelViews.RefreshDaybook();
                        ViewModels_Variables.ModelViews.RefreshGroupBook();

                    }

                    con.Close();
                }
            }

            catch (OleDbException ee)
            {

                MessageBox.Show("DB Execution interrupted:" + ee.Message.ToString());

            }
            return result;
        }
        public static bool delete(int eno, string entry)
        {
            bool res = false;
            try
            {
                var con = Connection.OpenConnection();
                if (con.State == System.Data.ConnectionState.Open)
                {
                    OleDbCommand tasks = new OleDbCommand("delete from recurrings where eno=" + eno + " and entry='" + entry + "'", con);
                    OleDbCommand command = new OleDbCommand("delete from transactions where eno=" + eno + " and entry='" + entry + "'", con);
                    int r = command.ExecuteNonQuery();
                    tasks.ExecuteNonQuery();
                    if (r > 0)
                    {
                        res = true;
                        var trsansaction = ViewModels_Variables.ModelViews.AccountTransactions.Where((t) => t.Eno == eno && t.Entry == entry).FirstOrDefault();
                        if (trsansaction != null) ViewModels_Variables.ModelViews.Remove(trsansaction);
                        ViewModels_Variables.ModelViews.Refresh_FrontPanelItems();
                        ViewModels_Variables.ModelViews.RefreshCashBook();
                        ViewModels_Variables.ModelViews.RefreshDaybook();
                        ViewModels_Variables.ModelViews.RefreshGroupBook();
                    }
                    con.Close();

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
