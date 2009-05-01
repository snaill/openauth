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

        public override string getLoginUrl(string next )//ֱ�ӽ�XML����֮�еĵ�¼URL����
        {
            return getLoginUrl(); 
        }
        public override void parseHandle(HttpContext page)//�����ת����
        {
            System.Collections.Specialized.NameValueCollection request = HttpUtility.ParseQueryString(page.Request.Url.Query, Encoding.UTF8);
            string id = request["id"];
            Token.UserID = id;
            Token.Service = name;
        }
    }
}
