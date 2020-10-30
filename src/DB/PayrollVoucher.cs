using System;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Windows;

namespace accounts.DB
{
    static class PayrollVoucher
    {
        public static DataTable PVoucherTable;
        public static void Fetch()
        {
            PVoucherTable = Connection._FetchTable("select * from payroll_posting order by pp_no");
        }
        public static Model.PayRollVoucherModel Find(int eno)
        {
            Model.PayRollVoucherModel model = new Model.PayRollVoucherModel();
            try
            {
                var receipts = DB.Connection._FetchTable("select * from payroll_posting where pp_no=" + eno);
                var jlist = (from r in receipts.AsEnumerable()
                             join e in ViewModels_Variables.ModelViews.Employees on r.Field<int>("eid") equals e.Account.ID
                             join a in ViewModels_Variables.ModelViews.Accounts on r.Field<int>("cash_ac") equals a.ID
                             select new
                             {
                                 obj = new Model.PayRollVoucherModel()
                                 {
                                     DrAccount = e,
                                     CrAccount = a,
                                     VoucherType = r.Field<string>("type"),
                                     PPDate = r.Field<DateTime>("post_date"),
                                     VNO = r.Field<int>("pp_no"),
                                     Amount = Convert.ToDouble(r.Field<decimal>("amount")),
                                     Narration = r.Field<string>("narration"),
                                     Task_ID = r.Field<int>("taskid"),
                                 }
                             }.obj
                           ).ToList<Model.PayRollVoucherModel>();
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
        public static int Save(Model.PayRollVoucherModel proll)
        {
            int pno = 0;
            try
            {

                var con = Connection.OpenConnection();
                if (con != null)
                {
                    OleDbParameter ppdate = new OleDbParameter("@pp_date", SqlDbType.DateTime); ppdate.Direction = ParameterDirection.Input;
                    OleDbParameter entry = new OleDbParameter("@type", SqlDbType.VarChar); entry.Direction = ParameterDirection.Input;
                    OleDbParameter ppamount = new OleDbParameter("@amount", SqlDbType.Decimal); ppamount.Direction = ParameterDirection.Input;
                    OleDbParameter narration = new OleDbParameter("@narration", SqlDbType.VarChar); narration.Direction = ParameterDirection.Input;
                    OleDbParameter eid = new OleDbParameter("@eid", SqlDbType.Int); eid.Direction = ParameterDirection.Input;
                    OleDbParameter cashid = new OleDbParameter("@cashid", SqlDbType.Int); cashid.Direction = ParameterDirection.Input; OleDbParameter taskid = new OleDbParameter("@taskid", SqlDbType.Int); taskid.Direction = ParameterDirection.Input;
                    OleDbParameter isrecurring = new OleDbParameter("@isrecurring", SqlDbType.Bit); isrecurring.Direction = ParameterDirection.Input;



                    entry.Value = proll.VoucherType;
                    ppamount.Value = proll.Amount;
                    narration.Value = proll.Narration;
                    ppdate.Value = proll.PPDate;
                    cashid.Value = proll.CrAccount.ID;
                    eid.Value = proll.DrAccount.Account.ID;
                    taskid.Value = proll.Task_ID;
                    OleDbCommand cmd = new OleDbCommand("insert into payroll_posting (post_date,eid,type,amount,narration,cash_ac,isrecurring,taskid) values(@pp_date" +
                        ",@eid,@type,@amount,@narration,@cashid,@isrecurring,@taskid)", con);
                    cmd.Parameters.Add(ppdate);
                    cmd.Parameters.Add(eid);
                    cmd.Parameters.Add(entry);
                    cmd.Parameters.Add(ppamount);
                    cmd.Parameters.Add(narration);
                    cmd.Parameters.Add(cashid);
                    cmd.Parameters.Add(isrecurring);
                    cmd.Parameters.Add(taskid);

                    int r = cmd.ExecuteNonQuery();

                    if (r != 0)
                    {
                        pno = Connection.NewEntryno(table: "payroll_posting", field: "pp_no", conn: con) - 1;
                        proll.VNO = pno;
                        Model.Trsansactions trans = new Model.Trsansactions();

                        var parent = ViewModels_Variables.ModelViews.AccountGroups.Where((p) => p.Name == "INDIRECT EXPENSE").FirstOrDefault();
                        int sac_id = 0;
                        var sacc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.Name == "Salary A / c").FirstOrDefault();
                        if (sacc != null)
                        {
                            sac_id = sacc.ID;
                        }
                        else
                        {
                            Model.AccountModel salac = new Model.AccountModel();
                            salac.Name = "Salary A/c";
                            salac.Short_Name = "Salary";
                            if (parent != null)
                            {
                                salac.ParentGroup = parent.ID;
                                salac.Parent = parent;
                            }
                            else
                            {
                                salac.ParentGroup = 1;
                            }
                            sac_id = DB.Accounts.Save(salac);

                        }

                        switch (proll.VoucherType)
                        {
                            case "Allowance":

                                trans = new Model.Trsansactions();
                                trans.Tr_date = proll.PPDate;
                                trans.Ac_Id = proll.CrAccount.ID;
                                trans.Op_Ac_Id = sac_id;
                                trans.Dr_Amount = proll.Amount;
                                trans.Cr_Amount = 0;
                                trans.Eno = proll.VNO;
                                trans.Entry = "PAYROLL VOUCHER";
                                trans.Cinv_no = proll.VNO.ToString();
                                Transactions.post(trans);

                                trans = new Model.Trsansactions();
                                trans.Tr_date = proll.PPDate;
                                trans.Op_Ac_Id = proll.CrAccount.ID;
                                trans.Ac_Id = sac_id;
                                trans.Cr_Amount = proll.Amount;
                                trans.Dr_Amount = 0;
                                trans.Eno = proll.VNO;
                                trans.Entry = "PAYROLL VOUCHER";
                                trans.Cinv_no = proll.VNO.ToString();
                                Transactions.post(trans);



                                break;
                            case "Advance":
                                trans = new Model.Trsansactions();
                                trans.Tr_date = proll.PPDate;
                                trans.Ac_Id = proll.CrAccount.ID;
                                trans.Op_Ac_Id = sac_id;
                                trans.Dr_Amount = proll.Amount;
                                trans.Cr_Amount = 0;
                                trans.Eno = proll.VNO;
                                trans.Entry = "PAYROLL VOUCHER";
                                trans.Cinv_no = proll.VNO.ToString();
                                Transactions.post(trans);

                                trans = new Model.Trsansactions();
                                trans.Tr_date = proll.PPDate;
                                trans.Op_Ac_Id = proll.CrAccount.ID;
                                trans.Ac_Id = sac_id;
                                trans.Cr_Amount = proll.Amount;
                                trans.Dr_Amount = 0;
                                trans.Eno = proll.VNO;
                                trans.Entry = "PAYROLL VOUCHER";
                                trans.Cinv_no = proll.VNO.ToString();
                                Transactions.post(trans);


                                break;
                            case "Deduction":

                                trans = new Model.Trsansactions();
                                trans.Tr_date = proll.PPDate;
                                trans.Ac_Id = proll.CrAccount.ID;
                                trans.Op_Ac_Id = proll.DrAccount.Account.ID;
                                trans.Cr_Amount = proll.Amount;
                                trans.Dr_Amount = 0;
                                trans.Eno = proll.VNO;
                                trans.Entry = "PAYROLL VOUCHER";
                                trans.Cinv_no = proll.VNO.ToString();
                                Transactions.post(trans);



                                break;
                            case "Commission":

                                trans = new Model.Trsansactions();
                                trans.Tr_date = proll.PPDate;
                                trans.Ac_Id = proll.CrAccount.ID;
                                trans.Op_Ac_Id = sac_id;
                                trans.Dr_Amount = proll.Amount;
                                trans.Cr_Amount = 0;
                                trans.Eno = proll.VNO;
                                trans.Entry = "PAYROLL VOUCHER";
                                trans.Cinv_no = proll.VNO.ToString();
                                Transactions.post(trans);

                                trans = new Model.Trsansactions();
                                trans.Tr_date = proll.PPDate;
                                trans.Op_Ac_Id = proll.CrAccount.ID;
                                trans.Ac_Id = sac_id;
                                trans.Cr_Amount = proll.Amount;
                                trans.Dr_Amount = 0;
                                trans.Eno = proll.VNO;
                                trans.Entry = "PAYROLL VOUCHER";
                                trans.Cinv_no = proll.VNO.ToString();
                                Transactions.post(trans);

                                break;
                        }
                        ViewModels_Variables.ModelViews.Add_Update(proll);
                    }
                }
            }

            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
            return pno;
        }
        public static bool Update(Model.PayRollVoucherModel proll, bool flag = false)
        {
            bool res = false;
            try
            {

                MessageBoxResult re = new MessageBoxResult();
                re = MessageBox.Show("Do you want Edit this Voucher", "Update Voucher", MessageBoxButton.YesNo);
                if (re == MessageBoxResult.Yes)
                {
                    Transactions.delete(proll.VNO, "PAYROLL VOUCHER");
                    var con = Connection.OpenConnection();
                    if (con != null)
                    {
                        OleDbParameter ppdate = new OleDbParameter("@pp_date", SqlDbType.DateTime); ppdate.Direction = ParameterDirection.Input;
                        OleDbParameter entry = new OleDbParameter("@type", SqlDbType.VarChar); entry.Direction = ParameterDirection.Input;
                        OleDbParameter ppamount = new OleDbParameter("@amount", SqlDbType.Decimal); ppamount.Direction = ParameterDirection.Input;
                        OleDbParameter narration = new OleDbParameter("@narration", SqlDbType.VarChar); narration.Direction = ParameterDirection.Input;
                        OleDbParameter eid = new OleDbParameter("@eid", SqlDbType.Int); eid.Direction = ParameterDirection.Input;
                        OleDbParameter cashid = new OleDbParameter("@cashid", SqlDbType.Int); cashid.Direction = ParameterDirection.Input; OleDbParameter taskid = new OleDbParameter("@taskid", SqlDbType.Int); taskid.Direction = ParameterDirection.Input;
                        OleDbParameter isrecurring = new OleDbParameter("@isrecurring", SqlDbType.Bit); isrecurring.Direction = ParameterDirection.Input;



                        entry.Value = proll.VoucherType;
                        ppamount.Value = proll.Amount;
                        narration.Value = proll.Narration;
                        ppdate.Value = proll.PPDate;
                        cashid.Value = proll.CrAccount.ID;
                        eid.Value = proll.DrAccount.Account.ID;
                        taskid.Value = proll.Task_ID;
                        OleDbCommand cmd = new OleDbCommand("update   payroll_posting set post_date=@pp_date,eid=@eid,type=@type,amount=@amount,narration=@narration,cash_ac=@cashid,isrecurring=@isrecurring,taskid=@taskid where pp_no=" + proll.VNO, con);
                        cmd.Parameters.Add(ppdate);
                        cmd.Parameters.Add(eid);
                        cmd.Parameters.Add(entry);
                        cmd.Parameters.Add(ppamount);
                        cmd.Parameters.Add(narration);
                        cmd.Parameters.Add(cashid);
                        cmd.Parameters.Add(isrecurring);
                        cmd.Parameters.Add(taskid);

                        int r = cmd.ExecuteNonQuery();

                        if (r != 0)
                        {

                            Model.Trsansactions trans = new Model.Trsansactions();
                            res = true;
                            var parent = ViewModels_Variables.ModelViews.AccountGroups.Where((p) => p.Name == "INDIRECT EXPENSE").FirstOrDefault();
                            int sac_id = 0;
                            var sacc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.Name == "Salary A / c").FirstOrDefault();
                            if (sacc != null)
                            {
                                sac_id = sacc.ID;
                            }
                            else
                            {
                                Model.AccountModel salac = new Model.AccountModel();
                                salac.Name = "Salary A/c";
                                salac.Short_Name = "Salary";
                                if (parent != null)
                                {
                                    salac.ParentGroup = parent.ID;
                                }
                                else
                                {
                                    salac.ParentGroup = 1;
                                }
                                sac_id = DB.Accounts.Save(salac);

                            }

                            switch (proll.VoucherType)
                            {
                                case "Allowance":

                                    trans = new Model.Trsansactions();
                                    trans.Tr_date = proll.PPDate;
                                    trans.Ac_Id = proll.CrAccount.ID;
                                    trans.Op_Ac_Id = sac_id;
                                    trans.Dr_Amount = proll.Amount;
                                    trans.Cr_Amount = 0;
                                    trans.Eno = proll.VNO;
                                    trans.Entry = "PAYROLL VOUCHER";
                                    trans.Cinv_no = proll.VNO.ToString();
                                    Transactions.post(trans);

                                    trans = new Model.Trsansactions();
                                    trans.Tr_date = proll.PPDate;
                                    trans.Op_Ac_Id = proll.CrAccount.ID;
                                    trans.Ac_Id = sac_id;
                                    trans.Cr_Amount = proll.Amount;
                                    trans.Dr_Amount = 0;
                                    trans.Eno = proll.VNO;
                                    trans.Entry = "PAYROLL VOUCHER";
                                    trans.Cinv_no = proll.VNO.ToString();
                                    Transactions.post(trans);


                                    break;
                                case "Advance":
                                    trans = new Model.Trsansactions();
                                    trans.Tr_date = proll.PPDate;
                                    trans.Ac_Id = proll.CrAccount.ID;
                                    trans.Op_Ac_Id = sac_id;
                                    trans.Dr_Amount = proll.Amount;
                                    trans.Cr_Amount = 0;
                                    trans.Eno = proll.VNO;
                                    trans.Entry = "PAYROLL VOUCHER";
                                    trans.Cinv_no = proll.VNO.ToString();
                                    Transactions.post(trans);

                                    trans = new Model.Trsansactions();
                                    trans.Tr_date = proll.PPDate;
                                    trans.Op_Ac_Id = proll.CrAccount.ID;
                                    trans.Ac_Id = sac_id;
                                    trans.Cr_Amount = proll.Amount;
                                    trans.Dr_Amount = 0;
                                    trans.Eno = proll.VNO;
                                    trans.Entry = "PAYROLL VOUCHER";
                                    trans.Cinv_no = proll.VNO.ToString();
                                    Transactions.post(trans);


                                    break;
                                case "Deduction":

                                    trans = new Model.Trsansactions();
                                    trans.Tr_date = proll.PPDate;
                                    trans.Ac_Id = proll.CrAccount.ID;
                                    trans.Op_Ac_Id = proll.DrAccount.Account.ID;
                                    trans.Cr_Amount = proll.Amount;
                                    trans.Dr_Amount = 0;
                                    trans.Eno = proll.VNO;
                                    trans.Entry = "PAYROLL VOUCHER";
                                    trans.Cinv_no = proll.VNO.ToString();
                                    Transactions.post(trans);



                                    break;
                                case "Commission":

                                    trans = new Model.Trsansactions();
                                    trans.Tr_date = proll.PPDate;
                                    trans.Ac_Id = proll.CrAccount.ID;
                                    trans.Op_Ac_Id = sac_id;
                                    trans.Dr_Amount = proll.Amount;
                                    trans.Cr_Amount = 0;
                                    trans.Eno = proll.VNO;
                                    trans.Entry = "PAYROLL VOUCHER";
                                    trans.Cinv_no = proll.VNO.ToString();
                                    Transactions.post(trans);

                                    trans = new Model.Trsansactions();
                                    trans.Tr_date = proll.PPDate;
                                    trans.Op_Ac_Id = proll.CrAccount.ID;
                                    trans.Ac_Id = sac_id;
                                    trans.Cr_Amount = proll.Amount;
                                    trans.Dr_Amount = 0;
                                    trans.Eno = proll.VNO;
                                    trans.Entry = "PAYROLL VOUCHER";
                                    trans.Cinv_no = proll.VNO.ToString();
                                    Transactions.post(trans);

                                    break;
                            }
                            ViewModels_Variables.ModelViews.Add_Update(proll);
                        }
                    }
                }
            }

            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
            return res;
        }
        public static bool Remove(int id)
        {
            bool e = false;
            try
            {
                MessageBoxResult res = new MessageBoxResult();
                res = System.Windows.MessageBox.Show("Do you want to Remove this Voucher", "Remove Voucher", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    try
                    {

                        Transactions.delete(id, "PAYROLL VOUCHER");
                        var con = Connection.OpenConnection();
                        if (con != null)
                        {

                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;


                            cmd.CommandText = "delete from recurrings where eno=" + id + " and entry='PAYROLL VOUCHER'";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "delete from payroll_posting where pp_no=" + id;
                            int r = cmd.ExecuteNonQuery();

                            if (r > 0)
                            {
                                e = true;
                                var acc = ViewModels_Variables.ModelViews.PayrollVouchers.Where((a) => a.VNO == id).FirstOrDefault();
                                if (acc != null) ViewModels_Variables.ModelViews.Remove(acc);
                                var task = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ENO == id && ts.ENTRY == "PAYROLL VOUCHER").FirstOrDefault();
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
        public static bool Tasker(Model.Task task, DateTime dt)
        {
            bool res = false;
            try
            {
                int crid = 0, lid = 0; double amount = 0;
                string entry = null;
                var vouchers = ViewModels_Variables.ModelViews.PayrollVouchers.Where((pv) => pv.VNO == task.ENO).FirstOrDefault();
                if (vouchers != null)
                {
                    string sql = "INSERT INTO [payroll_posting] ([post_date],[eid],[amount],[narration],[type],[cash_ac])" +
                        " select [post_date],[eid],[amount],[narration],[type],[cash_ac] from [payroll_posting] where pp_no =" + task.ENO;

                    var con = Connection.OpenConnection();
                    if (con != null)
                    {
                        OleDbCommand cmd = new OleDbCommand(sql, con);
                        int r = cmd.ExecuteNonQuery();
                        var no = Connection.NewEntryno(table: "payroll_posting", field: "pp_no", conn: con) - 1;
                        OleDbParameter pdate = new OleDbParameter("@p_date", SqlDbType.DateTime); pdate.Direction = ParameterDirection.Input;
                        pdate.Value = dt;

                        cmd = new OleDbCommand(" update payroll_posting set post_date=@p_date,narration='Posted by TaskMaster',taskid=" + task.ID + "  where pp_no=" + no, con);
                        cmd.Parameters.Add(pdate);
                        r = cmd.ExecuteNonQuery();

                        if (r != 0)
                        {
                            res = true;
                            Model.Trsansactions trans = new Model.Trsansactions();
                            var proll = DB.PayrollVoucher.Find(no);
                            var parent = ViewModels_Variables.ModelViews.AccountGroups.Where((p) => p.Name == "INDIRECT EXPENSE").FirstOrDefault();
                            int sac_id = 0;
                            var sacc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.Name == "Salary A / c").FirstOrDefault();
                            if (sacc != null)
                            {
                                sac_id = sacc.ID;
                            }
                            else
                            {
                                Model.AccountModel salac = new Model.AccountModel();
                                salac.Name = "Salary A/c";
                                salac.Short_Name = "Salary";
                                if (parent != null)
                                {
                                    salac.ParentGroup = parent.ID;
                                }
                                else
                                {
                                    salac.ParentGroup = 1;
                                }
                                sac_id = DB.Accounts.Save(salac);

                            }

                            switch (proll.VoucherType)
                            {
                                case "Allowance":

                                    trans = new Model.Trsansactions();
                                    trans.Tr_date = proll.PPDate;
                                    trans.Ac_Id = proll.CrAccount.ID;
                                    trans.Op_Ac_Id = sac_id;
                                    trans.Dr_Amount = proll.Amount;
                                    trans.Cr_Amount = 0;
                                    trans.Eno = proll.VNO;
                                    trans.Entry = "PAYROLL VOUCHER";
                                    trans.Cinv_no = proll.VNO.ToString();
                                    Transactions.post(trans);

                                    trans = new Model.Trsansactions();
                                    trans.Tr_date = proll.PPDate;
                                    trans.Op_Ac_Id = proll.CrAccount.ID;
                                    trans.Ac_Id = sac_id;
                                    trans.Cr_Amount = proll.Amount;
                                    trans.Dr_Amount = 0;
                                    trans.Eno = proll.VNO;
                                    trans.Entry = "PAYROLL VOUCHER";
                                    trans.Cinv_no = proll.VNO.ToString();
                                    Transactions.post(trans);


                                    break;
                                case "Advance":
                                    trans = new Model.Trsansactions();
                                    trans.Tr_date = proll.PPDate;
                                    trans.Ac_Id = proll.CrAccount.ID;
                                    trans.Op_Ac_Id = sac_id;
                                    trans.Dr_Amount = proll.Amount;
                                    trans.Cr_Amount = 0;
                                    trans.Eno = proll.VNO;
                                    trans.Entry = "PAYROLL VOUCHER";
                                    trans.Cinv_no = proll.VNO.ToString();
                                    Transactions.post(trans);

                                    trans = new Model.Trsansactions();
                                    trans.Tr_date = proll.PPDate;
                                    trans.Op_Ac_Id = proll.CrAccount.ID;
                                    trans.Ac_Id = sac_id;
                                    trans.Cr_Amount = proll.Amount;
                                    trans.Dr_Amount = 0;
                                    trans.Eno = proll.VNO;
                                    trans.Entry = "PAYROLL VOUCHER";
                                    trans.Cinv_no = proll.VNO.ToString();
                                    Transactions.post(trans);


                                    break;
                                case "Deduction":

                                    trans = new Model.Trsansactions();
                                    trans.Tr_date = proll.PPDate;
                                    trans.Ac_Id = proll.CrAccount.ID;
                                    trans.Op_Ac_Id = proll.DrAccount.Account.ID;
                                    trans.Cr_Amount = proll.Amount;
                                    trans.Dr_Amount = 0;
                                    trans.Eno = proll.VNO;
                                    trans.Entry = "PAYROLL VOUCHER";
                                    trans.Cinv_no = proll.VNO.ToString();
                                    Transactions.post(trans);



                                    break;
                                case "Commission":

                                    trans = new Model.Trsansactions();
                                    trans.Tr_date = proll.PPDate;
                                    trans.Ac_Id = proll.CrAccount.ID;
                                    trans.Op_Ac_Id = sac_id;
                                    trans.Dr_Amount = proll.Amount;
                                    trans.Cr_Amount = 0;
                                    trans.Eno = proll.VNO;
                                    trans.Entry = "PAYROLL VOUCHER";
                                    trans.Cinv_no = proll.VNO.ToString();
                                    Transactions.post(trans);

                                    trans = new Model.Trsansactions();
                                    trans.Tr_date = proll.PPDate;
                                    trans.Op_Ac_Id = proll.CrAccount.ID;
                                    trans.Ac_Id = sac_id;
                                    trans.Cr_Amount = proll.Amount;
                                    trans.Dr_Amount = 0;
                                    trans.Eno = proll.VNO;
                                    trans.Entry = "PAYROLL VOUCHER";
                                    trans.Cinv_no = proll.VNO.ToString();
                                    Transactions.post(trans);

                                    break;
                            }
                            ViewModels_Variables.ModelViews.Add_Update(proll);
                        }
                    }
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
