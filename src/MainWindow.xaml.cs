using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.IO;

namespace accounts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        public MainWindow()
        {
            InitializeComponent();
            public_members._sysDate.Clear();
            public_members._sysDate.Add(DateTime.Now.Date);
            ViewModels_Variables._sysDate = public_members._sysDate;
            txt_sys_date.DataContext = ViewModels_Variables._sysDate;
            tbl_ontody.DataContext = public_members._sysDate;
            txt_sys_date.DataContext = public_members._sysDate;
            ViewModels_Variables.ModelViews = new ModelViews.AccountsModelViews();
            this.DataContext = ViewModels_Variables.ModelViews;

            ViewModels_Variables.ModelViews.Refresh_FrontPanelItems();
        }


        private void Page1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GroupRegistration grp = new GroupRegistration();
                grp.Owner = this;
                grp.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void accounts_click(object sender, RoutedEventArgs e)
        {
            try
            {
                accountRegistration acc = new accountRegistration();
                acc.Owner = this;
                acc.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                journalposting jp = new journalposting();
                jp.Owner = this;
                jp.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void receipt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Receipt rc = new Receipt(); rc.Owner = this;
                rc.Show();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        private void pay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                payment pay = new payment();
                pay.Owner = this;
                pay.Show();
            }
            catch (Exception)
            {

                throw;
            }

        }







        private void Employee_reg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EmployeeRegistration emp = new EmployeeRegistration(); emp.Owner = this; emp.Show();
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void payroll_adv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                payroll_payments_deductions p = new payroll_payments_deductions(); p.Owner = this; p.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void payroll_address_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Report_list_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                ReportListTemplateForm frm = new ReportListTemplateForm();
                frm.Owner = this;
                frm.Show();
            }
            catch (Exception)
            {

                throw;
            }


        }

        private void receipt_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReceiptReport rp = new ReceiptReport();
                rp.Owner = this;
                rp.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void pament_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PaymentReport rp = new PaymentReport();
                rp.Owner = this;
                rp.Show();
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void cashBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CashBook1 cb = new CashBook1();
                cb.Owner = this;
                cb.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void day_book_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DayBook db = new DayBook();
                db.Owner = this;
                db.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    MessageBoxResult res = new MessageBoxResult();
                    res = MessageBox.Show("Do you want close this Application", "Close the Application", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void account_list_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                account_List aclist = new account_List();
                aclist.Owner = this;
                aclist.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void group_accounts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GroupList grplist = new GroupList();
                grplist.Owner = this;
                grplist.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void bank_receipt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bankReceipt br = new bankReceipt();
                br.Owner = this;
                br.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void bank_payment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BankPayment bp = new BankPayment();
                bp.Owner = this;
                bp.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btn_bank_receipt_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                brReport brReport = new brReport();
                brReport.Owner = this;
                brReport.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void bank_paymentReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bpReport bpr = new bpReport();
                bpr.Owner = this; bpr.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void group_accounts_setup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult res = new MessageBoxResult();
                res = MessageBox.Show("Do you want Run Autocreate Account Groups", "Group Creation", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    public_members.Accountsetup();

                    MessageBox.Show("Setup Finished successfully");
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message.ToString());
                throw;
            }
        }

        private void btn_trial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                trialBalance trial = new trialBalance();
                trial.Owner = this;
                trial.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txt_sys_date_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                ChangeDate changeDate = new ChangeDate(); changeDate.Owner = this;
                changeDate.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btn_payroll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                payroll payroll = new payroll();
                payroll.Owner = this;
                payroll.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void payable_recei_Click(object sender, RoutedEventArgs e)
        {

        }

        private void payroll_voucherList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ppReport ppReport = new ppReport();
                ppReport.Owner = this; ppReport.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void payroll_entry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                payroll pr = new payroll();
                pr.Owner = this; pr.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void payroll_entry_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                payrollreport pr = new payrollreport();
                pr.Owner = this;
                pr.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }



        private void btn_Purchas_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_sales_Click(object sender, RoutedEventArgs e)
        {

        }



        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (public_members.RefreshAll() == true)
                {
                    MessageBox.Show("Refreshed");
                }
                else { MessageBox.Show("All items not refreshed"); }
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {

            try
            {
                Company sc = new Company();
                sc.Owner = this;
                sc.Show();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message.ToString());
                throw;
            }
        }

        private void tbl_company_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Company sc = new Company();
                sc.Owner = this;
                sc.Show();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message.ToString());
                throw;
            }
        }

        private void btn_Configuration_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dbconfiguration dbsettings = new dbconfiguration();
                dbsettings.Owner = this;
                dbsettings.Show();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }


        private void btn_company_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Company sc = new Company();
                sc.Owner = this;
                sc.Show();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message.ToString());

            }
        }




        private void menu_acc_reg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                accountRegistration grp = new accountRegistration();
                grp.Owner = this;
                grp.Show();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_group_reg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GroupRegistration grp = new GroupRegistration();
                grp.Owner = this;
                grp.Show();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_jounal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_receipt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                receipt_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_payment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pay_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_br_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bank_receipt_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_bp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bank_payment_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_payroll_voucher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                payroll_adv_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_monthlypayroll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                payroll_entry_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_daybook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                day_book_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_receipt_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                receipt_report_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_payment_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bank_paymentReport_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_bp_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bank_paymentReport_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_br_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btn_bank_receipt_report_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_par_vouchers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                payroll_voucherList_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_monthlyPar_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                payroll_entry_report_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_groupBalance_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                group_accounts_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_trial_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btn_trial_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_account_list_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                account_list_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_config_company_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btn_company_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_dbconfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btn_Configuration_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_checkErr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (public_members.server != null && public_members.database != null)
                {
                    //SqlHelper.GenerateFields(public_members.database, public_members.server);
                    MessageBox.Show("DataBase Modified");
                }
                else
                {
                    MessageBox.Show("Database or server not available");
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }

        }

        private void menu_backup_Click(object sender, RoutedEventArgs e)
        {
            //Back up database
            bool flag = true; try
            {

                if (DB.Connection.Backup(backup_path: public_members.backuppath1) == true)
                {
                    MessageBox.Show("Backup completed");                 
                }
                else
                {
                    MessageBox.Show("Backup settings not configured");

                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }

        }

        private void txt_bank_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                var cashgroup = ViewModels_Variables.ModelViews.AccountGroups.Where((ac) => ac.Name == "BANK ACCOUNT").FirstOrDefault();
                if (cashgroup != null)
                {
                    GroupList cashbooks = new GroupList(cashgroup.ID);

                    cashbooks.Show();
                }


            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());

            }
        }

        private void txt_cash_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var cashgroup = ViewModels_Variables.ModelViews.AccountGroups.Where((ac) => ac.Name == "CASH" || ac.Name == "CASH IN HAND").FirstOrDefault();
                if (cashgroup != null)
                {
                    GroupList cashbooks = new GroupList(cashgroup.ID);

                    cashbooks.Show();
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());

            }
        }

        private void txt_exp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //public_members.RefreshCashBooktCollections();
                //var in_expcount = (from cash in public_members.cashbook_obscoll.AsEnumerable() where cash.DateCheck == public_members._sysDate[0] && cash.Type == "INDIRECT EXPENDITURE" select cash.VNo).Count();

                //var direct_expcount = (from cash in public_members.cashbook_obscoll.AsEnumerable() where cash.Date == public_members._sysDate[0] && cash.Type == "DIRECT EXPENDITURE" select cash.VNo).Count();

                //if (in_expcount > 0)
                //{
                //    CashBook1 cashBook = new CashBook1(public_members.Getgroupid("Indirect Expenditure"), true);
                //    cashBook.Owner = this;
                //    cashBook.Show();
                //}
                //if (direct_expcount > 0)
                //{
                //    CashBook1 cashBook = new CashBook1(public_members.Getgroupid("direct Expenditure"), true);
                //    cashBook.Owner = this;
                //    cashBook.Show();
                //}

            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());

            }
        }

        private void txt_di_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //public_members.RefreshCashBooktCollections();
                //var in_expcount = (from cash in public_members.cashbook_obscoll.AsEnumerable() where cash.DateCheck == public_members._sysDate[0] && cash.Type == "INDIRECT INCOME" select cash.VNo).Count();

                //var direct_expcount = (from cash in public_members.cashbook_obscoll.AsEnumerable() where cash.Date == public_members._sysDate[0] && cash.Type == "DIRECT INCOME" select cash.VNo).Count();

                //if (in_expcount > 0)
                //{
                //    CashBook1 cashBook = new CashBook1(public_members.Getgroupid("Indirect INCOME"), true);
                //    cashBook.Owner = this;
                //    cashBook.Show();
                //}
                //if (direct_expcount > 0)
                //{
                //    CashBook1 cashBook = new CashBook1(public_members.Getgroupid("direct Income"), true);
                //    cashBook.Owner = this;
                //    cashBook.Show();
                //}

            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());

            }
        }

        private void txt_payments_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                //var count = (from x in public_members.payments_obscoll.AsEnumerable() where x.Date == public_members._sysDate[0] select x.pno).Count();
                //if (count > 0)
                //{
                //    PaymentReport receiptReport = new PaymentReport();
                //    receiptReport.Owner = this;
                //    receiptReport.Show();
                //}
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());

            }
        }

        private void txt_receipts_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                
                var count = (from x in ViewModels_Variables.ModelViews.Receipts where x.Date == public_members._sysDate[0] select x.rno).Count();
                if (count > 0)
                {
                    ReceiptReport receiptReport = new ReceiptReport();
                    receiptReport.Owner = this;
                    receiptReport.Show();
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());

            }
        }

        private void txt_receivables_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                var cashgroup = ViewModels_Variables.ModelViews.AccountGroups.Where((ac) => ac.Catagory == "RECEIVABLE").FirstOrDefault();
                if (cashgroup != null)
                {
                    GroupList cashbooks = new GroupList(cashgroup.ID);

                    cashbooks.Show();
                }


            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());

            }
        }

        private void txt_payable_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var cashgroup = ViewModels_Variables.ModelViews.AccountGroups.Where((ac) => ac.Catagory == "PAYABLE").FirstOrDefault();
                if (cashgroup != null)
                {
                    GroupList cashbooks = new GroupList(cashgroup.ID);

                    cashbooks.Show();
                }

            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());

            }
        }

        private void bankpayment_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //BPAyment
            try
            {

                var count = (from x in ViewModels_Variables.ModelViews.BankPayments where x.Date == public_members._sysDate[0] select x.pno).Count();
                if (count > 0)
                {
                    bpReport bpreport = new bpReport();
                    bpreport.Owner = this;
                    bpreport.Show();
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());

            }
        }

        private void txt_breceipts_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                var count = (from x in ViewModels_Variables.ModelViews.BankReceipts.AsEnumerable() where x.Date == public_members._sysDate[0] select x.rno).Count();
                if (count > 0)
                {
                    brReport brreport = new brReport();
                    brreport.Owner = this;
                    brreport.Show();
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
                throw;
            }
        }

        private void menu_emp_reg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Employee_reg_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }


        private void menu_cashbook_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                cashBook_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_pkgmngr_CMdBinder_canexecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void menu_pkgmngr_CMdBinder_executed(object sender, ExecutedRoutedEventArgs e)
        {
            Pkg_manager();
        }
        void Pkg_manager()
        {
            try
            {
                PackageManager pkg = new PackageManager();
                pkg.Owner = this;
                pkg.Show();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());

            }
        }
        private void menu_pkgmgr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PackageManager pkg = new PackageManager();
                pkg.Owner = this;
                pkg.Show();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());

            }
        }

        private void btn_backup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                menu_backup_Click(sender, e);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());

            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menu_report_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menu_refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btn_refresh_Click(sender, e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                var r = File.Exists(public_members.backend_path);
                if (r == false)
                {
                    dbconfiguration dbconfig = new dbconfiguration();                       //dbconfig.Owner = this;
                    dbconfig.Owner = this; ;
                }
                else
                {

                    if (File.Exists(public_members.licenceFile))
                    {
                        var plist = SerializeHelper.DeserialilZe<ObservableCollection<PackageClass>>(public_members.licenceFile);
                        if (plist != null)
                        {
                            menu_pkgmgr.Visibility = Visibility.Collapsed;
                            if (File.Exists(public_members.factoryFile))
                            {
                                var factory = SerializeHelper.DeserialilZe<ObservableCollection<PackageClass>>(public_members.factoryFile);
                                if (factory != null)
                                {
                                    menu_pkgmgr.Visibility = Visibility.Visible;
                                }
                            }

                            public_members.LoadIniSettings();
                            if (public_members.sysbackgrouSource != null && public_members.sysbackgrouSource.Length > 0)
                            {
                                ImageBrush imageBrush = new ImageBrush(new BitmapImage(new Uri(public_members.sysbackgrouSource, UriKind.RelativeOrAbsolute)));
                                this.Background = imageBrush;

                            }

                        }
                        else
                        {
                            MessageBox.Show("Fake Licence file !"); this.Close();
                        }
                    }


                    else
                    {
                        MessageBox.Show("Licence Key Is missing");
                        this.Close();
                    }
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void menu_autocreate_Click(object sender, RoutedEventArgs e)
        {
            group_accounts_setup_Click(sender, e);
        }

        private void menu_taskMaster_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bt_Payables_rece_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                About pkg = new About();
                pkg.Owner = this;
                pkg.Show();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());

            }
        }

        private void bt_managea_ccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AccountManager acc_mgr = new AccountManager();
                acc_mgr.Owner = this;
                acc_mgr.Show();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());

            }
        }
    }

}
