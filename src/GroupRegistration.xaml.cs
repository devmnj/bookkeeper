using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

namespace accounts
{
    /// <summary>
    /// Interaction logic for GroupRegistration.xaml
    /// </summary>
    public partial class GroupRegistration : Window, IDataErrorInfo, INotifyPropertyChanged, CommonActions
    {
        int cid = 0;
        SqlConnection con = new SqlConnection();
        List<int> _ids = new List<int>();
        public event PropertyChangedEventHandler PropertyChanged;

        public string gName
        {
            get { return _gName; }
            set
            {
                _gName = value;
                OnPropertyChanged("gName");
            }
        }
        //public int SelectedIndex { get; set; } 
        public string dPercentage
        {
            get
            {

                return _dPercentage;
            }
            set
            {

                _dPercentage = value;
                OnPropertyChanged("dPercentage");
            }

        }
        public string drLock
        {
            get { return _drLock; }
            set { _drLock = value; OnPropertyChanged("drLock"); }
        }
        public string crLock
        {
            get { return _crLock; }
            set { _crLock = value; OnPropertyChanged("crLock"); }
        }

        //public string this[string columnName] => throw new NotImplementedException(); 
        public string NumberValidator(string t)
        {
            double v;
            try
            {
                v = Convert.ToDouble(t);
            }
            catch (Exception)
            {
                return "Value must be numberic";

            }
            if (v < 0) { return "Value must be + ve"; }
            else { return string.Empty; }


        }
        public string this[string columnName]
        {
            get
            {
                string result = String.Empty;
                double p;


                if (columnName == "dPercentage")

                {
                    //double t;
                    //double.TryParse(dPercentage, out t);
                    //if (t < 0)
                    //{
                    result = NumberValidator(dPercentage);

                    //}
                }
                else if (columnName == "drLock")
                {
                    result = NumberValidator(drLock);
                }
                else if (columnName == "crLock")
                {
                    result = NumberValidator(crLock);
                }
                else if (columnName == "gName")
                {
                    try
                    {
                        if (gName.Length <= 0 || string.IsNullOrEmpty(gName))
                        {
                            result = "Group Name can't be blank";
                        }
                    }
                    catch (Exception)
                    {
                        result = "Group Name can't be blank";
                    }


                }


                return result;
            }
        }
        public string Error
        {
            get { return string.Empty; }
        }




        public GroupRegistration()
        {
            //this.DataContext = this;
            InitializeComponent();
            
            NewButtonState();


        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            //Edit Group
            int.TryParse(txt_code.Text.ToString(), out int code);
            if (txt_gname.Text != null && txt_gname.Text.ToString().Trim().Length > 0 && code>0)

                
                {
                    try
                    {
                             Model.GroupModel groupModel = new Model.GroupModel();

                          
                            groupModel.Name = txt_gname.Text.ToString().ToUpper();
                    groupModel.ID = code;
                            long id = 0;
                            DataTable dt;
                            int pid = 0;

                            if (sub_list.SelectedItem != null && sub_list.Items.Count > 0)
                            {
                                pid = ((Model.GroupModel)sub_list.SelectedItem).ID;
                            }

                            groupModel.P_ID = pid;

                            double crlock, drlock, max;
                            double.TryParse(txt_crlock.Text.ToString(), out crlock);
                            groupModel.Cr_Loc = crlock;
                            double.TryParse(txt_drlock.Text.ToString(), out drlock);
                            groupModel.Dr_Loc = drlock;
                            double.TryParse(txt_disc.Text.ToString(), out max);
                            groupModel.Max_Disc = max;
                            groupModel.Description = txt_narration.Text.ToString();
                            if (cmb_catagory.Text != null && cmb_catagory.Text.ToString().Length > 0)
                            {
                                groupModel.Catagory = cmb_catagory.Text.ToUpper().ToString();
                            }
                            else
                            {
                                groupModel.Catagory = "None";
                            }

                            var re = DB.AccountGroup.Update(groupModel);
                            if (re == true)
                            {
                                System.Windows.Forms.MessageBox.Show("Group information Updated");
                                btn_reset_Click(sender, e);
                            }
                            else
                            {
                                System.Windows.Forms.MessageBox.Show("Something Went wrong");
                            }
                       

                    }
                    catch (SqlException exc)
                    {
                        System.Windows.Forms.MessageBox.Show(exc.Message.ToString());
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Something Went wrong");
                }
        
    }
 
        void FindButtonState()
        {
            btn_save.IsEnabled = false;
            btn_update.IsEnabled = true;
            btn_delete.IsEnabled = true;
            txt_gname.Focus();

        }
        void NewButtonState()
        {
            btn_save.IsEnabled = true;
            btn_update.IsEnabled = false;
            btn_delete.IsEnabled = false;
            txt_gname.Focus();
            cmb_catagory.Text = "None";

            

        }
        private void btn_reset_Click(object sender, EventArgs e)
        {
            txt_code.Text = "";
            dPercentage = "0.00";
            drLock = "0.00";
            crLock = "0.00";
            txt_gname.Text = "";
            cmb_catagory.Text = "";
            
            NewButtonState();
            txt_gname.Focus();
        }
        public void Find(int id)
        {
            try
            {
                NewButtonState();
                var row = ViewModels_Variables.ModelViews.AccountGroups.Where((ag) => ag.ID == id  ).FirstOrDefault();
                
                if (row!=null)
                {
                    string name = null;

                    var parent = ViewModels_Variables.ModelViews.AccountGroups.Where((ag) => ag.ID == row.P_ID).FirstOrDefault();
                    if (parent != null) { sub_list.SelectedItem = parent; }
                    else
                    {
                        MessageBox.Show("Parent Missing");
                    }
                    txt_gname.SelectedItem = row;
                    txt_crlock.Text = row.Cr_Loc.ToString("0.00");
                    txt_drlock.Text = row.Dr_Loc.ToString("0.00"); 
                    txt_disc.Text = row.Max_Disc.ToString("0.00");
                    txt_code.Text = row.ID.ToString();
                    txt_narration.Text = row.Description;
                    cmb_catagory.Text = row.Catagory;
                    FindButtonState();
                }
                else
                {
                    MessageBox.Show("Group not found !");
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        private void txt_gname_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            
            if (e.Key == Key.Enter && txt_gname.Text.ToString().Trim().Length >= 1 && btn_save.IsEnabled == true)
            {

                var sel = (Model.GroupModel)txt_gname.SelectedItem;
                if (sel != null)
                {
                    Find(sel.ID);
                }
                

            }
            public_members._TabPress(e);
        }
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var context = ViewModels_Variables.ModelViews;
            DataContext = context;
            _ids = (from id in context.AccountGroups.AsEnumerable() where id.P_ID != 0  select id.ID).ToList<int>();
        }



        private void btn_del_Click(object sender, EventArgs e)
        {
            if (txt_gname.Text.ToString().Trim().Length > 0 && txt_code.Text.Length>0 && btn_save.IsEnabled==false)
            {

                int.TryParse(txt_code.Text.ToString(), out int code);

                var rows = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.ID == code).FirstOrDefault();
                if (rows==null)
                {
                    System.Windows.MessageBox.Show("Group Not found");
                }
                else
                {
                    var isr = DB.AccountGroup.Remove(code);
                    if(isr == true){
                        MessageBox.Show("Group removed");
                        btn_reset_Click(sender, e);
                    }
                }
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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



        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        private string _dPercentage = "0.00";
        private string _drLock = "0.00";
        private string _crLock = "0.00";
        private string _gName;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(dPercentage);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this.gName);
        }

        private void sub_list_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void txt_crlock_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void txt_drlock_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void txt_disc_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void sub_list_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        void CommonActions.NewButtonState()
        {
            throw new NotImplementedException();
        }

        void CommonActions.FindButtonState()
        {
            throw new NotImplementedException();
        }

        public void TabToSave()
        {
            if (btn_save.IsEnabled == false)
            {
                btn_update.Focus();
            }
            else
            {
                btn_save.Focus();
            }


        }

        private void btn_report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GroupList groupList = new GroupList();
                groupList.Owner = this.Owner; groupList.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txt_narration_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TabToSave();
            }
        }

        private void sub_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void txt_gname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var sel = (Model.GroupModel)txt_gname.SelectedItem;
                if (sel != null)
                {
                    Find(sel.ID);
                }

            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }

        private void cmb_catagory_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            txt_code.Text = "";
            dPercentage = "0.00";
            drLock = "0.00";
            crLock = "0.00";
            txt_gname.Text = "";
            cmb_catagory.Text = "";
            txt_narration.Text = "";
            _ids = (from id in ViewModels_Variables.ModelViews.AccountGroups.AsEnumerable() where id.P_ID!=0 select id.ID ).ToList<int>();
            NewButtonState();
            txt_gname.Focus();
        }

        private void btn_movenext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_ids.Count > 0 && cid < _ids.Count - 1)
                {
                    cid++;
                    Find(_ids[cid]);
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
                if (_ids.Count > 0 && cid >= 0 && cid < _ids.Count)
                {
                    if (cid != 0) { cid--; }
                    Find(_ids[cid]);
                }
                else
                {
                    MessageBox.Show("No entey found");
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }

        private void btn_movefirst_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (_ids.Count > 0)
                {
                    int pno = _ids[0];
                    cid = 0;
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

        private void btn_movelast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_ids.Count > 0)
                {
                    int pno = _ids[_ids.Count - 1];
                    cid = _ids.Count - 1;
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

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {

            //Save Group
            if (txt_gname.Text != null && txt_gname.Text.ToString().Trim().Length > 0)
            {
                try
                {
                    var rows1 = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.Name == txt_gname.Text.ToString().ToUpper().Trim()).FirstOrDefault();

                    if (rows1 != null)
                    {
                        MessageBox.Show("Group already Registerd");
                    }
                    else
                    {

                        Model.GroupModel groupModel = new Model.GroupModel();


                        groupModel.Name = txt_gname.Text.ToString().ToUpper();

                        long id = 0;
                        DataTable dt;
                        int pid = 0;

                        if (sub_list.SelectedItem != null && sub_list.Items.Count > 0)
                        {
                            pid = ((Model.GroupModel)sub_list.SelectedItem).ID;
                        }

                        groupModel.P_ID = pid;

                        double crlock, drlock, max;
                        double.TryParse(txt_crlock.Text.ToString(), out crlock);
                        groupModel.Cr_Loc = crlock;
                        double.TryParse(txt_drlock.Text.ToString(), out drlock);
                        groupModel.Dr_Loc = drlock;
                        double.TryParse(txt_disc.Text.ToString(), out max);
                        groupModel.Max_Disc = max;
                        groupModel.Description = txt_narration.Text.ToString();
                        if (cmb_catagory.Text != null && cmb_catagory.Text.ToString().Length>0)
                        {
                            groupModel.Catagory = cmb_catagory.Text.ToUpper().ToString();
                        }
                        else
                        {
                            groupModel.Catagory = "None";
                        }

                        var re = DB.AccountGroup.Save(groupModel);
                        if (re > 0)
                        {
                            System.Windows.Forms.MessageBox.Show("Group information saved");
                            btn_reset_Click(sender, e);
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("Something Went wrong");
                        }
                    }

                }
                catch (SqlException exc)
                {
                    System.Windows.Forms.MessageBox.Show(exc.Message.ToString());
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Something Went wrong");
            }
        }

        private void txt_gname_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
    class CustomConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // throw new NotImplementedException();
            double t;
            //t= double.Parse(value.ToString().Trim() );
            return int.Parse(value.ToString());
        }
    }
    class StringToDoubleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double d;
            if (double.TryParse(value.ToString(), out d))
                return new ValidationResult(true, null);
            return new ValidationResult(false, "Please enter valid % value");
        }
    }

}
