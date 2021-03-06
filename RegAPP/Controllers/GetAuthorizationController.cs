﻿using RegAPP.Models;
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
    public class GetAuthorizationController : ApiController
    {
        string sqliteFilePath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/DSqlite.db");

        [HttpPost]
        public string GetAuthorization([FromBody]MachineInfo machineInfo)
        {
            Boolean auth = false;
            SQLiteParameter[] parameters;
            StringBuilder sb = new StringBuilder();
            sb.Append("Select Enrolment.LoginTime, Office.OfficeId, Enrolment.MachineCode From Enrolment ");
            sb.Append("Inner Join Office On Office.OfficeId = Enrolment.OfficeId ");
            sb.Append("Inner Join OfficeVersion On Office.OfficeId = OfficeVersion.OfficeId ");
            sb.Append("Where Office.Name = @Officename and ");
            sb.Append("Enrolment.UserName = @UserName and ");
            sb.Append("OfficeVersion.Version = @Version");
            parameters = new SQLiteParameter[]
            {
                SQLiteHelper.MakeSQLiteParameter("@Officename", DbType.String, machineInfo.Officename),
                SQLiteHelper.MakeSQLiteParameter("@UserName", DbType.String, machineInfo.UserName),
                SQLiteHelper.MakeSQLiteParameter("@Version", DbType.String, machineInfo.Version)
            };
            DataSet ds = SQLiteHelper.Query(sb.ToString(), parameters);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return "登陆信息有误，请重试";
            }

            DateTime dateTime = DateTime.Parse(ds.Tables[0].Rows[0][0].ToString());
            if (dateTime.AddHours(1) < DateTime.Now)
            {
                auth = true;
            }
            else if (ds.Tables[0].Rows[0][2].ToString() == machineInfo.MachineCode)
            {
                auth = true;
            }

            if (auth)
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
                    SQLiteHelper.MakeSQLiteParameter("@LoginTime", DbType.DateTime, DateTime.Now),
                    SQLiteHelper.MakeSQLiteParameter("@MachineCode", DbType.String, machineInfo.MachineCode),
                    SQLiteHelper.MakeSQLiteParameter("@UserName", DbType.String, machineInfo.UserName),
                    SQLiteHelper.MakeSQLiteParameter("@OfficeId", DbType.Int16, int.Parse(officeid))
                };
                if (SQLiteHelper.ExecuteSql(sb.ToString(), parameters) > 0)
                {
                    return "登陆成功";
                }

                return "更新失败";
            }

            return "该账号1小时前在其他电脑登陆过！请在原设备登出或等待1小时后登陆";
        }

    }
}
