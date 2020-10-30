using accounts.Model;
using System;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Windows;
namespace accounts.DB
{
    static class AccountGroup
    {
        public static int Save(Model.GroupModel groupModel)
        {
            int e = 0;
            try
            {

                string sql = "insert into groups (g_name,g_parent,g_cr_lock,g_dr_lock,g_maxdisc,g_narration,g_catagory) values(@g_name,@g_parent,@g_cr_lock,@g_dr_lock,@g_mdisc,@g_naration,@g_catagory)";
                try
                {
                    var con = Connection.OpenConnection();
                    if (con != null)
                    {
                        OleDbCommand cmd = new OleDbCommand(sql, con);


                        //Parameters
                        OleDbParameter g_name = new OleDbParameter("@g_name", OleDbType.VarChar);
                        g_name.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter g_catagory = new OleDbParameter("@g_catagory", OleDbType.VarChar);
                        g_catagory.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter g_naration = new OleDbParameter("@g_naration", OleDbType.VarChar);
                        g_naration.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter g_parent = new OleDbParameter("@g_parent", OleDbType.Integer);
                        g_parent.Direction = System.Data.ParameterDirection.Input;

                        OleDbParameter g_cr_lock = new OleDbParameter("@g_cr_lock", OleDbType.Numeric);
                        g_cr_lock.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter g_dr_lock = new OleDbParameter("@g_dr_lock", OleDbType.Numeric);
                        g_dr_lock.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter g_mdisc = new OleDbParameter("@g_mdisc", OleDbType.Numeric);
                        g_mdisc.Direction = System.Data.ParameterDirection.Input;

                        //Set values

                        g_name.Value = groupModel.Name.ToUpper();
                        if (groupModel.ParentGroup != null)
                        {
                            g_parent.Value = groupModel.ParentGroup.ID;
                        }
                        else
                        {
                            g_parent.Value =0;
                        }

                        g_cr_lock.Value = groupModel.Cr_Loc;

                        g_dr_lock.Value = groupModel.Dr_Loc;

                        g_mdisc.Value = groupModel.Max_Disc;
                        g_naration.Value = groupModel.Description;
                        g_catagory.Value = groupModel.Catagory;



                        cmd.Parameters.Add(g_name);
                        cmd.Parameters.Add(g_parent);
                        cmd.Parameters.Add(g_cr_lock);
                        cmd.Parameters.Add(g_dr_lock);
                        cmd.Parameters.Add(g_mdisc);
                        cmd.Parameters.Add(g_naration);
                        cmd.Parameters.Add(g_catagory);

                        int re = cmd.ExecuteNonQuery();
                        if (re >= 1)
                        {

                            e = Connection.NewEntryno("groups", "id", "id",conn:con) - 1;
                            groupModel.ID = e;
                            ViewModels_Variables.ModelViews.Add_Update(groupModel);


                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("Something Went wrong");
                        }

                        con.Close();


                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Server Not found");
                    }
                }
                catch (OverflowException exc)
                {
                    System.Windows.Forms.MessageBox.Show(exc.Message.ToString());
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
            return e;
        }
        public static bool Update(Model.GroupModel groupModel)
        {
            bool e = false;

            MessageBoxResult res = new MessageBoxResult();
            res = System.Windows.MessageBox.Show("Do you want Update this group", "Update Group", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {

                string sql = "update  groups set g_name=@g_name,g_parent=@g_parent,g_cr_lock=@g_cr_lock,g_dr_lock=@g_dr_lock,g_maxdisc=@g_mdisc,g_narration=@g_naration,g_catagory=@g_catagor  where id= " + groupModel.ID;
                try
                {
                    var con = Connection.OpenConnection();
                    if (con != null)
                    {
                        OleDbCommand cmd = new OleDbCommand(sql, con);


                        //Parameters
                        OleDbParameter g_name = new OleDbParameter("@g_name", OleDbType.VarChar);
                        g_name.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter g_catagory = new OleDbParameter("@g_catagory", OleDbType.VarChar);
                        g_catagory.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter g_naration = new OleDbParameter("@g_naration", OleDbType.VarChar);
                        g_naration.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter g_parent = new OleDbParameter("@g_parent", OleDbType.Integer);
                        g_parent.Direction = System.Data.ParameterDirection.Input;

                        OleDbParameter g_cr_lock = new OleDbParameter("@g_cr_lock", OleDbType.Numeric);
                        g_cr_lock.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter g_dr_lock = new OleDbParameter("@g_dr_lock", OleDbType.Numeric);
                        g_dr_lock.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter g_mdisc = new OleDbParameter("@g_mdisc", OleDbType.Numeric);
                        g_mdisc.Direction = System.Data.ParameterDirection.Input;

                        //Set values

                        g_name.Value = groupModel.Name.ToUpper();

                        g_parent.Value = groupModel.P_ID;

                        g_cr_lock.Value = groupModel.Cr_Loc;

                        g_dr_lock.Value = groupModel.Dr_Loc;

                        g_mdisc.Value = groupModel.Max_Disc;
                        g_naration.Value = groupModel.Description;
                        g_catagory.Value = groupModel.Catagory;



                        cmd.Parameters.Add(g_name);
                        cmd.Parameters.Add(g_parent);
                        cmd.Parameters.Add(g_cr_lock);
                        cmd.Parameters.Add(g_dr_lock);
                        cmd.Parameters.Add(g_mdisc);
                        cmd.Parameters.Add(g_naration);
                        cmd.Parameters.Add(g_catagory);

                        int re = cmd.ExecuteNonQuery();
                        if (re >= 1)
                        {

                            e = true;

                            ViewModels_Variables.ModelViews.Add_Update(groupModel);


                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("Something Went wrong");
                        }

                        con.Close();


                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Server Not found");
                    }
                }
                catch (OverflowException exc)
                {
                    System.Windows.Forms.MessageBox.Show(exc.Message.ToString());
                }

            }

            return e;

        }
        public static bool Remove(int id)
        {
            bool e = false;
            try
            {
                MessageBoxResult res = new MessageBoxResult();
                res = System.Windows.MessageBox.Show("Do you want Remove this Group", "Remove Grpup", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    bool flag = true;
                    var existing = ViewModels_Variables.ModelViews.AccountGroups.Where((g) => g.P_ID == id ).FirstOrDefault();
                    if (existing != null)
                    {
                        MessageBox.Show("Sub Groups found, can't Remove, remove Subs First and Try again");
                        flag = false;
                    }
                    var existing_acc = ViewModels_Variables.ModelViews.Accounts.Where((ac) => ac.ParentGroup == id);
                    if (existing != null)
                    {
                        MessageBox.Show("Accounts found, can't Remove, remove Accounts One by One and Try again");
                        flag = false;
                    }

                    var con = Connection.OpenConnection();
                    if (con != null && flag!=false)
                    {
                        OleDbCommand oleDbCommand = new OleDbCommand("delete from groups where id=" + id,con);
                        int r = oleDbCommand.ExecuteNonQuery();
                        con.Close();
                        if (r > 0)
                        {
                            e = true;
                            var g = ViewModels_Variables.ModelViews.AccountGroups.Where((g1) => g1.ID == id).FirstOrDefault();
                            if (g != null)
                            {
                                ViewModels_Variables.ModelViews.Remove(g);
                            }
                        }
                    }




                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
            return e;
        }
        public static Model.GroupModel Find(int id)
        {
            Model.GroupModel _group = new Model.GroupModel();
            try
            {
                var grp = DB.Connection._FetchTable("select * from groups where id="+id);
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
                _group = list1[0];
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
            return _group;
        }
    }
}
