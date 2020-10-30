using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace accounts
{
    /// <summary>
    /// Interaction logic for MiniReportPage.xaml
    /// </summary>
    public partial class MiniReportPage : Page
    {
        SqlConnection con = new SqlConnection();
        DataTable journals = new DataTable();
        DataTable ledgers = new DataTable();
        public MiniReportPage()
        {
            InitializeComponent();
            loadledgers();

           

        }
        void loadledgers(bool autfill = true)
        {
            con = public_members._OpenConnection();
            if (con != null)
            {
                ledgers = public_members._Fetch("select * from ledgers order by id", con);
                con.Close();
                if (ledgers.Rows.Count > 0 && autfill == true)
                {
                    var auttext = from g in ledgers.AsEnumerable() select g.Field<string>("l_short_name");
                    cmb_account.DataContext = auttext.ToList();

                }
            }
            else
            {
                System.Windows.MessageBox.Show("Server Not found");
            }
        }

        private void cmb_account_KeyDown(object sender, KeyEventArgs e)
        {
            //DataTable rep, ob, footer = new DataTable();
            //if (e.Key == Key.Enter && cmb_account.Text.Length > 0)
            //{
            //    long id = public_members.Ledgerid(cmb_account.Text);
            //    con = public_members._OpenConnection();
            //    if (con != null)
            //    {

            //        var acid = public_members.Ledgerid(cmb_account.Text.ToString());
            //        if (acid > 0)
            //        {

            //            double dr=0, cr=0;
                        
            //            ob = public_members._Fetch("SELECT      sum(dr) as sdr,sum(cr) as scr " +
            //                "  FROM         transactions  " +
            //                " WHERE(op_led_id =" + acid + " )  ", con);
            //            if (ob.Rows.Count > 0)
            //            {

            //                double.TryParse(ob.Rows[0][0].ToString(), out dr);
            //                double.TryParse(ob.Rows[0][1].ToString(), out cr);
            //            }

            //            rep = public_members._Fetch("SELECT     transactions.t_date, transactions.entry, transactions.eno, transactions.cr, transactions.dr, " +
            //                "  ledgers.l_name as led_id FROM         transactions INNER JOIN    ledgers ON transactions.led_id = ledgers.id" +
            //                " WHERE(transactions.op_led_id =" + acid + ")  order by t_date", con);

            //            //DataRow r = rep.NewRow();
            //            //if (dr > cr)
            //            //{
            //            //    r[3] = dr - cr; rep.Rows.Add(r);

            //            //}
            //            //else if (dr < cr)
            //            //{
            //            //    r[4] = cr - dr; rep.Rows.Add(r);
            //            //}

                       


            //            footer = public_members._Fetch("select sum(dr) as sdr,sum(cr) as scr , (' ') as bal from transactions where op_led_id=" + acid, con);
            //            con.Close();
            //            repotGrid.ItemsSource = null;
            //            repotGrid.Items.Clear();
            //            repotGrid.Items.Refresh();
            //            repotGrid.ItemsSource = rep.DefaultView;
            //            dr = cr = 0;

            //            double.TryParse(footer.Rows[0][0].ToString(), out dr);
            //            double.TryParse(footer.Rows[0][1].ToString(), out cr);

            //            if (dr > cr)
            //            {
            //                footer.Rows[0][2] = (dr - cr).ToString() + " Dr";

            //            }
            //            else if (dr < cr)
            //            {
            //                footer.Rows[0][2] = cr - dr + " Cr";
            //            }
            //            else
            //            {

            //                footer.Rows[0][2] = 0.00;
            //            }


            //            listViewFooter.ItemsSource = footer.DefaultView;
            //        }


            //    }
            //    else
            //    {
            //        System.Windows.MessageBox.Show("Server Not found");
            //    }

            //}
        }

        private void btn_x_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
