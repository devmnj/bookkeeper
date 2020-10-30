using System.Collections.ObjectModel;
using System.Linq;
using System.Data;
using System.Windows;
using System;
using accounts.Model;
using System.Collections.Generic;
using System.ComponentModel;

namespace accounts.ModelViews
{

    class AccountsModelViews : ViewModelBase
    {
        //Front panel
        public ObservableDictionary _front_panel = new accounts.ObservableDictionary();
        public ObservableDictionary FrontPanel
        {
            get => _front_panel;
            set => SetProperty(ref _front_panel, value);
        }
        public void Refresh_FrontPanelItems()
        {
            try
            {

                var inc = (from x in _trsansactions
                           where x.Entry == "RECEIPT" && x.Ac_Id
                           != x.Op_Ac_Id && x.Tr_date == public_members._sysDate[0]
                           select x.Dr_Amount).Sum();
                var rec = (from x in _trsansactions
                           where x.Entry == "RECEIPT" && x.Ac_Id
                           != x.Op_Ac_Id && x.Tr_date == public_members._sysDate[0]
                           select x.Dr_Amount).Sum();

                var br = (from x in _trsansactions
                          where x.Entry == "BANK RECEIPT" && x.Ac_Id
                          != x.Op_Ac_Id && x.Tr_date == public_members._sysDate[0]
                          select x.Dr_Amount).Sum();
                var bp = (from x in _trsansactions
                          where x.Entry == "BANK PAYMENT" && x.Ac_Id
                          != x.Op_Ac_Id && x.Tr_date == public_members._sysDate[0]
                          select x.Cr_Amount).Sum();
                var pay = (from x in _trsansactions
                           where x.Entry == "PAYMENT" && x.Ac_Id
                           != x.Op_Ac_Id && x.Tr_date == public_members._sysDate[0]
                           select x.Cr_Amount).Sum();
                var proll_vouchers = (from x in _trsansactions
                                      where x.Entry == "PAYROLL VOUCHER" && x.Ac_Id
                                      != x.Op_Ac_Id && x.Tr_date == public_members._sysDate[0]
                                      select x.Cr_Amount).Sum();
                var proll_ = (from x in _trsansactions
                              where x.Entry == "PAYROLL" && x.Ac_Id
                              != x.Op_Ac_Id && x.Tr_date == ViewModels_Variables._sysDate[0]
                              select x.Cr_Amount).Sum();


                _front_panel.UpdateOrAdd("PAy", string.Format("{0:0.00}", pay));
                _front_panel.UpdateOrAdd("REc", string.Format("{0:0.00}", rec));
                _front_panel.UpdateOrAdd("BPa", string.Format("{0:0.00}", bp));
                _front_panel.UpdateOrAdd("BRe", string.Format("{0:0.00}", br));
                _front_panel.UpdateOrAdd("PAv", string.Format("{0:0.00}", proll_vouchers));


                _front_panel.UpdateOrAdd("INC", (rec + br).ToString("0.00"));


                _front_panel.UpdateOrAdd("EXP", string.Format("{0:0.00}", bp + pay));


                //RECEIVABLE
                var _receivable = (from a in _accounts where a.Parent.Catagory == "RECEIVABLE" select new { amount = DB.Connection.GetActBalance(a.ID) }.amount).Sum();
                _front_panel.UpdateOrAdd("Receivable", string.Format("{0:0.00}", _receivable));
                //PAYABLE
                var _payable = (from a in _accounts where a.Parent.Catagory == "PAYABLE" select new { amount = DB.Connection.GetActBalance(a.ID) }.amount).Sum();
                _front_panel.UpdateOrAdd("Payable", string.Format("{0:0.00}", _payable));

                //CASH
                var _cash = (from a in _accounts where a.Parent.Name == "CASH" || a.Parent.Name == "CASH IN HAND" select new { amount = DB.Connection.GetActBalance(a.ID) }.amount).Sum();
                _front_panel.UpdateOrAdd("Cash", string.Format("{0:0.00}", _cash));
                //BANK
                var _bank = (from a in _accounts where a.Parent.Name == "BANK ACCOUNT" select new { amount = DB.Connection.GetActBalance(a.ID) }.amount).Sum();
                _front_panel.UpdateOrAdd("Bank", string.Format("{0:0.00}", _bank));

            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message.ToString());
            }
        }
        //Accounts

        ObservableCollection<AccountModel> _accounts = new ObservableCollection<AccountModel>();
        private AccountModel _selectedAccount = new AccountModel();
        private bool _isChecked = false;

        public AccountsModelViews()
        {
            Trans_To_list();
            GroupsToCollection();
            AccountToCollection();
            //Trans_To_list();
            RefreshCompany();

            ////Reciepts_To_List();
            //TasksToList();
            //Journals_To_list();
        }
       
        
        public void AccountToCollection()
        {
            try
            {
                DB.Accounts.Fetch();
                var acc = DB.Accounts.AccountTable;

                //var reader = acc.CreateDataReader();
                //List<Model.AccountModel> lst = new List<AccountModel>();
                //while (reader.Read())
                //{
                //    lst.Add(new AccountModel() { ID = reader.GetInt32(0) });       
                //    reader.NextResult();
                //}

                var list = (from ac in acc.AsEnumerable()
                            join g in _groups on ac.Field<int>("l_parent") equals g.ID
                            select new
                            {
                                obj = new AccountModel()
                                {
                                    Address = ac.Field<string>("l_address"),
                                    Catagory = ac.Field<string>("l_Catagory"),
                                    City = ac.Field<string>("l_City"),
                                    ParentGroup = ac.Field<int>("l_parent"),
                                    Name = ac.Field<string>("l_name"),
                                    Parent = g,

                                    Short_Name = ac.Field<string>("l_short_name"),
                                    Mob = ac.Field<string>("l_mob"),
                                    PhoneNo = ac.Field<string>("l_resi"),
                                    ID = ac.Field<int>("id"),
                                    //Balance = DB.Connection.GetActBalance(ac.Field<int>("id"), "")
                                }
                            }.obj).ToList<AccountModel>();
                _accounts = new ObservableCollection<AccountModel>(list);

            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }

        public bool Add_Update(AccountModel acc)
        {
            bool res = false;
            try
            {
                var mm = (_accounts.Where((m1, c1) => m1.ID == acc.ID && m1.ID != 0));
                if (mm != null && mm.Count() > 0)
                {
                    int i = _accounts.IndexOf(mm.FirstOrDefault());
                    _accounts[i] = acc;
                    var acc1 = _accounts;
                    Accounts = new ObservableCollection<AccountModel>();
                    Accounts = acc1;
                    //_accounts.Remove((mm.FirstOrDefault()));
                    //_accounts.Add(acc);
                }
                else
                {
                    _accounts.Add(acc);
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public bool Remove(AccountModel acc)
        {
            bool res = false;
            try
            {
                var mm = (_accounts.Where((m1, c1) => m1.ID == acc.ID && m1.ID != 0));
                if (mm != null && mm.Count() > 0)
                {
                    _accounts.Remove(mm.First());
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public ObservableCollection<AccountModel> Accounts
        {
            get => _accounts;
            set => SetProperty(ref _accounts, value);

        }


        public AccountModel SelectedAccount
        {
            get => _selectedAccount;
            set => SetProperty(ref _selectedAccount, value);
            //OnPropertyRaised("SelectedAccount");

        }

        //Group
        private ObservableCollection<GroupModel> _groups = new ObservableCollection<GroupModel>();
        private ObservableCollection<GroupModel> _parentgroups = new ObservableCollection<GroupModel>();
        private GroupModel _selectedGroup = new GroupModel();
        private GroupModel _selectedParent = new GroupModel();
        public void GroupsToCollection()
        {
            try
            {
                var grp = DB.Connection._FetchTable("select * from groups order by id");

                var list1 = (from g in grp.AsEnumerable()
                             select new
                             {
                                 obj = new GroupModel()
                                 {
                                     ID = g.Field<int>("id"),
                                     P_ID = g.Field<int>("g_parent"),
                                     Name = g.Field<string>("g_name"),
                                     Description = g.Field<string>("g_narration"),
                                     Catagory = g.Field<string>("g_catagory"),
                                     Max_Disc = Convert.ToDouble(g.Field<decimal>("g_maxdisc")),
                                     Cr_Loc = Convert.ToDouble(g.Field<decimal>("g_cr_lock")),
                                     Dr_Loc = Convert.ToDouble(g.Field<decimal>("g_dr_lock")),
                                 }
                             }.obj).ToList<GroupModel>();

                _groups = new ObservableCollection<GroupModel>(list1);

                var list2 = (from g in list1
                             join p in _groups on g.P_ID equals p.ID
                             where g.P_ID != 0
                             select new
                             {
                                 grp = new Model.GroupModel()
                                 {
                                     ID = g.ID,
                                     Name = g.Name,
                                     P_ID = g.P_ID,
                                     Catagory = g.Catagory,
                                     Cr_Loc = g.Cr_Loc,
                                     Description = g.Description,
                                     Dr_Loc = g.Dr_Loc,
                                     Max_Disc = g.Max_Disc,
                                     ParentGroup = p,
                                 }
                             }.grp).ToList<GroupModel>();
                _parentgroups = new ObservableCollection<GroupModel>(list2);



            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }
        public bool Add_Update(GroupModel gr)
        {
            bool res = false;
            try
            {
                var mm = (_groups.Where((m1, c1) => m1.ID == gr.ID && m1.ID != 0));
                if (mm != null && mm.Count() > 0)
                {
                    _groups.Remove(mm.First());

                }
                _groups.Add(gr);
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public bool Remove(GroupModel gr)
        {
            bool res = false;
            try
            {
                var mm = (_groups.Where((m1, c1) => m1.ID == gr.ID && m1.ID != 0));
                if (mm != null && mm.Count() > 0)
                {
                    _groups.Remove(mm.First());
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public ObservableCollection<GroupModel> AccountGroups
        {
            get { return _groups; }
        }
        public ObservableCollection<GroupModel> ParentGroups
        {
            get { return _parentgroups; }
        }
        public GroupModel SelectedGroup
        {
            get { return _selectedGroup; }
            set { _selectedGroup = value; }
        }
        public GroupModel SelectedParent
        {
            get { return _selectedParent; }
            set { _selectedParent = value; }
        }

        //Receipt
        ObservableCollection<ReceiptModel> _receipts = new ObservableCollection<ReceiptModel>();
        ReceiptModel _selectedReceipt = new ReceiptModel();
        public ObservableCollection<ReceiptModel> Receipts
        {
            get { return _receipts; }
        }

        public void Reciepts_To_List()
        {
            try
            {
                if (ViewModels_Variables.ModelViews.Accounts.Count <= 0)
                {
                    DB.Accounts.Fetch();
                    ViewModels_Variables.ModelViews.AccountToCollection();
                }

                DB.Receipt.Fetch();
                var receipts = DB.Receipt.ReceiptTable;
                var rlist = (from r in receipts.AsEnumerable()
                             join a in _accounts on r.Field<int>("r_cashledger") equals a.ID
                             join a1 in _accounts on r.Field<int>("r_cr_ledger") equals a1.ID
                             select new
                             {
                                 obj = new Model.ReceiptModel()
                                 {
                                     //Crid = r.Field<int>("r_cr_ledger"),
                                     //Drid = r.Field<int>("r_cashledger"),
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
                                     InvBalance = DB.Connection.GetActBalance(dt: r.Field<DateTime>("r_date"),
                                     id: r.Field<int>("r_cr_ledger"), byInvoice: r.Field<string>("r_invoice"))

                                 }
                             }.obj
                           ).ToList<Model.ReceiptModel>();
                _receipts = new ObservableCollection<ReceiptModel>(rlist);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        public bool Add_Update(Model.ReceiptModel r)
        {
            bool res = false;
            try
            {
                var mm = (_receipts.Where((m1, c1) => m1.rno == r.rno && m1.rno != 0));
                if (mm != null && mm.Count() > 0)
                {
                    var i = _receipts.IndexOf(mm.First());
                    _receipts[i] = r;

                }
                else
                {
                    _receipts.Add(r);
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public bool Remove(ReceiptModel tr)
        {
            bool res = false;
            try
            {
                var mm = (_receipts.Where((m1, c1) => m1.rno == tr.rno && m1.rno != 0));
                if (mm != null && mm.Count() > 0)
                {
                    _receipts.Remove(mm.First());

                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public ReceiptModel SelectedReceipt
        {
            get => _selectedReceipt;
            set => SetProperty(ref _selectedReceipt, value);


        }
        //Journal
        private ObservableCollection<Model.JournalModel> _journals = new ObservableCollection<JournalModel>();
        JournalModel _selectedJournal = new JournalModel();
        public JournalModel SelectedJournal
        {
            get => _selectedJournal;
            set => SetProperty(ref _selectedJournal, value);


        }
        public bool Add_Update(JournalModel j)
        {
            bool res = false;
            try
            {
                var mm = (_journals.Where((m1, c1) => m1.jno == j.jno));
                if (mm.FirstOrDefault() != null)
                {
                    var i = _journals.IndexOf(mm.First());
                    _journals[i] = j;
                    var t = _journals;
                    Journals = new ObservableCollection<JournalModel>();
                    Journals = t;
                }
                else
                {
                    _journals.Add(j);
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public bool Remove(JournalModel tr)
        {
            bool res = false;
            try
            {
                var mm = _journals.Where((m1, c1) => m1.jno == tr.jno);
                if (mm != null && mm.Count() > 0)
                {
                    _journals.Remove(mm.FirstOrDefault());
                    //foreach (var t in mm)
                    //{
                    //    _trsansactions.Remove(t);
                    //}
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public ObservableCollection<Model.JournalModel> Journals
        {
            get => _journals;
            set => SetProperty(ref _journals, value);
        }
        public void Journals_To_list()
        {
            try
            {
                if (ViewModels_Variables.ModelViews.Accounts.Count <= 0)
                {
                    DB.Accounts.Fetch();
                    ViewModels_Variables.ModelViews.AccountToCollection();
                }
                DB.Journal.Fetch();
                var receipts = DB.Journal.JournalTable;
                var jlist = (from r in receipts.AsEnumerable()
                             join a in _accounts on r.Field<int>("j_drledger") equals a.ID
                             join a1 in _accounts on r.Field<int>("j_crledger") equals a1.ID
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
                _journals = new ObservableCollection<JournalModel>(jlist);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        //Transaction
        private ObservableCollection<Model.Trsansactions> _trsansactions = new ObservableCollection<Trsansactions>();
        public bool Add_Update(Trsansactions tr)
        {
            bool res = false;
            try
            {
                //    var mm = (_trsansactions.Where((m1, c1) => m1.Eno == tr.Eno && m1.Entry =="RECEIPT"));
                //    if (mm != null && mm.Count() > 0)
                //    {
                //        foreach (var t in mm)
                //        {
                //            _trsansactions.Remove(t);
                //        }
                //    }
                _trsansactions.Add(tr);
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public bool Remove(Trsansactions tr)
        {
            bool res = false;
            try
            {
                var mm = (_trsansactions.Where((m1, c1) => m1.Eno == tr.Eno && m1.Entry == tr.Entry));
                if (mm != null && mm.Count() > 0)
                {
                    _trsansactions.Remove(mm.FirstOrDefault());
                    //foreach (var t in mm)
                    //{
                    //    _trsansactions.Remove(t);
                    //}
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public ObservableCollection<Model.Trsansactions> AccountTransactions
        {
            get { return _trsansactions; }
        }
        public void Trans_To_list()
        {
            try
            {
                DB.Transactions.Fetch();
                var tr = DB.Transactions.TransactionsTable;


                var tlist = (from t in tr.AsEnumerable()
                             select new
                             {
                                 obj = new Model.Trsansactions()
                                 {

                                     Id = t.Field<int>("id"),
                                     Ac_Id = t.Field<int>("led_id"),
                                     Op_Ac_Id = t.Field<int>("op_led_id"),
                                     Dr_Amount = Convert.ToDouble(t.Field<decimal>("dr")),
                                     Cr_Amount = Convert.ToDouble(t.Field<decimal>("cr")),
                                     Eno = t.Field<int>("eno"),
                                     Entry = t.Field<string>("entry"),
                                     Cinv_no = t.Field<string>("cinvno"),
                                     Tr_date = t.Field<DateTime>("t_date")
                                 }
                             }.obj).ToList<Model.Trsansactions>();
                _trsansactions = new ObservableCollection<Trsansactions>(tlist);

            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        //Tasks 
        private ObservableCollection<Task> _tasks = new ObservableCollection<Task>();
        public ObservableCollection<Task> Tasks { get { return _tasks; } }
        public bool Add_Update(Task t)
        {
            bool res = false;
            try
            {
                var mm = (_tasks.Where((m1, c1) => m1.ENO == t.ENO && m1.ENTRY == t.ENTRY));
                if (mm != null && mm.Count() > 0)
                {
                    int i = _tasks.IndexOf(mm.FirstOrDefault());
                    _tasks[i] = t;
                }
                else
                {
                    _tasks.Add(t);
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public bool Remove(Task tr)
        {
            bool res = false;
            try
            {
                var mm = (_tasks.Where((m1, c1) => m1.ENO == tr.ENO && m1.ENTRY == tr.ENTRY));
                if (mm != null && mm.Count() > 0)
                {
                    _tasks.Remove(mm.FirstOrDefault());
                    //foreach (var t in mm)
                    //{
                    //    _trsansactions.Remove(t);
                    //}
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public void TasksToList()
        {
            try
            {
                DB.Task.Fetch();
                var tsks = DB.Task.TaskTable;
                var tlist = (from t in tsks.AsEnumerable()
                             select new
                             {
                                 obj = new Model.Task()
                                 {
                                     ENO = t.Field<int>("eno"),
                                     ID = t.Field<int>("id"),
                                     T_LABEL = t.Field<string>("task_label"),
                                     T_AMOUNT = t.Field<decimal>("task_amount"),
                                     ENTRY = t.Field<string>("entry"),

                                 }
                             }.obj).ToList<Task>();
                _tasks = new ObservableCollection<Task>(tlist);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }


        //BankReceipt
        ObservableCollection<Model.BankReceiptModel> _breceipts = new ObservableCollection<BankReceiptModel>();
        BankReceiptModel _selectedBReceipt = new BankReceiptModel();
        public BankReceiptModel SelectedBReceipt
        {
            get => _selectedBReceipt;
            set => SetProperty(ref _selectedBReceipt, value);


        }
        public ObservableCollection<BankReceiptModel> BankReceipts
        {
            get { return _breceipts; }
        }

        public void BReceipts_To_List()
        {
            try
            {
                if (ViewModels_Variables.ModelViews.Accounts.Count <= 0)
                {
                    DB.Accounts.Fetch();
                    ViewModels_Variables.ModelViews.AccountToCollection();
                }
                DB.BankReceipt.Fetch();
                var receipts = DB.BankReceipt.BRTable;
                var plist = (from r in receipts.AsEnumerable()
                             join a in _accounts on r.Field<int>("br_drledger") equals a.ID
                             join a1 in _accounts on r.Field<int>("br_crledger") equals a1.ID
                             select new
                             {
                                 obj = new Model.BankReceiptModel()
                                 {

                                     CrAccount = a1,
                                     DrAccount = a,
                                     Status = r.Field<string>("br_status"),

                                     CheqNo = r.Field<string>("br_cheqno"),
                                     CheqDate = r.Field<DateTime>("br_cheqdate"),
                                     BankCharge = Convert.ToDouble(r.Field<decimal>("br_bcharge")),
                                     Type = r.Field<string>("br_type"),
                                     rno = r.Field<int>("br_no"),
                                     Date = r.Field<DateTime>("br_date"),
                                     Amount = Convert.ToDouble(r.Field<decimal>("br_amount")),
                                     DiscP = Convert.ToDouble(r.Field<decimal>("br_disc")),
                                     DiscAmount = Convert.ToDouble(r.Field<decimal>("br_damount")),
                                     Invno = r.Field<string>("br_invoice"),
                                     InvBalance = DB.Connection.GetActBalance(id: r.Field<int>("br_crledger"), byInvoice: r.Field<string>("br_invoice"))

                                 }
                             }.obj
                           ).ToList<Model.BankReceiptModel>();
                _breceipts = new ObservableCollection<BankReceiptModel>(plist);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        public bool Add_Update(Model.BankReceiptModel p)
        {
            bool res = false;
            try
            {
                var mm = (_breceipts.Where((m1, c1) => m1.rno == p.rno && m1.rno != 0));
                if (mm != null && mm.Count() > 0)
                {
                    var i = _breceipts.IndexOf(mm.First());
                    _breceipts[i] = p;

                }
                else
                {
                    _breceipts.Add(p);
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public bool Remove(Model.BankReceiptModel p)
        {
            bool res = false;
            try
            {
                var mm = (_breceipts.Where((m1, c1) => m1.rno == p.rno && m1.rno != 0));
                if (mm != null && mm.Count() > 0)
                {
                    _breceipts.Remove(mm.First());

                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }

        //BankPayment
        ObservableCollection<Model.BankPaymentModel> _bpayments = new ObservableCollection<BankPaymentModel>();
        BankPaymentModel _selectedBPayment = new BankPaymentModel();
        public BankPaymentModel SelectedBPayment
        {
            get => _selectedBPayment;
            set => SetProperty(ref _selectedBPayment, value);


        }
        public ObservableCollection<BankPaymentModel> BankPayments
        {
            get { return _bpayments; }
        }
        public void BPayments_To_List()
        {
            try
            {
                if (ViewModels_Variables.ModelViews.Accounts.Count <= 0)
                {
                    DB.Accounts.Fetch();
                    ViewModels_Variables.ModelViews.AccountToCollection();
                }
                DB.BankPayment.Fetch();
                var receipts = DB.BankPayment.BPTable;
                var plist = (from r in receipts.AsEnumerable()
                             join a in _accounts on r.Field<int>("bp_drledger") equals a.ID
                             join a1 in _accounts on r.Field<int>("bp_crledger") equals a1.ID
                             select new
                             {
                                 obj = new Model.BankPaymentModel()
                                 {
                                     Crid = r.Field<int>("bp_crledger"),
                                     Drid = r.Field<int>("bp_drledger"),
                                     CrAccount = a1,
                                     DrAccount = a,
                                     pno = r.Field<int>("bp_no"),
                                     status = r.Field<string>("bp_status"),
                                     Type = r.Field<string>("bp_type"),
                                     CheqNo = r.Field<string>("bp_cheqno"),
                                     CheqDate = r.Field<DateTime>("bp_cheqdate"),
                                     BankCharge = Convert.ToDouble(r.Field<decimal>("bp_bcharge")),
                                     Date = r.Field<DateTime>("bp_date"),
                                     Amount = Convert.ToDouble(r.Field<decimal>("bp_amount")),
                                     Disc = Convert.ToDouble(r.Field<decimal>("bp_disc")),
                                     DiscAmount = Convert.ToDouble(r.Field<decimal>("bp_damount")),
                                     Invno = r.Field<string>("bp_invoice"),
                                     InvBalance = DB.Connection.GetActBalance(id: r.Field<int>("bp_crledger"), byInvoice: r.Field<string>("bp_invoice"))

                                 }
                             }.obj
                           ).ToList<Model.BankPaymentModel>();
                _bpayments = new ObservableCollection<BankPaymentModel>(plist);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        public bool Add_Update(Model.BankPaymentModel p)
        {
            bool res = false;
            try
            {
                var mm = (_bpayments.Where((m1, c1) => m1.pno == p.pno && m1.pno != 0));
                if (mm != null && mm.Count() > 0)
                {
                    var i = _bpayments.IndexOf(mm.First());
                    _bpayments[i] = p;

                }
                else
                {
                    _bpayments.Add(p);
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public bool Remove(Model.BankPaymentModel p)
        {
            bool res = false;
            try
            {
                var mm = (_bpayments.Where((m1, c1) => m1.pno == p.pno && m1.pno != 0));
                if (mm != null && mm.Count() > 0)
                {
                    _bpayments.Remove(mm.First());

                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }

        //Payroll

        //Payment
        ObservableCollection<Model.PaymentModel> _payments = new ObservableCollection<PaymentModel>();
        PaymentModel _selectedPayment = new PaymentModel();
        public PaymentModel SelectedPayment
        {
            get => _selectedPayment;
            set => SetProperty(ref _selectedPayment, value);


        }
        public ObservableCollection<PaymentModel> Payments
        {
            get { return _payments; }
        }
        public void Payments_To_List()
        {
            try
            {
                if (ViewModels_Variables.ModelViews.Accounts.Count <= 0)
                {
                    DB.Accounts.Fetch();
                    ViewModels_Variables.ModelViews.AccountToCollection();
                }
                DB.Payment.Fetch();
                var receipts = DB.Payment.PaymentTable;
                var plist = (from r in receipts.AsEnumerable()
                             join a in _accounts on r.Field<int>("p_drledger") equals a.ID
                             join a1 in _accounts on r.Field<int>("p_crledger") equals a1.ID
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
                                     InvBalance = DB.Connection.GetActBalance(dt: r.Field<DateTime>("p_date"), id: r.Field<int>("p_crledger"), byInvoice: r.Field<string>("p_invoice"))

                                 }
                             }.obj
                           ).ToList<Model.PaymentModel>();
                _payments = new ObservableCollection<PaymentModel>(plist);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        public bool Add_Update(Model.PaymentModel p)
        {
            bool res = false;
            try
            {
                var mm = (_payments.Where((m1, c1) => m1.pno == p.pno && m1.pno != 0));
                if (mm != null && mm.Count() > 0)
                {
                    var i = _payments.IndexOf(mm.First());
                    _payments[i] = p;

                }
                else
                {
                    _payments.Add(p);
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public bool Remove(Model.PaymentModel p)
        {
            bool res = false;
            try
            {
                var mm = (_payments.Where((m1, c1) => m1.pno == p.pno && m1.pno != 0));
                if (mm != null && mm.Count() > 0)
                {
                    _payments.Remove(mm.First());

                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }

        //Employees
        ObservableCollection<Model.EmployeeModel> _employees = new ObservableCollection<EmployeeModel>();
        Model.EmployeeModel _selectedemployee = new EmployeeModel();
        public Model.EmployeeModel SelectedEmployee
        {
            get { return _selectedemployee; }
            set { _selectedemployee = value; }
        }
        public ObservableCollection<EmployeeModel> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }
        public void Employees_To_List()
        {
            try
            {
                if (DB.Accounts.AccountTable == null)
                {
                    DB.Accounts.Fetch();
                    ViewModels_Variables.ModelViews.AccountToCollection();

                }
                if (DB.Departments.DepTable == null)
                {
                    DB.Departments.Fetch();
                    ViewModels_Variables.ModelViews.Departments_To_List();

                }


                DB.Employee.Fetch();
                var receipts = DB.Employee.EmpTable;

                var plist = (from r in receipts.AsEnumerable()
                             join d in ViewModels_Variables.ModelViews.Departments on r.Field<int>("dep_id") equals d.Dep_id
                             join acc in ViewModels_Variables.ModelViews.Accounts on r.Field<int>("lid") equals acc.ID
                             select new
                             {
                                 obj = new Model.EmployeeModel()
                                 {
                                     Account = acc,
                                     Id = r.Field<int>("id"),
                                     Department = d,
                                     Emp_Id = r.Field<string>("eid"),
                                     Desig = r.Field<string>("designation"),
                                     Basic = Convert.ToDouble(r.Field<decimal>("basicpay")),
                                     Comm = Convert.ToDouble(r.Field<decimal>("comm")),
                                     DOJ = r.Field<DateTime>("doj"),//IsDailyWager=r.Field<bool>("isdailywager")
                                 }
                             }.obj
                           ).ToList<Model.EmployeeModel>();
                _employees = new ObservableCollection<EmployeeModel>(plist);
                var des = (from x in _employees select x.Desig).Distinct().ToList<string>();
                Designations = des;
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        public bool Add_Update(Model.EmployeeModel p)
        {
            bool res = false;
            try
            {
                var mm = (_employees.Where((m1, c1) => m1.Account.ID == p.Account.ID));
                if (mm != null && mm.Count() > 0)
                {
                    var i = _employees.IndexOf(mm.First());
                    _employees[i] = p;
                    var t = _employees;
                    Employees = new ObservableCollection<EmployeeModel>();
                    Employees = t;

                    Designations = (from x in _employees select x.Desig).Distinct().ToList<string>();

                }
                else
                {
                    _employees.Add(p);
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public bool Remove(Model.EmployeeModel p)
        {
            bool res = false;
            try
            {
                var mm = (_employees.Where((m1, c1) => m1.Account.ID == p.Account.ID));
                if (mm != null && mm.Count() > 0)
                {
                    _employees.Remove(mm.First());

                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        private string _selectedDesignation = null;
        private List<string> _designations;
        public string SelectDesignation
        {
            get => _selectedDesignation;

        }
        public List<string> Designations
        {
            get => _designations;
            set => SetProperty(ref _designations, value);
        }

        //Departments
        ObservableCollection<Model.DepartmentModel> _departments = new ObservableCollection<DepartmentModel>();
        Model.DepartmentModel _selecteddepartment = new DepartmentModel();
        public ObservableCollection<DepartmentModel> Departments
        {
            get { return _departments; }
        }
        public void Departments_To_List()
        {
            try
            {
                DB.Departments.Fetch();
                var receipts = DB.Departments.DepTable;
                var plist = (from r in receipts.AsEnumerable()
                             select new
                             {
                                 obj = new Model.DepartmentModel()
                                 {
                                     Dep_id = r.Field<int>("id"),
                                     Dep_Head = r.Field<string>("dhead"),
                                     Name = r.Field<string>("department"),
                                     Narration = r.Field<string>("narration"),
                                 }
                             }.obj
                           ).ToList<Model.DepartmentModel>();
                _departments = new ObservableCollection<DepartmentModel>(plist);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        public bool Add_Update(Model.DepartmentModel p)
        {
            bool res = false;
            try
            {
                var mm = (_departments.Where((m1, c1) => m1.Dep_id == p.Dep_id));
                if (mm != null && mm.Count() > 0)
                {
                    var i = _departments.IndexOf(mm.First());
                    _departments[i] = p;

                }
                else
                {
                    _departments.Add(p);
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public bool Remove(Model.DepartmentModel p)
        {
            bool res = false;
            try
            {
                var mm = (_departments.Where((m1, c1) => m1.Dep_id == p.Dep_id));
                if (mm != null && mm.Count() > 0)
                {
                    _departments.Remove(mm.First());

                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public Model.DepartmentModel SelectedDepartment
        {
            get { return _selecteddepartment; }
            set { _selecteddepartment = value; }
        }

        //PayRoll Vouchers
        ObservableCollection<Model.PayRollVoucherModel> _payroll_vouchers = new ObservableCollection<PayRollVoucherModel>();
        Model.PayRollVoucherModel _selectedVoucher = new PayRollVoucherModel();
        public ObservableCollection<PayRollVoucherModel> PayrollVouchers
        {
            get { return _payroll_vouchers; }
        }
        public void PayRolls_To_List()
        {
            try
            {
                Employees_To_List();
                DB.PayrollVoucher.Fetch();
                var receipts = DB.PayrollVoucher.PVoucherTable;
                var plist = (from r in receipts.AsEnumerable()
                             join a in _accounts on r.Field<int>("cash_ac") equals a.ID
                             join ea in _employees on r.Field<int>("eid") equals ea.Account.ID

                             select new
                             {
                                 obj = new Model.PayRollVoucherModel()
                                 {
                                     CrAccount = a,
                                     DrAccount = ea,
                                     VoucherType = r.Field<string>("type"),
                                     PPDate = r.Field<DateTime>("post_date"),
                                     VNO = r.Field<int>("pp_no"),
                                     Amount = Convert.ToDouble(r.Field<decimal>("amount")),
                                     Narration = r.Field<string>("narration"),
                                     Task_ID = r.Field<int>("taskid"),
                                 }
                             }.obj
                           ).ToList<Model.PayRollVoucherModel>();
                _payroll_vouchers = new ObservableCollection<PayRollVoucherModel>(plist);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        public bool Add_Update(Model.PayRollVoucherModel p)
        {
            bool res = false;
            try
            {
                var mm = (_payroll_vouchers.Where((m1, c1) => m1.VNO == p.VNO));
                if (mm != null && mm.Count() > 0)
                {
                    var i = _payroll_vouchers.IndexOf(mm.First());
                    _payroll_vouchers[i] = p;

                }
                else
                {
                    _payroll_vouchers.Add(p);
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public bool Remove(Model.PayRollVoucherModel p)
        {
            bool res = false;
            try
            {
                var mm = (_payroll_vouchers.Where((m1, c1) => m1.VNO == p.VNO));
                if (mm != null && mm.Count() > 0)
                {
                    _payroll_vouchers.Remove(mm.First());

                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public Model.PayRollVoucherModel SelectedVoucher
        {
            get { return _selectedVoucher; }
            set { _selectedVoucher = value; }
        }
        //PayRoll  
        ObservableCollection<Model.PayRollEntryModel> _payrolls = new ObservableCollection<PayRollEntryModel>();
        ObservableCollection<Model.PayrollEntryVoucher> _payrollEntryVouchers = new ObservableCollection<PayrollEntryVoucher>();
        Model.PayRollEntryModel _selectedPayroll = new PayRollEntryModel();
        public ObservableCollection<PayRollEntryModel> Payrolls
        {
            get { return _payrolls; }
        }
        public ObservableCollection<PayrollEntryVoucher> PayrollEntryVouchers
        {
            get { return _payrollEntryVouchers; }
        }
        public void MonthlyPayRolls_To_List()
        {
            try
            {
                if (ViewModels_Variables.ModelViews.Accounts.Count <= 0)
                {
                    DB.Accounts.Fetch();
                    ViewModels_Variables.ModelViews.AccountToCollection();
                }
                if (ViewModels_Variables.ModelViews.Employees.Count <= 0)
                {
                    DB.Employee.Fetch();
                    ViewModels_Variables.ModelViews.Employees_To_List();
                }

                DB.MonthlyPayroll.Fetch();

                var pv = (from p1 in DB.MonthlyPayroll.MPayrollVouchers.AsEnumerable()
                          select new
                          {
                              pvs = new Model.PayrollEntryVoucher()
                              {
                                  Id = p1.Field<int>("id"),
                                  PayrollNo = p1.Field<int>("pe_no"),
                                  Voucher = p1.Field<string>("voucher"),
                                  Amount = Convert.ToDouble(p1.Field<decimal>("amount")),
                                  IsGenerated = p1.Field<bool>("isgen"),

                              }
                          }.pvs).ToList<Model.PayrollEntryVoucher>();
                _payrollEntryVouchers = new ObservableCollection<PayrollEntryVoucher>(pv);

                if (_payrollEntryVouchers.Count > 0)
                {
                    var receipts = DB.MonthlyPayroll.MPayrollTable;
                    var plist = (from r in receipts.AsEnumerable()
                                 join all in _payrollEntryVouchers on r.Field<int>("pe_no") equals all.PayrollNo
                                 where all.Voucher == "Alownace"
                                 join com in _payrollEntryVouchers on r.Field<int>("pe_no") equals com.PayrollNo
                                 where com.Voucher == "Commission"
                                 join adv in _payrollEntryVouchers on r.Field<int>("pe_no") equals adv.PayrollNo
                                 where adv.Voucher == "Advance"
                                 join ded in _payrollEntryVouchers on r.Field<int>("pe_no") equals ded.PayrollNo
                                 where ded.Voucher == "Deduction"
                                 join ea1 in _employees on r.Field<int>("lid") equals ea1.Account.ID
                                 join cra in _accounts on r.Field<int>("crledger") equals cra.ID


                                 select new
                                 {
                                     obj = new Model.PayRollEntryModel()
                                     {
                                         VNO = r.Field<int>("pe_no"),
                                         DrAcid = r.Field<int>("eid"),
                                         EID = r.Field<int>("eid"),
                                         Allownaces = all,
                                         Commission = com,
                                         Advance = adv,
                                         Deductions = ded,
                                         CrAccount = cra,
                                         DrAccount = ea1,
                                         IsRecurring = r.Field<bool>("isrecurring"),
                                         Task_ID = r.Field<int>("taskid"),
                                         DATE = r.Field<DateTime>("pe_date"),
                                         Basic = Convert.ToDouble(r.Field<decimal>("bp")),
                                         Total = Convert.ToDouble(r.Field<decimal>("total")),
                                         WDs = Convert.ToDouble(r.Field<decimal>("wdays")),
                                         //WHs = Convert.ToDouble(r.Field<decimal>("whours")),
                                         //Narration = r.Field<string>("narration"),

                                     }
                                 }.obj
                               ).ToList<Model.PayRollEntryModel>();
                    _payrolls = new ObservableCollection<PayRollEntryModel>(plist);
                }
                else
                {
                    var receipts = DB.MonthlyPayroll.MPayrollTable;
                    var plist = (from r in receipts.AsEnumerable()
                                 join ea1 in _employees on r.Field<int>("eid") equals ea1.Account.ID
                                 join cra in _accounts on r.Field<int>("crledger") equals cra.ID


                                 select new
                                 {
                                     obj = new Model.PayRollEntryModel()
                                     {
                                         VNO = r.Field<int>("pe_no"),
                                         DrAcid = r.Field<int>("eid"),
                                         EID = r.Field<int>("eid"),
                                         CrAccount = cra,
                                         DrAccount = ea1,
                                         IsRecurring = r.Field<bool>("isrecurring"),
                                         Task_ID = r.Field<int>("taskid"),
                                         DATE = r.Field<DateTime>("pe_date"),
                                         Basic = Convert.ToDouble(r.Field<decimal>("bp")),
                                         Total = Convert.ToDouble(r.Field<decimal>("total")),
                                         WDs = Convert.ToDouble(r.Field<decimal>("wdays")),
                                         //WHs = Convert.ToDouble(r.Field<decimal>("whours")),
                                         //Narration = r.Field<string>("narration"),

                                     }
                                 }.obj
                               ).ToList<Model.PayRollEntryModel>();
                    foreach (var t in plist)
                    {
                        _payrolls.Add(t);
                    }
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        public bool Add_Update(Model.PayRollEntryModel p)
        {
            bool res = false;
            try
            {
                var mm = (_payrolls.Where((m1, c1) => m1.VNO == p.VNO));
                if (mm != null && mm.Count() > 0)
                {
                    var i = _payrolls.IndexOf(mm.First());
                    _payrolls[i] = p;

                }
                else
                {
                    _payrolls.Add(p);
                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public bool Remove(Model.PayRollEntryModel p)
        {
            bool res = false;
            try
            {
                var mm = (_payrolls.Where((m1, c1) => m1.VNO == p.VNO));
                if (mm != null && mm.Count() > 0)
                {
                    _payrolls.Remove(mm.First());

                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public Model.PayRollEntryModel SelectedPayroll
        {
            get { return _selectedPayroll; }
            set { _selectedPayroll = value; }
        }

        //Casshbook
        private ObservableCollection<Model.CashBookModel> _cashbook = new ObservableCollection<CashBookModel>();
        public ObservableCollection<CashBookModel> CashBook
        {
            get => _cashbook;
        }
        public bool Add_Update(Model.CashBookModel cashBook)
        {
            bool res = false;
            try
            {
                var mm = (_cashbook.Where((m1, c1) => m1.VNo == cashBook.VNo && m1.Voucher == cashBook.Voucher));
                if (mm != null && mm.Count() > 0)
                {
                    _cashbook.Remove(mm.FirstOrDefault());

                }

                _cashbook.Add(cashBook);

                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public bool Remove(CashBookModel cash)
        {
            bool res = false;
            try
            {
                var mm = (_cashbook.Where((m1, c1) => m1.VNo == cash.VNo && m1.Voucher == cash.Voucher));
                if (mm != null && mm.Count() > 0)
                {
                    _cashbook.Remove(mm.First());

                }
                res = true;
            }
            catch (System.Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }

            return res;
        }
        public void RefreshCashBook()
        {
            try
            {

                var cashBook = (from t1 in _trsansactions
                                join lop in _accounts on t1.Op_Ac_Id equals lop.ID
                                join l in _accounts on t1.Ac_Id equals l.ID
                                select new
                                {
                                    cash = new CashBookModel()
                                    {
                                        Date = t1.Tr_date,
                                        DrAccount = lop,
                                        CrAccount = l,
                                        Dr_Amount = t1.Dr_Amount,
                                        Cr_Amount = t1.Cr_Amount,
                                        VNo = t1.Eno,
                                        Voucher = t1.Entry,
                                        Invno = t1.Cinv_no,
                                        Balance = DB.Connection.GetActBalance(t1.Op_Ac_Id),
                                        Opening = DB.Connection.GetOB(t1.Tr_date, t1.Op_Ac_Id, t1.Cinv_no),
                                        Narration = ""
                                    }
                                }.cash).ToList<CashBookModel>();
                _cashbook = new ObservableCollection<CashBookModel>(cashBook);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }
        //Company
        ObservableCollection<Model.SCompany> _company = new ObservableCollection<SCompany>();

        public ObservableCollection<Model.SCompany> CompanyProfile
        {
            get => _company;
            set => SetProperty(ref _company, value);
        }
        public void RefreshCompany()
        {
            try
            {



                DB.Company.Fetch();
                if (DB.Company.CompanyTable.Rows.Count > 0)
                {
                    var cob = (from cm in DB.Company.CompanyTable.AsEnumerable()
                               select new
                               {
                                   c = new SCompany()
                                   {
                                       company = cm.Field<string>("company"),
                                       lmark = cm.Field<string>("lmark"),
                                       place = cm.Field<string>("place"),
                                       street = cm.Field<string>("street"),
                                       post = cm.Field<string>("post"),
                                       zipcode = cm.Field<string>("zipcode"),
                                       TAXID = cm.Field<string>("taxid"),
                                       DLNO = cm.Field<string>("dlno"),
                                       expno = cm.Field<string>("expno"),
                                       email = cm.Field<string>("email"),
                                       officeno = cm.Field<string>("officeno"),
                                       Mobile = cm.Field<string>("mobile"),
                                       f_date1 = cm.Field<DateTime>("f_date1"),
                                       f_date2 = cm.Field<DateTime>("f_date2"),
                                       SoftWareCaption = "Book Keeper - " + cm.Field<string>("company") + " [" + cm.Field<DateTime>("f_date1").ToShortDateString() + "]"
                                   }
                               }.c).ToList<SCompany>();

                    ObservableCollection<Model.SCompany> company_ = new ObservableCollection<SCompany>(cob);
                    _company.Clear();
                    CompanyProfile = company_;

                    //Other settings
                    var plist = SerializeHelper.DeserialilZe<ObservableCollection<PackageClass>>(public_members.licenceFile);
                    var version = (from p in plist.AsEnumerable() where p.PName == "VERSION" select p.Pvalue).ElementAt(0);
                    var developer = (from p in plist.AsEnumerable() where p.PName == "DEVELOPER" select p.Pvalue).ElementAt(0);
                    var lkey = (from p in plist.AsEnumerable() where p.PName == "LICENSE_KEY" select p.Pvalue).ElementAt(0);
                    var package = (from p in plist.AsEnumerable() where p.PName == "PACKAGE" select p.Pvalue).ElementAt(0);
                    var helpline = (from p in plist.AsEnumerable() where p.PName == "CONTACT_HELPLINE" select p.Pvalue).ElementAt(0);
                    var mode = (from p in plist.AsEnumerable() where p.PName == "MODE" select p.Pvalue).ElementAt(0);
                    var Enabled_Auto_Account_Creation = (from p in plist.AsEnumerable() where p.PName == "AUTO ACCOUNTS" select p.Pvalue).ElementAt(0);
                    _front_panel.UpdateOrAdd("AUTO ACCOUNTS", Enabled_Auto_Account_Creation);
                    var ENABLED_COMMERCIAL_ACCOUNT = (from p in plist.AsEnumerable() where p.PName == "COMMERCIAL ACCOUNT" select p.Pvalue).ElementAt(0);
                    _front_panel.UpdateOrAdd("IS COMMERCIAL ACCOUNT", ENABLED_COMMERCIAL_ACCOUNT);
                    var ENABLED_PERSONAL_ACCOUNT = (from p in plist.AsEnumerable() where p.PName == "INDIVIDUAL ACCOUNT" select p.Pvalue).ElementAt(0);
                    _front_panel.UpdateOrAdd("IS INDIVIDUAL ACCOUNT", ENABLED_PERSONAL_ACCOUNT);

                    _front_panel.UpdateOrAdd("MODE", mode);
                    _front_panel.UpdateOrAdd("Softwarecaption", package);
                    _front_panel.UpdateOrAdd("Version", version);
                    if (developer != null) _front_panel.UpdateOrAdd("Developer", developer);
                    if (lkey != null) _front_panel.UpdateOrAdd("Licensekey", lkey);
                    if (helpline != null) { _front_panel.UpdateOrAdd("Contact_Helpline", helpline); }


                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        ObservableCollection<Model.CashBookModel> _daybook = new ObservableCollection<CashBookModel>();
        public ObservableCollection<CashBookModel> DayBook
        {
            get => _daybook;
            set => SetProperty(ref _daybook, value);
        }
        public void RefreshDaybook(string casname = "Cash")
        {
            try
            {
                var daybook = (from t1 in _trsansactions
                               join opl in _accounts on t1.Op_Ac_Id equals opl.ID
                               join l in _accounts on t1.Ac_Id equals l.ID

                               where l.Name != casname
                               select new
                               {
                                   db = new CashBookModel()
                                   {
                                       Date = t1.Tr_date,
                                       DrAccount = l,
                                       CrAccount = opl,
                                       Dr_Amount = t1.Dr_Amount,
                                       Cr_Amount = t1.Cr_Amount,
                                       VNo = t1.Eno,
                                       Voucher = t1.Entry,

                                       Invno = t1.Cinv_no,
                                       Balance = DB.Connection.GetActBalance(t1.Tr_date, t1.Ac_Id, t1.Cinv_no),
                                   }
                               }.db).ToList<CashBookModel>();
                _daybook = new ObservableCollection<CashBookModel>(daybook);
            }
            catch (Exception er)
            {

                throw;
            }
        }
        ObservableCollection<CashBookModel> _groupbook = new ObservableCollection<CashBookModel>();
        public ObservableCollection<CashBookModel> GroupBook
        {
            get => _groupbook;
            set => SetProperty(ref _groupbook, value);
        }
        public void RefreshGroupBook()
        {
            try
            {

                var gb = (from t in _accounts
                          select new
                          {
                              b = new CashBookModel()
                              {
                                  CrAccount = t,
                                  Dr_Amount = DB.Connection.GetDrCr(t.ID)["Dr"],
                                  Cr_Amount = DB.Connection.GetDrCr(t.ID)["Cr"],
                                  Balance = DB.Connection.GetActBalance(t.ID),
                              }
                          }.b).ToList<CashBookModel>();
                _groupbook = new ObservableCollection<CashBookModel>(gb);

            }
            catch (Exception)
            {

                throw;
            }
        }
        //Account List with balance
        ObservableCollection<CashBookModel> _accountList = new ObservableCollection<CashBookModel>();
        public ObservableCollection<CashBookModel> AccountList
        {
            get => _accountList;
            set => SetProperty(ref _accountList, value);
        }
        public void RefreshAccountList()
        {
            try
            {
                var accs = (from a in _accounts
                            select new
                            {
                                accs = new CashBookModel()
                                {
                                    DrAccount = a,
                                    Balance = DB.Connection.GetActBalance(a.ID),
                                    Dr_Amount = DB.Connection.GetOBDrCr(public_members._sysDate[0].AddDays(1), a.ID)["Dr"],
                                    Cr_Amount = DB.Connection.GetOBDrCr(public_members._sysDate[0].AddDays(1), a.ID)["Cr"],
                                }
                            }.accs).ToList<CashBookModel>();
                _accountList = new ObservableCollection<CashBookModel>(accs);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }


}
