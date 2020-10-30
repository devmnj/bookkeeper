using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace accounts.DB
{
    static class MonthlyPayroll
    {
        public static DataTable MPayrollTable;
        public static DataTable MPayrollVouchers;
        public static void Fetch()
        {
            MPayrollTable = Connection._FetchTable("select * from payroll_entry order by pe_no");
            MPayrollVouchers = Connection._FetchTable("select * from payroll_entry_vouchers order by id");
        }
        public static int Save(Model.PayRollEntryModel payRoll, bool[] flag)
        {
            int peno = 0;
            try
            {
                var con = DB.Connection.OpenConnection();
                if (con != null)
                {
                    OleDbParameter eid = new OleDbParameter("@eid", SqlDbType.VarChar); eid.Direction = ParameterDirection.Input;
                    OleDbParameter narration = new OleDbParameter("@narration", SqlDbType.VarChar); eid.Direction = ParameterDirection.Input;
                    OleDbParameter pedate = new OleDbParameter("@pe_date", SqlDbType.DateTime); pedate.Direction = ParameterDirection.Input;
                    OleDbParameter basicpay = new OleDbParameter("@bp", SqlDbType.Float); basicpay.Direction = ParameterDirection.Input;

                    OleDbParameter totals = new OleDbParameter("@total", SqlDbType.Float); totals.Direction = ParameterDirection.Input;

                    OleDbParameter wh = new OleDbParameter("@whours", SqlDbType.Float); wh.Direction = ParameterDirection.Input;
                    OleDbParameter wd = new OleDbParameter("@wdays", SqlDbType.Float); wd.Direction = ParameterDirection.Input;
                    OleDbParameter isrecurring = new OleDbParameter("@isrecurring", OleDbType.Boolean); isrecurring.Direction = ParameterDirection.Input;
                    OleDbParameter crledger = new OleDbParameter("@crledger", SqlDbType.Int); crledger.Direction = ParameterDirection.Input;


                    isrecurring.Value = payRoll.IsRecurring;
                    crledger.Value = payRoll.CrAccount.ID;
                    eid.Value = payRoll.DrAccount.Account.ID;
                    pedate.Value = payRoll.DATE;
                    basicpay.Value = payRoll.Basic;
                    wh.Value = payRoll.WHs;
                    wd.Value = payRoll.WDs;
                    narration.Value = payRoll.Narration;
                    double tot = (payRoll.Basic + payRoll.Commission.Amount + payRoll.Allownaces.Amount) - (payRoll.Advance.Amount + payRoll.Deductions.Amount);
                    totals.Value = tot;

                    OleDbCommand cmd = new OleDbCommand("insert into payroll_entry ([crledger],[eid],[bp]" +
                     ",[whours],[wdays],[narration],[isrecurring],[pe_date],[total]) values(@crledger,@eid,@bp,@whours,@wdays,@narration,@isrecurring,@pe_date,@total)", con);

                    cmd.Parameters.Add(crledger);
                    cmd.Parameters.Add(eid);
                    cmd.Parameters.Add(basicpay);
                    cmd.Parameters.Add(wh);
                    cmd.Parameters.Add(wd);
                    cmd.Parameters.Add(narration);
                    cmd.Parameters.Add(isrecurring);
                    cmd.Parameters.Add(pedate);
                    cmd.Parameters.Add(totals);

                    int r = cmd.ExecuteNonQuery();
                    if (r > 0)
                    {
                        peno = DB.Connection.NewEntryno(table: "payroll_entry", field: "pe_no", conn: con) - 1;
                        payRoll.VNO = peno;
                        ViewModels_Variables.ModelViews.Add_Update(payRoll);

                        OleDbParameter voucher = new OleDbParameter("@voucher", SqlDbType.VarChar); voucher.Direction = ParameterDirection.Input;
                        OleDbParameter pe_no = new OleDbParameter("@pe_no", SqlDbType.Int); pe_no.Direction = ParameterDirection.Input;
                        OleDbParameter isgenerated = new OleDbParameter("@isgenerated", OleDbType.Boolean); isgenerated.Direction = ParameterDirection.Input;
                        OleDbParameter amount = new OleDbParameter("@amount", SqlDbType.Float); amount.Direction = ParameterDirection.Input;
                        //Vouchers
                        if (payRoll.Allownaces.Amount > 0)
                        {
                            voucher.Value = "Allowance";
                            pe_no.Value = peno;
                            amount.Value = payRoll.Allownaces.Amount;
                            isgenerated.Value = flag[0];
                            cmd = new OleDbCommand("insert into payroll_entry_vouchers (voucher,pe_no,isgen,amount) values(@voucher,@pe_no,@isgenerated,@amount)", con);
                            cmd.Parameters.Add(voucher);
                            cmd.Parameters.Add(pe_no);
                            cmd.Parameters.Add(isgenerated);
                            cmd.Parameters.Add(amount);
                            cmd.ExecuteNonQuery();
                        }
                        if (payRoll.Advance.Amount > 0)
                        {
                            voucher = new OleDbParameter("@voucher", SqlDbType.VarChar); voucher.Direction = ParameterDirection.Input;
                            pe_no = new OleDbParameter("@pe_no", SqlDbType.Int); pe_no.Direction = ParameterDirection.Input;
                            isgenerated = new OleDbParameter("@isgenerated", SqlDbType.Bit); isgenerated.Direction = ParameterDirection.Input;
                            amount = new OleDbParameter("@amount", SqlDbType.Float); amount.Direction = ParameterDirection.Input;
                            voucher.Value = "Advance";
                            pe_no.Value = peno;
                            amount.Value = payRoll.Advance;
                            isgenerated.Value = flag[1];
                            cmd = new OleDbCommand("insert into payroll_entry_vouchers (voucher,pe_no,isgen,amount) values(@voucher,@pe_no,@isgenerated,@amount)", con);
                            cmd.Parameters.Add(voucher);
                            cmd.Parameters.Add(pe_no);
                            cmd.Parameters.Add(isgenerated);
                            cmd.Parameters.Add(amount);
                            cmd.ExecuteNonQuery();
                        }
                        if (payRoll.Commission.Amount > 0)
                        {
                            voucher = new OleDbParameter("@voucher", SqlDbType.VarChar); voucher.Direction = ParameterDirection.Input;
                            pe_no = new OleDbParameter("@pe_no", SqlDbType.Int); pe_no.Direction = ParameterDirection.Input;
                            isgenerated = new OleDbParameter("@isgenerated", SqlDbType.Bit); isgenerated.Direction = ParameterDirection.Input;
                            amount = new OleDbParameter("@amount", SqlDbType.Float); amount.Direction = ParameterDirection.Input;
                            voucher.Value = "Commission";
                            pe_no.Value = peno;
                            amount.Value = payRoll.Commission;
                            isgenerated.Value = flag[2];
                            cmd = new OleDbCommand("insert into payroll_entry_vouchers (voucher,pe_no,isgen,amount) values(@voucher,@pe_no,@isgenerated,@amount)", con);
                            cmd.Parameters.Add(voucher);
                            cmd.Parameters.Add(pe_no);
                            cmd.Parameters.Add(isgenerated);
                            cmd.Parameters.Add(amount);
                            cmd.ExecuteNonQuery();
                        }
                        if (payRoll.Deductions.Amount > 0)
                        {
                            voucher = new OleDbParameter("@voucher", SqlDbType.VarChar); voucher.Direction = ParameterDirection.Input;
                            pe_no = new OleDbParameter("@pe_no", SqlDbType.Int); pe_no.Direction = ParameterDirection.Input;
                            isgenerated = new OleDbParameter("@isgenerated", SqlDbType.Bit); isgenerated.Direction = ParameterDirection.Input;
                            amount = new OleDbParameter("@amount", SqlDbType.Float); amount.Direction = ParameterDirection.Input;

                            voucher.Value = "Deduction";
                            pe_no.Value = peno;
                            amount.Value = payRoll.Deductions;
                            isgenerated.Value = flag[3];
                            cmd = new OleDbCommand("insert into payroll_entry_vouchers (voucher,pe_no,isgen,amount) values(@voucher,@pe_no,@isgenerated,@amount)", con);
                            cmd.Parameters.Add(voucher);
                            cmd.Parameters.Add(pe_no);
                            cmd.Parameters.Add(isgenerated);
                            cmd.Parameters.Add(amount);
                            cmd.ExecuteNonQuery();
                        }

                        con.Close();
                        Model.Trsansactions tr = new Model.Trsansactions();
                        if (flag[0] == false && payRoll.Allownaces.Amount > 0)
                        {
                            //allowance
                            tr = new Model.Trsansactions();
                            tr.Tr_date = payRoll.DATE;
                            tr.Ac_Id = payRoll.CrAccount.ID;
                            tr.Op_Ac_Id = payRoll.DrAccount.Account.ID;
                            tr.Dr_Amount = payRoll.Allownaces.Amount;
                            tr.Cr_Amount = 0;
                            tr.Cinv_no = payRoll.VNO.ToString();
                            tr.Eno = payRoll.VNO;
                            tr.Entry = "PAYROLL";
                            Transactions.post(tr);


                            tr = new Model.Trsansactions();
                            tr.Tr_date = payRoll.DATE;
                            tr.Op_Ac_Id = payRoll.CrAccount.ID;
                            tr.Ac_Id = payRoll.DrAccount.Account.ID;
                            tr.Cr_Amount = payRoll.Allownaces.Amount;
                            tr.Dr_Amount = 0;
                            tr.Cinv_no = payRoll.VNO.ToString();
                            tr.Eno = payRoll.VNO;
                            tr.Entry = "PAYROLL";
                            Transactions.post(tr);


                        }
                        if (flag[1] == false && payRoll.Commission.Amount > 0)
                        {
                            //comm

                            tr = new Model.Trsansactions();
                            tr.Tr_date = payRoll.DATE;
                            tr.Ac_Id = payRoll.CrAccount.ID;
                            tr.Op_Ac_Id = payRoll.DrAccount.Account.ID;
                            tr.Dr_Amount = payRoll.Commission.Amount;
                            tr.Cr_Amount = 0;
                            tr.Cinv_no = payRoll.VNO.ToString();
                            tr.Eno = payRoll.VNO;
                            tr.Entry = "PAYROLL";
                            Transactions.post(tr);

                            tr = new Model.Trsansactions();
                            tr.Tr_date = payRoll.DATE;
                            tr.Op_Ac_Id = payRoll.CrAccount.ID;
                            tr.Ac_Id = payRoll.DrAccount.Account.ID;
                            tr.Cr_Amount = payRoll.Commission.Amount;
                            tr.Dr_Amount = 0;
                            tr.Cinv_no = payRoll.VNO.ToString();
                            tr.Eno = payRoll.VNO;
                            tr.Entry = "PAYROLL";
                            Transactions.post(tr);
                        }
                        if (flag[2] == false && payRoll.Advance.Amount > 0)
                        {

                            //Avances

                            tr = new Model.Trsansactions();
                            tr.Tr_date = payRoll.DATE;
                            tr.Ac_Id = payRoll.CrAccount.ID;
                            tr.Op_Ac_Id = payRoll.DrAccount.Account.ID;
                            tr.Dr_Amount = payRoll.Advance.Amount;
                            tr.Cr_Amount = 0;
                            tr.Cinv_no = payRoll.VNO.ToString();
                            tr.Eno = payRoll.VNO;
                            tr.Entry = "PAYROLL";
                            Transactions.post(tr);

                            tr = new Model.Trsansactions();
                            tr.Tr_date = payRoll.DATE;
                            tr.Op_Ac_Id = payRoll.CrAccount.ID;
                            tr.Ac_Id = payRoll.DrAccount.Account.ID;
                            tr.Cr_Amount = payRoll.Advance.Amount;
                            tr.Dr_Amount = 0;
                            tr.Cinv_no = payRoll.VNO.ToString();
                            tr.Eno = payRoll.VNO;
                            tr.Entry = "PAYROLL";
                            Transactions.post(tr);


                        }
                        if (flag[3] == false && payRoll.Deductions.Amount > 0)
                        {

                            //comm

                            tr = new Model.Trsansactions();
                            tr.Tr_date = payRoll.DATE;
                            tr.Ac_Id = payRoll.CrAccount.ID;
                            tr.Op_Ac_Id = payRoll.DrAccount.Account.ID;
                            tr.Cr_Amount = payRoll.Deductions.Amount;
                            tr.Dr_Amount = 0;
                            tr.Cinv_no = payRoll.VNO.ToString();
                            tr.Eno = payRoll.VNO;
                            tr.Entry = "PAYROLL";
                            Transactions.post(tr);

                            tr = new Model.Trsansactions();
                            tr.Tr_date = payRoll.DATE;
                            tr.Op_Ac_Id = payRoll.CrAccount.ID;
                            tr.Ac_Id = payRoll.DrAccount.Account.ID;
                            tr.Dr_Amount = payRoll.Deductions.Amount;
                            tr.Cr_Amount = 0;
                            tr.Cinv_no = payRoll.VNO.ToString();
                            tr.Eno = payRoll.VNO;
                            tr.Entry = "PAYROLL";
                            Transactions.post(tr);


                        }
                        if (payRoll.Basic > 0)
                        {

                            //Basic Pay

                            tr = new Model.Trsansactions();
                            tr.Tr_date = payRoll.DATE;
                            tr.Ac_Id = payRoll.CrAccount.ID;
                            tr.Op_Ac_Id = payRoll.DrAccount.Account.ID;
                            tr.Dr_Amount = payRoll.Basic;
                            tr.Cr_Amount = 0;
                            tr.Cinv_no = payRoll.VNO.ToString();
                            tr.Eno = payRoll.VNO;
                            tr.Entry = "PAYROLL";
                            Transactions.post(tr);

                            tr = new Model.Trsansactions();
                            tr.Tr_date = payRoll.DATE;
                            tr.Op_Ac_Id = payRoll.CrAccount.ID;
                            tr.Ac_Id = payRoll.DrAccount.Account.ID;
                            tr.Cr_Amount = payRoll.Basic;
                            tr.Dr_Amount = 0;
                            tr.Cinv_no = payRoll.VNO.ToString();
                            tr.Eno = payRoll.VNO;
                            tr.Entry = "PAYROLL";
                            Transactions.post(tr);


                        }

                    }
                    con.Close();
                }
                else
                {
                    MessageBox.Show("SQL Conncetion error");
                }
            }



            catch (Exception m)
            {
                MessageBox.Show(m.Message.ToString());

            }
            return peno;
        }
        public static bool Update(Model.PayRollEntryModel payRoll, bool[] flag)
        {
            bool peno = false;
            MessageBoxResult re = new MessageBoxResult();
            re = MessageBox.Show("Do you want Edit this Payroll", "Update Payroll", MessageBoxButton.YesNo);
            if (re == MessageBoxResult.Yes)
            {
                try
                {
                    Transactions.delete(eno: payRoll.VNO, entry: "PAYROLL");
                    var con = DB.Connection.OpenConnection();
                    if (con != null)
                    {
                        OleDbParameter eid = new OleDbParameter("@eid", SqlDbType.VarChar); eid.Direction = ParameterDirection.Input;
                        OleDbParameter narration = new OleDbParameter("@narration", SqlDbType.VarChar); eid.Direction = ParameterDirection.Input;
                        OleDbParameter pedate = new OleDbParameter("@pe_date", SqlDbType.DateTime); pedate.Direction = ParameterDirection.Input;
                        OleDbParameter basicpay = new OleDbParameter("@bp", SqlDbType.Float); basicpay.Direction = ParameterDirection.Input;

                        OleDbParameter totals = new OleDbParameter("@total", SqlDbType.Float); totals.Direction = ParameterDirection.Input;

                        OleDbParameter wh = new OleDbParameter("@whours", SqlDbType.Float); wh.Direction = ParameterDirection.Input;
                        OleDbParameter wd = new OleDbParameter("@wdays", SqlDbType.Float); wd.Direction = ParameterDirection.Input;
                        OleDbParameter isrecurring = new OleDbParameter("@isrecurring", OleDbType.Boolean); isrecurring.Direction = ParameterDirection.Input;
                        OleDbParameter crledger = new OleDbParameter("@crledger", SqlDbType.Int); crledger.Direction = ParameterDirection.Input;

                        isrecurring.Value = payRoll.IsRecurring;
                        crledger.Value = payRoll.CrAccount.ID;
                        eid.Value = payRoll.DrAccount.Account.ID;
                        pedate.Value = payRoll.DATE;
                        basicpay.Value = payRoll.Basic;
                        wh.Value = payRoll.WHs;
                        wd.Value = payRoll.WDs;
                        narration.Value = payRoll.Narration;
                        double tot = (payRoll.Basic + payRoll.Commission.Amount + payRoll.Allownaces.Amount) - (payRoll.Advance.Amount + payRoll.Deductions.Amount);
                        totals.Value = tot;

                        OleDbCommand cmd = new OleDbCommand("update payroll_entry set [crledger]=@crledger,[eid]=@eid,[bp]=@bp" +
                         ",[whours]=@whours,[wdays]=@wdays,[narration]=@narration,[isrecurring]=@isrecurring,[pe_date]=@pe_date,[total]=@total where pe_no=" + payRoll.VNO, con);

                        cmd.Parameters.Add(crledger);
                        cmd.Parameters.Add(eid);
                        cmd.Parameters.Add(basicpay);
                        cmd.Parameters.Add(wh);
                        cmd.Parameters.Add(wd);
                        cmd.Parameters.Add(narration);
                        cmd.Parameters.Add(isrecurring);
                        cmd.Parameters.Add(pedate);
                        cmd.Parameters.Add(totals);

                        int r = cmd.ExecuteNonQuery();
                        if (r > 0)
                        {
                            cmd = new OleDbCommand("delete from payroll_entry_vouchers where pe_no=" + payRoll.VNO, con);
                            cmd.ExecuteNonQuery();

                            ViewModels_Variables.ModelViews.Add_Update(payRoll);

                            OleDbParameter voucher = new OleDbParameter("@voucher", SqlDbType.VarChar); voucher.Direction = ParameterDirection.Input;
                            OleDbParameter pe_no = new OleDbParameter("@pe_no", SqlDbType.Int); pe_no.Direction = ParameterDirection.Input;
                            OleDbParameter isgenerated = new OleDbParameter("@isgenerated", OleDbType.Boolean); isgenerated.Direction = ParameterDirection.Input;
                            OleDbParameter amount = new OleDbParameter("@amount", SqlDbType.Float); amount.Direction = ParameterDirection.Input;
                            //Vouchers
                            if (payRoll.Allownaces.Amount > 0)
                            {
                                voucher.Value = "Allowance";
                                pe_no.Value = peno;
                                amount.Value = payRoll.Allownaces.Amount;
                                isgenerated.Value = flag[0];
                                cmd = new OleDbCommand("insert into payroll_entry_vouchers (voucher,pe_no,isgen,amount) values(@voucher,@pe_no,@isgenerated,@amount)", con);
                                cmd.Parameters.Add(voucher);
                                cmd.Parameters.Add(pe_no);
                                cmd.Parameters.Add(isgenerated);
                                cmd.Parameters.Add(amount);
                                cmd.ExecuteNonQuery();
                            }
                            if (payRoll.Advance.Amount > 0)
                            {
                                voucher = new OleDbParameter("@voucher", SqlDbType.VarChar); voucher.Direction = ParameterDirection.Input;
                                pe_no = new OleDbParameter("@pe_no", SqlDbType.Int); pe_no.Direction = ParameterDirection.Input;
                                isgenerated = new OleDbParameter("@isgenerated", SqlDbType.Bit); isgenerated.Direction = ParameterDirection.Input;
                                amount = new OleDbParameter("@amount", SqlDbType.Float); amount.Direction = ParameterDirection.Input;
                                voucher.Value = "Advance";
                                pe_no.Value = peno;
                                amount.Value = payRoll.Advance.Amount;
                                isgenerated.Value = flag[1];
                                cmd = new OleDbCommand("insert into payroll_entry_vouchers (voucher,pe_no,isgen,amount) values(@voucher,@pe_no,@isgenerated,@amount)", con);
                                cmd.Parameters.Add(voucher);
                                cmd.Parameters.Add(pe_no);
                                cmd.Parameters.Add(isgenerated);
                                cmd.Parameters.Add(amount);
                                cmd.ExecuteNonQuery();
                            }
                            if (payRoll.Commission.Amount > 0)
                            {
                                voucher = new OleDbParameter("@voucher", SqlDbType.VarChar); voucher.Direction = ParameterDirection.Input;
                                pe_no = new OleDbParameter("@pe_no", SqlDbType.Int); pe_no.Direction = ParameterDirection.Input;
                                isgenerated = new OleDbParameter("@isgenerated", SqlDbType.Bit); isgenerated.Direction = ParameterDirection.Input;
                                amount = new OleDbParameter("@amount", SqlDbType.Float); amount.Direction = ParameterDirection.Input;
                                voucher.Value = "Commission";
                                pe_no.Value = peno;
                                amount.Value = payRoll.Commission.Amount;
                                isgenerated.Value = flag[2];
                                cmd = new OleDbCommand("insert into payroll_entry_vouchers (voucher,pe_no,isgen,amount) values(@voucher,@pe_no,@isgenerated,@amount)", con);
                                cmd.Parameters.Add(voucher);
                                cmd.Parameters.Add(pe_no);
                                cmd.Parameters.Add(isgenerated);
                                cmd.Parameters.Add(amount);
                                cmd.ExecuteNonQuery();
                            }
                            if (payRoll.Deductions.Amount > 0)
                            {
                                voucher = new OleDbParameter("@voucher", SqlDbType.VarChar); voucher.Direction = ParameterDirection.Input;
                                pe_no = new OleDbParameter("@pe_no", SqlDbType.Int); pe_no.Direction = ParameterDirection.Input;
                                isgenerated = new OleDbParameter("@isgenerated", SqlDbType.Bit); isgenerated.Direction = ParameterDirection.Input;
                                amount = new OleDbParameter("@amount", SqlDbType.Float); amount.Direction = ParameterDirection.Input;

                                voucher.Value = "Deduction";
                                pe_no.Value = peno;
                                amount.Value = payRoll.Deductions;
                                isgenerated.Value = flag[3];
                                cmd = new OleDbCommand("insert into payroll_entry_vouchers (voucher,pe_no,isgen,amount) values(@voucher,@pe_no,@isgenerated,@amount)", con);
                                cmd.Parameters.Add(voucher);
                                cmd.Parameters.Add(pe_no);
                                cmd.Parameters.Add(isgenerated);
                                cmd.Parameters.Add(amount);
                                cmd.ExecuteNonQuery();
                            }

                            con.Close();
                            Model.Trsansactions tr = new Model.Trsansactions();
                            if (flag[0] == false && payRoll.Allownaces.Amount > 0)
                            {
                                //allowance
                                tr = new Model.Trsansactions();
                                tr.Tr_date = payRoll.DATE;
                                tr.Ac_Id = payRoll.CrAccount.ID;
                                tr.Op_Ac_Id = payRoll.DrAccount.Account.ID;
                                tr.Dr_Amount = payRoll.Allownaces.Amount;
                                tr.Cr_Amount = 0;
                                tr.Cinv_no = payRoll.VNO.ToString();
                                tr.Eno = payRoll.VNO;
                                tr.Entry = "PAYROLL";
                                Transactions.post(tr);


                                tr = new Model.Trsansactions();
                                tr.Tr_date = payRoll.DATE;
                                tr.Op_Ac_Id = payRoll.CrAccount.ID;
                                tr.Ac_Id = payRoll.DrAccount.Account.ID;
                                tr.Cr_Amount = payRoll.Allownaces.Amount;
                                tr.Dr_Amount = 0;
                                tr.Cinv_no = payRoll.VNO.ToString();
                                tr.Eno = payRoll.VNO;
                                tr.Entry = "PAYROLL";
                                Transactions.post(tr);


                            }
                            if (flag[1] == false && payRoll.Commission.Amount > 0)
                            {
                                //comm

                                tr = new Model.Trsansactions();
                                tr.Tr_date = payRoll.DATE;
                                tr.Ac_Id = payRoll.CrAccount.ID;
                                tr.Op_Ac_Id = payRoll.DrAccount.Account.ID;
                                tr.Dr_Amount = payRoll.Commission.Amount;
                                tr.Cr_Amount = 0;
                                tr.Cinv_no = payRoll.VNO.ToString();
                                tr.Eno = payRoll.VNO;
                                tr.Entry = "PAYROLL";
                                Transactions.post(tr);

                                tr = new Model.Trsansactions();
                                tr.Tr_date = payRoll.DATE;
                                tr.Op_Ac_Id = payRoll.CrAccount.ID;
                                tr.Ac_Id = payRoll.DrAccount.Account.ID;
                                tr.Cr_Amount = payRoll.Commission.Amount;
                                tr.Dr_Amount = 0;
                                tr.Cinv_no = payRoll.VNO.ToString();
                                tr.Eno = payRoll.VNO;
                                tr.Entry = "PAYROLL";
                                Transactions.post(tr);  
                            }
                            if (flag[2] == false && payRoll.Advance.Amount > 0)
                            {

                                //Avances

                                tr = new Model.Trsansactions();
                                tr.Tr_date = payRoll.DATE;
                                tr.Ac_Id = payRoll.CrAccount.ID;
                                tr.Op_Ac_Id = payRoll.EID;
                                tr.Dr_Amount = payRoll.Advance.Amount;
                                tr.Cr_Amount = 0;
                                tr.Cinv_no = payRoll.VNO.ToString();
                                tr.Eno = payRoll.VNO;
                                tr.Entry = "PAYROLL";
                                Transactions.post(tr);

                                tr = new Model.Trsansactions();
                                tr.Tr_date = payRoll.DATE;
                                tr.Op_Ac_Id = payRoll.CrAccount.ID;
                                tr.Ac_Id = payRoll.DrAccount.Account.ID;
                                tr.Cr_Amount = payRoll.Advance.Amount;
                                tr.Dr_Amount = 0;
                                tr.Cinv_no = payRoll.VNO.ToString();
                                tr.Eno = payRoll.VNO;
                                tr.Entry = "PAYROLL";
                                Transactions.post(tr);


                            }
                            if (flag[3] == false && payRoll.Deductions.Amount > 0)
                            {

                                //comm

                                tr = new Model.Trsansactions();
                                tr.Tr_date = payRoll.DATE;
                                tr.Ac_Id = payRoll.CrAccount.ID;
                                tr.Op_Ac_Id = payRoll.DrAccount.Account.ID;
                                tr.Cr_Amount = payRoll.Deductions.Amount;
                                tr.Dr_Amount = 0;
                                tr.Cinv_no = payRoll.VNO.ToString();
                                tr.Eno = payRoll.VNO;
                                tr.Entry = "PAYROLL";
                                Transactions.post(tr);

                                tr = new Model.Trsansactions();
                                tr.Tr_date = payRoll.DATE;
                                tr.Op_Ac_Id = payRoll.CrAccount.ID;
                                tr.Ac_Id = payRoll.DrAccount.Account.ID;
                                tr.Dr_Amount = payRoll.Deductions.Amount;
                                tr.Cr_Amount = 0;
                                tr.Cinv_no = payRoll.VNO.ToString();
                                tr.Eno = payRoll.VNO;
                                tr.Entry = "PAYROLL";
                                Transactions.post(tr);


                            }
                            if (payRoll.Basic > 0)
                            {

                                //Basic Pay

                                tr = new Model.Trsansactions();
                                tr.Tr_date = payRoll.DATE;
                                tr.Ac_Id = payRoll.CrAccount.ID;
                                tr.Op_Ac_Id = payRoll.DrAccount.Account.ID;
                                tr.Dr_Amount = payRoll.Basic;
                                tr.Cr_Amount = 0;
                                tr.Cinv_no = payRoll.VNO.ToString();
                                tr.Eno = payRoll.VNO;
                                tr.Entry = "PAYROLL";
                                Transactions.post(tr);

                                tr = new Model.Trsansactions();
                                tr.Tr_date = payRoll.DATE;
                                tr.Op_Ac_Id = payRoll.CrAccount.ID;
                                tr.Ac_Id = payRoll.DrAccount.Account.ID;
                                tr.Cr_Amount = payRoll.Basic;
                                tr.Dr_Amount = 0;
                                tr.Cinv_no = payRoll.VNO.ToString();
                                tr.Eno = payRoll.VNO;
                                tr.Entry = "PAYROLL";
                                Transactions.post(tr);


                            }

                        }
                        con.Close();
                    }
                    else
                    {
                        MessageBox.Show("SQL Conncetion error");
                    }
                }



                catch (Exception m)
                {
                    MessageBox.Show(m.Message.ToString());

                }
            }
            return peno;
        }
        public static bool Remove(int id)
        {
            bool e = false;
            try
            {
                MessageBoxResult res = new MessageBoxResult();
                res = System.Windows.MessageBox.Show("Do you want to Remove the Voucher", "Remove Voucher", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    try
                    {

                        Transactions.delete(id, "PAYROLL");
                        var con = Connection.OpenConnection();
                        if (con != null)
                        {

                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;


                            cmd.CommandText = "delete from recurrings where eno=" + id + " and entry='PAYROLL'";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "delete from payroll_entry_vouchers where pe_no=" + id;
                            int r = cmd.ExecuteNonQuery();
                            cmd.CommandText = "delete from payroll_entry where pe_no=" + id;
                            r = cmd.ExecuteNonQuery();


                            if (r > 0)
                            {
                                e = true;
                                var acc = ViewModels_Variables.ModelViews.Payrolls.Where((a) => a.VNO == id).FirstOrDefault();
                                if (acc != null) ViewModels_Variables.ModelViews.Remove(acc);
                                var task = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ENO == id && ts.ENTRY == "PAYROLL").FirstOrDefault();
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

        public static void Insert_Vouchers()
        {

        }
        public static void Find()
        {

        }
        //Tasker
        public static bool Tasker(Model.Task task, DateTime date)
        {
           
            bool res = false;
            try
            {
                var payroll = ViewModels_Variables.ModelViews.Payrolls.Where((pr) => pr.VNO == task.ENO).FirstOrDefault();
                if (payroll != null)
                {
                    int drid, crid;

                    drid = payroll.DrAccount.Account.ID;
                    crid = payroll.CrAccount.ID;
                    double bp = payroll.Basic;

                    var isgernerated = (from p in ViewModels_Variables.ModelViews.Payrolls where p.DATE.Month == date.Month && p.DATE.Year == date.Year && p.EID == payroll.EID select p.VNO).ToList<int>();
                    if (isgernerated.Count > 0)
                    {
                        MessageBox.Show("Payroll already generated for this month ");
                    }
                    else
                    {
                        bool all_flag = false, comm_flag = false, adv_flag = false, ded_flag = false;
                        var con = DB.Connection.OpenConnection();
                        if (con != null)
                        {
                            OleDbCommand cmd = new OleDbCommand(" insert into payroll_entry ([crledger],[eid],[bp]" +
                                 ",[whours],[wdays],[narration],[pe_date],[total]) select [crledger],[eid],[bp]" +
                                 ",[whours],[wdays],[narration],[pe_date],[total] from  payroll_entry where pe_no= " + task.ENO, con);
                            int r = cmd.ExecuteNonQuery(); if (r != 0) res = true;
                            var no = DB.Connection.NewEntryno(table: "payroll_entry", field: "pe_no", conn: con) - 1;

                            OleDbParameter pdate = new OleDbParameter("@p_date", SqlDbType.DateTime); pdate.Direction = ParameterDirection.Input;
                            pdate.Value = date;
                            cmd = new OleDbCommand(" update payroll_entry set pe_date=@p_date,narration='Posted by TaskMaster',taskid=" + task.ID + "  where pe_no=" + no, con);
                            cmd.Parameters.Add(pdate);
                            cmd.ExecuteNonQuery();

                            var isposted = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == date.Month && p.PPDate.Year == date.Year && p.DrAccount.Account.ID == drid select p).Count();
                            if (isposted > 0)
                            {
                                var allows = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == date.Month && p.PPDate.Year == date.Year && p.DrAccount.Account.ID == drid && p.VoucherType == "Allowance" select p.Amount).Sum();
                                var comms = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == date.Month && p.PPDate.Year == date.Year && p.DrAccount.Account.ID == drid && p.VoucherType == "Commission" select p.Amount).Sum();
                                var advs = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == date.Month && p.PPDate.Year == date.Year && p.DrAccount.Account.ID == drid && p.VoucherType == "Advance" select p.Amount).Sum();
                                var deds = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == date.Month && p.PPDate.Year == date.Year && p.DrAccount.Account.ID == drid && p.VoucherType == "Deduction" select p.Amount).Sum();
                                var wages = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == date.Month && p.PPDate.Year == date.Year && p.DrAccount.Account.ID == drid && p.VoucherType == "Wage" select p.Amount).Sum();

                                if (allows > 0) all_flag = true;
                                if (comms > 0) comm_flag = true;
                                if (advs > 0) adv_flag = true;
                                if (deds > 0) ded_flag = true;

                                OleDbParameter voucher = new OleDbParameter("@voucher", SqlDbType.VarChar); voucher.Direction = ParameterDirection.Input;
                                OleDbParameter pe_no = new OleDbParameter("@pe_no", SqlDbType.Int); pe_no.Direction = ParameterDirection.Input;
                                OleDbParameter isgenerated = new OleDbParameter("@isgenerated", SqlDbType.Bit); isgenerated.Direction = ParameterDirection.Input;
                                OleDbParameter amount = new OleDbParameter("@amount", SqlDbType.Float); amount.Direction = ParameterDirection.Input;
                                if (allows > 0)
                                {
                                    voucher.Value = "Allowance";
                                    pe_no.Value = no;
                                    amount.Value = allows;
                                    isgenerated.Value = all_flag;
                                    cmd = new OleDbCommand("insert into payroll_entry_vouchers (voucher,pe_no,isgen,amount) values(@voucher,@pe_no,@isgenerated,@amount)", con);
                                    cmd.Parameters.Add(voucher);
                                    cmd.Parameters.Add(pe_no);
                                    cmd.Parameters.Add(isgenerated);
                                    cmd.Parameters.Add(amount);
                                    cmd.ExecuteNonQuery();
                                }

                                if (advs > 0)
                                {
                                    voucher = new OleDbParameter("@voucher", SqlDbType.VarChar); voucher.Direction = ParameterDirection.Input;
                                    pe_no = new OleDbParameter("@pe_no", SqlDbType.Int); pe_no.Direction = ParameterDirection.Input;
                                    isgenerated = new OleDbParameter("@isgenerated", SqlDbType.Bit); isgenerated.Direction = ParameterDirection.Input;
                                    amount = new OleDbParameter("@amount", SqlDbType.Float); amount.Direction = ParameterDirection.Input;
                                    voucher.Value = "Advance";
                                    pe_no.Value = no;
                                    amount.Value = advs;
                                    isgenerated.Value = adv_flag;
                                    cmd = new OleDbCommand("insert into payroll_entry_vouchers (voucher,pe_no,isgen,amount) values(@voucher,@pe_no,@isgenerated,@amount)", con);
                                    cmd.Parameters.Add(voucher);
                                    cmd.Parameters.Add(pe_no);
                                    cmd.Parameters.Add(isgenerated);
                                    cmd.Parameters.Add(amount);
                                    cmd.ExecuteNonQuery();
                                }
                                if (comms > 0)
                                {
                                    voucher = new OleDbParameter("@voucher", SqlDbType.VarChar); voucher.Direction = ParameterDirection.Input;
                                    pe_no = new OleDbParameter("@pe_no", SqlDbType.Int); pe_no.Direction = ParameterDirection.Input;
                                    isgenerated = new OleDbParameter("@isgenerated", SqlDbType.Bit); isgenerated.Direction = ParameterDirection.Input;
                                    amount = new OleDbParameter("@amount", SqlDbType.Float); amount.Direction = ParameterDirection.Input;
                                    voucher.Value = "Commission";
                                    pe_no.Value = no;
                                    amount.Value = comms;
                                    isgenerated.Value = comm_flag;
                                    cmd = new OleDbCommand("insert into payroll_entry_vouchers (voucher,pe_no,isgen,amount) values(@voucher,@pe_no,@isgenerated,@amount)", con);
                                    cmd.Parameters.Add(voucher);
                                    cmd.Parameters.Add(pe_no);
                                    cmd.Parameters.Add(isgenerated);
                                    cmd.Parameters.Add(amount);
                                    cmd.ExecuteNonQuery();
                                }
                                if (deds > 0)
                                {
                                    voucher = new OleDbParameter("@voucher", SqlDbType.VarChar); voucher.Direction = ParameterDirection.Input;
                                    pe_no = new OleDbParameter("@pe_no", SqlDbType.Int); pe_no.Direction = ParameterDirection.Input;
                                    isgenerated = new OleDbParameter("@isgenerated", SqlDbType.Bit); isgenerated.Direction = ParameterDirection.Input;
                                    amount = new OleDbParameter("@amount", SqlDbType.Float); amount.Direction = ParameterDirection.Input;

                                    voucher.Value = "Deduction";
                                    pe_no.Value = no;
                                    amount.Value = deds;
                                    isgenerated.Value = ded_flag;
                                    cmd = new OleDbCommand("insert into payroll_entry_vouchers (voucher,pe_no,isgen,amount) values(@voucher,@pe_no,@isgenerated,@amount)", con);
                                    cmd.Parameters.Add(voucher);
                                    cmd.Parameters.Add(pe_no);
                                    cmd.Parameters.Add(isgenerated);
                                    cmd.Parameters.Add(amount);
                                    cmd.ExecuteNonQuery();
                                }
                                

                                con.Close();

                                Model.Trsansactions tr = new Model.Trsansactions();
                                if (all_flag == false && allows > 0)
                                {
                                    //allowance
                                    tr = new Model.Trsansactions();
                                    tr.Tr_date = date;
                                    tr.Ac_Id = crid;
                                    tr.Op_Ac_Id = drid;
                                    tr.Dr_Amount = allows;
                                    tr.Cr_Amount = 0;
                                    tr.Cinv_no = no.ToString();
                                    tr.Eno = no;
                                    tr.Entry = "PAYROLL";
                                    Transactions.post(tr);


                                    tr = new Model.Trsansactions();
                                    tr.Tr_date = date;
                                    tr.Op_Ac_Id = crid;
                                    tr.Ac_Id = drid;
                                    tr.Cr_Amount = allows;
                                    tr.Dr_Amount = 0;
                                    tr.Cinv_no = no.ToString();
                                    tr.Eno = no;
                                    tr.Entry = "PAYROLL";
                                    Transactions.post(tr);


                                }
                                if (comm_flag == false && comms > 0)
                                {
                                    //comm

                                    tr = new Model.Trsansactions();
                                    tr.Tr_date = date;
                                    tr.Ac_Id = crid;
                                    tr.Op_Ac_Id = drid;
                                    tr.Dr_Amount = comms;
                                    tr.Cr_Amount = 0;
                                    tr.Cinv_no = no.ToString();
                                    tr.Eno = no;
                                    tr.Entry = "PAYROLL";
                                    Transactions.post(tr);

                                    tr = new Model.Trsansactions();
                                    tr.Tr_date = date;
                                    tr.Op_Ac_Id = crid;
                                    tr.Ac_Id = drid;
                                    tr.Cr_Amount = comms;
                                    tr.Dr_Amount = 0;
                                    tr.Cinv_no = no.ToString();
                                    tr.Eno = no;
                                    tr.Entry = "PAYROLL";
                                    Transactions.post(tr);
                                }
                                if (adv_flag == false && advs > 0)
                                {

                                    //Avances

                                    tr = new Model.Trsansactions();
                                    tr.Tr_date = date;
                                    tr.Ac_Id = crid;
                                    tr.Op_Ac_Id = drid;
                                    tr.Dr_Amount = advs;
                                    tr.Cr_Amount = 0;
                                    tr.Cinv_no = no.ToString();
                                    tr.Eno = no;
                                    tr.Entry = "PAYROLL";
                                    Transactions.post(tr);

                                    tr = new Model.Trsansactions();
                                    tr.Tr_date = date;
                                    tr.Op_Ac_Id = crid;
                                    tr.Ac_Id = drid;
                                    tr.Cr_Amount = advs;
                                    tr.Dr_Amount = 0;
                                    tr.Cinv_no = no.ToString();
                                    tr.Eno = no;
                                    tr.Entry = "PAYROLL";
                                    Transactions.post(tr);


                                }
                                if (ded_flag == false && deds > 0)
                                {

                                    //ded

                                    tr = new Model.Trsansactions();
                                    tr.Tr_date = date;
                                    tr.Ac_Id = crid;
                                    tr.Op_Ac_Id = drid;
                                    tr.Cr_Amount = deds;
                                    tr.Dr_Amount = 0;
                                    tr.Cinv_no = no.ToString();
                                    tr.Eno = no;
                                    tr.Entry = "PAYROLL";
                                    Transactions.post(tr);

                                    tr = new Model.Trsansactions();
                                    tr.Tr_date = date;
                                    tr.Op_Ac_Id = crid;
                                    tr.Ac_Id = drid;
                                    tr.Dr_Amount = deds;
                                    tr.Cr_Amount = 0;
                                    tr.Cinv_no = no.ToString();
                                    tr.Eno = no;
                                    tr.Entry = "PAYROLL";
                                    Transactions.post(tr);


                                }
                                if (bp > 0)
                                {

                                    //Basic Pay

                                    tr = new Model.Trsansactions();
                                    tr.Tr_date = date;
                                    tr.Ac_Id = crid;
                                    tr.Op_Ac_Id = drid;
                                    tr.Dr_Amount = bp;
                                    tr.Cr_Amount = 0;
                                    tr.Cinv_no = no.ToString();
                                    tr.Eno = no;
                                    tr.Entry = "PAYROLL";
                                    Transactions.post(tr);

                                    tr = new Model.Trsansactions();
                                    tr.Tr_date = date;
                                    tr.Op_Ac_Id = crid;
                                    tr.Ac_Id = drid;
                                    tr.Cr_Amount = bp;
                                    tr.Dr_Amount = 0;
                                    tr.Cinv_no = no.ToString();
                                    tr.Eno = no;
                                    tr.Entry = "PAYROLL";
                                    Transactions.post(tr);


                                }

                                
                            }
                           

                        }

                        else
                        {
                            MessageBox.Show("SQL Server Error");

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
    }
}
