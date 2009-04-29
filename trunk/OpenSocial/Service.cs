using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Xml;

namespace OpenAuth.OpenSocial
{
    public class GS : Base
    {
        public static string name = "opensocial";

        private string loginUrl;

        public GS( string url )
        {
            loginUrl = url;
        }

        public override string getLoginUrl(string next )//直接将XML配置之中的登录URL返回
        {
            return loginUrl; 
        }
        public override void parseHandle(HttpContext page)//处理回转请求
        {
            System.Collections.Specialized.NameValueCollection request = HttpUtility.ParseQueryString(page.Request.Url.Query, Encoding.UTF8);
            string id = request["id"];
            string name = request["name"];
            setUserInfo(id, name, name);//设置用户的Cookie
        }
    }
}
