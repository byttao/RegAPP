using RegAPP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace RegAPP.Controllers
{
    public class LoginOutController : ApiController
    {
        // POST api/<controller>
        [HttpPost]
        public string LoginOut([FromBody]MachineInfo machineInfo)
        {
            SQLiteParameter[] parameters;
            StringBuilder sb = new StringBuilder();
            sb.Append("Select Enrolment.LoginTime, Office.OfficeId From Enrolment ");
            sb.Append("Inner Join Office On Office.OfficeId = Enrolment.OfficeId ");
            sb.Append("Inner Join OfficeVersion On Office.OfficeId = OfficeVersion.OfficeId ");
            sb.Append("Where Office.Name = @Officename and ");
            sb.Append("Enrolment.UserName = @UserName and ");
            sb.Append("OfficeVersion.Version = @Version and ");
            sb.Append("Enrolment.MachineCode = @MachineCode");
            parameters = new SQLiteParameter[]
            {
                SQLiteHelper.MakeSQLiteParameter("@Officename", DbType.String, machineInfo.Officename),
                SQLiteHelper.MakeSQLiteParameter("@UserName", DbType.String, machineInfo.UserName),
                SQLiteHelper.MakeSQLiteParameter("@Version", DbType.String, machineInfo.Version),
                SQLiteHelper.MakeSQLiteParameter("@MachineCode", DbType.String, machineInfo.MachineCode)
            };
            DataSet ds = SQLiteHelper.Query(sb.ToString(), parameters);
            if (ds.Tables[0].Rows.Count>0)
            {
                string officeid = ds.Tables[0].Rows[0][1].ToString();
                sb = new StringBuilder();
                sb.Append("update Enrolment set ");
                sb.Append("LoginTime=@LoginTime , ");
                sb.Append("MachineCode=@MachineCode ");
                sb.Append("where UserName=@UserName ");
                sb.Append("and OfficeId=@OfficeId ");
                parameters = new SQLiteParameter[]
                {
                    SQLiteHelper.MakeSQLiteParameter("@LoginTime", DbType.DateTime, DateTime.Now.AddMinutes(-55)),
                    SQLiteHelper.MakeSQLiteParameter("@MachineCode", DbType.String, machineInfo.MachineCode),
                    SQLiteHelper.MakeSQLiteParameter("@UserName", DbType.String, machineInfo.UserName),
                    SQLiteHelper.MakeSQLiteParameter("@OfficeId", DbType.Int16, int.Parse(officeid))
                };
                if (SQLiteHelper.ExecuteSql(sb.ToString(), parameters) > 0)
                {
                    return "登出成功，请5分钟后在其他电脑上重新登陆。";
                }
                return "退出失败";
            }
            return "未在本机登陆，无需退出";
        }

    }
}