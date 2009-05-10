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
        /// 生成登录的URL
        /// </summary>
        /// <returns></returns>
        public override string getLoginUrl( string nextUrl )
        {
            return urlAuthSubRequest + "?next=" + HttpUtility.UrlEncode(nextUrl) +
                "&scope=" + HttpUtility.UrlEncode(urlScope);
        }

        /// <summary>
        /// 处理回转请求
        /// </summary>
        /// <param name="page"></param>
        public override void parseHandle(HttpContext page)
        {
            string token = page.Request["token"];

            //访问通讯录数据
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(urlData));
            //这一句将token令牌加入到数据访问请求之中
            request.Headers.Add("Authorization", "AuthSub token=\"" + token + "\"");
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)getResponse(request);
            XmlDocument doc = new XmlDocument();
            if (response != null)
            {
                doc.Load(response.GetResponseStream());

                //读取用户的ID（E-mail地址）
                XmlNode node = doc.SelectSingleNode("*/*[local-name()='id']");
                string account = node != null ? node.InnerText : "";

                Token.UserID = account;
                Token.Service = name;
            }
        }
    }
}
