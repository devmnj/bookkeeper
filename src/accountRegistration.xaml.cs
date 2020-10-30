using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace accounts
{
    /// <summary>
    /// Interaction logic for accountRegistration.xaml
    /// </summary>
    public partial class accountRegistration : Window, IDataErrorInfo, INotifyPropertyChanged
    {
        SqlConnection con = new SqlConnection();

        int cindex = 0;
        List<int> rnos = new List<int>();
        public event PropertyChangedEventHandler PropertyChanged;

        public string Error
        {
            get { return string.Empty; }
        }



        private string _acName = null;
        private string _acAddress;
        private string _acCity;
        private string _acMob;
        private string _acResi;
        private string _acCrLimit = "0.00";
        private string _acDrLimit = "0.00";
        private string _acDisc = "0.00";
        private string _acCatagory = "A";
        private string _acSName = "A/c alias here";


        public string acSName
        {
            get { return _acSName; }
            set
            {
                _acSName = value;
                OnPropertyChanged("acSName");
            }
        }

        public string acResi
        {
            get { return _acResi; }
            set { _acResi = value; OnPropertyChanged("acResi"); }
        }
        public string acName
        {
            set { _acName = value; OnPropertyChanged("acName"); }
            get { return _acName; }
        }
        public string acCity
        {
            get { return _acCity; }
            set { _acCity = value; OnPropertyChanged("acCity"); }
        }
        public string acAddress
        {
            get { return _acAddress; }
            set { _acAddress = value; OnPropertyChanged("acAddress"); }
        }
        public string acMob
        {
            get { return _acMob; }
            set { _acMob = value; OnPropertyChanged("acMob"); }
        }
        public string acCrLimit
        {
            get { return _acCrLimit; }
            set { _acCrLimit = value; OnPropertyChanged("acCrLimit"); }
        }
        public string acDrLimit
        {
            get { return _acDrLimit; }
            set { _acDrLimit = value; OnPropertyChanged("acDrLimit"); }
        }

        public string acDisc
        {
            get { return _acDisc; }
            set { _acDisc = value; OnPropertyChanged("acDisc"); }
        }
        public string acCatagory
        {
            get { return _acCatagory; }
            set { _acCatagory = value; OnPropertyChanged("acCatagory"); }
        }
        public string this[string columnName]
        {
            get
            {
                string result = String.Empty;
                switch (columnName)
                {
                    case "acName":
                        if (string.IsNullOrWhiteSpace(acName) || string.IsNullOrEmpty(acName) || acName.Trim().Length <= 0 || acName.Trim().Length > 0 && acName.Trim().Length < 2)
                        {
                            result = "Can't be balnk [2-100 Maximum length]";
                        }
                        else if (public_members.NumberValidator(acName) == true)
                        {
                            result = "Should begin with an Alphabet";
                        }
                        break;
                    case "acAddress":

                        break;
                    case "acCatagory":
                        result = "Optional catagory";
                        break;
                    case "acResi":
                        if (string.IsNullOrEmpty(acResi) == false && string.IsNullOrWhiteSpace(acResi) == false && acResi.Length > 1)
                        {
                            if (public_members.NumberValidator(acResi) == false) { result = "It should be a valid Phone number"; }
                        }
                        break;

                    case "acCity":
                        if (string.IsNullOrWhiteSpace(acCity) || string.IsNullOrEmpty(acCity) || acCity.Trim().Length <= 0 || acCity.Trim().Length > 0 && acCity.Trim().Length < 2)
                        {
                            result = "Can't be balnk [2-50 Maximum length]";
                        }
                        else if (public_members.NumberValidator(acCity) == true)
                        {
                            result = "Should begin with an Alphabet";
                        }

                        break;
                    case "acMob":
                        if (string.IsNullOrWhiteSpace(acMob) || string.IsNullOrEmpty(acMob) || acMob.Trim().Length <= 0 || acMob.Trim().Length > 0 && acMob.Trim().Length < 2)
                        {
                            result = "Mob No Can't be blank";
                        }
                        else
                        {
                            result = "Mob No Can't be blank";
                        }
                        break;
                    case "acCrLimit":
                        result = public_members.NumberValidator(acCrLimit, 0);
                        break;
                    case "acDrLimit":
                        result = public_members.NumberValidator(acDrLimit, 0);
                        break;
                    case "acDisc":
                        result = public_members.NumberValidator(acDisc, 0);
                        break;
                    case "acSName":
                        if (string.IsNullOrWhiteSpace(acSName) || string.IsNullOrEmpty(acSName) || acSName.Trim().Length <= 0 || acSName.Trim().Length > 0 && acSName.Trim().Length < 2)
                        {
                            result = "Can't be balnk [3-20 Maximum length]-Make it short-";
                        }
                        else if (public_members.NumberValidator(acSName) == true)
                        {
                            result = "Should begin with an Alphabet";
                        }
                        break;

                }
                return result;
            }
        }



        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public accountRegistration()
        {
            InitializeComponent();

            NewButtonState();



        }
        public accountRegistration(int id)
        {
            InitializeComponent();
            Find(id);
             



        }
        void Refresh()
        {
            btn_save.IsEnabled = true;
            btn_update.IsEnabled = false;
            btn_delete.IsEnabled = false;
            acAddress = null;
            acCatagory = "A";
            txt_code.Text = "";
            acName = null;
            acCity = null;
            acCrLimit = "0.00";
            acDrLimit = "0.00";
            acDisc = "0.00";
            acMob = null;
            acResi = null;

            rnos = (from p in ViewModels_Variables.ModelViews.Accounts select p.ID).ToList<int>();
            //loadledgers();
            cmb_shortname.Focus();

        }
        void NewButtonState()
        {
            btn_save.IsEnabled = true;
            btn_update.IsEnabled = false;
            btn_delete.IsEnabled = false;
            acAddress = null;
            acCatagory = "A";
            acName = null;
            acSName = null;
            acCity = null;
            acCrLimit = "0.00";
            acDrLimit = "0.00";
            acDisc = "0.00";
            acMob = null;
            acResi = null;

            rnos = (from p in ViewModels_Variables.ModelViews.Accounts select p.ID).ToList();
            //loadledgers();
            if (lst_sub.Items.Count > 0) lst_sub.SelectedIndex = 0;
            cmb_shortname.Focus();

        }
        void FindButtonState()
        {
            btn_save.IsEnabled = false;
            btn_update.IsEnabled = true;
            btn_delete.IsEnabled = true;
            cmb_shortname.Focus();

        }
        public void Find(int sid)
        {
            var rows = ViewModels_Variables.ModelViews.Accounts.Where((a) => a.ID == sid).FirstOrDefault();
            if (rows != null)
            {
                FindButtonState();
                var parent = ViewModels_Variables.ModelViews.AccountGroups.Where((p) => p.ID == rows.ParentGroup).FirstOrDefault();


                if (parent != null)
                {

                    lst_sub.SelectedItem = parent;

                }
                else
                {
                    System.Windows.MessageBox.Show("Parent Missing");
                }

                cmb_shortname.SelectedItem = rows;
                txt_crlock.Text = rows.CrLimit.ToString("0.00");
                txt_drlock.Text = rows.DrLimit.ToString("0.00");

                txt_disc.Text = rows.MaxDisc.ToString("0.00");
                txt_code.Text = rows.ID.ToString();
                txt_shopname.Text = rows.Name;
                txt_address.Text = rows.Address;
                txt_city.Text = rows.City;
                txt_mobileno.Text = rows.Mob;
                txt_resi.Text = rows.PhoneNo;
                cmb_catagory.Text = rows.Catagory;

                FindButtonState();
            }

        }
        private void txt_name_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {


            if (cmb_shortname.Text != null && e.Key == Key.Enter && cmb_shortname.Text.ToString().Trim().Length >= 1 && btn_save.IsEnabled == true)
            {
                //Refresh();
                var rows = (Model.AccountModel)cmb_shortname.SelectedItem;

                if (rows != null)
                {
                    NewButtonState();

                    var parent = ViewModels_Variables.ModelViews.AccountGroups.Where((p) => p.ID == rows.ParentGroup).FirstOrDefault();


                    if (parent != null)
                    {

                        lst_sub.SelectedItem = parent;

                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Parent Missing");
                    }


                    txt_crlock.Text = rows.CrLimit.ToString("0.00");
                    txt_drlock.Text = rows.DrLimit.ToString("0.00");

                    txt_disc.Text = rows.MaxDisc.ToString("0.00");
                    txt_code.Text = rows.ID.ToString();
                    txt_shopname.Text = rows.Name;
                    txt_address.Text = rows.Address;
                    txt_city.Text = rows.City;
                    txt_mobileno.Text = rows.Mob;
                    txt_resi.Text = rows.PhoneNo;
                    cmb_catagory.Text = rows.Catagory;

                    FindButtonState();
                }
            }
            public_members._TabPress(e);

        }
        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            //Edit
            int.TryParse(txt_code.Text.ToString(), out int code);
            var slctd = ViewModels_Variables.ModelViews.Accounts.Where((a) => a.ID == code).FirstOrDefault();
            if (cmb_shortname.Text != null && txt_shopname.Text != null & cmb_shortname.Text.Trim().Length > 0 && txt_shopname.Text.Trim().ToString().Length > 0  && lst_sub.SelectedItem!=null && slctd!=null)
            {
                try
                {

                    MessageBoxResult res = new MessageBoxResult();
                    res = System.Windows.MessageBox.Show("Do you want Edit this Account", "Update Account", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        Model.AccountModel accountModel = slctd;
                        
                        accountModel.Name = txt_shopname.Text.ToUpper().Trim();
                        accountModel.Short_Name = cmb_shortname.Text.ToString().Trim().ToUpper();
                        accountModel.Catagory = cmb_catagory.Text.ToString().ToUpper();
                        accountModel.Address = txt_address.Text.ToString().ToUpper();
                        accountModel.City = txt_city.Text.ToString().ToUpper();
                        accountModel.Mob = txt_mobileno.Text.ToString();
                        accountModel.PhoneNo = txt_resi.Text.ToString();

                        var parent = (Model.GroupModel)lst_sub.SelectedItem;

                        if (parent != null)
                        {
                            accountModel.ParentGroup = parent.ID; 
                            accountModel.Parent = parent;
                        }
                        else
                        {
                            accountModel.ParentGroup = 1;
                        }

                        double disc, due, crlk, drlk, cram, dram;
                        double.TryParse(txt_crlock.Text.ToString(), out crlk);
                        double.TryParse(txt_drlock.Text.ToString(), out drlk);
                        double.TryParse(txt_disc.Text.ToString(), out disc);

                        accountModel.CrLimit = crlk;
                        accountModel.DrLimit = drlk;
                        accountModel.MaxDisc = disc;

                        var rr = DB.Accounts.Update(accountModel);
                        if (rr == true)
                        {
                            System.Windows.MessageBox.Show("Account Updated successfully");
                            btn_reset_Click(sender, e);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Something Went Wrong");
                        }
                    }
                }
                catch (Exception e1)
                {
                    System.Windows.MessageBox.Show(e1.Message.ToString());
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Enter data correctly");
            }
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {

            txt_code.Text = "";
            cmb_shortname.Text = "";
            txt_shopname.Text = "";
            //txt_due.Text = "";
            cmb_catagory.Text = "";
            //txt_cr.Text = "";
            //txt_dr.Text = "";
            txt_crlock.Text = "";
            txt_drlock.Text = "";
            txt_address.Text = "";
            txt_city.Text = "";
            txt_mobileno.Text = ""; //loadledgers(); 
            NewButtonState();
            cmb_shortname.Focus();
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (cmb_shortname.Text != null && txt_shopname.Text != null & cmb_shortname.Text.Trim().Length > 0 && txt_shopname.Text.Trim().ToString().Length > 0 && lst_sub.SelectedItem != null)
            {
                try
                {
                    Model.AccountModel accountModel = new Model.AccountModel();
                    accountModel.Name = txt_shopname.Text.ToUpper().Trim();
                    accountModel.Short_Name = cmb_shortname.Text.ToString().Trim().ToUpper();
                    accountModel.Catagory = cmb_catagory.Text.ToString().ToUpper();
                    accountModel.Address = txt_address.Text.ToString().ToUpper();
                    accountModel.City = txt_city.Text.ToString().ToUpper();
                    accountModel.Mob = txt_mobileno.Text.ToString();
                    accountModel.PhoneNo = txt_resi.Text.ToString();
                    var parent = (Model.GroupModel)lst_sub.SelectedItem;
                    if (parent != null)
                    {
                        accountModel.ParentGroup = parent.ID;
                        accountModel.Parent = parent;
                    }
                    else
                    {
                        accountModel.ParentGroup = 1;
                    }

                    double disc, due, crlk, drlk, cram, dram;
                    double.TryParse(txt_crlock.Text.ToString(), out crlk);
                    double.TryParse(txt_drlock.Text.ToString(), out drlk);
                    double.TryParse(txt_disc.Text.ToString(), out disc);

                    accountModel.CrLimit = crlk;
                    accountModel.DrLimit = drlk;
                    accountModel.MaxDisc = disc;

                    var rr = DB.Accounts.Save(accountModel);
                    if (rr > 0)
                    {
                        System.Windows.MessageBox.Show("Ledger registered successfully");
                        btn_reset_Click(sender, e);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Something Went Wrong");
                    }

                }
                catch (Exception e1)
                {
                    System.Windows.MessageBox.Show(e1.Message.ToString());
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Enter data correctly");
            }
        }

        private void btn_del_Click(object sender, RoutedEventArgs e)
        {
            //delete
            if (txt_code.Text.ToString().Trim().Length > 0)
            {
                try
                {
                    int id1;
                    int.TryParse(txt_code.Text.ToString(), out id1);
                    var r = DB.Accounts.Remove(id1);
                    if (r == true) { MessageBox.Show("Account Removed"); } else { MessageBox.Show("Entry Not Removed"); }


                }
                catch (SqlException e1)
                {
                    System.Windows.MessageBox.Show("Server Error");
                }
            }


        }






        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MessageBoxResult res = new MessageBoxResult();
                res = System.Windows.MessageBox.Show("Do you want close this Window", "Close the Window", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
        }



        private void txt_shopname_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void txt_address_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void txt_city_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void txt_mobileno_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void lst_sub_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void txt_crlock_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void txt_drlock_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void txt_disc_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        public void TabToSave()
        {

            if (btn_save.IsEnabled == true)
            {
                btn_save.Focus();
            }
            else
            {
                btn_update.Focus();
            }
        }

        private void cmb_catagory_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                TabToSave();

            }
        }





        private void btn_moveprevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (rnos.Count > 0 && cindex >= 0 && cindex < rnos.Count)
                {
                    if (cindex != 0) { cindex--; }

                    Find(rnos[cindex]);
                }


                else
                {
                    System.Windows.MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                System.Windows.MessageBox.Show(perror.Message.ToString());
            }
        }

        private void btn_movefirst_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rnos.Count > 0)
                {
                    int pno = rnos[0];
                    cindex = 0;
                    Find(pno);
                }


                else
                {
                    System.Windows.MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                System.Windows.MessageBox.Show(perror.Message.ToString());
            }
        }

        private void btn_movenext_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (rnos.Count > 0 && cindex < rnos.Count - 1)
                {

                    cindex++;

                    Find(rnos[cindex]);
                }


                else
                {
                    System.Windows.MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                System.Windows.MessageBox.Show(perror.Message.ToString());
            }
        }

        private void btn_movelast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rnos.Count > 0)
                {
                    int pno = rnos[rnos.Count - 1];
                    cindex = rnos.Count - 1;
                    Find(pno);
                }


                else
                {
                    System.Windows.MessageBox.Show("No entey found");
                }
            }
            catch (Exception perror)
            {
                System.Windows.MessageBox.Show(perror.Message.ToString());
            }

        }

        private void txt_resi_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var t = lst_sub.SelectedIndex;
                var t1 = lst_sub.Items.CurrentItem;
                lst_sub.Items.MoveCurrentTo(t);

            }
            public_members._TabPress(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                account_List acc = new account_List();
                acc.Owner = this;
                acc.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void lst_sub_GotFocus(object sender, RoutedEventArgs e)
        {
            lst_sub.Items.MoveCurrentTo(lst_sub.SelectedItem);

        }

        private void lst_sub_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            { 
                if (ViewModels_Variables.ModelViews.Accounts.Count <= 0)
                {
                    DB.Accounts.Fetch();
                    ViewModels_Variables.ModelViews.AccountToCollection();
                }

                
                DataContext=ViewModels_Variables.ModelViews;
               
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }

        }

        private void cmb_shortname_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {

                //var rows = (Model.AccountModel)cmb_shortname.SelectedItem;

                //if (rows != null)
                //{
                //    NewButtonState();

                //    var parent = ViewModels_Variables.ModelViews.AccountGroups.Where((p) => p.ID == rows.ParentGroup).FirstOrDefault();


                //    if (parent != null)
                //    {

                //        lst_sub.SelectedItem = parent;

                //    }
                //    else
                //    {
                //        System.Windows.MessageBox.Show("Parent Missing");
                //    }


                //    txt_crlock.Text = rows.CrLimit.ToString("0.00");
                //    txt_drlock.Text = rows.DrLimit.ToString("0.00");

                //    txt_disc.Text = rows.MaxDisc.ToString("0.00");
                //    txt_code.Text = rows.ID.ToString();
                //    txt_shopname.Text = rows.Name;
                //    txt_address.Text = rows.Address;
                //    txt_city.Text = rows.City;
                //    txt_mobileno.Text = rows.Mob;
                //    txt_resi.Text = rows.PhoneNo;
                //    cmb_catagory.Text = rows.Catagory;

                //    FindButtonState();
                //}
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
    }
}

