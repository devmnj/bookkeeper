using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace accounts
{
    /// <summary>
    /// Interaction logic for dbconfiguration.xaml
    /// </summary>
    public partial class dbconfiguration : Window
    {
        public dbconfiguration()
        {
            InitializeComponent();
            LoadSettings();
            
            //if (txt_instance.Text != null && txt_instance.Text.Length > 0) { txt_db.ItemsSource = SqlHelper.GetDatabaseList(txt_instance.Text.ToString()); }
            this.Show();

        }

        private void btn_createDB_check_db_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txt_db.Text != null && txt_instance.Text != null)
                {
                    string app_path = AppDomain.CurrentDomain.BaseDirectory;
                    accounts.INIProfile inif = new INIProfile(app_path + "config.ini");
                    inif.Write("DB", "DB_PATH", txt_instance.Text.ToString().ToUpper());
                    inif.Write("DB", "DB", txt_db.Text.ToString().ToUpper());
                    inif.Write("DB", "USER", txt_user.Text.ToString().ToUpper());
                    inif.Write("DB", "PASS", txt_password.Text.ToString().ToUpper());
                    inif.Write("BACKGROUND", "Image", txt_backwall.Text.ToString());
                    inif.Write("BACKUP", "folderpath1", txt_backup.Text.ToString());

                    MessageBox.Show("Configuration Saved");
                }
                else
                {

                }

            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        void LoadSettings()
        {
            try
            {
                
                //var inst = SqlHelper.GetSQLInstances();

            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }

        }



        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                btn_save.Focus();
            }

        }

        private void txt_user_KeyDown(object sender, KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void txt_db_KeyDown(object sender, KeyEventArgs e)
        {
            public_members._TabPress(e);
        }

        private void txt_instance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txt_instance.Text != null && txt_instance.Text.ToString().Length > 0)
                {
                    if (Directory.Exists(txt_instance.Text.Trim()))
                    {
                        var files = Directory.GetFiles(txt_instance.Text, "*.md").ToList();
                        txt_db.ItemsSource = files;
                    }
                    else
                    {
                        MessageBox.Show("Enter a valid path");
                    }
                }

            }
            public_members._TabPress(e);
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

        private void txt_backup_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txt_backwall_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                public_members.LoadIniSettings();
                txt_instance.Text = public_members.server;
                txt_db.Text = public_members.database;
                txt_user.Text = public_members.uname;
                txt_password.Text = public_members.pas;
                txt_backwall.Text = public_members.sysbackgrouSource;
                txt_backup.Text = public_members.backuppath1;

                if (ViewModels_Variables.ModelViews.FrontPanel["MODE"] == "SINGLE_USER")
                {
                    txt_instance.IsEnabled = false;
                    txt_db.IsEnabled = false;
                    btn_createDB_check_db.IsEnabled = false;
                }

                else if (ViewModels_Variables.ModelViews.FrontPanel["MODE"] == "MULTI_USER")
                {
                    txt_instance.IsEnabled = true;
                    txt_db.IsEnabled = true;
                    btn_createDB_check_db.IsEnabled = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
