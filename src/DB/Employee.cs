using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Windows;

namespace accounts.DB
{
    static class Employee
    {
        public static DataTable EmpTable;
        public static void Fetch()
        {
            EmpTable = Connection._FetchTable("select * from emp_registration order by id");
        }
        public static int Save(Model.EmployeeModel employee)
        {
            int eid = 0;
            try
            {

                int em_gid = 0;
                var emgrp = ViewModels_Variables.ModelViews.AccountGroups.Where((g1) => g1.Name == "EMPLOYEES").FirstOrDefault();
                if (emgrp != null)
                {
                    em_gid = emgrp.ID;
                }
                else
                {
                    var asset = ViewModels_Variables.ModelViews.AccountGroups.Where((g1) => g1.Name == "ASSET").FirstOrDefault();
                    if (asset != null)
                    {
                        Model.GroupModel new_group = new Model.GroupModel();
                        new_group.Name = "EMPLOYEES";
                        new_group.Max_Disc = 0;
                        new_group.P_ID = asset.ID;
                        new_group.Catagory = "PAYABLE";
                        new_group.Cr_Loc = 0;
                        new_group.Dr_Loc = 0;
                        new_group.Description = "";
                        em_gid = DB.AccountGroup.Save(new_group);
                    }
                }

                if (ViewModels_Variables.ModelViews.Accounts.Count <= 0)
                {
                    DB.Accounts.Fetch();
                    ViewModels_Variables.ModelViews.AccountToCollection();
                }

                var g = ViewModels_Variables.ModelViews.AccountGroups.Where((gg) => gg.ID == em_gid).FirstOrDefault();
                if(g!=null)employee.Account.Parent = g;
                var accid = DB.Accounts.Save(employee.Account);
                employee.Account.ID=accid;


                var look_for_dep = ViewModels_Variables.ModelViews.Departments.Where((dp) => dp.Dep_id == employee.Department.Dep_id && employee.Department.Dep_id != 0).FirstOrDefault();
                int depid = 0;
                if (look_for_dep == null)
                {
                    depid = DB.Departments.Save(employee.Department);
                    employee.Department.Dep_id = depid;

                }
                else
                {
                    depid = look_for_dep.Dep_id;
                    //employee.Department = look_for_dep;
                }








                var con = DB.Connection.OpenConnection();
                if (con != null)
                {
                    if (accid > 0 && depid > 0)
                    {
                        //Employee Reg
                        string sql = "insert into emp_registration (eid,lid,dep_id,designation,doj,basicpay,comm,isdailywager)  values(@eid,@lid, @department,@designation,@doj,@basicpay,@comm,@isdaily)";


                        OleDbCommand cmd = new OleDbCommand(sql, con);

                        OleDbParameter eid1 = new OleDbParameter("@eid", SqlDbType.VarChar);
                        eid1.Direction = ParameterDirection.Input;
                        eid1.Value = employee.Emp_Id;


                        OleDbParameter edep = new OleDbParameter("@department", SqlDbType.Int);
                        edep.Direction = ParameterDirection.Input;
                        edep.Value = depid;

                        OleDbParameter elid = new OleDbParameter("@lid", SqlDbType.Int);
                        elid.Direction = ParameterDirection.Input;
                        elid.Value = accid;

                        OleDbParameter edesig = new OleDbParameter("@designation", SqlDbType.VarChar);
                        edesig.Direction = ParameterDirection.Input;
                        edesig.Value = employee.Desig.ToUpper();

                        OleDbParameter edoj = new OleDbParameter("@doj", SqlDbType.DateTime);
                        edoj.Direction = ParameterDirection.Input;
                        edoj.Value = employee.DOJ;

                        OleDbParameter epay = new OleDbParameter("@basicpay", SqlDbType.Float);
                        epay.Direction = ParameterDirection.Input;
                        epay.Value = employee.Basic;

                        OleDbParameter ecom = new OleDbParameter("@comm", SqlDbType.Float);
                        ecom.Direction = ParameterDirection.Input;
                        ecom.Value = employee.Comm;

                        OleDbParameter daily = new OleDbParameter("@isdaily", SqlDbType.Bit);
                        daily.Direction = ParameterDirection.Input;
                        daily.Value = employee.IsDailyWager;

                        cmd.Parameters.Add(eid1);
                        cmd.Parameters.Add(elid);
                        cmd.Parameters.Add(edep);
                        cmd.Parameters.Add(edesig);
                        cmd.Parameters.Add(edoj);
                        cmd.Parameters.Add(epay);
                        cmd.Parameters.Add(ecom);
                        cmd.Parameters.Add(daily);
                        int r = cmd.ExecuteNonQuery();
                        if (r > 0)
                        {
                            eid = Connection.NewEntryno(table: "emp_registration", field: "id", conn: con) - 1;
                            employee.Id = eid;
                            ViewModels_Variables.ModelViews.Add_Update(employee);

                        }
                        con.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Server Not Found");
                }
            }
            catch (OleDbException e1)
            {
                MessageBox.Show(e1.Message.ToString());
            }

            return eid;

        }
        public static bool Update(Model.EmployeeModel employee)
        {
            bool res = false;

            MessageBoxResult re = new MessageBoxResult();
            re = MessageBox.Show("Do you want Edit this Registration", "Update", MessageBoxButton.YesNo);
            if (re == MessageBoxResult.Yes)
            {
                try
                {

                    int em_gid = 0;
                    var emgrp = ViewModels_Variables.ModelViews.AccountGroups.Where((g2) => g2.Name == "EMPLOYEES").FirstOrDefault();
                    if (emgrp != null)
                    {
                        em_gid = emgrp.ID;
                    }
                    else
                    {
                        var asset = ViewModels_Variables.ModelViews.AccountGroups.Where((g1) => g1.Name == "ASSET").FirstOrDefault();
                        if (asset != null)
                        {
                            Model.GroupModel new_group = new Model.GroupModel();
                            new_group.Name = "EMPLOYEES";
                            new_group.Max_Disc = 0;
                            new_group.P_ID = asset.ID;
                            new_group.Catagory = "None";
                            new_group.Cr_Loc = 0;
                            new_group.Dr_Loc = 0;
                            new_group.Description = "";
                            em_gid = DB.AccountGroup.Save(new_group);
                        }
                    }

                    if (ViewModels_Variables.ModelViews.Accounts.Count <= 0)
                    {
                        DB.Accounts.Fetch();
                        ViewModels_Variables.ModelViews.AccountToCollection();
                    }



                    var g = DB.AccountGroup.Find(em_gid);
                    employee.Account.Parent = g;



                    var acc = ViewModels_Variables.ModelViews.Accounts.Where((ax) => ax.ID == employee.Account.ID).FirstOrDefault();
                    if (acc != null)
                    {

                        DB.Accounts.Update(employee.Account);
                        var id = employee.Account.ID;
                        employee.Account = DB.Accounts.Find(id);

                    }
                    else
                    {
                        int id=DB.Accounts.Save(employee.Account);                        
                        employee.Account = DB.Accounts.Find(id);
                    }


                    var look_for_dep = ViewModels_Variables.ModelViews.Departments.Where((dp) => dp.Dep_id == employee.Department.Dep_id && employee.Department.Dep_id != 0).FirstOrDefault();
                    int depid = 0;
                    if (look_for_dep == null)
                    {
                        depid = DB.Departments.Save(employee.Department);
                        employee.Department = DB.Departments.Find(depid);
                    }
                    else { depid = look_for_dep.Dep_id;
                        employee.Department = look_for_dep;
                    }

                    




                    var con = DB.Connection.OpenConnection();
                    if (con != null)
                    {
                        if (employee.Account.ID > 0 && employee.Department.Dep_id > 0)
                        {
                            //Employee Reg
                            string sql = "update emp_registration set eid=@eid,lid=@lid,dep_id=@department,designation=@designation,doj=@doj,basicpay=@basicpay,comm=@comm,isdailywager=@isdaily where id=" + employee.Id;


                            OleDbCommand cmd = new OleDbCommand(sql, con);

                            OleDbParameter eid1 = new OleDbParameter("@eid", SqlDbType.VarChar);
                            eid1.Direction = ParameterDirection.Input;
                            eid1.Value = employee.Emp_Id;


                            OleDbParameter edep = new OleDbParameter("@department", SqlDbType.Int);
                            edep.Direction = ParameterDirection.Input;
                            edep.Value = employee.Department.Dep_id;

                            OleDbParameter elid = new OleDbParameter("@lid", SqlDbType.Int);
                            elid.Direction = ParameterDirection.Input;
                            elid.Value = employee.Account.ID;

                            OleDbParameter edesig = new OleDbParameter("@designation", SqlDbType.VarChar);
                            edesig.Direction = ParameterDirection.Input;
                            edesig.Value = employee.Desig.ToUpper();

                            OleDbParameter edoj = new OleDbParameter("@doj", SqlDbType.DateTime);
                            edoj.Direction = ParameterDirection.Input;
                            edoj.Value = employee.DOJ;

                            OleDbParameter epay = new OleDbParameter("@basicpay", SqlDbType.Float);
                            epay.Direction = ParameterDirection.Input;
                            epay.Value = employee.Basic;

                            OleDbParameter ecom = new OleDbParameter("@comm", SqlDbType.Float);
                            ecom.Direction = ParameterDirection.Input;
                            ecom.Value = employee.Comm;

                            OleDbParameter daily = new OleDbParameter("@isdaily", SqlDbType.Bit);
                            daily.Direction = ParameterDirection.Input;
                            daily.Value = employee.IsDailyWager;

                            cmd.Parameters.Add(eid1);
                            cmd.Parameters.Add(elid);
                            cmd.Parameters.Add(edep);
                            cmd.Parameters.Add(edesig);
                            cmd.Parameters.Add(edoj);
                            cmd.Parameters.Add(epay);
                            cmd.Parameters.Add(ecom);
                            cmd.Parameters.Add(daily);
                            int r = cmd.ExecuteNonQuery();
                            if (r > 0)
                            {

                                ViewModels_Variables.ModelViews.Add_Update(employee);
                                res = true;
                            }
                            con.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Server Not Found");
                    }
                }
                catch (OleDbException e1)
                {
                    MessageBox.Show(e1.Message.ToString());
                }
            }

            return res;

        }
        public static bool Remove(int id)
        {
            bool e = false;

            MessageBoxResult res = new MessageBoxResult();
            res = System.Windows.MessageBox.Show("Do you want to Remove this Entry", "Remove Employee", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                try
                {


                    var emp_ = ViewModels_Variables.ModelViews.Employees.Where((em) => em.Id == id).FirstOrDefault();
                    if (emp_ != null)
                    {
                        DB.Accounts.Remove(emp_.Account.ID);
                    }
                    var con = Connection.OpenConnection();
                    if (con != null)
                    {
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "delete from emp_registration where id=" + id;
                        int r = cmd.ExecuteNonQuery();

                        if (r > 0)
                        {
                            e = true;
                            var acc = ViewModels_Variables.ModelViews.Employees.Where((a) => a.Id == id).FirstOrDefault();
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


            return e;
        }
    }
}
