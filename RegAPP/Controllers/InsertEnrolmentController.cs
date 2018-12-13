using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using RegAPP.Models;

namespace RegAPP.Controllers
{
    public class InsertEnrolmentController : ApiController
    {

        string sqliteFilePath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/DSqlite.db");

        public static string 加密(string s,string Company)//转换为授权码
        {
            string str = "";
            string company = GetMd5Hash(Company);

            for (int i = 0; i <= s.Length - 1; i++)
            {
                str = str + ((char)(((int)s[i] + i + (int)company[i]) % 61 + 65)).ToString();
            }
            return GetMd5Hash(str).ToUpper();
        }
        public static string GetMd5Hash(string input)
        {
            if (input == null)
            {
                return null;
            }

            MD5 md5Hash = MD5.Create();

            // 将输入字符串转换为字节数组并计算哈希数据 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // 创建一个 Stringbuilder 来收集字节并创建字符串 
            StringBuilder sBuilder = new StringBuilder();

            // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // 返回十六进制字符串 
            return sBuilder.ToString();
        }
        [HttpPost]
        public string InsertEnrolment([FromBody]UpdataMachine users)
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
                    comm.CommandText = "Select Enrolment.OfficeId,Enrolment.MachineCode From OfficeVersion " +
                                       "Inner Join Office On Office.OfficeId = OfficeVersion.OfficeId " +
                                       "Where Office.Name = '" + users.Officename + "' and " +
                                       "Office.License = '" + users.License + "'"  ;
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(comm))
                    {
                        adapter.Fill(ds);
                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            SQLiteCommand command = conn.CreateCommand();
                            command.CommandText = "delete from Enrolment " +
                                                  "Where OfficeId = '" + ds.Tables[0].Rows[0][0] + "' and " +
                                                  "MachineCode = '" + ds.Tables[0].Rows[0][1] + "'";
                            command.ExecuteNonQuery();
                            return "添加成功";
                        }
                        else
                        {
                            return "存在重复记录";
                        }
                    }

                }
            }
        }
    }
}
