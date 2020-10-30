using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace accounts
{
    /// <summary>
    /// Interaction logic for Company.xaml
    /// </summary>
    public partial class Company : Window
    {
        public Company()
        {
            InitializeComponent();
            LoadCompany();
        }
        void LoadCompany()
        {
            try
            {

                if (ViewModels_Variables.ModelViews.CompanyProfile!=null && ViewModels_Variables.ModelViews.CompanyProfile.Count> 0)
                {
                    txt_id.Text = "0";
                    txt_company.Text = ViewModels_Variables.ModelViews.CompanyProfile[0].company;
                    txt_landmark.Text = ViewModels_Variables.ModelViews.CompanyProfile[0].lmark;
                    txt_place.Text = ViewModels_Variables.ModelViews.CompanyProfile[0].place;
                    txt_street.Text = ViewModels_Variables.ModelViews.CompanyProfile[0].street;
                    txt_post.Text = ViewModels_Variables.ModelViews.CompanyProfile[0].post;
                    txt_zipcode.Text = ViewModels_Variables.ModelViews.CompanyProfile[0].zipcode;
                    txt_taxid.Text =ViewModels_Variables.ModelViews.CompanyProfile[0].TAXID;
                    txt_dlno.Text = ViewModels_Variables.ModelViews.CompanyProfile[0].DLNO;
                    txt_expno.Text = ViewModels_Variables.ModelViews.CompanyProfile[0].expno;
                    txt_email.Text = ViewModels_Variables.ModelViews.CompanyProfile[0].email;
                    txt_officeno.Text = ViewModels_Variables.ModelViews.CompanyProfile[0].officeno;
                    txt_mobile.Text = ViewModels_Variables.ModelViews.CompanyProfile[0].Mobile;
                    cal_fyear.SelectedDate = Convert.ToDateTime(ViewModels_Variables.ModelViews.CompanyProfile[0].f_date1);
                    //f1.Value = cal_fyear.SelectedDate.Value.AddDays(360); 
                }
                else
                {
                    txt_id.Text = "";
                    txt_company.Text = "";
                    txt_landmark.Text = "";
                    txt_place.Text = "";
                    txt_street.Text = "";
                    txt_post.Text = "";
                    txt_zipcode.Text = "";
                    txt_taxid.Text = "";
                    txt_dlno.Text = "";
                    txt_expno.Text = "";
                    txt_email.Text = "";
                    txt_officeno.Text = "";
                    txt_mobile.Text = "";
                    cal_fyear.SelectedDate = public_members._sysDate[0];
                }
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
                if (txt_company.Text != null && txt_company.Text.Length > 0)
                {
                    Model.SCompany comp = new Model.SCompany();

                    comp.company = txt_company.Text.ToString().ToUpper();
                    comp.lmark = txt_landmark.Text.ToString().ToString().ToUpper();
                    comp.place = txt_place.Text.ToString().ToUpper();
                    comp.street = txt_street.Text.ToString().ToUpper();
                    comp.post = txt_post.Text.ToString().ToUpper();
                    comp.zipcode = txt_zipcode.Text.ToString();
                    comp.TAXID = txt_taxid.Text.ToString().ToUpper();
                    comp.DLNO = txt_dlno.Text.ToString().ToUpper();
                    comp.expno = txt_expno.Text.ToString().ToUpper();
                    comp.email = txt_email.Text.ToString();
                    comp.officeno = txt_officeno.Text.ToString();
                    comp.Mobile = txt_mobile.Text.ToString();
                    comp.f_date1 = cal_fyear.SelectedDate.Value;
                    comp.f_date2 = cal_fyear.SelectedDate.Value.AddDays(360);

                    var r = DB.Company.SaveOrUpdate(comp);
                    if (r == true)
                    {
                        ViewModels_Variables.ModelViews.RefreshCompany();
                        MessageBox.Show("Company Information Updated");
                        ViewModels_Variables.ModelViews.RefreshCompany();

                    }

                }
            }
            catch (Exception e11)
            {

                MessageBox.Show(e11.Message.ToString());
            }
        }

        private void txt_company_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void txt_landmark_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void txt_place_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void txt_street_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void txt_post_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void txt_zipcode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void txt_taxid_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void txt_dlno_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void txt_expno_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void txt_email_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void txt_officeno_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void txt_mobile_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                public_members._TabPress(e);
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message.ToString());
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
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
            catch (Exception)
            {

                throw;
            }
        }
    }
}
