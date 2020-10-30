using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Windows;
using System.Data;

namespace accounts.DB
{
    static class Receipt
    {
        public static DataTable ReceiptTable;
        public static void Fetch()
        {
            ReceiptTable = Connection._FetchTable("select * from receipts order by r_no");
        }
        public static int Save(Model.ReceiptModel receipt, bool flag = true)
        {
            int rno = 0;


            try
            {


                //Save
                if (flag == false)
                {
                    MessageBox.Show("Debit lock reached");

                }
                if (receipt.CrAccount.ID > 0 && receipt.DrAccount.ID > 0 && flag == true)

                {
                    var con = DB.Connection.OpenConnection();
                    if (con != null)
                    {

                        //AccountLedger receipt_transactions = new fin_app.AccountLedger(public_members._sql_con);
                        OleDbCommand cmd = new OleDbCommand("insert into receipts (r_cashledger,r_cr_ledger,r_cramount,r_narration,r_date,r_disc,r_damount,r_isrecurring,r_invoice)values" +
                              "(@r_cashledger,@r_cr_ledger,@r_cramount,@r_narration,@r_date,@r_disc,@r_damount,@r_isrecurring,@r_invoice)", con);

                        OleDbParameter cash = new OleDbParameter("@r_cashledger", OleDbType.Integer); cash.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter crledger = new OleDbParameter("@r_cr_ledger", OleDbType.Integer); crledger.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter amount = new OleDbParameter("@r_cramount", OleDbType.Double); amount.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter narration = new OleDbParameter("@r_narration", OleDbType.VarChar); narration.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter rdate = new OleDbParameter("@r_date", OleDbType.Date); rdate.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter disc = new OleDbParameter("@r_disc", OleDbType.Double); disc.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter damount = new OleDbParameter("@r_damount", OleDbType.Double); damount.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter isrecurr = new OleDbParameter("@r_isrecurring", OleDbType.Boolean); isrecurr.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter invoice = new OleDbParameter("@r_invoice", OleDbType.VarChar); invoice.Direction = System.Data.ParameterDirection.Input;
                        string inv = null;

                        invoice.Value = receipt.Invno;
                        cash.Value = receipt.DrAccount.ID;
                        crledger.Value = receipt.CrAccount.ID;
                        isrecurr.Value = receipt.isRecurr;
                        amount.Value = receipt.DrAmount;
                        disc.Value = receipt.DiscP;
                        damount.Value = receipt.DAmount;
                        narration.Value = receipt.Narration;
                        rdate.Value = receipt.Date;

                        cmd.Parameters.Add(cash);
                        cmd.Parameters.Add(crledger);
                        cmd.Parameters.Add(amount);
                        cmd.Parameters.Add(narration);
                        cmd.Parameters.Add(rdate);
                        cmd.Parameters.Add(disc);
                        cmd.Parameters.Add(damount);
                        cmd.Parameters.Add(isrecurr);
                        cmd.Parameters.Add(invoice);


                        int r = cmd.ExecuteNonQuery();

                        if (r > 0)
                        {
                            rno = Connection.NewEntryno("receipts", "r_no",conn:con) - 1;
                            receipt.rno = rno;


                            int disc_id = 0;
                            var disc_ac = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.Name == "DISCOUNT ALLOWED ON RECEIPTS" && ac.Parent.Name == "INDIRECT EXPENSE").FirstOrDefault();
                            if (disc_ac != null) { disc_id = disc_ac.ID; }

                            if (disc_id == 0)
                            {
                                int gid = 0;
                                var ind = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.Name == "INDIRECT EXPENSE").FirstOrDefault();
                                if (ind != null) { gid = ind.ID; }
                                if (gid == 0)
                                {
                                    int parent = 0;
                                    var is_parent = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.Name == "EXPENSE").FirstOrDefault();
                                    if (is_parent != null)
                                    {
                                        parent = is_parent.ID;
                                    }
                                    else
                                    {
                                        MessageBox.Show("'EXPENSE' Parent Group Missing");
                                    }

                                    if (parent > 0)
                                    {
                                        Model.GroupModel group = new Model.GroupModel()
                                        {
                                            Name = "INDIRECT EXPENSE",
                                            P_ID = parent,
                                            Catagory = "None",
                                            Cr_Loc = 0,
                                            Dr_Loc = 0,
                                            Max_Disc = 0,
                                            Description = "Created By Program",
                                            ID = 0
                                        };
                                        gid = DB.AccountGroup.Save(group);
                                        if (gid > 0)
                                        {
                                            Model.AccountModel account = new Model.AccountModel()
                                            {
                                                Name = "DISCOUNT ALLOWED ON RECEIPTS",
                                                ParentGroup = 0,
                                                Short_Name = "DISCOUNT ALLOWED ON RECEIPTS",
                                                Address = "",
                                                City = "",
                                                Mob = "",
                                                PhoneNo = "",
                                                CrLimit = 0,
                                                DrLimit = 0,
                                                Catagory = "",
                                                MaxDisc = 0

                                            };
                                            disc_id = DB.Accounts.Save(account);
                                        }
                                    }
                                }
                            }

                            con.Close();
                            if (rno > 0)
                            {
                                var trsansaction = ViewModels_Variables.ModelViews.AccountTransactions.Where((t) => t.Eno == receipt.rno && t.Entry == "RECEIPT").FirstOrDefault();
                                if (trsansaction != null) ViewModels_Variables.ModelViews.Remove(trsansaction);

                                Model.Trsansactions trsansactions = new Model.Trsansactions();
                                trsansactions.Tr_date = receipt.Date;
                                trsansactions.Ac_Id = receipt.CrAccount.ID;
                                trsansactions.Op_Ac_Id = receipt.DrAccount.ID;
                                trsansactions.Dr_Amount = receipt.DrAmount;
                                trsansactions.Cr_Amount = 0;
                                trsansactions.Entry = "RECEIPT";
                                trsansactions.Eno = receipt.rno;
                                trsansactions.Cinv_no = receipt.Invno;
                                Transactions.post(trsansactions);

                                trsansactions = new Model.Trsansactions();
                                trsansactions.Tr_date = receipt.Date;
                                trsansactions.Ac_Id = receipt.DrAccount.ID;
                                trsansactions.Op_Ac_Id = receipt.CrAccount.ID;
                                trsansactions.Cr_Amount = receipt.DrAmount;
                                trsansactions.Dr_Amount = 0;
                                trsansactions.Entry = "RECEIPT";
                                trsansactions.Eno = receipt.rno;
                                trsansactions.Cinv_no = receipt.Invno;
                                Transactions.post(trsansactions);

                                if (receipt.DAmount > 0)
                                {
                                    trsansactions = new Model.Trsansactions();
                                    trsansactions.Tr_date = receipt.Date;
                                    trsansactions.Ac_Id = receipt.CrAccount.ID;
                                    trsansactions.Op_Ac_Id = disc_id;
                                    trsansactions.Dr_Amount = receipt.DrAmount;
                                    trsansactions.Cr_Amount = 0;
                                    trsansactions.Entry = "RECEIPT";
                                    trsansactions.Eno = receipt.rno;
                                    trsansactions.Cinv_no = receipt.Invno;
                                    Transactions.post(trsansactions);


                                    trsansactions = new Model.Trsansactions();
                                    trsansactions.Tr_date = receipt.Date;
                                    trsansactions.Op_Ac_Id = receipt.CrAccount.ID;
                                    trsansactions.Ac_Id = disc_id;
                                    trsansactions.Cr_Amount = receipt.DrAmount;
                                    trsansactions.Dr_Amount = 0;
                                    trsansactions.Entry = "RECEIPT";
                                    trsansactions.Eno = receipt.rno;
                                    trsansactions.Cinv_no = receipt.Invno;
                                    Transactions.post(trsansactions);
                                }


                                //MessageBox.Show("Receipt saved!");


                                receipt.InvBalance = DB.Connection.GetActBalance(dt:receipt.Date, id: receipt.CrAccount.ID, byInvoice: receipt.Invno);
                                var rcpts = ViewModels_Variables.ModelViews.Receipts.Where((r1) => r1.Invno == receipt.Invno && r1.CrAccount.ID == receipt.CrAccount.ID);
                                    foreach (var rr in rcpts)
                                {
                                    rr.InvBalance = DB.Connection.GetActBalance(dt: rr.Date, id: rr.CrAccount.ID, byInvoice: rr.Invno);
                                }
                                ViewModels_Variables.ModelViews.Add_Update(receipt);

                            }
                        }

                    }
                }
            }

            catch (Exception rr)
            {
                MessageBox.Show(rr.Message.ToString());
            }
            return rno;
        }
        public static bool Update(Model.ReceiptModel receipt, bool flag = true)
        {
            bool res = false;
            MessageBoxResult re = new MessageBoxResult();
            re = MessageBox.Show("Do you want Edit this ledger", "Update ledger", MessageBoxButton.YesNo);
            if (re == MessageBoxResult.Yes)
            {
                try
                {


                    //flag = public_members.CheckCreditLocks((receipt.DrAmount - receipt.DAmount), creditLimit, gcriditLock, crid);
                    //flag = public_members.CheckCreditLocks((_amount - _damount), dreditLimit, gdriditLock, drid);

                    //Save
                    if (flag == false)
                    {
                        MessageBox.Show("Debit lock reached");

                    }
                    if (receipt.CrAccount.ID > 0 && receipt.DrAccount.ID > 0 && flag == true)

                    {
                        var con = DB.Connection.OpenConnection();
                        if (con != null)
                        {
                            Transactions.delete(receipt.rno, "RECEIPT");

                            //AccountLedger receipt_transactions = new fin_app.AccountLedger(public_members._sql_con);
                            OleDbCommand cmd = new OleDbCommand("update  receipts set r_cashledger=@r_cashledger,r_cr_ledger=@r_cr_ledger,r_cramount=@r_cramount,r_narration=@r_narration,r_date=@r_date,r_disc=@r_disc,r_damount=@r_damount,r_isrecurring=@r_isrecurring,r_invoice=@r_invoice where r_no=" + receipt.rno, con);

                            OleDbParameter cash = new OleDbParameter("@r_cashledger", OleDbType.Integer); cash.Direction = System.Data.ParameterDirection.Input;
                            OleDbParameter crledger = new OleDbParameter("@r_cr_ledger", OleDbType.Integer); crledger.Direction = System.Data.ParameterDirection.Input;
                            OleDbParameter amount = new OleDbParameter("@r_cramount", OleDbType.Double); amount.Direction = System.Data.ParameterDirection.Input;
                            OleDbParameter narration = new OleDbParameter("@r_narration", OleDbType.VarChar); narration.Direction = System.Data.ParameterDirection.Input;
                            OleDbParameter rdate = new OleDbParameter("@r_date", OleDbType.Date); rdate.Direction = System.Data.ParameterDirection.Input;
                            OleDbParameter disc = new OleDbParameter("@r_disc", OleDbType.Double); disc.Direction = System.Data.ParameterDirection.Input;
                            OleDbParameter damount = new OleDbParameter("@r_damount", OleDbType.Double); damount.Direction = System.Data.ParameterDirection.Input;
                            OleDbParameter isrecurr = new OleDbParameter("@r_isrecurring", OleDbType.Boolean); isrecurr.Direction = System.Data.ParameterDirection.Input;
                            OleDbParameter invoice = new OleDbParameter("@r_invoice", OleDbType.VarChar); invoice.Direction = System.Data.ParameterDirection.Input;
                            string inv = null;

                            invoice.Value = receipt.Invno;
                            cash.Value = receipt.DrAccount.ID;
                            crledger.Value = receipt.CrAccount.ID;
                            isrecurr.Value = receipt.isRecurr;
                            amount.Value = receipt.DrAmount;
                            disc.Value = receipt.DiscP;
                            damount.Value = receipt.DAmount;
                            narration.Value = receipt.Narration;
                            rdate.Value = receipt.Date;

                            cmd.Parameters.Add(cash);
                            cmd.Parameters.Add(crledger);
                            cmd.Parameters.Add(amount);
                            cmd.Parameters.Add(narration);
                            cmd.Parameters.Add(rdate);
                            cmd.Parameters.Add(disc);
                            cmd.Parameters.Add(damount);
                            cmd.Parameters.Add(isrecurr);
                            cmd.Parameters.Add(invoice);


                            int r = cmd.ExecuteNonQuery();

                            if (r > 0)
                            {
                                int disc_id = 0;
                                var disc_ac = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.Name == "DISCOUNT ALLOWED ON RECEIPTS" && ac.Parent.Name == "INDIRECT EXPENSE").FirstOrDefault();
                                if (disc_ac != null) { disc_id = disc_ac.ID; }

                                if (disc_id == 0)
                                {
                                    int gid = 0;
                                    var ind = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.Name == "INDIRECT EXPENSE").FirstOrDefault();
                                    if (ind != null) { gid = ind.ID; }
                                    if (gid == 0)
                                    {
                                        int parent = 0;
                                        var is_parent = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.Name == "EXPENSE").FirstOrDefault();
                                        if (is_parent != null)
                                        {
                                            parent = is_parent.ID;
                                        }
                                        else
                                        {
                                            MessageBox.Show("'EXPENSE' Parent Group Missing");
                                        }

                                        if (parent > 0)
                                        {
                                            Model.GroupModel group = new Model.GroupModel()
                                            {
                                                Name = "INDIRECT EXPENSE",
                                                P_ID = parent,
                                                Catagory = "None",
                                                Cr_Loc = 0,
                                                Dr_Loc = 0,
                                                Max_Disc = 0,
                                                Description = "Created By Program",
                                                ID = 0
                                            };
                                            gid = DB.AccountGroup.Save(group);
                                            if (gid > 0)
                                            {
                                                Model.AccountModel account = new Model.AccountModel()
                                                {
                                                    Name = "DISCOUNT ALLOWED ON RECEIPTS",
                                                    ParentGroup = 0,
                                                    Short_Name = "DISCOUNT ALLOWED ON RECEIPTS",
                                                    Address = "",
                                                    City = "",
                                                    Mob = "",
                                                    PhoneNo = "",
                                                    CrLimit = 0,
                                                    DrLimit = 0,
                                                    Catagory = "",
                                                    MaxDisc = 0

                                                };
                                                disc_id = DB.Accounts.Save(account);
                                            }
                                        }
                                    }
                                }


                                con.Close();
                                if (r > 0)
                                {
                                    var trsansaction = ViewModels_Variables.ModelViews.AccountTransactions.Where((t) => t.Eno == receipt.rno && t.Entry == "RECEIPT").FirstOrDefault();
                                    if (trsansaction != null) ViewModels_Variables.ModelViews.Remove(trsansaction);

                                    Model.Trsansactions trsansactions = new Model.Trsansactions();
                                    trsansactions.Tr_date = receipt.Date;
                                    trsansactions.Ac_Id = receipt.CrAccount.ID;
                                    trsansactions.Op_Ac_Id = receipt.DrAccount.ID;
                                    trsansactions.Dr_Amount = receipt.DrAmount;
                                    trsansactions.Cr_Amount = 0;
                                    trsansactions.Entry = "RECEIPT";
                                    trsansactions.Eno = receipt.rno;
                                    trsansactions.Cinv_no = receipt.Invno;



                                    Transactions.post(trsansactions);

                                    trsansactions = new Model.Trsansactions();
                                    trsansactions.Tr_date = receipt.Date;
                                    trsansactions.Ac_Id = receipt.DrAccount.ID;
                                    trsansactions.Op_Ac_Id = receipt.CrAccount.ID;
                                    trsansactions.Cr_Amount = receipt.DrAmount;
                                    trsansactions.Dr_Amount = 0;
                                    trsansactions.Entry = "RECEIPT";
                                    trsansactions.Eno = receipt.rno;
                                    trsansactions.Cinv_no = receipt.Invno;
                                    Transactions.post(trsansactions);

                                    if (receipt.DAmount > 0)
                                    {
                                        trsansactions = new Model.Trsansactions();
                                        trsansactions.Tr_date = receipt.Date;
                                        trsansactions.Ac_Id = receipt.CrAccount.ID;
                                        trsansactions.Op_Ac_Id = disc_id;
                                        trsansactions.Dr_Amount = receipt.DrAmount;
                                        trsansactions.Cr_Amount = 0;
                                        trsansactions.Entry = "RECEIPT";
                                        trsansactions.Eno = receipt.rno;
                                        trsansactions.Cinv_no = receipt.Invno;
                                        Transactions.post(trsansactions);


                                        trsansactions = new Model.Trsansactions();
                                        trsansactions.Tr_date = receipt.Date;
                                        trsansactions.Op_Ac_Id = receipt.CrAccount.ID;
                                        trsansactions.Ac_Id = disc_id;
                                        trsansactions.Cr_Amount = receipt.DrAmount;
                                        trsansactions.Dr_Amount = 0;
                                        trsansactions.Entry = "RECEIPT";
                                        trsansactions.Eno = receipt.rno;
                                        trsansactions.Cinv_no = receipt.Invno;
                                        Transactions.post(trsansactions);
                                    }
                                    res = true;
                                    //MessageBox.Show("Receipt saved!");
                                    receipt.InvBalance = DB.Connection.GetActBalance(dt: receipt.Date, id: receipt.CrAccount.ID, byInvoice: receipt.Invno);
                                    var rcpts = ViewModels_Variables.ModelViews.Receipts.Where((r1) => r1.Invno == receipt.Invno && r1.CrAccount.ID == receipt.CrAccount.ID);
                                    foreach (var rr in rcpts)
                                    {
                                        rr.InvBalance = DB.Connection.GetActBalance(dt: rr.Date, id: rr.CrAccount.ID, byInvoice: rr.Invno);
                                    }

                                    ViewModels_Variables.ModelViews.Add_Update(receipt);

                                }

                            }
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
                res = System.Windows.MessageBox.Show("Do you want to Remove this Entry", "Remove Receipt", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    try
                    {
                        var con = Connection.OpenConnection();
                        if (con != null)
                        {
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;
                            Transactions.delete(id, "RECEIPT");

                            cmd.CommandText = "delete from recurrings where eno=" + id + " and entry='RECEIPT'";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "delete from receipts where r_no=" + id;
                            int r = cmd.ExecuteNonQuery();

                            if (r > 0)
                            {
                                e = true;
                                var acc = ViewModels_Variables.ModelViews.Receipts.Where((a) => a.rno == id).FirstOrDefault();
                                if (acc != null) ViewModels_Variables.ModelViews.Remove(acc);
                                var task = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ENO == id && ts.ENTRY == "RECEIPT").FirstOrDefault();
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
        public static Model.ReceiptModel Find(int eno)
        {
            Model.ReceiptModel model = new Model.ReceiptModel();
            try
            {
                var receipts = DB.Connection._FetchTable("select * from receipts where r_no=" + eno);
                var rlist = (from r in receipts.AsEnumerable()
                             join a in ViewModels_Variables.ModelViews.Accounts on r.Field<int>("r_cashledger") equals a.ID
                             join a1 in ViewModels_Variables.ModelViews.Accounts on r.Field<int>("r_cr_ledger") equals a1.ID
                             select new
                             {
                                 obj = new Model.ReceiptModel()
                                 {
                                      
                                     CrAccount = a1,
                                     DrAccount = a,
                                     rno = r.Field<int>("r_no"),
                                     Date = r.Field<DateTime>("r_date"),
                                     DrAmount = Convert.ToDouble(r.Field<decimal>("r_cramount")),
                                     DiscP = Convert.ToDouble(r.Field<decimal>("r_disc")),
                                     DAmount = Convert.ToDouble(r.Field<decimal>("r_damount")),
                                     isRecurr = r.Field<bool>("r_isrecurring"),
                                     Task_Id = r.Field<int?>("r_taskid"),
                                     Invno = r.Field<string>("r_invoice"),
                                     InvBalance = DB.Connection.GetActBalance( dt: r.Field<DateTime>("r_date"), id: r.Field<int>("r_cr_ledger"), byInvoice: r.Field<string>("r_invoice"))

                                 }
                             }.obj
                           ).ToList<Model.ReceiptModel>();
                if (rlist != null)
                {
                    model = rlist[0];
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }

            return model;
        }
        public static bool Tasker(Model.Task t1, DateTime cdate)
        {
            bool res = false;

            long crid = 0, drid = 0;
            bool flag = false;


            try
            {
                var row = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ID == t1.ID && ts.ENO==t1.ENO  && ts.ENTRY == "RECEIPT").FirstOrDefault();
                if (row != null)
                {
                    var entry = ViewModels_Variables.ModelViews.Receipts.Where((r) => r.rno == row.ENO).FirstOrDefault();
                    if (entry != null)
                    {


                        if (entry.Invno.Length > 0)
                        {
                            var acb = Connection.GetActBalance(cdate, entry.CrAccount.ID, entry.Invno);
                            if (acb <= 0) { MessageBox.Show("Balance settled, No need to post again"); flag = true; }
                        }
                    }


                    if (entry.CrAccount.ID> 0 && entry.DrAccount.ID > 0 && flag == false)
                    {

                        var con = Connection.OpenConnection();
                        if (con != null)
                        {
                            OleDbCommand cmd = new OleDbCommand("insert into receipts  ([r_cashledger],[r_invoice] ,[r_cr_ledger] ,[r_cramount],[r_date],[r_disc],[r_damount] ) " +
                             " select [r_cashledger],[r_invoice] ,[r_cr_ledger] ,[r_cramount],[r_date],[r_disc],[r_damount]  from receipts where r_no=" + entry.rno, con);

                            int r = cmd.ExecuteNonQuery();
                            if (r > 0)
                            {

                                var no = Connection.NewEntryno("receipts", "r_no",conn:con) - 1;

                                OleDbParameter pdate = new OleDbParameter("@p_date", OleDbType.Date); pdate.Direction = System.Data.ParameterDirection.Input;
                                pdate.Value = cdate;
                                cmd = new OleDbCommand(" update receipts set r_date=@p_date,r_narration='Posted by TaskMaster',r_taskid=" + t1.ID + "  where r_no=" + no, con);
                                cmd.Parameters.Add(pdate);
                                cmd.ExecuteNonQuery();
                                Model.ReceiptModel receipt1 = new Model.ReceiptModel();
                                receipt1 = DB.Receipt.Find(no);
                                int disc_id = 0;
                                var disc_ac = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.Name == "DISCOUNT ALLOWED ON RECEIPTS" && ac.Parent.Name == "INDIRECT EXPENSE").FirstOrDefault();
                                if (disc_ac != null) { disc_id = disc_ac.ID; }
                                if (disc_id == 0)
                                {
                                    int gid = 0;
                                    var ind = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.Name == "INDIRECT EXPENSE").FirstOrDefault();
                                    if (ind != null) { gid = ind.ID; }
                                    if (gid == 0)
                                    {
                                        int parent = 0;
                                        var is_parent = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.Name == "EXPENSE").FirstOrDefault();
                                        if (is_parent != null)
                                        {
                                            parent = is_parent.ID;
                                        }
                                        else
                                        {
                                            MessageBox.Show("'EXPENSE' Parent Group Missing");
                                        }

                                        if (parent > 0)
                                        {
                                            Model.GroupModel group = new Model.GroupModel()
                                            {
                                                Name = "INDIRECT EXPENSE",
                                                P_ID = parent,
                                                Catagory = "None",
                                                Cr_Loc = 0,
                                                Dr_Loc = 0,
                                                Max_Disc = 0,
                                                Description = "Created By Program",
                                                ID = 0
                                            };
                                            gid = DB.AccountGroup.Save(group);
                                            if (gid > 0)
                                            {
                                                Model.AccountModel account = new Model.AccountModel()
                                                {
                                                    Name = "DISCOUNT ALLOWED ON RECEIPTS",
                                                    ParentGroup = 0,
                                                    Short_Name = "DISCOUNT ALLOWED ON RECEIPTS",
                                                    Address = "",
                                                    City = "",
                                                    Mob = "",
                                                    PhoneNo = "",
                                                    CrLimit = 0,
                                                    DrLimit = 0,
                                                    Catagory = "",
                                                    MaxDisc = 0

                                                };
                                                disc_id = DB.Accounts.Save(account);
                                            }
                                        }
                                    }
                                }
                                con.Close();
                                var trsansaction = ViewModels_Variables.ModelViews.AccountTransactions.Where((t) => t.Eno == entry.rno && t.Entry == "RECEIPT").FirstOrDefault();
                                if (trsansaction != null) ViewModels_Variables.ModelViews.Remove(trsansaction);
                                Model.Trsansactions trsansactions = new Model.Trsansactions();
                                trsansactions.Tr_date = cdate;
                                trsansactions.Ac_Id = entry.CrAccount.ID;
                                trsansactions.Op_Ac_Id = entry.DrAccount.ID;
                                trsansactions.Dr_Amount = entry.DrAmount;
                                trsansactions.Cr_Amount = 0;
                                trsansactions.Entry = "RECEIPT";
                                trsansactions.Eno = no;
                                trsansactions.Cinv_no = entry.Invno;
                                Transactions.post(trsansactions);

                                trsansactions = new Model.Trsansactions();
                                trsansactions.Tr_date = cdate;
                                trsansactions.Ac_Id = entry.DrAccount.ID;
                                trsansactions.Op_Ac_Id = entry.CrAccount.ID;
                                trsansactions.Cr_Amount = entry.DrAmount;
                                trsansactions.Dr_Amount = 0;
                                trsansactions.Entry = "RECEIPT";
                                trsansactions.Eno = entry.rno;
                                trsansactions.Cinv_no = entry.Invno;
                                Transactions.post(trsansactions);

                                if (entry.DAmount > 0)
                                {
                                    trsansactions = new Model.Trsansactions();
                                    trsansactions.Tr_date = cdate;
                                    trsansactions.Ac_Id = entry.CrAccount.ID;
                                    trsansactions.Op_Ac_Id = disc_id;
                                    trsansactions.Dr_Amount = entry.DrAmount;
                                    trsansactions.Cr_Amount = 0;
                                    trsansactions.Entry = "RECEIPT";
                                    trsansactions.Eno = entry.rno;
                                    trsansactions.Cinv_no = entry.Invno;
                                    Transactions.post(trsansactions);


                                    trsansactions = new Model.Trsansactions();
                                    trsansactions.Tr_date = cdate;
                                    trsansactions.Op_Ac_Id = entry.CrAccount.ID;
                                    trsansactions.Ac_Id = disc_id;
                                    trsansactions.Cr_Amount = entry.DrAmount;
                                    trsansactions.Dr_Amount = 0;
                                    trsansactions.Entry = "RECEIPT";
                                    trsansactions.Eno = entry.rno;
                                    trsansactions.Cinv_no = entry.Invno;
                                    Transactions.post(trsansactions);
                                }


                                MessageBox.Show("Receipt saved!");
                                ViewModels_Variables.ModelViews.Add_Update(receipt1);


                            }
                        }
                        else
                        {
                            MessageBox.Show("Task error, entry missing");
                        }
                    }
                }
            }

            catch (OleDbException rr)
            {
                MessageBox.Show(rr.Message.ToString());
            }



            return res;
        }
    }
}
