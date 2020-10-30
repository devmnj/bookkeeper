using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Words.NET;
using System.IO;
using System.Diagnostics;
using System.Windows.Xps.Packaging;

namespace accounts
{
    /// <summary>
    /// Interaction logic for payroll.xaml
    /// </summary>
    public partial class payroll : Window
    {
        List<int> p_nos = new List<int>();
        int cindex = 0;
        bool[] flag = new bool[3];
        double crLimit, gcrLimit, drLimit, gdrLimit;
        bool all_flag = false, adv_flag = false, ded_flag = false, comm_flag = false;
        double allow, ded, comm, adv;
        string wordFname = null;
        string xpsFname = null;
        string pdfFname = null;
        string excelFname = null;
        void ClearPrintCache()
        {
            try
            {

                wordFname = null;
                xpsFname = null;
                pdfFname = null;
                excelFname = null;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public payroll()
        {
            InitializeComponent();



        }
        void FindTask(int eno)
        {
            try
            {
                var t1 = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ENO == eno && ts.ENTRY == "PAYROLL").FirstOrDefault();
                if (t1 != null)
                {
                    taskgroup.IsEnabled = true;
                    btn_task_del.IsEnabled = true;
                    btn_task_add.IsEnabled = true;
                    txt_task_Amount.Text = t1.T_AMOUNT.ToString("0.00");
                    txt_task_label.Text = t1.T_LABEL;
                    chk_isrecurr.IsChecked = true;
                }
                else
                {
                    btn_task_add.IsEnabled = true;
                    taskgroup.IsEnabled = true;
                    btn_task_del.IsEnabled = false;
                    chk_isrecurr.IsChecked = false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        void NewButtonState()
        {
            btn_task_add.IsEnabled = false;
            btn_task_del.IsEnabled = false;
            ClearPrintCache();
            txt_task_Amount.Text = "";
            txt_task_label.Text = "";

            flag = new bool[] { false, false, false, false };
            btn_print.IsEnabled = false;
            btn_save.IsEnabled = true;
            btn_update1.IsEnabled = false;
            btn_delete.IsEnabled = false;
            txt_allowances.Text = "0.00";
            txt_basicpay.Text = "0.00";
            txt_commission.Text = "0.00";
            txt_advancepaid.Text = "0.00";
            cmb_dr_short_name.Text = "";
            txt_eid.Text = "";
            txt_deductions.Text = "";
            CMB_CRACC.Text = "";
            txt_wh.Text = "";
            txt_info.Text = "";
            lbl_taskflag.Visibility = Visibility.Collapsed;
            dtp_jdate.SelectedDate = public_members._sysDate[0];
            var tlist = ViewModels_Variables.ModelViews.Tasks.Where((t) => t.ENTRY == "PAYROLL");
            DataTemplate dataTemplate = this.FindResource("Tasks") as DataTemplate;
            lst_tasks.ItemTemplate = dataTemplate;
            lst_tasks.ItemsSource = tlist;
            hist_task_lbl.Content = "TASKS";
            txt_wd.Text = "";
            txt_narration.Text = "";

            chk_isrecurr.IsChecked = false;
            dtp_jdate.Focus();
            txt_jno.Text = DB.Connection.NewEntryno(table: "payroll_entry", field: "pe_no").ToString();
            p_nos = (from p in ViewModels_Variables.ModelViews.Payrolls select p.VNO).ToList<int>();
        }


        public void Find(int eno)
        {
            try
            {
                var rows = ViewModels_Variables.ModelViews.Payrolls.Where((pr) => pr.VNO == eno).FirstOrDefault();
                if (rows != null)
                {
                    //NewButtonState();
                    txt_jno.Text = rows.VNO.ToString();
                    txt_eid.Text = rows.EID.ToString();

                    if (rows.DrAccount != null) { cmb_dr_short_name.SelectedItem = rows.DrAccount; }

                    if (rows.CrAccount != null) { CMB_CRACC.SelectedItem = rows.CrAccount; }

                    if (rows.Allownaces != null)
                    {
                        txt_allowances.Text = rows.Allownaces.Amount.ToString("0.00");
                        comm_flag = rows.Allownaces.IsGenerated;
                    }


                    if (rows.Commission != null)
                    {
                        txt_commission.Text = rows.Commission.Amount.ToString("0.00");
                        comm_flag = rows.Commission.IsGenerated;
                    }
                    else { txt_commission.Text = string.Format("{0:0.00}", 0); }


                    if (rows.Advance != null)
                    {
                        txt_advancepaid.Text = rows.Advance.Amount.ToString("0.00");
                        adv_flag = rows.Advance.IsGenerated;
                    }
                    else { txt_advancepaid.Text = string.Format("{0:0.00}", 0); }

                    if (rows.Deductions != null)
                    {
                        txt_deductions.Text = rows.Deductions.Amount.ToString("0.00");
                        ded_flag = rows.Deductions.IsGenerated;
                    }
                    else { txt_deductions.Text = string.Format("{0:0.00}", 0); }

                    txt_wh.Text = rows.WHs.ToString();
                    txt_wd.Text = rows.WDs.ToString();
                    txt_total.Text = rows.Total.ToString("0.00");



                    lbl_taskflag.Visibility = Visibility.Collapsed;
                    if (rows.Task_ID > 0) { lbl_taskflag.Visibility = Visibility.Visible; }
                    FindTask(eno: eno);
                    FindButtonState();
                }
                else
                {
                    MessageBox.Show("Entry Not found");
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }





        public void TabToSave()
        {

            if (btn_save.IsEnabled == true)
            {
                btn_save.Focus();
            }
            else
            {
                btn_update1.Focus();
            }
        }
        void FindButtonState()
        {
            btn_save.IsEnabled = false;
            btn_update1.IsEnabled = true;
            btn_delete.IsEnabled = true;
            btn_print.IsEnabled = true;
            ClearPrintCache();

        }
        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            NewButtonState();
        }
        void SumUp()
        {
            try
            {
                double total, bp, comm, adv, ded, allow;

                double.TryParse(txt_total.Text.ToString(), out total);
                double.TryParse(txt_basicpay.Text.ToString(), out bp);
                double.TryParse(txt_commission.Text.ToString(), out comm);
                double.TryParse(txt_allowances.Text.ToString(), out allow);
                double.TryParse(txt_advancepaid.Text.ToString(), out adv);
                double.TryParse(txt_deductions.Text.ToString(), out ded);
                double tot = (bp + comm + allow) - (adv + ded);
                txt_total.Text = string.Format("{0:0.00}", tot);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            double total, bp, comm, adv, ded, whs, wds, allow;
            try
            {
                double.TryParse(txt_total.Text.ToString(), out total);
                double.TryParse(txt_basicpay.Text.ToString(), out bp);
                double.TryParse(txt_commission.Text.ToString(), out comm);
                double.TryParse(txt_allowances.Text.ToString(), out allow);
                double.TryParse(txt_advancepaid.Text.ToString(), out adv);
                double.TryParse(txt_deductions.Text.ToString(), out ded);
                double.TryParse(txt_wh.Text.ToString(), out whs);
                double.TryParse(txt_wd.Text.ToString(), out wds);

                if (cmb_dr_short_name.SelectedItem != null && CMB_CRACC.SelectedItem != null)
                {
                    var cracc = (Model.AccountModel)CMB_CRACC.SelectedItem;
                    var emp_acc = (Model.EmployeeModel)cmb_dr_short_name.SelectedItem;
                    Model.PayRollEntryModel payRoll = new Model.PayRollEntryModel();
                    Model.PayrollEntryVoucher _comm, _allow, _deduct, _adva;

                    _comm = new Model.PayrollEntryVoucher();
                    _comm.Id = 0;
                    _comm.PayrollNo = 0;
                    _comm.Voucher = "Commission";
                    _comm.IsGenerated = flag[1];
                    _comm.Amount = comm;

                    _allow = new Model.PayrollEntryVoucher();
                    _allow.Id = 0;
                    _allow.PayrollNo = 0;
                    _allow.Voucher = "Allowance";
                    _allow.IsGenerated = flag[0];
                    _allow.Amount = allow;

                    _adva = new Model.PayrollEntryVoucher();
                    _adva.Id = 0;
                    _adva.PayrollNo = 0;
                    _adva.Voucher = "Advance";
                    _adva.IsGenerated = flag[2];
                    _adva.Amount = adv;

                    _deduct = new Model.PayrollEntryVoucher();
                    _deduct.Id = 0;
                    _deduct.PayrollNo = 0;
                    _deduct.Voucher = "Deduction";
                    _deduct.IsGenerated = flag[3];
                    _deduct.Amount = adv;


                    payRoll.DATE = dtp_jdate.SelectedDate.Value;
                    payRoll.Total = total;
                    payRoll.Basic = bp;
                    payRoll.Commission = _comm;
                    payRoll.Allownaces = _allow;
                    payRoll.Advance = _adva;
                    payRoll.Deductions = _deduct;
                    payRoll.WHs = whs;
                    payRoll.WDs = wds;
                    payRoll.CrAccount = cracc;
                    payRoll.DrAccount = emp_acc;
                    payRoll.EID = emp_acc.Id;
                    payRoll.Narration = txt_narration.Text.ToString();
                    if (cracc.ID != 0 && emp_acc.Account.ID != 0)
                    {
                        var r = DB.MonthlyPayroll.Save(payRoll: payRoll, flag: flag);
                        if (r > 0)
                        {
                            MessageBox.Show("Entry Saved");
                            btn_Reset_Click(sender, e);
                        }
                        else
                        {
                            MessageBox.Show("Something went wromg");
                        }
                    }
                    else
                    {
                        MessageBox.Show("SQL Conncetion error");
                    }
                }

                else
                {
                    MessageBox.Show("Enter data correctly");
                }

            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message.ToString());

            }
        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {

            double total, bp, comm, adv, ded, whs, wds, allow;
            try
            {
                double.TryParse(txt_total.Text.ToString(), out total);
                double.TryParse(txt_basicpay.Text.ToString(), out bp);
                double.TryParse(txt_commission.Text.ToString(), out comm);
                double.TryParse(txt_allowances.Text.ToString(), out allow);
                double.TryParse(txt_advancepaid.Text.ToString(), out adv);
                double.TryParse(txt_deductions.Text.ToString(), out ded);
                double.TryParse(txt_wh.Text.ToString(), out whs);
                double.TryParse(txt_wd.Text.ToString(), out wds);

                if (cmb_dr_short_name.SelectedItem != null && CMB_CRACC.SelectedItem != null)
                {
                    var cracc = (Model.AccountModel)CMB_CRACC.SelectedItem;
                    var emp_acc = (Model.EmployeeModel)cmb_dr_short_name.SelectedItem;
                    Model.PayRollEntryModel payRoll = new Model.PayRollEntryModel();
                    Model.PayrollEntryVoucher _comm, _allow, _deduct, _adva;

                    _comm = new Model.PayrollEntryVoucher();
                    _comm.Id = 0;
                    _comm.PayrollNo = 0;
                    _comm.Voucher = "Commission";
                    _comm.IsGenerated = flag[1];
                    _comm.Amount = comm;

                    _allow = new Model.PayrollEntryVoucher();
                    _allow.Id = 0;
                    _allow.PayrollNo = 0;
                    _allow.Voucher = "Allowance";
                    _allow.IsGenerated = flag[0];
                    _allow.Amount = allow;

                    _adva = new Model.PayrollEntryVoucher();
                    _adva.Id = 0;
                    _adva.PayrollNo = 0;
                    _adva.Voucher = "Advance";
                    _adva.IsGenerated = flag[2];
                    _adva.Amount = adv;

                    _deduct = new Model.PayrollEntryVoucher();
                    _deduct.Id = 0;
                    _deduct.PayrollNo = 0;
                    _deduct.Voucher = "Deduction";
                    _deduct.IsGenerated = flag[3];
                    _deduct.Amount = adv;


                    payRoll.DATE = dtp_jdate.SelectedDate.Value;
                    payRoll.Total = total;
                    payRoll.Basic = bp;
                    payRoll.Commission = _comm;
                    payRoll.Allownaces = _allow;
                    payRoll.Advance = _adva;
                    payRoll.Deductions = _deduct;
                    payRoll.WHs = whs;
                    payRoll.WDs = wds;
                    payRoll.CrAccount = cracc;
                    payRoll.DrAccount = emp_acc;
                    payRoll.EID = emp_acc.Id;
                    payRoll.Narration = txt_narration.Text.ToString();
                    if (cracc.ID != 0 && emp_acc.Account.ID != 0)
                    {


                        var r = DB.MonthlyPayroll.Update(payRoll: payRoll, flag: flag);
                        if (r == true)
                        {
                            MessageBox.Show("Payroll Updated Saved");
                            btn_Reset_Click(sender, e);
                        }
                        else
                        {
                            MessageBox.Show("Something went wromg");
                        }
                    }

                    else
                    {
                        MessageBox.Show("Enter data correctly");
                    }


                }

                else
                {
                    MessageBox.Show("Enter data correctly");
                }

            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message.ToString());

            }
        }

        private void btn_del_Click(object sender, RoutedEventArgs e)
        {
            if (cmb_dr_short_name.SelectedItem != null && CMB_CRACC.SelectedItem != null)
            {

                int.TryParse(txt_jno.Text.ToString(), out int jno);
                var rr = DB.MonthlyPayroll.Remove(jno);
                if (rr == true)
                {
                    MessageBox.Show("Payroll Removed");
                    btn_Reset_Click(sender, e);

                }
                else
                {
                    MessageBox.Show("Something went wrong");
                }


            }


        }

        private void btn_report_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
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

        private void btn_movefirst_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (p_nos.Count > 0)
                {
                    int pno = p_nos[0];
                    cindex = 0;
                    Find(pno);
                }


                else
                {
                    MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                MessageBox.Show(perror.Message.ToString());
            }

        }

        private void btn_moveprevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (p_nos.Count > 0 && cindex >= 0 && cindex < p_nos.Count)
                {
                    if (cindex != 0) { cindex--; }

                    Find(p_nos[cindex]);
                }


                else
                {
                    MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                MessageBox.Show(perror.Message.ToString());
            }
        }

        private void btn_movenext_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (p_nos.Count > 0 && cindex < p_nos.Count - 1)
                {

                    cindex++;

                    Find(p_nos[cindex]);
                }


                else
                {
                    MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                MessageBox.Show(perror.Message.ToString());
            }
        }

        private void btn_movelast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (p_nos.Count > 0)
                {
                    int pno = p_nos[p_nos.Count - 1];
                    cindex = p_nos.Count - 1;
                    Find(pno);
                }


                else
                {
                    MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                MessageBox.Show(perror.Message.ToString());
            }
        }

        private void btn_find_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txt_pnofind_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txt_cramount_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MessageBoxResult res = new MessageBoxResult();
                res = MessageBox.Show("Do you want close this Window", "Close the Window", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
        }



        private void txt_cramount_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txt_cramount_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txt_narration_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    TabToSave();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmb_dr_short_name_KeyDown(object sender, KeyEventArgs e)
        {

            int drid;
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (cmb_dr_short_name.SelectedItem != null)
                    {
                        var row = ViewModels_Variables.ModelViews.Accounts.Where((acc) => acc.ID == ((Model.EmployeeModel)cmb_dr_short_name.SelectedItem).Account.ID).FirstOrDefault();
                        if (row != null)
                        {
                            drid = row.ID;
                            drLimit = row.DrLimit;
                            var emp = ViewModels_Variables.ModelViews.Employees.Where((em) => em.Emp_Id == txt_eid.Text.ToString()).FirstOrDefault();
                            if (emp != null)
                            {
                                txt_basicpay.Text = string.Format("{0:0.00}", emp.Basic);
                            }

                            gdrLimit = row.Parent.Dr_Loc;
                            string infotxt;
                            infotxt = row.Name;
                            if (crLimit > 0)
                            {
                                infotxt += " Cr.Lock : " + drLimit;
                            }
                            else if (gcrLimit > 0)
                            {
                                infotxt += " Cr.Lock : " + gdrLimit;
                            }
                            var ob = DB.Connection.GetActBalance(drid);
                            if (ob > 0) { infotxt += ", OB: " + ob; }
                            txt_info.Text = infotxt;
                        }
                    }
                }
                public_members._TabPress(e);

            }
            catch (Exception err)
            {

                MessageBox.Show(err.Message.ToString());
            }
        }

        private void cmb_dr_short_name_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void dtp_jdate_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txt_jnofind_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dtp_jdate_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txt_dramount_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dtp_jdate_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void CMB_CRACC_KeyDown(object sender, KeyEventArgs e)
        {

            int drid;
            try
            {

                if (e.Key == Key.Enter)
                {
                    if (CMB_CRACC.SelectedItem != null)
                    {

                        var slctd = (Model.AccountModel)CMB_CRACC.SelectedItem;
                        var row = ViewModels_Variables.ModelViews.Accounts.Where((s) => s.ID == slctd.ID).FirstOrDefault();
                        if (row != null)
                        {
                            drid = row.ID;
                            crLimit = row.CrLimit;

                            var p = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.ID == row.ParentGroup).FirstOrDefault();
                            if (p != null)
                            {
                                gcrLimit = p.Cr_Loc;
                            }
                            string infotxt;
                            infotxt = row.Name;
                            if (crLimit > 0)
                            {
                                infotxt += " Cr.Lock : " + crLimit;
                            }
                            else if (gcrLimit > 0)
                            {
                                infotxt += " Cr.Lock : " + gcrLimit;
                            }

                            var ob = DB.Connection.GetActBalance(drid, "");

                            if (ob > 0) { infotxt += ", OB: " + ob; }
                            txt_info.Text = infotxt;
                        }
                    }



                }
                public_members._TabPress(e);
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void CMB_CRACC_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void lst_tasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lst_tasks.SelectedItem != null)
                {
                    var t = (Model.Task)lst_tasks.SelectedItem;
                    MessageBoxResult res1 = new MessageBoxResult();
                    res1 = MessageBox.Show("Do you want to execute this task", "Task", MessageBoxButton.YesNo);
                    if (res1 == MessageBoxResult.Yes)
                    {
                        if (t != null)
                        {
                            var res = DB.MonthlyPayroll.Tasker(t, dtp_jdate.SelectedDate.Value);
                            if (res == true)
                            {
                                MessageBox.Show("Task completed");

                                btn_Reset_Click(sender, e);
                            }
                        }
                    }
                }
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        void check_onPosting()
        {
            try
            {
                int.TryParse(txt_eid.Text.ToString(), out int eeid);
                var allows = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid && p.VoucherType == "Allowance" select p.Amount).Sum();
                var comms = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid && p.VoucherType == "Commission" select p.Amount).Sum();
                var advs = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid && p.VoucherType == "Advance" select p.Amount).Sum();
                var deds = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid && p.VoucherType == "Deduction" select p.Amount).Sum();
                var wages = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid && p.VoucherType == "Wage" select p.Amount).Sum();

                if (allows > 0) all_flag = true;
                if (comms > 0) comm_flag = true;
                if (advs > 0) adv_flag = true;
                if (deds > 0) ded_flag = true;
                flag[0] = all_flag;
                flag[1] = comm_flag;
                flag[2] = adv_flag;
                flag[3] = ded_flag;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void btn_generate_payroll_Click(object sender, RoutedEventArgs e)
        {
            all_flag = false; comm_flag = false; adv_flag = false; ded_flag = false;
            try
            {
                int.TryParse(txt_eid.Text.ToString(), out int eeid);
                if (txt_eid.Text != null && txt_eid.Text.Length > 0 && eeid > 0)
                {

                    var isposted = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid select p.DrAccount.Account.ID).Count();
                    if (isposted > 0)
                    {
                        var isgernerated = (from p in ViewModels_Variables.ModelViews.Payrolls where p.DATE.Month == dtp_jdate.SelectedDate.Value.Month && p.DATE.Year == dtp_jdate.SelectedDate.Value.Year && p.EID == eeid select p.VNO).ToList<int>();
                        if (isgernerated.Count > 0)
                        {
                            MessageBox.Show("Payroll already generated ");
                            Find(isgernerated[0]);
                        }
                        else
                        {
                            var allows = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid && p.VoucherType == "Allowance" select p.Amount).Sum();
                            var comms = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid && p.VoucherType == "Commission" select p.Amount).Sum();
                            var advs = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid && p.VoucherType == "Advance" select p.Amount).Sum();
                            var deds = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid && p.VoucherType == "Deduction" select p.Amount).Sum();
                            var wages = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid && p.VoucherType == "Wage" select p.Amount).Sum();

                            if (allows > 0) all_flag = true;
                            if (comms > 0) comm_flag = true;
                            if (advs > 0) adv_flag = true;
                            if (deds > 0) ded_flag = true;

                            flag[0] = all_flag;
                            flag[1] = comm_flag;
                            flag[2] = adv_flag;
                            flag[3] = ded_flag;

                            txt_allowances.Text = string.Format("{0:0.00}", allows);
                            txt_advancepaid.Text = string.Format("{0:0.00}", advs);
                            txt_commission.Text = string.Format("{0:0.00}", comms);
                            txt_deductions.Text = string.Format("{0:0.00}", deds);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No Payroll posting found");
                    }
                }

                txt_basicpay.Focus();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }

        private void txt_eid_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txt_basicpay_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txt_basicpay_KeyDown_1(object sender, KeyEventArgs e)
        {

            try
            {
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txt_allowances_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int.TryParse(txt_eid.Text.ToString(), out int eeid);
                if (txt_eid.Text != null && txt_eid.Text.Length > 0 && eeid > 0)
                {
                    var allow_list = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid && p.VoucherType == "Allowance" select new { amount = p.Amount, eno = p.VNO, edate = p.PPDate }).ToList();


                    hist_task_lbl.Content = "ALLOWANCES PAID";
                    DataTemplate commtemp = FindResource("tr_History5") as DataTemplate;
                    lst_tasks.ItemsSource = allow_list;
                    lst_tasks.ItemTemplate = commtemp;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }

        private void txt_commission_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int.TryParse(txt_eid.Text.ToString(), out int eeid);
                if (txt_eid.Text != null && txt_eid.Text.Length > 0 && eeid > 0)
                {
                    var comm_list = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid && p.VoucherType == "Commission" select new { amount = p.Amount, eno = p.VNO, edate = p.PPDate }).ToList();

                    hist_task_lbl.Content = "COMMISSION PAID";
                    DataTemplate commtemp = FindResource("tr_History5") as DataTemplate;
                    lst_tasks.ItemsSource = comm_list;
                    lst_tasks.ItemTemplate = commtemp;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }

        private void txt_advancepaid_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int.TryParse(txt_eid.Text.ToString(), out int eeid);
                if (txt_eid.Text != null && txt_eid.Text.Length > 0 && eeid > 0)
                {
                    var adv_list = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid && p.VoucherType == "Advance" select new { amount = p.Amount, eno = p.VNO, edate = p.PPDate }).ToList();

                    hist_task_lbl.Content = "ADVANCES GRANTED";
                    DataTemplate commtemp = FindResource("tr_History5") as DataTemplate;
                    lst_tasks.ItemsSource = adv_list;
                    lst_tasks.ItemTemplate = commtemp;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }

        private void txt_deductions_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int.TryParse(txt_eid.Text.ToString(), out int eeid);
                if (txt_eid.Text != null && txt_eid.Text.Length > 0 && eeid > 0)
                {
                    var ded_list = (from p in ViewModels_Variables.ModelViews.PayrollVouchers where p.PPDate.Month == dtp_jdate.SelectedDate.Value.Month && p.PPDate.Year == dtp_jdate.SelectedDate.Value.Year && p.DrAccount.Account.ID == eeid && p.VoucherType == "Deductions" select new { amount = p.Amount, eno = p.VNO, edate = p.PPDate }).ToList();

                    hist_task_lbl.Content = "DEDUCTIONS";
                    DataTemplate commtemp = FindResource("tr_History5") as DataTemplate;
                    lst_tasks.ItemsSource = ded_list;
                    lst_tasks.ItemTemplate = commtemp;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }

        private void btn_dr_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CMB_CRACC.SelectedItem != null)
                {
                    var id = ((Model.AccountModel)CMB_CRACC.SelectedItem).ID;
                    if (id > 0)
                    {
                        CashBook1 cashsow = new CashBook1(id);
                        cashsow.Show();
                    }
                    else
                    {
                        MessageBox.Show("Not a valid account");
                    }
                }
                else
                {
                    MessageBox.Show("Not a valid account");
                }
            }
            catch (Exception eee)
            {
                MessageBox.Show(eee.Message.ToString());
            }
        }

        private void btn_voucher_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmb_dr_short_name.Text.Length > 0)
                {


                    payrollreport cashsow = new payrollreport(cmb_dr_short_name.Text.ToString());
                    cashsow.Show();

                }
                else
                {
                    MessageBox.Show("Not a valid account");
                }
            }
            catch (Exception eee)
            {
                MessageBox.Show(eee.Message.ToString());
            }
        }

        private void btn_report_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                payrollreport rp = new payrollreport();
                rp.Owner = this;
                rp.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wordFname == null)
                {
                    wordFname = CreateWordDoc();
                }

                if (wordFname != null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }
                if (xpsFname != null)
                {
                    XpsDocument xps = new XpsDocument(xpsFname, FileAccess.Read);
                    accounts.PrintDialogue print = new PrintDialogue(xps, this.Title + " " + txt_jno.Text.ToString());
                    print.Show();
                    xps.Close();
                }
                 

            }
            catch (Exception w1)
            {

                MessageBox.Show(w1.Message.ToString());
            }
        }
        string CreateWordDoc()
        {
            string fileName = null;
            try
            {
                fileName = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\doc", ".doc");
                using (var document = DocX.Create(fileName))
                {
                    // Add a title
                    document.ApplyTemplate(@"DocTemplates\MonthlyPayRoll.dotx");

                    //Footer
                    document.ReplaceText("[MyCompany]", ViewModels_Variables.ModelViews.CompanyProfile[0].company);
                    document.ReplaceText("[Clandmark]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].lmark);
                    document.ReplaceText("[CCity]", " | " + ViewModels_Variables.ModelViews.CompanyProfile[0].place);
                    document.ReplaceText("[CPhone]", "Phone :" + ViewModels_Variables.ModelViews.CompanyProfile[0].officeno);

                    //  customer Information
                    var cust = ((Model.EmployeeModel)cmb_dr_short_name.SelectedItem).Account;
                    if (cust != null)
                    {
                        document.ReplaceText("[Employee]", cust.Name);
                        document.ReplaceText("[Address]", cust.Address);
                        document.ReplaceText("[City]", cust.City);
                        document.ReplaceText("[Phone]", cust.PhoneNo);

                    }

                    //Employee Info
                    document.ReplaceText("[EID]", txt_eid.Text.ToString());
                    document.ReplaceText("[VNo]", txt_jno.Text.ToString());
                    document.ReplaceText("[Date]", dtp_jdate.SelectedDate.Value.ToShortDateString());
                    string jinv = null;
                    document.ReplaceText("[Allowance]", txt_allowances.Text.ToString());
                    document.ReplaceText("[Basic]", txt_basicpay.Text.ToString());
                    document.ReplaceText("[Commission]", txt_commission.Text.ToString());
                    document.ReplaceText("[Advance]", txt_advancepaid.Text.ToString());
                    document.ReplaceText("[Deduction]", txt_deductions.Text.ToString());

                    double d, ad, al, co;
                    double.TryParse(txt_deductions.Text.ToString(), out d);
                    double.TryParse(txt_advancepaid.Text.ToString(), out ad);
                    double.TryParse(txt_allowances.Text.ToString(), out al);
                    double.TryParse(txt_commission.Text.ToString(), out co);
                    document.ReplaceText("[Allowance+Commission]", string.Format("{0:0.00}", (co + al)));
                    document.ReplaceText("[Deduction+Advance]", string.Format("{0:0.00}", (d + ad)));
                    document.ReplaceText("[Total]", txt_total.Text.ToString());

                    string narration = null;
                    if (txt_narration.Text != null && txt_narration.Text.Length > 0) { narration = txt_narration.Text.ToString(); } else { narration = ""; }
                    document.ReplaceText("[Narration]", "Note:" + narration);
                    document.Save();
                }
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
            return fileName;
        }
        private void btn_doc_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (wordFname == null)
                {
                    wordFname = CreateWordDoc();
                }

                if (wordFname != null) Process.Start("winword.exe", wordFname);


            }
            catch (Exception w1)
            {

                MessageBox.Show(w1.Message.ToString());
            }
        }

        private void btn_pdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (wordFname == null)
                {
                    wordFname = CreateWordDoc();
                }

                if (wordFname != null)
                {
                    pdfFname = ReportExport.WordExport.CreatePDF(wordFname);
                }
                if (pdfFname != null) Process.Start("AcroRd32.exe", pdfFname);


            }
            catch (Exception w1)
            {

                MessageBox.Show(w1.Message.ToString());
            }
        }

        private void btn_xps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wordFname == null)
                {
                    wordFname = CreateWordDoc();
                }

                if (wordFname != null)
                {
                    xpsFname = ReportExport.WordExport.CreateXPS(wordFname);
                }
                if(xpsFname!=null)Process.Start("explorer.exe", xpsFname);


            }
            catch (Exception w1)
            {

                MessageBox.Show(w1.Message.ToString());
            }
        }

        private void chk_isrecurr_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_task_add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btn_save.IsEnabled == false && chk_isrecurr.IsChecked == true)
                {
                    Model.Task task = new Model.Task();
                    int.TryParse(txt_jno.Text.ToString(), out int t);
                    decimal.TryParse(txt_task_Amount.Text.ToString(), out decimal a);
                    task.T_LABEL = txt_task_label.Text.ToString();
                    task.T_AMOUNT = a;
                    task.ENO = t;
                    task.ENTRY = "PAYROLL";
                    var r = DB.Task.AddTask(task);
                    if (r > 0)
                    {
                        MessageBox.Show("Task Added");
                        btn_Reset_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Some thing went wrong");
                    }

                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void btn_task_del_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btn_save.IsEnabled == false)
                {
                    int.TryParse(txt_jno.Text.ToString(), out int t);
                    var t1 = ViewModels_Variables.ModelViews.Tasks.Where((ts) => ts.ENO == t && ts.ENTRY == "PAYROLL").FirstOrDefault();
                    if (t1 != null)
                    {
                        var tt = DB.Task.DeleteTask(eno: t1.ENO, entry: "PAYROLL");
                        if (tt == true)
                        {
                            MessageBox.Show("Task removed");
                            btn_Reset_Click(sender, e);
                        }
                    }
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void chk_isrecurr_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chk_isrecurr.IsChecked == true)
                {
                    var cr = (Model.EmployeeModel)cmb_dr_short_name.SelectedItem;
                    if (cr != null)
                    {
                        txt_task_label.Text = cr.Account.Name;
                        txt_task_Amount.Text = txt_basicpay.Text.ToString();
                    }
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void cmb_dr_short_name_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var slctd = (Model.EmployeeModel)cmb_dr_short_name.SelectedItem;
                if (slctd != null)
                {
                    txt_eid.Text = slctd.Id.ToString();
                    txt_basicpay.Text = slctd.Basic.ToString("0.00");
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void CMB_CRACC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int drid;
            try
            {
                if (CMB_CRACC.SelectedItem != null)
                {

                    var slctd = (Model.AccountModel)CMB_CRACC.SelectedItem;
                    var row = ViewModels_Variables.ModelViews.Accounts.Where((s) => s.ID == slctd.ID).FirstOrDefault();
                    if (row != null)
                    {
                        drid = row.ID;
                        crLimit = row.CrLimit;

                        var p = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.ID == row.ParentGroup).FirstOrDefault();
                        if (p != null)
                        {
                            gcrLimit = p.Cr_Loc;
                        }
                        string infotxt;
                        infotxt = row.Name;
                        if (crLimit > 0)
                        {
                            infotxt += " Cr.Lock : " + crLimit;
                        }
                        else if (gcrLimit > 0)
                        {
                            infotxt += " Cr.Lock : " + gcrLimit;
                        }

                        var ob = DB.Connection.GetActBalance(drid, "");

                        if (ob > 0) { infotxt += ", OB: " + ob; }
                        txt_info.Text = infotxt;
                    }
                }
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

                //if (ViewModels_Variables.ModelViews.Accounts.Count <= 0)
                //{
                //    DB.Accounts.Fetch();
                //    ViewModels_Variables.ModelViews.AccountToCollection();
                //}
                //if (ViewModels_Variables.ModelViews.Employees.Count <= 0)
                //{
                //    DB.Employee.Fetch();
                //    ViewModels_Variables.ModelViews.Employees_To_List();
                //}
                //if (ViewModels_Variables.ModelViews.PayrollVouchers.Count <= 0)
                //{
                //    DB.PayrollVoucher.Fetch();
                //    ViewModels_Variables.ModelViews.PayRolls_To_List();
                //}
                //if (ViewModels_Variables.ModelViews.Payrolls.Count <= 0)
                //{
                //    DB.MonthlyPayroll.Fetch();
                //    ViewModels_Variables.ModelViews.MonthlyPayRolls_To_List();
                //}

                var v = ViewModels_Variables.ModelViews;
                DataContext = v;
                NewButtonState();
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void txt_allowances_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {

                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btn_emp_reg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EmployeeRegistration emp = new EmployeeRegistration();
                emp.Owner = this;
                emp.Show();
            }
            catch (Exception err)
            {

                MessageBox.Show(err.Message.ToString());
            }
        }

        private void btn_posting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                payroll_payments_deductions rollp = new payroll_payments_deductions();
                rollp.Owner = this;
                rollp.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        private void txt_commission_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txt_advancepaid_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btn_generate_payroll_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txt_deductions_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                public_members._TabPress(e);
                SumUp();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void txt_wh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MessageBoxResult res = new MessageBoxResult();
                res = MessageBox.Show("Do you want close this Window", "Close the Window", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
        }
    }
}
