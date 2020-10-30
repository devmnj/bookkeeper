using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace accounts.DB
{
    static class Departments
    {

        public static DataTable DepTable;
        public static void Fetch()
        {
            DepTable = Connection._FetchTable("select * from department_registration order by id");
        }
        public static Model.DepartmentModel Find(int id)
        {
            Model.DepartmentModel dep = new Model.DepartmentModel();
            try
            {                
                var receipts = DB.Connection._FetchTable("select * from department_registration where id= "+id);
                var plist = (from r in receipts.AsEnumerable()
                             select new
                             {
                                 obj = new Model.DepartmentModel()
                                 {
                                     Dep_id = r.Field<int>("id"),
                                     Dep_Head = r.Field<string>("dhead"),
                                     Name = r.Field<string>("department"),
                                     Narration = r.Field<string>("narration"),
                                 }
                             }.obj
                           ).ToList<Model.DepartmentModel>();
                dep = plist[0];
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
          
            return dep;
        }
        public static int Save(Model.DepartmentModel model )
        {
            int did = 0;
            try
            {
                var cn = DB.Connection.OpenConnection();
                if (cn != null)
                {
                    string sql = "insert into department_registration (department,narration,dhead) values(@department,@narration,@dhead)";
                    OleDbParameter dep = new OleDbParameter("@department", OleDbType.VarChar);dep.Direction = ParameterDirection.Input;
                    OleDbParameter narration = new OleDbParameter("@narration", OleDbType.VarChar);narration.Direction = ParameterDirection.Input;
                    OleDbParameter dhead = new OleDbParameter("@dhead", OleDbType.VarChar);dhead.Direction = ParameterDirection.Input;
                    dep.Value = model.Name;
                    narration.Value = model.Narration;
                    dhead.Value = model.Dep_Head;

                    OleDbCommand command = new OleDbCommand(sql, cn);
                    command.Parameters.Add(dep);
                    command.Parameters.Add(narration);
                    command.Parameters.Add(dhead);

                    var r = command.ExecuteNonQuery();
                    if (r > 0)
                    {
                        did = Connection.NewEntryno( table:"department_registration", field: "id", conn: cn)-1;
                        model.Dep_id = did;
                        ViewModels_Variables.ModelViews.Add_Update(model);
                    }
                    cn.Close();

                }                
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }

            return did;
        }
    }
}
