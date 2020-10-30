using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Windows;

namespace accounts.DB
{
    static class Company
    {
        public static DataTable CompanyTable;
        public static void Fetch()
        {
            try
            {
                CompanyTable = DB.Connection._FetchTable("select * from company order by id");
            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }
        }
        public static bool SaveOrUpdate(Model.SCompany sCompany)
        {
            bool res = false;
            try
            {
                if (sCompany != null)
                {


                    var con = Connection.OpenConnection();
                    if (con != null)
                    {
                        OleDbParameter comp = new OleDbParameter("@company", System.Data.SqlDbType.VarChar); comp.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter lmark = new OleDbParameter("@lmark", System.Data.SqlDbType.VarChar); lmark.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter place = new OleDbParameter("@place", System.Data.SqlDbType.VarChar); place.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter street = new OleDbParameter("@street", System.Data.SqlDbType.VarChar); street.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter post = new OleDbParameter("@post", System.Data.SqlDbType.VarChar); post.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter zipcode = new OleDbParameter("@zipcode", System.Data.SqlDbType.VarChar); zipcode.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter taxid = new OleDbParameter("@taxid", System.Data.SqlDbType.VarChar); taxid.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter dlno = new OleDbParameter("@dlno", System.Data.SqlDbType.VarChar); dlno.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter expno = new OleDbParameter("@expno", System.Data.SqlDbType.VarChar); expno.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter email = new OleDbParameter("@email", System.Data.SqlDbType.VarChar); email.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter officeno = new OleDbParameter("@officeno", System.Data.SqlDbType.VarChar); officeno.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter mobno = new OleDbParameter("@mobile", System.Data.SqlDbType.VarChar); mobno.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter f1 = new OleDbParameter("@f_date1", System.Data.SqlDbType.DateTime); f1.Direction = System.Data.ParameterDirection.Input;
                        OleDbParameter f2 = new OleDbParameter("@f_date2", System.Data.SqlDbType.DateTime); f2.Direction = System.Data.ParameterDirection.Input;

                        string sqlcomd = "";
                        if (ViewModels_Variables.ModelViews.CompanyProfile.Count>0)
                        {
                            sqlcomd = "update  company set [company]=@company,[lmark]=@lmark,[place]=@place,[street]=@street,[post]=@post,[zipcode]=@zipcode,[TAXID]=@taxid,[DLNO]=@dlno,[expno]=@expno,[email]=@email,[officeno]=@officeno,[Mobile]=@mobile,[f_date1]=@f_date1,[f_date2]=@f_date2";
                        }
                        else
                        {
                            sqlcomd = "INSERT INTO company ([company],[lmark] ,[place] ,[street],[post],[zipcode],[TAXID],[DLNO],[expno],[email],[officeno],[Mobile],[f_date1],[f_date2])VALUES   ( @company,@lmark,@place,@street,@post,@zipcode,@TAXID,@DLNO,@expno,@email,@officeno,@Mobile,@f_date1,@f_date2)";
                        }
                        OleDbCommand cmd = new OleDbCommand(sqlcomd, con);
                        comp.Value = sCompany.company;
                        lmark.Value = sCompany.lmark;
                        place.Value = sCompany.place;
                        street.Value = sCompany.street;
                        post.Value = sCompany.post;
                        zipcode.Value = sCompany.zipcode;
                        taxid.Value = sCompany.TAXID;
                        dlno.Value = sCompany.DLNO;
                        expno.Value = sCompany.expno;
                        email.Value = sCompany.email;
                        officeno.Value = sCompany.officeno;
                        mobno.Value = sCompany.Mobile;
                        f1.Value = sCompany.f_date1;
                        f2.Value = sCompany.f_date2;

                        cmd.Parameters.Add(comp);
                        cmd.Parameters.Add(lmark);
                        cmd.Parameters.Add(place);
                        cmd.Parameters.Add(street);
                        cmd.Parameters.Add(post);
                        cmd.Parameters.Add(zipcode);
                        cmd.Parameters.Add(taxid);
                        cmd.Parameters.Add(dlno);
                        cmd.Parameters.Add(expno);
                        cmd.Parameters.Add(email);
                        cmd.Parameters.Add(officeno);
                        cmd.Parameters.Add(mobno);
                        cmd.Parameters.Add(f1);
                        cmd.Parameters.Add(f2);

                        var r = cmd.ExecuteNonQuery();

                        if (r != 0)
                        {
                            res = true;
                            
                        }
                        con.Close();
                    }
                    else
                    {
                        MessageBox.Show("SQL Connection lost,avail technical support");
                    }

                }
            }
            catch (Exception e11)
            {

                MessageBox.Show(e11.Message.ToString());
            }

            return res;
        }
    }
}
