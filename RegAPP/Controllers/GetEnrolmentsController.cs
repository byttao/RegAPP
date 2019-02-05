using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RegAPP.Models;
using System.Data.SQLite;
using System.Data;
using System.Reflection;

namespace RegAPP.Controllers
{
    public class GetEnrolmentsController : ApiController
    {
        string sqliteFilePath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/DSqlite.db");

        //[HttpPost]
        public IEnumerable<Enrolment> GetEnrolments([FromBody]Office users)
        {
            DataSet ds = new DataSet();
            string officeid;
            //声明一个Sqlite数据库的链接
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + sqliteFilePath))
            {
                //创建sqlite命令
                using (SQLiteCommand comm = conn.CreateCommand())
                {
                    //打开数据库链接
                    conn.Open();
                    comm.CommandText = "Select OfficeId From Office where Name='" + users.Officename + "' and License='"+users.License +"'";
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(comm))
                    {
                        adapter.Fill(ds);
                        if (ds.Tables[0].Rows.Count==0)
                        {
                            var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                            {
                                Content = new StringContent(string.Format("没有找到id={0}的对象", users.Officename)),
                                ReasonPhrase = "object is not found"
                            };
                            throw new HttpResponseException(resp);
                        }
                        officeid=ds.Tables[0].Rows[0][0].ToString();
                    }

                    comm.CommandText = "Select * From Enrolment where OfficeId=" + officeid ;
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(comm))
                    {
                        ds.Clear();
                        adapter.Fill(ds);
                    }

                    return Func.DataSetToIList<Enrolment>(ds, 0);
                }
            }
        }

    }
}
