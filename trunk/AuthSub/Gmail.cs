using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Xml;

namespace OpenAuth.AuthSub
{
    /// <summary>
    /// Google AuthSub API
    /// https://www.google.com/accounts/ManageDomains
    /// </summary>
    public class Gmail:Base
    {
        public static string name = "gmail";
        private string urlAuthSubRequest = "https://www.google.com/accounts/AuthSubRequest";
        private string urlScope = "http://www.google.com/m8/feeds/";
        private string urlData = "http://www.google.com/m8/feeds/contacts/default/thin?max-results=0";

        public Gmail()
        {
        }

        public Gmail(System.Xml.XmlNode node, string UserID)
        {
        }

        /// <summary>
        /// ���ɵ�¼��URL
        /// </summary>
        /// <returns></returns>
        public override string getLoginUrl( string nextUrl )
        {
            return urlAuthSubRequest + "?next=" + HttpUtility.UrlEncode(nextUrl) +
                "&scope=" + HttpUtility.UrlEncode(urlScope);
        }

        /// <summary>
        /// �����ת����
        /// </summary>
        /// <param name="page"></param>
        public override void parseHandle(HttpContext page)
        {
            string token = page.Request["token"];

            //����ͨѶ¼����
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(urlData));
            //��һ�佫token���Ƽ��뵽���ݷ�������֮��
            request.Headers.Add("Authorization", "AuthSub token=\"" + token + "\"");
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)getResponse(request);
            XmlDocument doc = new XmlDocument();
            if (response != null)
            {
                doc.Load(response.GetResponseStream());

                //��ȡ�û���ID��E-mail��ַ��
                XmlNode node = doc.SelectSingleNode("*/*[local-name()='id']");
                string account = node != null ? node.InnerText : "";

                Token.UserID = account;
                Token.Service = name;
            }
        }
    }
}
