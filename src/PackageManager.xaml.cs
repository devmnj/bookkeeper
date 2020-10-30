using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace accounts
{
    /// <summary>
    /// Interaction logic for PackageManager.xaml
    /// </summary>
    public partial class PackageManager : Window
    {
        ObservableCollection<PackageClass> packList = new ObservableCollection<PackageClass>();
        PackageClass pkges = new PackageClass();
        string app_path = AppDomain.CurrentDomain.BaseDirectory;

        public PackageManager()
        {
            InitializeComponent();
            rep_grid.ItemsSource = packList;
            var files = Directory.GetFiles(app_path, "*.pk");
            cmb_files.ItemsSource = files;
        }


        private void btn_update_Click(object sender, RoutedEventArgs e)
        {

            if (txt_new_package.Text.Length > 0)
            {

                pkges = new PackageClass(txt_new_package.Text.ToString(), txt_pack_parameter.Text, txt_parameter_value.Text);

                packList.Add(pkges);
            }

        }

        private void bt_clearBoxes_Click(object sender, RoutedEventArgs e)
        {
            txt_new_package.Text = "";
            txt_pack_parameter.Text = "";
            txt_parameter_value.Text = "";
        }

        private void bt_clear_Click(object sender, RoutedEventArgs e)
        {
            var files = Directory.GetFiles(app_path, "*.pk");
            cmb_files.ItemsSource = files;
            packList.Clear();
            rep_grid.ItemsSource = packList;

        }

        private void rep_grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var cur = (PackageClass)rep_grid.CurrentItem;
            if (cur != null && cur.PName != null)
            {
                txt_new_package.Text = cur.PName;
                txt_pack_parameter.Text = cur.Parameter.ToString();
                txt_parameter_value.Text = cur.Pvalue;
            }
        }
        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (packList.Count > 0)
                {
                   
                    if (cmb_files.Text != null && cmb_files.Text.Length > 0)
                    {
                        if (cmb_files.Text != null && File.Exists(cmb_files.Text))
                        {
                            File.Delete(cmb_files.Text.ToString());
                            SerializeHelper.SerialilZe<ObservableCollection<PackageClass>>(packList, cmb_files.Text.ToString());
                        }
                        else
                        {
                            SerializeHelper.SerialilZe<ObservableCollection<PackageClass>>(packList, app_path + cmb_files.Text.ToString());
                        }
                    }
                    else
                    {
                        SerializeHelper.SerialilZe<ObservableCollection<PackageClass>>(packList, @"data.pk");
                    }

                    MessageBox.Show("Packages Saved"); bt_clear_Click(sender, e);bt_clearBoxes_Click(sender,e);
                }
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.Message.ToString());
            }
        }

        private void btn_load_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<PackageClass> list;
            try
            {
                if (cmb_files.Text != null && cmb_files.Text.Length > 0)
                {
                    list = SerializeHelper.DeserialilZe<ObservableCollection<PackageClass>>(cmb_files.Text.ToString());
                }
                else
                {
                    list = SerializeHelper.DeserialilZe<ObservableCollection<PackageClass>>(@"data.pk");
                }

                if (list.Count > 0)

                {
                    packList.Clear();
                    packList = list;
                    rep_grid.ItemsSource = list;
                    MessageBox.Show("Packages Loaded");
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void btn_update1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rep_grid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
