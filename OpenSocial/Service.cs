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
    public class Default : Base
    {
        public static string name = "opensocial";

        public Default()
        {
        }

        public Default( System.Xml.XmlNode node, string UserID )
        {
        }

        protected virtual string getLoginUrl()
        {
            System.Diagnostics.Debug.Assert(false);
            return "";
        }

        public override string getLoginUrl(string next )//直接将XML配置之中的登录URL返回
        {
            return getLoginUrl(); 
        }
        public override void parseHandle(HttpContext page)//处理回转请求
        {
            System.Collections.Specialized.NameValueCollection request = HttpUtility.ParseQueryString(page.Request.Url.Query, Encoding.UTF8);
            string id = request["id"];
            Token.UserID = id;
            Token.Service = name;
        }
    }
}
