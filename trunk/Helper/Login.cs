using System;
using System.Web;
using System.Web.SessionState;
using OpenAuth;

namespace OpenAuth.Helper
{
    public class Login : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string userId = "";
            string service = "";
            string nextUrl = ""; 
            
            if ("logout" == context.Request.QueryString["action"])
            {
                //退出系统，返回登录页
                Token.clear();
                context.Response.Redirect("index.html");
                return;
            }
            else if ("handle" == context.Request.QueryString["action"])
            {
                // 验证服务器返回
                service = context.Request.QueryString["service"];
                nextUrl = context.Request.QueryString["nextUrl"];
                if (null != context.Request.QueryString["userId"])
                    userId = context.Request.QueryString["userId"];

                nextUrl = context.Server.UrlDecode(nextUrl);
                Base auth = Creator.Create(service, userId);
                auth.parseHandle(context);
                context.Response.Redirect(nextUrl);
                return;
            }

            // 开始登录
            if (null != context.Request.Form["userId"])
            {
                userId = context.Request.Form["userId"];
                service = context.Request.Form["service"];
                nextUrl = context.Server.UrlEncode(context.Request.Form["nextUrl"]);
            }
            else
            {
                service = context.Request.QueryString["service"];
                nextUrl = context.Request.QueryString["nextUrl"];
            }

            //
            Base s = Creator.Create(service, userId);
            string url = context.Request.Url.AbsoluteUri;

            url = url.Substring(0, url.LastIndexOf("/") + 1) + "login.ashx?action=handle&service=" + service;
            url += "&nextUrl=" + nextUrl;
            if ("" != userId)
                url += "&userId=" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(userId));

            url = s.getLoginUrl(url);
            context.Response.Redirect(url);
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
