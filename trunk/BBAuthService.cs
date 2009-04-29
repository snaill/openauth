using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Xml;

namespace AuthService
{
    public class BBAuthServer:Base
    {
        public static string name = "yahoomail";
        private string appid
            , secret, server, pathLogin, pathPwtoken_login;
        private int timeout=300;
		//����Web.Config֮�е�XML�ڵ���Ϊ���캯������
        public BBAuthServer(System.Xml.XmlNode node)
            : base(node)
        {
            for (int i = 0; i < node.Attributes.Count; i++)
            {
                switch (node.Attributes[i].LocalName)
                {
                    case "appid":
                        appid = node.Attributes[i].Value;
                        break;
                    case "secret":
                        secret = node.Attributes[i].Value;
                        break;
                    case "server":
                        server = node.Attributes[i].Value;
                        break;
                    case "pathLogin":
                        pathLogin = node.Attributes[i].Value;
                        break;
                    case "pathPwtoken_login":
                        pathPwtoken_login = node.Attributes[i].Value;
                        break;
                    case "timeout":
                        timeout = int.Parse(node.Attributes[i].Value);
                        break;
                }
            }
        }
        public bool checkRequest(HttpRequest Request)
        {
            string ts = Request["ts"];
            string sig = Request["sig"];
            //�ȼ��ʱ��
            if (Math.Abs(long.Parse(ts) - ((long)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds))) > timeout)
            {
           //     throw new Exception("Error parsing timeout.");
            }
            //�ټ��ǩ��
            string baseString = System.Text.RegularExpressions.Regex.Replace(Request.Url.PathAndQuery, "&sig=[^&]+", "");
            if (System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(baseString + secret, "MD5").ToLower() != sig)
            {
                throw new Exception("Signature mismatch:" + baseString);
            }
            return true;
        }
        public string getWSSID(string token,out string cookie)
        {
            string ts = ((long)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds)).ToString();
            string baseString = pathPwtoken_login+"appid=" + HttpUtility.UrlEncode(appid) + "&token=" + HttpUtility.UrlEncode(token) + "&ts=" + ts + "";
            string sig = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(baseString + secret, "MD5").ToLower();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(server + baseString + "&sig=" + sig));
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            XmlDocument doc = new XmlDocument();
            doc.Load(response.GetResponseStream());
            cookie = doc.SelectSingleNode("//*[local-name()='Cookie']").InnerText.Trim().Substring(2);
            return doc.SelectSingleNode("//*[local-name()='WSSID']").InnerText.Trim();
        }
        public string getUserName(string token,string wssid,string cookie)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("http://mail.yahooapis.com/ws/mail/v1.1/soap?appid=" + HttpUtility.UrlEncode(appid) + "&WSSID=" + HttpUtility.UrlEncode(wssid)));
                request.Method = "POST";
                request.CookieContainer = new System.Net.CookieContainer();
                CookieCollection collection = new CookieCollection();
                request.CookieContainer.Add(new Uri("http://mail.yahooapis.com/"), new Cookie("Y", cookie));
                byte[] bytes = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?><SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/1999/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/1999/XMLSchema\" SOAP-ENV:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><SOAP-ENV:Body><GetUserData xmlns=\"urn:yahoo:ymws\"></GetUserData></SOAP-ENV:Body></SOAP-ENV:Envelope>");
                request.Headers.Add("SOAPAction", "\"\"");
                request.ContentType = "application/soap+xml; charset=utf-8";
                request.ContentLength = bytes.Length;
                Stream rs = request.GetRequestStream();
                rs.Write(bytes, 0, bytes.Length);
                rs.Close();
                HttpWebResponse response = getResponse(request);
                XmlDocument doc = new XmlDocument();
                doc.Load(response.GetResponseStream());
                return doc.SelectSingleNode("//*[local-name()='defaultID']").InnerText;
                /*
                ymws ymwsInstance = new ymws();
                ymwsInstance.Url = "http://mail.yahooapis.com/ws/mail/v1.1/soap?appid=" + appid + "&wssid=" + wssid;
                ymwsInstance.CookieContainer = new System.Net.CookieContainer();
                CookieCollection collection = new CookieCollection();
                ymwsInstance.CookieContainer.Add(new Uri("http://mail.yahooapis.com/"), new Cookie("Y", cookie));
                GetUserDataResponse userData = ymwsInstance.GetUserData(new GetUserData());
                return userData.data.userSendPref.defaultID;*/
            }
            catch (Exception)
            {
                return wssid;
            }
        }
        public override string getLoginUrl()//���ɵ�¼��URL
        {
            string ts = ((long)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds)).ToString();
            string baseString = pathLogin+"appid=" + HttpUtility.UrlEncode(appid) + "&appdata=" + HttpUtility.UrlEncode(name) + "&send_userhash=1&ts=" + ts + "";
            string sig = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(baseString + secret, "MD5").ToLower();
            return server + baseString + "&sig=" + sig;
        }
        public override void parseHandle(HttpContext page)//�����ת����
        {
            checkRequest(page.Request);//���ش������ǲ��ǺϷ�
            string cookie;
            string wssid = getWSSID(page.Request["token"],out cookie);//�Ȼ�ȡwssid
            string name = getUserName(page.Request["token"], wssid, cookie);//ͨ��wssid��ȡ�û���
            //�����ϣ���ʼ����û���WSSID
            AccountHelper.setUserInfo(page.Request["userhash"], name, this.name);
            AccountHelper.returnOpener();
            page.Response.End();
        }
        public static HttpWebResponse getResponse(HttpWebRequest request)
        {
            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                HttpContext.Current.Response.Write(e.Message);
                string result = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                HttpContext.Current.Response.Write(result);
                HttpContext.Current.Response.End();
            }
            return null;
        }
    }
}
