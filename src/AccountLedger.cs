using accounts.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace accounts
{
    class AccountLedger : System.ComponentModel.INotifyPropertyChanged
    {
        SqlConnection con = new SqlConnection();
        double _cash, _cp, _cr;
        // static AccountLedger ac = new AccountLedger();
        public event PropertyChangedEventHandler PropertyChanged;

        public AccountLedger(SqlConnection conn)
        {
            con = conn;

        }
        public AccountLedger()
        {
            con = public_members._OpenConnection();
            ShopCash = 100;
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public void CopyTransaction(string entry,int eno,DateTime dt)
        {
            try
            {
                System.Data.SqlClient.SqlConnection  con = new  SqlConnection();
                con = public_members._OpenConnection();
                if (con != null)
                {
                    string sql = "INSERT INTO   [transactions] ([led_id],[op_led_id],[dr],[cr],[eno],[entry],[cinvno],[t_date]) select [led_id],[op_led_id],[dr],[cr],[eno],[entry],[cinvno],[t_date] from  transactions where eno=" + eno +" and entry='" + entry + "'";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.ExecuteNonQuery();
                    SqlParameter date = new SqlParameter("@tdate", SqlDbType.DateTime);date.Direction = ParameterDirection.Input;
                    cmd = new SqlCommand("update transactions set t_date=@tdate where eno=" + eno + " and entry='" + entry + "'", con);
                    date.Value = dt;
                    cmd.Parameters.Add(date);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message.ToString());
            }
        }


        //public static double RefreshCashBag
        //{
        //    set
        //    {
        //        ac.ShopCash = value;
        //    }
        //}
        public bool post(string td, long acid, long op_acid, double dr, double cr, long inv = 0, string enty = " ", string cinvno = " ")
        {

            bool result = false;
            try
            {
                con = public_members._OpenConnection();
                if (con.State != ConnectionState.Closed)
                {
                    SqlCommand cmd = new SqlCommand(); cmd.Connection = con;
                    cmd.CommandText = "insert into transactions ([cinvno],led_id,op_led_id,dr,cr,eno,entry,t_date) values (@cinvno,@led_id,@op_led_id,@dr,@cr,@eno,@entry,@t_date)";
                    SqlParameter c_inv = new SqlParameter("@cinvno", SqlDbType.VarChar); c_inv.Direction = ParameterDirection.Input;
                    SqlParameter lid = new SqlParameter("@led_id", SqlDbType.Int); lid.Direction = ParameterDirection.Input;
                    SqlParameter oplid = new SqlParameter("@op_led_id", SqlDbType.Int); oplid.Direction = ParameterDirection.Input;
                    SqlParameter dr1 = new SqlParameter("@dr", SqlDbType.Decimal); dr1.Direction = ParameterDirection.Input;
                    SqlParameter cr1 = new SqlParameter("@cr", SqlDbType.Decimal); cr1.Direction = ParameterDirection.Input;
                    SqlParameter eno = new SqlParameter("@eno", SqlDbType.Int); eno.Direction = ParameterDirection.Input;
                    SqlParameter entry_n = new SqlParameter("@entry", SqlDbType.VarChar); entry_n.Direction = ParameterDirection.Input;
                    SqlParameter tdate = new SqlParameter("@t_date", SqlDbType.DateTime); tdate.Direction = ParameterDirection.Input;

                    lid.Value = acid;
                    oplid.Value = op_acid;
                    dr1.Value = dr;
                    cr1.Value = cr;
                    eno.Value = inv;
                    entry_n.Value = enty;
                    c_inv.Value = cinvno;
                    tdate.Value = DateTime.Parse(td);

                    cmd.Parameters.Add(c_inv);
                    cmd.Parameters.Add(lid);
                    cmd.Parameters.Add(oplid);
                    cmd.Parameters.Add(dr1);
                    cmd.Parameters.Add(cr1);
                    cmd.Parameters.Add(eno);
                    cmd.Parameters.Add(entry_n);
                    cmd.Parameters.Add(tdate);
                    int r = cmd.ExecuteNonQuery();
                    if (r > 0) result = true;


                    //this.ShopCash = 100;
                    //this.ShopPayable = 100;
                    //this.ShopReceivable = 100;
                }
                else
                {
                    MessageBox.Show("Can't connect server Now");
                }
                con.Close();

            }
            catch (SqlException ee)
            {

                MessageBox.Show("DB Execution interrupted:" + ee.Message.ToString());

            }
            return result;
        }



        public void UpdateCashBookViewCollection(int inv, string enty, int acid)
        {
            //string narr = null;

            //var c = public_members.cashbook_obscoll.Cast<CashBookModel>().Where<CashBookModel>((c1, r1) => c1.VNo == inv.ToString() && c1.Voucher == enty && c1.ACID == acid.ToString());
            //if (c.Count<CashBookModel>() > 0)
            //{
            //    public_members.cashbook_obscoll.Remove(c.AsEnumerable().First());
            //}

            //var rows = public_members.account_transactions.Select("entry='" + enty + "' and eno=" + inv + "and op_led_id=" + acid);
            //if (rows.Length > 0)
            //{

            //    narr = null;
            //    switch (enty)
            //    {
            //        case "RECEIPT":

            //            var r1 = public_members.receipts.Select("r_no=" + inv);
            //            if (r1.Length > 0)
            //            {

            //                narr = r1[0]["r_narration"].ToString();
            //            }
            //            break;
            //        case "PAYMENT":
            //            var p = public_members.payments.Select("p_no=" + inv);
            //            if (p.Length > 0)
            //            {

            //                narr = p[0]["p_narration"].ToString();
            //            }
            //            break;
            //        case "JOURNAL":
            //            var j = public_members.journals.Select("j_no=" + inv);
            //            if (j.Length > 0)
            //            {

            //                narr = j[0]["j_narration"].ToString();
            //            }
            //            break;
            //        case "BANK RECEIPT":
            //            var p2 = public_members.bankreceipts.Select("br_no=" + inv);
            //            if (p2.Length > 0)
            //            {

            //                narr = p2[0]["br_narration"].ToString();
            //            }
            //            break;
            //        case "BANK PAYMENT":
            //            var p1 = public_members.bankpayments.Select("bp_no=" + inv);
            //            if (p1.Length > 0)
            //            {

            //                narr = p1[0]["bp_narration"].ToString();
            //            }
            //            break;
            //    }


            //  CashBookModel newvalues = new CashBookModel ()

            //    {
            //        Date = Convert.ToDateTime(rows[0]["t_date"].ToString()),
            //        DateString = Convert.ToDateTime(rows[0]["t_date"].ToString()).ToString(),
            //        DateCheck = Convert.ToDateTime(rows[0]["t_date"].ToString()),
            //        Account = public_members.LedgeName(rows[0]["led_id"].ToString(), true, false),
            //        AccountCheck = public_members.LedgeName(rows[0]["led_id"].ToString(), true, false),
            //        Dr_Amount = rows[0]["dr"].ToString(),
            //        Cr_Amount = rows[0]["cr"].ToString(),
            //        VNo = rows[0]["eno"].ToString(),
            //        VNoCheck = rows[0]["eno"].ToString(),
            //        Voucher = rows[0]["entry"].ToString(),
            //        VoucherCheck = rows[0]["entry"].ToString(),
            //        OpAccount = public_members.LedgeName(rows[0]["op_led_id"].ToString(), true),
            //        ACID = rows[0]["led_id"].ToString(),
            //        Narration = narr,
            //        Type = public_members.getLedgerGroup(Convert.ToInt32(rows[0]["op_led_id"].ToString())),
            //    };

                
            //   if (newvalues!=null) public_members.cashbook_obscoll.Add(newvalues);
            //}
        }
        public bool post(DateTime td, long acid, long op_acid, double dr, double cr, long inv = 0, string enty = " ", string cinvno = " ")
        {

            bool result = false;
            try
            {
                con = public_members._OpenConnection();
                if (con.State != ConnectionState.Closed)
                {
                    SqlCommand cmd = new SqlCommand(); cmd.Connection = con;
                    cmd.CommandText = "insert into transactions ([cinvno],led_id,op_led_id,dr,cr,eno,entry,t_date) values (@cinvno,@led_id,@op_led_id,@dr,@cr,@eno,@entry,@t_date)";

                    SqlParameter c_inv = new SqlParameter("@cinvno", SqlDbType.VarChar); c_inv.Direction = ParameterDirection.Input;
                    SqlParameter lid = new SqlParameter("@led_id", SqlDbType.Int); lid.Direction = ParameterDirection.Input;
                    SqlParameter oplid = new SqlParameter("@op_led_id", SqlDbType.Int); oplid.Direction = ParameterDirection.Input;
                    SqlParameter dr1 = new SqlParameter("@dr", SqlDbType.Decimal); dr1.Direction = ParameterDirection.Input;
                    SqlParameter cr1 = new SqlParameter("@cr", SqlDbType.Decimal); cr1.Direction = ParameterDirection.Input;
                    SqlParameter eno = new SqlParameter("@eno", SqlDbType.Int); eno.Direction = ParameterDirection.Input;
                    SqlParameter entry_n = new SqlParameter("@entry", SqlDbType.VarChar); entry_n.Direction = ParameterDirection.Input;

                    SqlParameter tdate = new SqlParameter("@t_date", SqlDbType.DateTime); tdate.Direction = ParameterDirection.Input;

                    lid.Value = acid;
                    oplid.Value = op_acid;
                    dr1.Value = dr;
                    cr1.Value = cr;
                    eno.Value = inv;
                    entry_n.Value = enty;
                    c_inv.Value = cinvno;
                    tdate.Value = td;

                    cmd.Parameters.Add(c_inv);
                    cmd.Parameters.Add(lid);
                    cmd.Parameters.Add(oplid);
                    cmd.Parameters.Add(dr1);
                    cmd.Parameters.Add(cr1);
                    cmd.Parameters.Add(eno);
                    cmd.Parameters.Add(entry_n);

                    cmd.Parameters.Add(tdate);

                    int r = cmd.ExecuteNonQuery();
                    if (r > 0) result = true;
                    //public_members.Refresh_Transactions();

                    if (result == true)
                    {
                        //Update GroupList Collection
                        //var c = (IEnumerable<CashBookModel>)public_members.groupList_obscoll.Cast<CashBookModel>().Where<CashBookModel>((c1, r1) => c1.ACID == op_acid.ToString());
                        //if (c.Count<CashBookModel>() > 0)
                        //{
                        //    public_members.groupList_obscoll.Remove(c.AsEnumerable().First());
                        //}


                        //Dictionary<string, double> b = new Dictionary<string, double>();
                        //Dictionary<string, double> ba = new Dictionary<string, double>();
                        //ba = public_members.GetActBalance(public_members._sysDate[0], int.Parse(op_acid.ToString()));
                        //b = public_members.GetDrCr(op_acid);
                        //double bal = ba["Dr"] + ba["Cr"];
                        ////CashBookModel newvalue = new CashBookModel()
                        ////{
                        ////    ACID = op_acid.ToString(),
                        ////    Account = public_members.LedgeName(op_acid.ToString(), true, false),
                        ////    Dr_Amount = b["Dr"].ToString(),
                        ////    Cr_Amount = b["Cr"].ToString(),
                        ////    VNo = bal.ToString(),
                        ////    Voucher = public_members.getLedgerGroup(int.Parse(op_acid.ToString())),
                        ////};

                        //if (newvalue != null) public_members.groupList_obscoll.Add(newvalue);
                        //public_members.transactions.UpdateCashBookViewCollection(Convert.ToInt32( inv), enty,Convert.ToInt32( op_acid));
                    }
                    this.ShopCash = 100;
                    this.ShopPayable = 100;
                    this.ShopReceivable = 100;
                }
                else
                {
                    MessageBox.Show("Can't connect server Now");
                }
                con.Close();

            }
            catch (SqlException ee)
            {

                MessageBox.Show("DB Execution interrupted:" + ee.Message.ToString());

            }
            return result;
        }

        public double ShopCash
        {
            get
            {
              //  _cash = public_members.GroupBalance("CASH IN HAND");
                return _cash;
            }
            set
            {
                _cash = value;
                OnPropertyChanged("ShopCash");
            }
        }
        public double ShopPayable
        {
            get
            {
              //  _cp = public_members.GroupBalance("SUPPLIERS");
                return _cp;
            }
            set
            {
                _cp = value;
                OnPropertyChanged("ShopPayable");
            }
        }
        public double ShopReceivable
        {
            get
            {
              //  _cr = public_members.GroupBalance("CUSTOMERS");
                return _cr;
            }
            set
            {
                _cr = value;
                OnPropertyChanged("ShopReceivable");
            }
        }
        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
        public void DeleteCollectionView(string entry_name,int eno)
        {
            string narr = null;
            int drid=0, crid=0;
            switch (entry_name)
            {
                case "RECEIPT":

                    var r = public_members.receipts.Select("r_no=" + eno);
                    if (r.Length > 0)
                    {
                        int.TryParse(r[0]["r_cr_ledger"].ToString(), out crid);
                        int.TryParse(r[0]["r_cashledger"].ToString(), out drid);

                       

                    }
                    break;
                case "PAYMENT":
                    var p = public_members.payments.Select("p_no=" + eno);
                    if (p.Length > 0)
                    {
                        int.TryParse(p[0]["r_crledger"].ToString(), out crid);
                        int.TryParse(p[0]["r_drledger"].ToString(), out drid);
                        narr = p[0]["p_narration"].ToString();
                    }
                    break;
                case "JOURNAL":
                    var j = public_members.journals.Select("j_no=" + eno);
                    if (j.Length > 0)
                    {
                        int.TryParse(j[0]["j_crledger"].ToString(), out crid);
                        int.TryParse(j[0]["j_drledger"].ToString(), out drid);
                        narr = j[0]["j_narration"].ToString();
                    }
                    break;
                case "BANK RECEIPT":
                    var p2 = public_members.bankreceipts.Select("br_no=" + eno);
                    if (p2.Length > 0)
                    {
                        int.TryParse(p2[0]["br_crledger"].ToString(), out crid);
                        int.TryParse(p2[0]["br_drledger"].ToString(), out drid);
                        narr = p2[0]["br_narration"].ToString();
                    }
                    break;
                case "BANK PAYMENT":
                    var p1 = public_members.payments.Select("bp_no=" + eno);
                    if (p1.Length > 0)
                    {
                        int.TryParse(p1[0]["bp_crledger"].ToString(), out crid);
                        int.TryParse(p1[0]["bp_drledger"].ToString(), out drid);
                        narr = p1[0]["bp_narration"].ToString();
                    }
                    break;


            }
            //var c11 = public_members.cashbook_obscoll.Cast<CashBookModel>().Where<CashBookModel>((c1, r1) => c1.VNo == eno.ToString() && c1.Voucher == entry_name && c1.ACID == drid.ToString());
            //if (c11.Count<CashBookModel>() > 0)
            //{

            //    public_members.cashbook_obscoll.Remove(c11.AsEnumerable().First());

            //}
            //c11 = public_members.cashbook_obscoll.Cast<CashBookModel>().Where<CashBookModel>((c1, r1) => c1.VNo == eno.ToString() && c1.Voucher == entry_name && c1.ACID == crid.ToString());
            //if (c11.Count<CashBookModel>() > 0)
            //{

            //    public_members.cashbook_obscoll.Remove(c11.AsEnumerable().First());

            //}
 
        }
        public bool delete(long eno, string entry_name, SqlConnection conn = null)
        {
            bool res = false;
            try
            {
                con = public_members._OpenConnection();
                if (con.State != ConnectionState.Closed)
                {
                    SqlCommand cmd = new SqlCommand("delete from transactions where entry='" + entry_name.Trim() + "' and eno=" + eno.ToString(), con);
                    int rr = cmd.ExecuteNonQuery();
                    if (rr > 0)
                    {
                        res = true;
                    }

                   // public_members.Refresh_Transactions();
                    int drid = 0, crid = 0;
                    if (res == true)
                    {
                        string narr = null;
                        switch (entry_name)
                        {
                            case "RECEIPT":

                                var r = public_members.receipts.Select("r_no=" + eno);
                                if (r.Length > 0)
                                {
                                    int.TryParse(r[0]["r_cr_ledger"].ToString(), out crid);
                                    int.TryParse(r[0]["r_cashledger"].ToString(), out drid);
                                    narr = r[0]["r_narration"].ToString();

                                }
                                break;
                            case "PAYMENT":
                                var p = public_members.payments.Select("p_no=" + eno);
                                if (p.Length > 0)
                                {
                                    int.TryParse(p[0]["p_crledger"].ToString(), out crid);
                                    int.TryParse(p[0]["p_drledger"].ToString(), out drid);
                                    narr = p[0]["p_narration"].ToString();
                                }
                                break;
                            case "JOURNAL":
                                var j = public_members.journals.Select("j_no=" + eno);
                                if (j.Length > 0)
                                {
                                    int.TryParse(j[0]["j_crledger"].ToString(), out crid);
                                    int.TryParse(j[0]["j_drledger"].ToString(), out drid);
                                    narr = j[0]["j_narration"].ToString();
                                }
                                break;
                            case "BANK RECEIPT":
                                var p2 = public_members.bankreceipts.Select("br_no=" + eno);
                                if (p2.Length > 0)
                                {
                                    int.TryParse(p2[0]["br_crledger"].ToString(), out crid);
                                    int.TryParse(p2[0]["br_drledger"].ToString(), out drid);
                                    narr = p2[0]["br_narration"].ToString();
                                }
                                break;
                            case "BANK PAYMENT":
                                var p1 = public_members.bankpayments.Select("bp_no=" + eno);
                                if (p1.Length > 0)
                                {
                                    int.TryParse(p1[0]["bp_crledger"].ToString(), out crid);
                                    int.TryParse(p1[0]["bp_drledger"].ToString(), out drid);
                                    narr = p1[0]["bp_narration"].ToString();
                                }
                                break;
                        }
                        //UpdateCashBookViewCollection(Convert.ToInt32( eno), entry_name, drid);
                        //UpdateCashBookViewCollection(Convert.ToInt32( eno), entry_name, crid);
                        //Update GroupList Collection
                        //var c = (IEnumerable<CashBookModel>)public_members.groupList_obscoll.Cast<CashBookModel>().Where<CashBookModel>((c1, r1) => c1.ACID == drid.ToString());
                        //if (c.Count<CashBookModel>() > 0)
                        //{
                        //    public_members.groupList_obscoll.Remove(c.AsEnumerable().First());
                        //}

                        //c = (IEnumerable<CashBookModel>)public_members.groupList_obscoll.Cast<CashBookModel>().Where<CashBookModel>((c1, r1) => c1.ACID == crid.ToString());
                        //if (c.Count<CashBookModel>() > 0)
                        //{
                        //    public_members.groupList_obscoll.Remove(c.AsEnumerable().First());
                        //}

                        if (drid > 0)
                        {

                            Dictionary<string, double> b = new Dictionary<string, double>();
                            Dictionary<string, double> ba = new Dictionary<string, double>();
                            //ba = public_members.GetActBalance(public_members._sysDate[0], int.Parse(drid.ToString()));
                            //b = public_members.GetDrCr(drid);
                            double bal = ba["Dr"] + ba["Cr"];
                            CashBookModel newvalue = new CashBookModel()
                            {
                                //ACID = drid.ToString(),
                                //Account = public_members.LedgeName(drid.ToString(), true, false),
                                //Dr_Amount = b["Dr"].ToString(),
                                //Cr_Amount = b["Cr"].ToString(),
                                //VNo = bal.ToString(),
                              //  Voucher = public_members.getLedgerGroup(int.Parse(drid.ToString())),
                            };

                            if (newvalue != null) public_members.groupList_obscoll.Add(newvalue);

                        }
                        if (crid > 0)
                        {

                            Dictionary<string, double> b = new Dictionary<string, double>();
                            Dictionary<string, double> ba = new Dictionary<string, double>();
                         //   ba = public_members.GetActBalance(DateTime.Now.Date, int.Parse(crid.ToString()));
                         //   b = public_members.GetDrCr(crid);
                            double bal = ba["Dr"] + ba["Cr"];
                            CashBookModel newvalue = new CashBookModel()
                            {
                                //ACID = crid.ToString(),
                                //Account = public_members.LedgeName(crid.ToString(), true, false),
                                //Dr_Amount = b["Dr"].ToString(),
                                //Cr_Amount = b["Cr"].ToString(),
                                //VNo = bal.ToString(),
                            //    Voucher = public_members.getLedgerGroup(int.Parse(crid.ToString())),
                            };

                            if (newvalue != null) public_members.groupList_obscoll.Add(newvalue);

                        }
                      // public_members.transactions.UpdateCashBookViewCollection(Convert.ToInt32( eno), entry_name,Convert.ToInt32( drid));
                      // public_members.transactions.UpdateCashBookViewCollection(Convert.ToInt32( eno), entry_name,Convert.ToInt32( crid));
                        //Deleting CashBookView entries


                        //c = (IEnumerable<CashBookModel>)public_members.cashbook_obscoll.Cast<CashBookModel>().Where<CashBookModel>((c1, r1) => c1.VNo == eno.ToString() && c1.Voucher == entry_name);
                        //if (c.Count<CashBookModel>() > 0)
                        //{
                        //    foreach (var t in c)
                        //    {
                        //        public_members.cashbook_obscoll.Remove(t);
                        //    }
                        //}
                    }
                    this.ShopCash = 101;
                    this.ShopPayable = 100;
                    this.ShopReceivable = 100;
                }
                else
                {
                    MessageBox.Show("Can't connect server Now");
                }
                con.Close();
            }
            catch (SqlException ee)
            {
                MessageBox.Show(ee.Message.ToString());
            }
            return res;
        }
    }
}
