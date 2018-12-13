using RegAPP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RegAPP.Controllers
{
    public class GetAuthorizationController : ApiController
    {

        string sqliteFilePath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/DSqlite.db");
        [HttpPost]
        public Boolean GetAuthorization([FromBody] MachineInfo machineInfo)
        {
            DataSet ds = new DataSet();

            //声明一个Sqlite数据库的链接
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + sqliteFilePath))
            {
                //创建sqlite命令
                using (SQLiteCommand comm = conn.CreateCommand())
                {
                    //打开数据库链接
                    conn.Open();
                    comm.CommandText = "Select Enrolment.AuthorizationCode From Enrolment " +
                                       "Inner Join Office On Office.OfficeId = Enrolment.OfficeId " +
                                       "Inner Join OfficeVersion On Office.OfficeId = OfficeVersion.OfficeId " +
                                       "Where Office.Name = '" + machineInfo.Officename + "' and " +
                                       "Enrolment.MachineCode = '" + machineInfo.MachineCode + "' and " +
                                       "OfficeVersion.Version = '" + machineInfo.Version + "'";
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(comm))
                    {
                        adapter.Fill(ds);
                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            return false;
                        }
                        return true;
                    }

                }
            }
        }
    }
}
