using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1
{
    /// <summary>
    /// save 的摘要说明
    /// </summary>
    public class save : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.Files.Count > 0)
            {
                for (int i = 0, j = context.Request.Files.Count; i < j; i++)
                {
                    HttpPostedFile f = context.Request.Files[i];
                    f.SaveAs(context.Server.MapPath("upload/" + f.FileName));
                }
                //不能使用foreach遍历HttpFileCollection对象，要不有些时候会报无法将类型为“System.String”的对象强制转换为类型“System.Web.HttpPostedFile”。错误
                /*foreach (HttpPostedFile f in context.Request.Files)
                    if (f.FileName != "")
                    {
                        f.SaveAs(context.Server.MapPath("upload/" + f.FileName));
                    }*/
                context.Response.Write(1);
            }
            else context.Response.Write("没有文件上传！");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}