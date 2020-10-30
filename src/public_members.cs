using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Forms.Design;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.IO;

namespace accounts
{
    public static class SendKeys
    {
        /// <summary>
        ///   Sends the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public static void Send(Key key)
        {
            if (Keyboard.PrimaryDevice != null)
            {
                if (Keyboard.PrimaryDevice.ActiveSource != null)
                {
                    var e1 = new System.Windows.Input.KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Enter) { RoutedEvent = Keyboard.KeyDownEvent };
                    InputManager.Current.ProcessInput(e1);
                }
            }
        }
    }

    static class public_members
    {

        //public static string master_db, master_code;
        public static string server, uname, pas, database;
        public static string sysbackgrouSource = null;

        public static string backuppath1;
        public static string backuppath2;
        public static ObservableCollection<DateTime> _sysDate = new ObservableCollection<DateTime>();
        public static string app_path = Path.GetDirectoryName(Application.ExecutablePath);
        public static string licenceFile = app_path + "/license.pk";
        public static string factoryFile = app_path + "/fatctory_licence.pk";
        public static string ConfigFile = app_path + "/config.ini";
        public static string datapath = "";
        public static string reportPath = "";
        public static string backend_path = app_path +"/backend.accdb";
        public static string GnenerateFileName(string path, string ext = ".docx", string prefix = null)
        {
            Random random = new Random();
            string fname = null;
            try
            {
                if (prefix == null)
                {
                    fname = path + "\\" + random.Next(181) + ext;
                }
                else
                {
                    fname = path + "\\" + prefix + random.Next(181) + ext;
                }
                if (File.Exists(fname) == true)
                {
                    File.Delete(fname);
                    GnenerateFileName(path, ext);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return fname;
        }

        public static void GetFilepaths()
        {
            datapath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            app_path = Path.GetDirectoryName(Application.ExecutablePath);
            licenceFile = app_path + "/license.pk";
            if (File.Exists(app_path + "/fatctory_licence.pk") == true)
            {
                factoryFile = app_path + "/fatctory_licence.pk";
            }
            else
            {
                factoryFile = "";
            }
            ConfigFile = app_path + "/config.ini";
            reportPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
        public static void Accountsetup()
        {
            try
            {

                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                AutoCreateDirectory(path + "/BookKeeper");
                AutoCreateDirectory(path + "/BookKeeper/Doc");
                AutoCreateDirectory(path + "/BookKeeper/XPS");
                AutoCreateDirectory(path + "/BookKeeper/PDF");
                AutoCreateDirectory(path + "/BookKeeper/WorkBook");

                AutoCreateDirectory(reportPath + "/BookKeeper");
                AutoCreateDirectory(reportPath + "/BookKeeper/Doc");
                AutoCreateDirectory(reportPath + "/BookKeeper/XPS");
                AutoCreateDirectory(reportPath + "/BookKeeper/PDF");
                AutoCreateDirectory(reportPath + "/BookKeeper/WorkBook");

                public_members.RefreshAll();

                List<Model.GroupModel> ind_base_groups = new List<Model.GroupModel>()
                {
                    new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="ASSET"},
                    new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="LIABILITY"},
                    new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="INCOME"},
                    new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="EXPENSE"},
                };




                List<Model.GroupModel> commercial_base_groups = new List<Model.GroupModel>()
                {
                    //base groups
                    new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="ASSET"},
                    new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="LIABILITY"},
                    new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="INCOME"},
                    new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="EXPENSE"},
                    new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="CAPITAL"},
                    new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="PROFIT"},
                };
                //sub groups
                //ASSETS


                //Necessary Account
                if (ViewModels_Variables.ModelViews._front_panel["AUTO ACCOUNTS"] == "TRUE")
                {
                    if (ViewModels_Variables.ModelViews._front_panel["IS COMMERCIAL ACCOUNT"] == "TRUE")
                    {
                        foreach (var g in commercial_base_groups)
                        {
                            var g1 = ViewModels_Variables.ModelViews.AccountGroups.Where((ag) => ag.Name == g.Name).FirstOrDefault();
                            if (g1 == null)
                            {
                                DB.AccountGroup.Save(g);
                            }
                        }
                        List<Model.GroupModel> commercial_groups = new List<Model.GroupModel>()
                {
                new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="CASH IN HAND",ParentGroup=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="ASSET").FirstOrDefault()},
                     new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="FIXED ASSETES",ParentGroup=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="ASSET").FirstOrDefault()},
                     new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="TANGIBLE ASSETS",ParentGroup=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="ASSET").FirstOrDefault()},
                     new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="BANK ACCOUNT",ParentGroup=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="ASSET").FirstOrDefault()},
                     //EXPENSES
                       new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="DIRECT EXPENSE",ParentGroup=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="EXPENSE").FirstOrDefault()},
                       new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="INDIRECT EXPENSE",ParentGroup=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="EXPENSE").FirstOrDefault()},
                    //INCOME
                       new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="DIRECT INCOME",ParentGroup=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="INCOME").FirstOrDefault()},
                       new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="INDIRECT INCOM",ParentGroup=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="INCOME").FirstOrDefault()},
                };

                        foreach (var g in commercial_groups)
                        {
                            var g1 = ViewModels_Variables.ModelViews.AccountGroups.Where((ag) => ag.Name == g.Name).FirstOrDefault();
                            if (g1 == null)
                            {
                                DB.AccountGroup.Save(g);
                            }
                        }

                        List<Model.AccountModel> comm_accouts = new List<Model.AccountModel>() {
                new Model.AccountModel(){Short_Name="CASH",Name="CASH IN HAND",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="CASH IN HAND").FirstOrDefault()},
                new Model.AccountModel(){Short_Name="BANK",Name="BANK ACCOUNT",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="BANK ACCOUNT").FirstOrDefault()},
                new Model.AccountModel(){Short_Name="INCOME 1",Name="INCOME SOURCE",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="INCOME").FirstOrDefault()},
                new Model.AccountModel(){Short_Name="SALARY",Name="SALARY ACCOUNT",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="INCOME").FirstOrDefault()},
                new Model.AccountModel(){Short_Name="INSURANCE",Name="INSURANCE POLICY PAYMENT",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="EXPENSE").FirstOrDefault()},
                new Model.AccountModel(){Short_Name="LOAN REPAYMENT",Name="LOAN PAYMENT",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="EXPENSE").FirstOrDefault()},
                new Model.AccountModel(){Short_Name="BILLS",Name="BILLS PAYMENT",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="EXPENSE").FirstOrDefault()},
                new Model.AccountModel(){Short_Name="RENT",Name="RENT PAYMENT",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="EXPENSE").FirstOrDefault()},
                };

                        foreach (var ac in comm_accouts)
                        {
                            var a1 = ViewModels_Variables.ModelViews.Accounts.Where((a) => a.Short_Name == ac.Short_Name).FirstOrDefault();
                            if (a1 == null)
                            {
                                DB.Accounts.Save(ac);
                            }
                        }

                    }

                    else if (ViewModels_Variables.ModelViews._front_panel["IS INDIVIDUAL ACCOUNT"] == "TRUE")
                    {
                        foreach (var g in ind_base_groups)
                        {
                            var g1 = ViewModels_Variables.ModelViews.AccountGroups.Where((ag) => ag.Name == g.Name).FirstOrDefault();
                            if (g1 == null)
                            {
                                DB.AccountGroup.Save(g);
                            }
                        }

                        List<Model.GroupModel> individual_groups = new List<Model.GroupModel>()
                {

                    new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="CASH IN HAND",ParentGroup=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="ASSET").FirstOrDefault()},
                    new Model.GroupModel(){Description="",Catagory="",Max_Disc=0,Dr_Loc=0,Cr_Loc=0,Name="BANK ACCOUNT",ParentGroup= ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="ASSET").FirstOrDefault()},

                };
                        foreach (var g in individual_groups)
                        {
                            var g1 = ViewModels_Variables.ModelViews.AccountGroups.Where((ag) => ag.Name == g.Name).FirstOrDefault();
                            if (g1 == null)
                            {
                                DB.AccountGroup.Save(g);
                            }
                        }

                        List<Model.AccountModel> individual_accouts = new List<Model.AccountModel>() {
                new Model.AccountModel(){Short_Name="CASH",Name="CASH IN HAND",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="CASH IN HAND").FirstOrDefault()},
                new Model.AccountModel(){Short_Name="BANK",Name="BANK ACCOUNT",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="BANK ACCOUNT").FirstOrDefault()},
                new Model.AccountModel(){Short_Name="INCOME 1",Name="INCOME SOURCE",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="INCOME").FirstOrDefault()},
                new Model.AccountModel(){Short_Name="SALARY",Name="SALARY ACCOUNT",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="INCOME").FirstOrDefault()},
                new Model.AccountModel(){Short_Name="INSURANCE",Name="INSURANCE POLICY PAYMENT",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="EXPENSE").FirstOrDefault()},
                new Model.AccountModel(){Short_Name="LOAN REPAYMENT",Name="LOAN PAYMENT",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="EXPENSE").FirstOrDefault()},
                new Model.AccountModel(){Short_Name="BILLS",Name="BILLS PAYMENT",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="EXPENSE").FirstOrDefault()},
                new Model.AccountModel(){Short_Name="HOUSE RENT",Name="HOUESE RENT PAYMENT",Parent=ViewModels_Variables.ModelViews.AccountGroups.Where((gn)=>gn.Name=="EXPENSE").FirstOrDefault()},
                };

                        foreach (var ac in individual_accouts)
                        {
                            var a1 = ViewModels_Variables.ModelViews.Accounts.Where((a) => a.Short_Name == ac.Short_Name).FirstOrDefault();
                            if (a1 == null)
                            {
                                DB.Accounts.Save(ac);
                            }
                        }

                    }
                }


            }
            catch (Exception ee)
            {

                MessageBox.Show(ee.Message.ToString());
            }
        }
        public static bool NumberValidator(string t)
        {
            double v;
            try
            {
                v = Convert.ToDouble(t);
            }
            catch (Exception)
            {
                return false;

            }
            if (v < 0) { return true; }
            else { return true; }


        }
        public static string NumberValidator(string t, int t1 = 0)
        {
            double v;
            try
            {
                v = Convert.ToDouble(t);
            }
            catch (Exception)
            {
                return " Should be numberic";

            }
            if (v < 0) { return " Should be + ve"; }
            else { return string.Empty; }


        }
        public static string AccountValidattor(string sn)
        {
            string r = null;
            if (string.IsNullOrWhiteSpace(sn) || string.IsNullOrEmpty(sn) || sn.Trim().Length <= 0 || sn.Trim().Length > 0 && sn.Trim().Length < 2)
            {
                r = "Please choose a valid account";

            }
            else if (public_members.NumberValidator(sn) == true)
            {
                r = "Should begin with an Alphabet";
            }
            else if (string.IsNullOrWhiteSpace(sn) == false && string.IsNullOrEmpty(sn) == false)
            {
                //int id = public_members.Ledgerid(sn);
                //if (id == 0) { r = "Account doesn't exist"; }
            }
            return r;
        }
        public static void Delete_reseed(string tb, string fld, long fldvalue, long seedNo)
        {
            //SqlCommand cmd = new SqlCommand();
            //try
            //{
            //    _sql_con = _OpenConnection();
            //    if (_sql_con != null)
            //    {
            //        cmd.Connection = _sql_con;
            //        cmd.CommandText = "delete from " + tb + " where " + fld + " = " + fldvalue;
            //        cmd.ExecuteNonQuery();



            //        //cmd.CommandText = "IF EXISTS(SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[" + tb + constr + "]') AND type = 'D') BEGIN ALTER TABLE " + tb + " DROP CONSTRAINT[" + tb + constr + "] END ";
            //        //cmd.ExecuteNonQuery();

            //        cmd.CommandText = "DBCC CHECKIDENT(" + tb + ", RESEED," + (seedNo - 1) + ")";
            //        cmd.ExecuteNonQuery();

            //        //cmd.CommandText = "IF NOT EXISTS(SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[" + tb + constr + "]') AND type = 'D') BEGIN ALTER TABLE " + tb + " ADD CONSTRAINT[" + tb + constr + "]" + "  PRIMARY KEY (" + fld + ")  END ";
            //        //cmd.CommandText = "IF  EXISTS (SELECT * FROM sysobjects WHERE Name = '" + tb + "')  ALTER TABLE " + tb + " ADD CONSTRAINT " + constr + tb + "  PRIMARY KEY (" + fld + ")";
            //        // cmd.ExecuteNonQuery();

            //    }
            //}
            //catch (SqlException e)
            //{
            //    MessageBox.Show(e.Message.ToString());
            //}
        }
        public static void AutoCreateDirectory(string dpath)
        {
            try
            {
                if (!Directory.Exists(dpath))
                {
                    Directory.CreateDirectory(dpath);
                }
            }
            catch (Exception ee)
            {

                MessageBox.Show(ee.Message.ToString());
            }

        }
        public static bool LoadIniSettings()
        {
            bool r = false;
            try
            {
                public_members.GetFilepaths();
                //app_path = Path.GetDirectoryName(Application.ExecutablePath);
                //ConfigFile = app_path + "/config.ini";
                accounts.INIProfile inif = new INIProfile(ConfigFile);
                public_members.server = inif.Read("DB", "DB_PATH");
                //if (db == null) db = master_db;
                public_members.database = inif.Read("DB", "DB");
                public_members.uname = inif.Read("DB", "USER");
                public_members.pas = inif.Read("DB", "PASS");
                public_members.sysbackgrouSource = inif.Read("BACKGROUND", "image");
                public_members.backuppath1 = inif.Read("BACKUP", "folderpath1");

            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
            return r;
        }
        public static void _TabPress(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                System.Windows.Input.KeyEventArgs e1 = new System.Windows.Input.KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource,
                0, Key.Tab);
                e1.RoutedEvent = Keyboard.KeyDownEvent;
                InputManager.Current.ProcessInput(e1);
            }
        }
        public static bool RefreshAll()
        {
            bool res = false;
            try
            {
                ViewModels_Variables.ModelViews.RefreshCompany();
                ViewModels_Variables.ModelViews.GroupsToCollection();
                ViewModels_Variables.ModelViews.AccountToCollection();
                ViewModels_Variables.ModelViews.Trans_To_list();
                ViewModels_Variables.ModelViews.Journals_To_list();
                ViewModels_Variables.ModelViews.Reciepts_To_List();
                ViewModels_Variables.ModelViews.Payments_To_List();
                ViewModels_Variables.ModelViews.BPayments_To_List();
                ViewModels_Variables.ModelViews.BReceipts_To_List();
                ViewModels_Variables.ModelViews.Employees_To_List();
                ViewModels_Variables.ModelViews.PayRolls_To_List();
                ViewModels_Variables.ModelViews.MonthlyPayRolls_To_List();
                ViewModels_Variables.ModelViews.RefreshCashBook();
                ViewModels_Variables.ModelViews.RefreshDaybook();
                ViewModels_Variables.ModelViews.RefreshGroupBook();
                ViewModels_Variables.ModelViews.Refresh_FrontPanelItems();
                ViewModels_Variables.ModelViews.TasksToList();
                res = true;
            }
            catch (Exception er)
            {
                res = false;
                MessageBox.Show(er.Message.ToString());
            }
            return res;
        }


    }

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
    public class BindableToolStripStatusLabel : ToolStripStatusLabel, IBindableComponent
    {
        private BindingContext _context = null;
        public BindingContext BindingContext
        {
            get
            {
                if (null == _context)
                {
                    _context = new BindingContext();
                }

                return _context;
            }
            set { _context = value; }
        }

        private ControlBindingsCollection _bindings;

        public ControlBindingsCollection DataBindings
        {
            get
            {
                if (null == _bindings)
                {
                    _bindings = new ControlBindingsCollection(this);
                }
                return _bindings;
            }
            set { _bindings = value; }
        }
    }




}
