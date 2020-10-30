using System;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Windows;
using accounts.Model;

namespace accounts.DB
{
    static class Accounts
    {
        public static DataTable AccountTable;
        public static void Fetch()
        {
            AccountTable = Connection._FetchTable("select * from ledgers order by id");
        }
        public static Model.AccountModel Find(int id,OleDbConnection con=null)
        {
            Model.AccountModel _ac = new Model.AccountModel();
            try
            {
                var acc = Connection._FetchTable("select * from ledgers where id=" + id);
                var list = (from ac in acc.AsEnumerable()
                            join g in ViewModels_Variables.ModelViews.AccountGroups on ac.Field<int>("l_parent") equals g.ID
                            select new
                            {
                                obj = new AccountModel()
                                {
                                    Address = ac.Field<string>("l_address"),
                                    Catagory = ac.Field<string>("l_Catagory"),
                                    City = ac.Field<string>("l_City"),
                                    ParentGroup = ac.Field<int>("l_parent"),
                                    Name = ac.Field<string>("l_name"),
                                    Parent = g,

                                    Short_Name = ac.Field<string>("l_short_name"),
                                    Mob = ac.Field<string>("l_mob"),
                                    PhoneNo = ac.Field<string>("l_resi"),
                                    ID = ac.Field<int>("id"),
                                    //Balance = DB.Connection.GetActBalance(ac.Field<int>("id"), "")
                                }
                            }.obj).ToList<AccountModel>();
                _ac = list[0];
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
            return _ac;
        }
        public static int Save(Model.AccountModel accountModel)
        {
            int e = 0;
            try
            {
                var con = Connection.OpenConnection();
                if (con != null && accountModel.Parent!=null)
                {

                    OleDbCommand cmd = new OleDbCommand("insert into ledgers (l_name,l_short_name,l_parent,l_address,l_city,l_mob,l_resi,l_dr_lock,l_cr_lock,l_maxdisc,l_catagory" +
                        ") values(@l_name,@l_short_name,@l_parent,@l_address,@l_city,@l_mob,@l_resi,@l_dr_lock,@l_cr_lock,@l_mdisc,@l_catagory)", con);

                    OleDbParameter lname = new OleDbParameter("@l_name", OleDbType.VarChar);
                    lname.Direction = System.Data.ParameterDirection.Input;
                    lname.Value = accountModel.Name;

                    OleDbParameter short_name = new OleDbParameter("@l_short_name", OleDbType.VarChar);
                    short_name.Direction = System.Data.ParameterDirection.Input;
                    short_name.Value = accountModel.Short_Name;

                    OleDbParameter group = new OleDbParameter("@l_parent", OleDbType.Integer);
                    group.Direction = System.Data.ParameterDirection.Input;
                    group.Value = accountModel.Parent.ID;

                    OleDbParameter address = new OleDbParameter("@l_address", OleDbType.VarChar);
                    address.Direction = System.Data.ParameterDirection.Input;
                    address.Value = accountModel.Address;

                    OleDbParameter city = new OleDbParameter("@l_city", OleDbType.VarChar);
                    city.Direction = System.Data.ParameterDirection.Input;
                    city.Value = accountModel.City;

                    OleDbParameter mob = new OleDbParameter("@l_mob", OleDbType.VarChar);
                    mob.Direction = System.Data.ParameterDirection.Input;
                    mob.Value = accountModel.Mob;

                    OleDbParameter resi = new OleDbParameter("@l_resi", OleDbType.VarChar);
                    resi.Direction = System.Data.ParameterDirection.Input;
                    resi.Value = accountModel.PhoneNo;

                    OleDbParameter drlock = new OleDbParameter("@l_dr_lock", OleDbType.Decimal);
                    drlock.Direction = System.Data.ParameterDirection.Input;
                    drlock.Value = accountModel.DrLimit;

                    OleDbParameter crlock = new OleDbParameter("@l_cr_lock", OleDbType.Decimal);
                    crlock.Direction = System.Data.ParameterDirection.Input;
                    crlock.Value = accountModel.CrLimit;

                    OleDbParameter mdisc = new OleDbParameter("@l_mdisc", OleDbType.Decimal);
                    mdisc.Direction = System.Data.ParameterDirection.Input;
                    mdisc.Value = accountModel.MaxDisc;

                    OleDbParameter cat = new OleDbParameter("@l_catagory", OleDbType.VarChar);
                    cat.Direction = System.Data.ParameterDirection.Input;
                    cat.Value = accountModel.Catagory;

                    cmd.Parameters.Add(lname);
                    cmd.Parameters.Add(short_name);
                    cmd.Parameters.Add(group);
                    cmd.Parameters.Add(address);
                    cmd.Parameters.Add(city);
                    cmd.Parameters.Add(mob);
                    cmd.Parameters.Add(resi);
                    cmd.Parameters.Add(crlock);
                    cmd.Parameters.Add(drlock);
                    cmd.Parameters.Add(mdisc);
                    cmd.Parameters.Add(cat);

                    int r = cmd.ExecuteNonQuery();
                    if (r > 0)
                    {
                        e = Connection.NewEntryno("ledgers", "id", conn: con) - 1;
                        accountModel.ID = e;
                        ViewModels_Variables.ModelViews.Add_Update(accountModel);


                    }
                    con.Close();
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
            return e;
        }

        public static bool Update(Model.AccountModel accountModel)
        {
            bool re = false;
            try
            {
                var con = Connection.OpenConnection();
                if (con != null && accountModel != null)
                {

                    OleDbCommand cmd = new OleDbCommand("update  ledgers set l_name=@l_name, l_short_name=@l_short_name, l_parent=@l_parent, l_address=@l_address, l_city=@l_city, l_mob=@l_mob, l_resi=@l_resi, l_dr_lock=@l_dr_lock, l_cr_lock=@l_cr_lock, l_maxdisc=@l_mdisc, l_catagory=@l_catagory where id=" + accountModel.ID, con);

                    OleDbParameter lname = new OleDbParameter("@l_name", OleDbType.VarChar);
                    lname.Direction = System.Data.ParameterDirection.Input;
                    lname.Value = accountModel.Name;

                    OleDbParameter short_name = new OleDbParameter("@l_short_name", OleDbType.VarChar);
                    short_name.Direction = System.Data.ParameterDirection.Input;
                    short_name.Value = accountModel.Short_Name;

                    OleDbParameter group = new OleDbParameter("@l_parent", OleDbType.Integer);
                    group.Direction = System.Data.ParameterDirection.Input;
                    group.Value = accountModel.Parent.ID;

                    OleDbParameter address = new OleDbParameter("@l_address", OleDbType.VarChar);
                    address.Direction = System.Data.ParameterDirection.Input;
                    address.Value = accountModel.Address;

                    OleDbParameter city = new OleDbParameter("@l_city", OleDbType.VarChar);
                    city.Direction = System.Data.ParameterDirection.Input;
                    city.Value = accountModel.City;

                    OleDbParameter mob = new OleDbParameter("@l_mob", OleDbType.VarChar);
                    mob.Direction = System.Data.ParameterDirection.Input;
                    mob.Value = accountModel.Mob;

                    OleDbParameter resi = new OleDbParameter("@l_resi", OleDbType.VarChar);
                    resi.Direction = System.Data.ParameterDirection.Input;
                    resi.Value = accountModel.PhoneNo;

                    OleDbParameter drlock = new OleDbParameter("@l_dr_lock", OleDbType.Decimal);
                    drlock.Direction = System.Data.ParameterDirection.Input;
                    drlock.Value = accountModel.DrLimit;

                    OleDbParameter crlock = new OleDbParameter("@l_cr_lock", OleDbType.Decimal);
                    crlock.Direction = System.Data.ParameterDirection.Input;
                    crlock.Value = accountModel.CrLimit;

                    OleDbParameter mdisc = new OleDbParameter("@l_mdisc", OleDbType.Decimal);
                    mdisc.Direction = System.Data.ParameterDirection.Input;
                    mdisc.Value = accountModel.MaxDisc;

                    OleDbParameter cat = new OleDbParameter("@l_catagory", OleDbType.VarChar);
                    cat.Direction = System.Data.ParameterDirection.Input;
                    cat.Value = accountModel.Catagory;

                    cmd.Parameters.Add(lname);
                    cmd.Parameters.Add(short_name);
                    cmd.Parameters.Add(group);
                    cmd.Parameters.Add(address);
                    cmd.Parameters.Add(city);
                    cmd.Parameters.Add(mob);
                    cmd.Parameters.Add(resi);
                    cmd.Parameters.Add(crlock);
                    cmd.Parameters.Add(drlock);
                    cmd.Parameters.Add(mdisc);
                    cmd.Parameters.Add(cat);

                    int r = cmd.ExecuteNonQuery();
                    if (r > 0)
                    {

                        ViewModels_Variables.ModelViews.Add_Update(accountModel);
                        con.Close();
                        re = true;
                    }
                }
            }



            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
            return re;
        }

        public static bool Remove(int id)
        {
            bool e = false;
            try
            {
                MessageBoxResult res = new MessageBoxResult();
                res = System.Windows.MessageBox.Show("Do you want to Delete this Account", "Remove Account", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    try
                    {
                        var con = Connection.OpenConnection();
                        if (con != null)
                        {
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;
                            cmd.CommandText = "delete from ledgers where id=" + id;
                            int r = cmd.ExecuteNonQuery();
                            if (r > 0)
                            {
                                var acc = ViewModels_Variables.ModelViews.Accounts.Where((a) => a.ID == id).FirstOrDefault();
                                if (acc != null) ViewModels_Variables.ModelViews.Remove(acc);
                                e = true;

                            }
                        }
                    }
                    catch (OleDbException e1)
                    {
                        System.Windows.MessageBox.Show("Server Error");
                    }
                }
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
            return e;
        }
    }
}
