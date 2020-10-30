using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace accounts.DB
{
    class Journal
    {
        public static DataTable JournalTable;
        public static void Fetch()
        {
            JournalTable = Connection._FetchTable("select * from journal_entry order by j_no");
        }
        public static int Save(Model.JournalModel journal, bool flag = true)
        {
            int rno = 0;
            try
            {
                var con = DB.Connection.OpenConnection();
                if (con != null)
                {
                    OleDbCommand cmd = new OleDbCommand("insert into journal_entry (j_date,j_crledger,j_drledger,j_cramount,j_dramount,j_narration,j_isrecurring,j_cinvno)" +
                        " values(@j_date,@j_crledger,@j_drledger,@j_cramount,@j_dramount,@j_narration,@j_isrecurring,@j_cinvno)", con);
                    OleDbParameter jdate = new OleDbParameter("@j_date", OleDbType.Date);
                    jdate.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter crledger = new OleDbParameter("@j_crledger", OleDbType.Integer);
                    crledger.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter drledger = new OleDbParameter("@j_drledger", OleDbType.Integer);
                    drledger.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter cramount = new OleDbParameter("@j_cramount", OleDbType.Decimal);
                    cramount.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter dramount = new OleDbParameter("@j_dramount", OleDbType.Decimal);
                    dramount.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter narration = new OleDbParameter("@j_narration", OleDbType.VarChar);
                    narration.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter invno = new OleDbParameter("@j_cinvno", OleDbType.VarChar);
                    invno.Direction = System.Data.ParameterDirection.Input;
                    OleDbParameter isrecurr = new OleDbParameter("@j_isrecurring", OleDbType.Boolean);
                    isrecurr.Direction = System.Data.ParameterDirection.Input;

                    //Save
                    if (flag == false)
                    {
                        MessageBox.Show("Debit lock reached");

                    }
                    if (journal.CrAccount.ID > 0 && journal.DrAccount.ID > 0 && flag == true)
                    {
                        crledger.Value = journal.CrAccount.ID;
                        drledger.Value = journal.DrAccount.ID;
                        narration.Value = journal.Narration;
                        jdate.Value = journal.Date;
                        cramount.Value = journal.Cr_Amount;
                        dramount.Value = journal.Dr_Amount;
                        narration.Value = journal.Narration;
                        isrecurr.Value = journal.Isrecurring;
                        invno.Value = journal.Invoice;
                        cmd.Parameters.Add(jdate);
                        cmd.Parameters.Add(crledger);
                        cmd.Parameters.Add(drledger);
                        cmd.Parameters.Add(cramount);
                        cmd.Parameters.Add(dramount);
                        cmd.Parameters.Add(narration);
                        cmd.Parameters.Add(isrecurr);
                        cmd.Parameters.Add(invno);

                        int r = cmd.ExecuteNonQuery();
                        if (r > 0)
                        {
                            var eno = Connection.NewEntryno(table: "journal_entry", field: "j_no", conn: con) - 1;
                            rno = eno;
                            journal.jno = eno;

                            con.Close();
                            if (r > 0)
                            {
                                Model.Trsansactions trsansactions = new Model.Trsansactions();
                                trsansactions.Tr_date = journal.Date;
                                trsansactions.Ac_Id = journal.CrAccount.ID;
                                trsansactions.Op_Ac_Id = journal.DrAccount.ID;
                                trsansactions.Dr_Amount = journal.Cr_Amount;
                                trsansactions.Cr_Amount = 0;
                                trsansactions.Entry = "JOURNAL";
                                trsansactions.Eno = journal.jno;
                                trsansactions.Cinv_no = journal.Invoice.ToUpper();
                                Transactions.post(trsansactions);

                                trsansactions = new Model.Trsansactions();
                                trsansactions.Tr_date = journal.Date;
                                trsansactions.Op_Ac_Id = journal.CrAccount.ID;
                                trsansactions.Ac_Id = journal.DrAccount.ID;
                                trsansactions.Cr_Amount = journal.Dr_Amount;
                                trsansactions.Dr_Amount = 0;
                                trsansactions.Entry = "JOURNAL";
                                trsansactions.Eno = journal.jno;
                                trsansactions.Cinv_no = journal.Invoice.ToUpper();
                                Transactions.post(trsansactions);

                                ViewModels_Variables.ModelViews.Add_Update(journal);
                                // eno = rno;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Enter data correctly");
                    }
                }
            }


            catch (Exception rr)
            {
                MessageBox.Show(rr.Message.ToString());
            }
            return rno;
        }
        public static bool Update(Model.JournalModel journal, bool flag = true)
        {
            bool res = false;
            MessageBoxResult re = new MessageBoxResult();
            re = MessageBox.Show("Do you want Edit this ledger", "Update ledger", MessageBoxButton.YesNo);
            if (re == MessageBoxResult.Yes)
            {

                try
                {

                    var con = DB.Connection.OpenConnection();
                    if (con != null)
                    {
                        Transactions.delete(journal.jno, "JOURNAL");
                        OleDbCommand cmd = new OleDbCommand("update journal_entry set  j_date=@j_date,j_crledger=@j_crledger,j_drledger=@j_drledger,j_cramount=@j_cramount,j_dramount=@j_dramount,j_narration=@j_narration,j_isrecurring=@j_isrecurring,j_cinvno=@j_cinvno where j_no= " + journal.jno, con);

                        OleDbParameter jdate = new OleDbParameter("@j_date", OleDbType.Date);
                        jdate.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter crledger = new OleDbParameter("@j_crledger", OleDbType.Integer);
                        crledger.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter drledger = new OleDbParameter("@j_drledger", OleDbType.Integer);
                        drledger.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter cramount = new OleDbParameter("@j_cramount", OleDbType.Decimal);
                        cramount.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter dramount = new OleDbParameter("@j_dramount", OleDbType.Decimal);
                        dramount.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter narration = new OleDbParameter("@j_narration", OleDbType.VarChar);
                        narration.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter invno = new OleDbParameter("@j_cinvno", OleDbType.VarChar);
                        invno.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter isrecurr = new OleDbParameter("@j_isrecurring", OleDbType.Boolean);
                        isrecurr.Direction = System.Data.ParameterDirection.Input;


                        //Save
                        if (flag == false)
                        {
                            MessageBox.Show("Debit lock reached");

                        }
                        if (journal.CrAccount.ID > 0 && journal.DrAccount.ID > 0 && flag == true)
                        {
                            jdate.Value = journal.Date;
                            crledger.Value = journal.CrAccount.ID;
                            drledger.Value = journal.DrAccount.ID;
                            cramount.Value = journal.Cr_Amount;
                            dramount.Value = journal.Dr_Amount;
                            narration.Value = journal.Narration;
                            invno.Value = journal.Invoice;
                            isrecurr.Value = journal.Isrecurring;

                            cmd.Parameters.Add(jdate);
                            cmd.Parameters.Add(crledger);
                            cmd.Parameters.Add(drledger);
                            cmd.Parameters.Add(cramount);
                            cmd.Parameters.Add(dramount);
                            cmd.Parameters.Add(narration);
                            cmd.Parameters.Add(isrecurr);
                            cmd.Parameters.Add(invno);

                            int r = cmd.ExecuteNonQuery();
                            if (r > 0)
                            {


                                con.Close();


                                if (r > 0)
                                {
                                    Model.Trsansactions trsansactions = new Model.Trsansactions();
                                    trsansactions.Tr_date = journal.Date;
                                    trsansactions.Ac_Id = journal.CrAccount.ID;
                                    trsansactions.Op_Ac_Id = journal.DrAccount.ID;
                                    trsansactions.Dr_Amount = journal.Cr_Amount;
                                    trsansactions.Cr_Amount = 0;
                                    trsansactions.Entry = "JOURNAL";
                                    trsansactions.Eno = journal.jno;
                                    trsansactions.Cinv_no = journal.Invoice.ToUpper();
                                    Transactions.post(trsansactions);

                                    trsansactions = new Model.Trsansactions();
                                    trsansactions.Tr_date = journal.Date;
                                    trsansactions.Op_Ac_Id = journal.CrAccount.ID;
                                    trsansactions.Ac_Id = journal.DrAccount.ID;
                                    trsansactions.Cr_Amount = journal.Dr_Amount;
                                    trsansactions.Dr_Amount = 0;
                                    trsansactions.Entry = "JOURNAL";
                                    trsansactions.Eno = journal.jno;
                                    trsansactions.Cinv_no = journal.Invoice.ToUpper();
                                    Transactions.post(trsansactions);

                                    ViewModels_Variables.ModelViews.Add_Update(journal);
                                    res = true;


                                }

                            }

                        }
                        else
                        {
                            MessageBox.Show("Enter data correctly");

                        }
                    }


                }
                catch (Exception rr)
                {
                    MessageBox.Show(rr.Message.ToString());
                }

            }


            return res;
        }
        public static bool Remove(int id)
        {
            bool e = false;
            try
            {
                MessageBoxResult res = new MessageBoxResult();
                res = System.Windows.MessageBox.Show("Do you want to Remove this Entry", "Remove Journal", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    try
                    {

                        Transactions.delete(id, "JOURNAL");
                        var con = Connection.OpenConnection();
                        if (con != null)
                        {

                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;
                            Transactions.delete(id, "JOURNAL");

                            cmd.CommandText = "delete from recurrings where eno=" + id + " and entry='JOURNAL'";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "delete from journal_entry where j_no=" + id;
                            int r = cmd.ExecuteNonQuery();

                            if (r > 0)
                            {
                                e = true;
                                var acc = ViewModels_Variables.ModelViews.Journals.Where((a) => a.jno == id).FirstOrDefault();
                                if (acc != null) ViewModels_Variables.ModelViews.Remove(acc);
                                var task = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ENO == id && ts.ENTRY == "JOURNAL").FirstOrDefault();
                                if (task != null) { ViewModels_Variables.ModelViews.Remove(task); }

                                e = true;
                            }
                        }
                    }
                    catch (OleDbException e1)
                    {
                        System.Windows.MessageBox.Show("Server Error");
                    }
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
            return e;
        }
        public static bool Tasker(int tid, DateTime cdate)
        {
            bool res = false;
            try
            {
                var task = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ID == tid && ts.ENTRY == "JOURNAL").FirstOrDefault();
                if (task != null)
                {

                    var row = ViewModels_Variables.ModelViews.Journals.Where((j) => j.jno == task.ENO).FirstOrDefault();
                    if (row != null)
                    {
                        double dram = 0, cram = 0; int drid = 0, crid = 0;
                        crid = row.CrAccount.ID;
                        drid = row.DrAccount.ID;
                        dram = row.Dr_Amount;
                        cram = row.Cr_Amount;

                        var con = Connection.OpenConnection();
                        if (con != null)
                        {

                            OleDbCommand cmd = new OleDbCommand("INSERT INTO  journal_entry ([j_date] ,[j_crledger],[j_cramount] " +
                           ",[j_drledger],[j_dramount]) select [j_date] ,[j_crledger],[j_cramount] " +
                           ",[j_drledger],[j_dramount] from journal_entry  where j_no=" + row.jno, con);

                            int r = cmd.ExecuteNonQuery();
                            if (r > 0)
                            {
                                var eno = Connection.NewEntryno(table: "journal_entry", field: "j_no", conn: con) - 1;

                                Model.JournalModel newj = new Model.JournalModel();

                                OleDbParameter jdate = new OleDbParameter("@j_date", OleDbType.Date);
                                jdate.Direction = System.Data.ParameterDirection.Input;
                                jdate.Value = cdate;
                                cmd = new OleDbCommand(" update journal_entry set j_date=@j_date,j_narration='Posted by TaskMaster',J_taskid=" + task.ID + "  where j_no=" + eno, con);
                                cmd.Parameters.Add(jdate);
                                var xr = cmd.ExecuteNonQuery();
                                newj = DB.Journal.Find(eno);
                                if (r > 0)
                                {
                                    res = true;
                                    Model.Trsansactions trsansactions = new Model.Trsansactions();
                                    trsansactions.Tr_date = newj.Date;
                                    trsansactions.Ac_Id = crid;
                                    trsansactions.Op_Ac_Id = drid;
                                    trsansactions.Dr_Amount = dram;
                                    trsansactions.Cr_Amount = 0;
                                    trsansactions.Entry = "JOURNAL";
                                    trsansactions.Eno = newj.jno;
                                    trsansactions.Cinv_no = row.Invoice;
                                    Transactions.post(trsansactions);

                                    trsansactions = new Model.Trsansactions();
                                    trsansactions.Tr_date = newj.Date;
                                    trsansactions.Op_Ac_Id = crid;
                                    trsansactions.Ac_Id = drid;
                                    trsansactions.Cr_Amount = cram;
                                    trsansactions.Dr_Amount = 0;
                                    trsansactions.Entry = "JOURNAL";
                                    trsansactions.Eno = eno;
                                    trsansactions.Cinv_no = row.Invoice;
                                    Transactions.post(trsansactions);
                                    ViewModels_Variables.ModelViews.Add_Update(newj);

                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Database connection error");
                        }
                    }
                }

            }
            catch (OleDbException e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }



            return res;
        }
        public static Model.JournalModel Find(int eno)
        {
            Model.JournalModel model = new Model.JournalModel();
            try
            {
                var receipts = DB.Connection._FetchTable("select * from journal_entry where j_no=" + eno);
                var jlist = (from r in receipts.AsEnumerable()
                             join a in ViewModels_Variables.ModelViews.Accounts on r.Field<int>("j_drledger") equals a.ID
                             join a1 in ViewModels_Variables.ModelViews.Accounts on r.Field<int>("j_crledger") equals a1.ID
                             select new
                             {
                                 obj = new Model.JournalModel()
                                 {
                                      
                                     DrAccount = a,
                                     CrAccount = a1,
                                     jno = r.Field<int>("j_no"),
                                     Date = r.Field<DateTime>("j_date"),
                                     Cr_Amount = Convert.ToDouble(r.Field<decimal>("j_cramount")),
                                     Dr_Amount = Convert.ToDouble(r.Field<decimal>("j_cramount")),
                                     Task_Id = r.Field<int?>("j_taskid"),
                                     Invoice = r.Field<string>("j_cinvno"),
                                     Narration = r.Field<string>("j_narration")
                                 }
                             }.obj
                           ).ToList<Model.JournalModel>();
                if (jlist != null)
                {
                    model = jlist[0];
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }

            return model;
        }
    }
}
