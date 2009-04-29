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

        public override string getLoginUrl(string next )//ֱ�ӽ�XML����֮�еĵ�¼URL����
        {
            return loginUrl; 
        }
        public override void parseHandle(HttpContext page)//�����ת����
        {
            System.Collections.Specialized.NameValueCollection request = HttpUtility.ParseQueryString(page.Request.Url.Query, Encoding.UTF8);
            string id = request["id"];
            string name = request["name"];
            setUserInfo(id, name, name);//�����û���Cookie
        }
    }
}
