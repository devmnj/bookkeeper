using System;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Windows;

namespace accounts.DB
{
    static class Payment
    {
        public static DataTable PaymentTable;
        public static void Fetch()
        {
            PaymentTable = Connection._FetchTable("select * from payments order by p_no");
        }
        public static int Save(Model.PaymentModel payment, bool flag = false)
        {
            int res = 0;

            try
            {

                var con = Connection.OpenConnection();
                if (con != null)
                {


                    OleDbCommand cmd = new OleDbCommand("insert into payments (p_crledger,p_drledger,p_cr_amount,p_narration,p_date,p_disc,p_damount,p_invoice)values(@r_cashledger,@r_dr_ledger,@r_cramount,@r_narration,@r_date,@r_disc,@r_damount,@r_invoice)", con);

                    OleDbParameter cash = new OleDbParameter("@r_cashledger", OleDbType.Integer); cash.Direction = ParameterDirection.Input;
                    OleDbParameter crledger = new OleDbParameter("@r_dr_ledger", OleDbType.Integer); crledger.Direction = ParameterDirection.Input;
                    OleDbParameter amount = new OleDbParameter("@r_cramount", OleDbType.Double); amount.Direction = ParameterDirection.Input;
                    OleDbParameter narration = new OleDbParameter("@r_narration", OleDbType.VarChar); narration.Direction = ParameterDirection.Input;
                    OleDbParameter rdate = new OleDbParameter("@r_date", OleDbType.Date); rdate.Direction = ParameterDirection.Input;
                    OleDbParameter disc = new OleDbParameter("@r_disc", OleDbType.Double); disc.Direction = ParameterDirection.Input;
                    OleDbParameter damount = new OleDbParameter("@r_damount", OleDbType.Double); damount.Direction = ParameterDirection.Input;
                    OleDbParameter invoice = new OleDbParameter("@r_invoice", OleDbType.VarChar); invoice.Direction = ParameterDirection.Input;

                    invoice.Value = payment.Invno;
                    cash.Value = payment.CrAccount.ID;
                    crledger.Value = payment.DrAccount.ID;
                    amount.Value = payment.Amount;
                    narration.Value = payment.Narration;
                    rdate.Value = payment.Date;
                    disc.Value = payment.Disc;
                    damount.Value = payment.DiscAmount;

                    cmd.Parameters.Add(cash);
                    cmd.Parameters.Add(crledger);
                    cmd.Parameters.Add(amount);
                    cmd.Parameters.Add(narration);
                    cmd.Parameters.Add(rdate);
                    cmd.Parameters.Add(disc);
                    cmd.Parameters.Add(damount);
                    cmd.Parameters.Add(invoice);

                    int r = cmd.ExecuteNonQuery();
                    if (r > 0)
                    {
                        var no = Connection.NewEntryno("payments", "p_no", conn: con) - 1;
                        payment.pno = no;
                        int disc_id = 0;
                        var disc_ac = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.Name == "DISCOUNT RECEIVED ON PAYMENT" && ac.Parent.Name == "INDIRECT INCOME").FirstOrDefault();
                        if (disc_ac != null) { disc_id = disc_ac.ID; }

                        if (disc_id == 0)
                        {
                            int gid = 0;
                            var ind = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.Name == "INDIRECT ICOME").FirstOrDefault();
                            if (ind != null) { gid = ind.ID; }
                            if (gid == 0)
                            {
                                int parent = 0;
                                var is_parent = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.Name == "INCOME").FirstOrDefault();
                                if (is_parent != null)
                                {
                                    parent = is_parent.ID;
                                }
                                else
                                {
                                    MessageBox.Show("'INCOME' Parent Group Missing");
                                }

                                if (parent > 0)
                                {
                                    Model.GroupModel group = new Model.GroupModel()
                                    {
                                        Name = "INDIRECT INCOME",
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
                                            Name = "DISCOUNT RECEIVED ON PAYMENT",
                                            ParentGroup = 0,
                                            Short_Name = "DISCOUNT RECEIVED ON PAYMENT",
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

                        var trsansaction = ViewModels_Variables.ModelViews.AccountTransactions.Where((t) => t.Eno == payment.pno && t.Entry == "PAYMENT").FirstOrDefault();
                        if (trsansaction != null) ViewModels_Variables.ModelViews.Remove(trsansaction);

                        Model.Trsansactions trsansactions = new Model.Trsansactions();
                        trsansactions.Tr_date = payment.Date;
                        trsansactions.Ac_Id = payment.CrAccount.ID;
                        trsansactions.Op_Ac_Id = payment.DrAccount.ID;
                        trsansactions.Dr_Amount = payment.Amount;
                        trsansactions.Cr_Amount = 0;
                        trsansactions.Entry = "PAYMENT";
                        trsansactions.Eno = no;
                        trsansactions.Cinv_no = payment.Invno;
                        Transactions.post(trsansactions);

                        trsansactions = new Model.Trsansactions();
                        trsansactions.Tr_date = payment.Date;
                        trsansactions.Op_Ac_Id = payment.CrAccount.ID;
                        trsansactions.Ac_Id = payment.DrAccount.ID;
                        trsansactions.Cr_Amount = payment.Amount;
                        trsansactions.Dr_Amount = 0;
                        trsansactions.Entry = "PAYMENT";
                        trsansactions.Eno = no;
                        trsansactions.Cinv_no = payment.Invno;
                        Transactions.post(trsansactions);

                        if (payment.DiscAmount > 0)
                        {
                            trsansactions = new Model.Trsansactions();
                            trsansactions.Tr_date = payment.Date;
                            trsansactions.Op_Ac_Id = payment.DrAccount.ID;
                            trsansactions.Ac_Id = disc_id;
                            trsansactions.Dr_Amount = payment.DiscAmount;
                            trsansactions.Cr_Amount = 0;
                            trsansactions.Entry = "PAYMENT";
                            trsansactions.Eno = no;
                            trsansactions.Cinv_no = payment.Invno;
                            Transactions.post(trsansactions);

                            trsansactions = new Model.Trsansactions();
                            trsansactions.Tr_date = payment.Date;
                            trsansactions.Ac_Id = payment.DrAccount.ID;
                            trsansactions.Op_Ac_Id = disc_id;
                            trsansactions.Cr_Amount = payment.DiscAmount;
                            trsansactions.Dr_Amount = 0;
                            trsansactions.Entry = "PAYMENT";
                            trsansactions.Eno = no;
                            trsansactions.Cinv_no = payment.Invno;
                            Transactions.post(trsansactions);
                        }
                        res = no;
                        payment.InvBalance = DB.Connection.GetActBalance(dt: payment.Date, id: payment.DrAccount.ID, byInvoice: payment.Invno);
                        var rcpts = ViewModels_Variables.ModelViews.Payments.Where((r1) => r1.Invno == payment.Invno && r1.DrAccount.ID == payment.DrAccount.ID);
                        foreach (var rr in rcpts)
                        {
                            rr.InvBalance = DB.Connection.GetActBalance(dt: rr.Date, id: rr.DrAccount.ID, byInvoice: rr.Invno);
                        }
                        ViewModels_Variables.ModelViews.Add_Update(payment);
                    }
                }
            }

            catch (OleDbException rr)
            {
                MessageBox.Show(rr.Message.ToString());
            }

            return res;
        }
        public static bool Update(Model.PaymentModel payment, bool flag = false)
        {
            bool res = false;

            try
            {
                MessageBoxResult re = new MessageBoxResult();
                re = MessageBox.Show("Do you want Update this Payment", "Update Payment", MessageBoxButton.YesNo);
                if (re == MessageBoxResult.Yes)
                {
                    var con = Connection.OpenConnection();
                    if (con != null)
                    {

                        Transactions.delete(payment.pno, entry: "PAYMENT");
                        OleDbCommand cmd = new OleDbCommand("update payments set p_crledger=@r_cashledger,p_drledger=@r_dr_ledger,p_cr_amount=@r_cramount,p_narration=@r_narration,p_date=@r_date,p_disc=@r_disc,p_damount=@r_damount,p_invoice=@r_invoice where p_no= " + payment.pno, con);

                        OleDbParameter cash = new OleDbParameter("@r_cashledger", OleDbType.Integer); cash.Direction = ParameterDirection.Input;
                        OleDbParameter crledger = new OleDbParameter("@r_dr_ledger", OleDbType.Integer); crledger.Direction = ParameterDirection.Input;
                        OleDbParameter amount = new OleDbParameter("@r_cramount", OleDbType.Double); amount.Direction = ParameterDirection.Input;
                        OleDbParameter narration = new OleDbParameter("@r_narration", OleDbType.VarChar); narration.Direction = ParameterDirection.Input;
                        OleDbParameter rdate = new OleDbParameter("@r_date", OleDbType.Date); rdate.Direction = ParameterDirection.Input;
                        OleDbParameter disc = new OleDbParameter("@r_disc", OleDbType.Double); disc.Direction = ParameterDirection.Input;
                        OleDbParameter damount = new OleDbParameter("@r_damount", OleDbType.Double); damount.Direction = ParameterDirection.Input;
                        OleDbParameter invoice = new OleDbParameter("@r_invoice", OleDbType.VarChar); invoice.Direction = ParameterDirection.Input;

                        invoice.Value = payment.Invno;
                        cash.Value = payment.CrAccount.ID;
                        crledger.Value = payment.DrAccount.ID;
                        amount.Value = payment.Amount;
                        narration.Value = payment.Narration;
                        rdate.Value = payment.Date;
                        disc.Value = payment.Disc;
                        damount.Value = payment.DiscAmount;

                        cmd.Parameters.Add(cash);
                        cmd.Parameters.Add(crledger);
                        cmd.Parameters.Add(amount);
                        cmd.Parameters.Add(narration);
                        cmd.Parameters.Add(rdate);
                        cmd.Parameters.Add(disc);
                        cmd.Parameters.Add(damount);
                        cmd.Parameters.Add(invoice);

                        int r = cmd.ExecuteNonQuery();
                        if (r > 0)
                        {

                            int disc_id = 0;
                            var disc_ac = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.Name == "DISCOUNT RECEIVED ON PAYMENT" && ac.Parent.Name == "INDIRECT INCOME").FirstOrDefault();
                            if (disc_ac != null) { disc_id = disc_ac.ID; }

                            if (disc_id == 0)
                            {
                                int gid = 0;
                                var ind = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.Name == "INDIRECT ICOME").FirstOrDefault();
                                if (ind != null) { gid = ind.ID; }
                                if (gid == 0)
                                {
                                    int parent = 0;
                                    var is_parent = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.Name == "INCOME").FirstOrDefault();
                                    if (is_parent != null)
                                    {
                                        parent = is_parent.ID;
                                    }
                                    else
                                    {
                                        MessageBox.Show("'INCOME' Parent Group Missing");
                                    }

                                    if (parent > 0)
                                    {
                                        Model.GroupModel group = new Model.GroupModel()
                                        {
                                            Name = "INDIRECT INCOME",
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
                                                Name = "DISCOUNT RECEIVED ON PAYMENT",
                                                ParentGroup = gid,
                                                Short_Name = "DISCOUNT RECEIVED ON PAYMENT",
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

                            var trsansaction = ViewModels_Variables.ModelViews.AccountTransactions.Where((t) => t.Eno == payment.pno && t.Entry == "PAYMENT").FirstOrDefault();
                            if (trsansaction != null) ViewModels_Variables.ModelViews.Remove(trsansaction);

                            Model.Trsansactions trsansactions = new Model.Trsansactions();
                            trsansactions.Tr_date = payment.Date;
                            trsansactions.Ac_Id = payment.CrAccount.ID;
                            trsansactions.Op_Ac_Id = payment.DrAccount.ID;
                            trsansactions.Dr_Amount = payment.Amount;
                            trsansactions.Cr_Amount = 0;
                            trsansactions.Entry = "PAYMENT";
                            trsansactions.Eno = payment.pno;
                            trsansactions.Cinv_no = payment.Invno;
                            Transactions.post(trsansactions);

                            trsansactions = new Model.Trsansactions();
                            trsansactions.Tr_date = payment.Date;
                            trsansactions.Op_Ac_Id = payment.CrAccount.ID;
                            trsansactions.Ac_Id = payment.DrAccount.ID;
                            trsansactions.Cr_Amount = payment.Amount;
                            trsansactions.Dr_Amount = 0;
                            trsansactions.Entry = "PAYMENT";
                            trsansactions.Eno = payment.pno;
                            trsansactions.Cinv_no = payment.Invno;
                            Transactions.post(trsansactions);

                            if (payment.DiscAmount > 0)
                            {
                                trsansactions = new Model.Trsansactions();
                                trsansactions.Tr_date = payment.Date;
                                trsansactions.Op_Ac_Id = payment.DrAccount.ID;
                                trsansactions.Ac_Id = disc_id;
                                trsansactions.Dr_Amount = payment.DiscAmount;
                                trsansactions.Cr_Amount = 0;
                                trsansactions.Entry = "PAYMENT";
                                trsansactions.Eno = payment.pno;
                                trsansactions.Cinv_no = payment.Invno;
                                Transactions.post(trsansactions);

                                trsansactions = new Model.Trsansactions();
                                trsansactions.Tr_date = payment.Date;
                                trsansactions.Ac_Id = payment.DrAccount.ID;
                                trsansactions.Op_Ac_Id = disc_id;
                                trsansactions.Cr_Amount = payment.DiscAmount;
                                trsansactions.Dr_Amount = 0;
                                trsansactions.Entry = "PAYMENT";
                                trsansactions.Eno = payment.pno;
                                trsansactions.Cinv_no = payment.Invno;
                                Transactions.post(trsansactions);
                            }
                            res = true;

                            payment.InvBalance = DB.Connection.GetActBalance(dt: payment.Date, id: payment.DrAccount.ID, byInvoice: payment.Invno);
                            var rcpts = ViewModels_Variables.ModelViews.Payments.Where((r1) => r1.Invno == payment.Invno && r1.DrAccount.ID == payment.DrAccount.ID);
                            foreach (var rr in rcpts)
                            {
                                rr.InvBalance = DB.Connection.GetActBalance(dt: rr.Date, id: rr.DrAccount.ID, byInvoice: rr.Invno);
                            }
                            ViewModels_Variables.ModelViews.Add_Update(payment);

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
        public static bool Remove(int id)
        {
            bool e = false;
            try
            {
                MessageBoxResult res = new MessageBoxResult();
                res = System.Windows.MessageBox.Show("Do you want to Remove this Entry", "Remove Payment", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    try
                    {
                        var con = Connection.OpenConnection();
                        if (con != null)
                        {
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;
                            Transactions.delete(id, "PAYMENT");

                            cmd.CommandText = "delete from recurrings where eno=" + id + " and entry='PAYMENT'";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "delete from payments where p_no=" + id;
                            int r = cmd.ExecuteNonQuery();

                            if (r > 0)
                            {
                                e = true;
                                var acc = ViewModels_Variables.ModelViews.Payments.Where((a) => a.pno == id).FirstOrDefault();
                                if (acc != null) ViewModels_Variables.ModelViews.Remove(acc);
                                var task = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ENO == id && ts.ENTRY == "PAYMENT").FirstOrDefault();
                                if (task != null) { ViewModels_Variables.ModelViews.Remove(task); }

                                e = true;
                                var payment = ViewModels_Variables.ModelViews.Payments.Where((py) => py.pno == id).FirstOrDefault();
                                var rcpts = ViewModels_Variables.ModelViews.Payments.Where((r1) => r1.Invno == payment.Invno && r1.DrAccount.ID == payment.DrAccount.ID);
                                foreach (var rr in rcpts)
                                {
                                    rr.InvBalance = DB.Connection.GetActBalance(dt: rr.Date, id: rr.DrAccount.ID, byInvoice: rr.Invno);
                                }
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
        public static Model.PaymentModel Find(int eno)
        {
            Model.PaymentModel model = new Model.PaymentModel();
            try
            {
                var receipts = DB.Connection._FetchTable("select * from payments where p_no=" + eno);
                var rlist = (from r in receipts.AsEnumerable()
                             join a in ViewModels_Variables.ModelViews.Accounts on r.Field<int>("p_crledger") equals a.ID
                             join a1 in ViewModels_Variables.ModelViews.Accounts on r.Field<int>("p_drledger") equals a1.ID
                             select new
                             {
                                 obj = new Model.PaymentModel()
                                 {
                                     
                                     CrAccount = a1,
                                     DrAccount = a,
                                     pno = r.Field<int>("p_no"),
                                     Date = r.Field<DateTime>("p_date"),
                                     Amount = Convert.ToDouble(r.Field<decimal>("p_cr_amount")),
                                     Disc = Convert.ToDouble(r.Field<decimal>("p_disc")),
                                     DiscAmount = Convert.ToDouble(r.Field<decimal>("p_damount")),
                                     IsRecurring = r.Field<bool>("p_isrecurring"),
                                     Task_Id = r.Field<int?>("p_taskid"),
                                     Invno = r.Field<string>("p_invoice"),
                                     InvBalance = DB.Connection.GetActBalance(id: r.Field<int>("P_crledger"), byInvoice: r.Field<string>("P_invoice"))

                                 }
                             }.obj
                           ).ToList<Model.PaymentModel>();
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
                var row = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ID == t1.ID && ts.ENO == t1.ENO && ts.ENTRY == "PAYMENT").FirstOrDefault();
                if (row != null)
                {
                    var entry = ViewModels_Variables.ModelViews.Payments.Where((r) => r.pno == row.ENO).FirstOrDefault();
                    if (entry != null)
                    {


                        if (entry.Invno.Length > 0)
                        {
                            var acb = Connection.GetActBalance(cdate, entry.CrAccount.ID, entry.Invno);
                            if (acb <= 0) { MessageBox.Show("Balance settled, No need to post again"); flag = true; }
                        }
                    }


                    if (entry.CrAccount.ID > 0 && entry.DrAccount.ID > 0 && flag == false)
                    {

                        var con = Connection.OpenConnection();
                        if (con != null)
                        {
                            OleDbCommand cmd = new OleDbCommand("insert into paymentS  ([p_crledger],[p_invoice] ,[p_drledger] ,[p_cr_amount],[p_date],[p_disc],[p_damount] ) " +
                             " select [p_crledger],[p_invoice] ,[p_drledger] ,[p_cr_amount],[p_date],[p_disc],[p_damount]  from payments where p_no=" + entry.pno, con);

                            int r = cmd.ExecuteNonQuery();
                            if (r > 0)
                            {

                                var no = Connection.NewEntryno("payments", "p_no", conn: con) - 1;

                                OleDbParameter pdate = new OleDbParameter("@p_date", OleDbType.Date); pdate.Direction = System.Data.ParameterDirection.Input;
                                pdate.Value = cdate;
                                cmd = new OleDbCommand(" update payments set p_date=@p_date,p_narration='Posted by TaskMaster',p_taskid=" + t1.ID + "  where p_no=" + no, con);
                                cmd.Parameters.Add(pdate);
                                cmd.ExecuteNonQuery();
                                Model.PaymentModel payment = new  Model.PaymentModel();
                                payment = DB.Payment.Find(no);
                                int disc_id = 0;
                                var disc_ac = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.Name == "DISCOUNT RECEIEVED ON PAYMENTS" && ac.Parent.Name == "INDIRECT INCOME").FirstOrDefault();
                                if (disc_ac != null) { disc_id = disc_ac.ID; }
                                if (disc_id == 0)
                                {
                                    int gid = 0;
                                    var ind = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.Name == "INDIRECT INCOME").FirstOrDefault();
                                    if (ind != null) { gid = ind.ID; }
                                    if (gid == 0)
                                    {
                                        int parent = 0;
                                        var is_parent = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.Name == "INCOME").FirstOrDefault();
                                        if (is_parent != null)
                                        {
                                            parent = is_parent.ID;
                                        }
                                        else
                                        {
                                            MessageBox.Show("'INCOME' Parent Group Missing");
                                        }

                                        if (parent > 0)
                                        {
                                            Model.GroupModel group = new Model.GroupModel()
                                            {
                                                Name = "INDIRECT INCOME",
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
                                                    Name = "DISCOUNT RECEIEVED ON PAYMENT",
                                                    ParentGroup = 0,
                                                    Short_Name = "DISCOUNT RECEIVED ON PAYMENT",
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
                                res = true;
                                Model.Trsansactions trsansactions = new Model.Trsansactions();
                                trsansactions.Tr_date = payment.Date;
                                trsansactions.Ac_Id = payment.CrAccount.ID;
                                trsansactions.Op_Ac_Id = payment.DrAccount.ID;
                                trsansactions.Dr_Amount = payment.Amount;
                                trsansactions.Cr_Amount = 0;
                                trsansactions.Entry = "PAYMENT";
                                trsansactions.Eno = no;
                                trsansactions.Cinv_no = payment.Invno;
                                Transactions.post(trsansactions);

                                trsansactions = new Model.Trsansactions();
                                trsansactions.Tr_date = payment.Date;
                                trsansactions.Op_Ac_Id = payment.CrAccount.ID;
                                trsansactions.Ac_Id = payment.DrAccount.ID;
                                trsansactions.Cr_Amount = payment.Amount;
                                trsansactions.Dr_Amount = 0;
                                trsansactions.Entry = "PAYMENT";
                                trsansactions.Eno = no;
                                trsansactions.Cinv_no = payment.Invno;
                                Transactions.post(trsansactions);

                                if (payment.DiscAmount > 0)
                                {
                                    trsansactions = new Model.Trsansactions();
                                    trsansactions.Tr_date = payment.Date;
                                    trsansactions.Op_Ac_Id = payment.DrAccount.ID;
                                    trsansactions.Ac_Id = disc_id;
                                    trsansactions.Dr_Amount = payment.DiscAmount;
                                    trsansactions.Cr_Amount = 0;
                                    trsansactions.Entry = "PAYMENT";
                                    trsansactions.Eno = no;
                                    trsansactions.Cinv_no = payment.Invno;
                                    Transactions.post(trsansactions);

                                    trsansactions = new Model.Trsansactions();
                                    trsansactions.Tr_date = payment.Date;
                                    trsansactions.Ac_Id = payment.DrAccount.ID;
                                    trsansactions.Op_Ac_Id = disc_id;
                                    trsansactions.Cr_Amount = payment.DiscAmount;
                                    trsansactions.Dr_Amount = 0;
                                    trsansactions.Entry = "PAYMENT";
                                    trsansactions.Eno = no;
                                    trsansactions.Cinv_no = payment.Invno;
                                    Transactions.post(trsansactions);
                                }
                                payment.InvBalance = DB.Connection.GetActBalance(dt: payment.Date, id: payment.DrAccount.ID, byInvoice: payment.Invno);
                                var rcpts = ViewModels_Variables.ModelViews.Payments.Where((r1) => r1.Invno == payment.Invno && r1.DrAccount.ID == payment.DrAccount.ID);
                                foreach (var rr in rcpts)
                                {
                                    rr.InvBalance = DB.Connection.GetActBalance(dt: rr.Date, id: rr.DrAccount.ID, byInvoice: rr.Invno);
                                }
                                ViewModels_Variables.ModelViews.Add_Update(payment);                              


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
