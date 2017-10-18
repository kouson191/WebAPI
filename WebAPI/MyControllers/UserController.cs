using System.Text;
using System.Web.Http;
using System.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Web;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Web.Security;

namespace WebAPI.MyControllers
{
    /// <summary>
    /// 用户
    /// </summary>
    public class UserController : ApiController
    {  
        public UserController()
        { 
        }  

        [HttpGet]
        public UserInfo Login(string strUser, string strPwd)
        {
            UserInfo returnObj = new UserInfo();
            if (!ValidateUser(strUser, strPwd))
            {
               
            }
            else
            {
                strPwd = strUser; // GetRandomString(8);
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(0, strUser, DateTime.Now,
                                DateTime.Now.AddHours(1), true, string.Format("{0}&{1}", strUser, strPwd),
                                FormsAuthentication.FormsCookiePath);
                //返回登录结果、用户信息、用户验证票据信息
                returnObj = new UserInfo { bRes = true, UserName = strUser, Password = "", Ticket = FormsAuthentication.Encrypt(ticket) };

                CacheHelper.SetCache(strUser, strPwd); 
            }

            return returnObj;
        }



        protected virtual void JsonpCallback(string json)
        {
            HttpResponse Response = HttpContext.Current.Response;
            string callback = HttpContext.Current.Request["callback"];

            //如果callback是空, 就是普通的json, 否则就是jsonp
            Response.Write(callback == null ? json : string.Format("{0}({1})", callback, json));
            Response.End();
        }

        //校验用户名密码（正式环境中应该是数据库校验）
        private bool ValidateUser(string strUser, string strPwd)
        {
            return true;

            //if (strUser == "admin" && strPwd == "123456")
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        public static string GetRandomString(int iLength)
        {
            string buffer = "0123456789*/.#~";// 随机字符中也可以为汉字（任何）  
            StringBuilder sb = new StringBuilder();
            Random r = new Random();
            int range = buffer.Length;
            for (int i = 0; i < iLength; i++)
            {
                sb.Append(buffer.Substring(r.Next(range), 1));
            }
            return sb.ToString();
        }
    }

    public class UserInfo
    {
        public bool bRes { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Ticket { get; set; }
    }



}