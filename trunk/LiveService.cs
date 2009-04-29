using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using System.Net;
using System.IO;
using WindowsLive;

namespace OpenAuth
{
    /// <summary>
    /// Microsoft Live
    /// Azure Services Developer Portal
    /// http://go.microsoft.com/fwlink/?LinkID=144070
    /// </summary>
    public class Live : Base
    {
        public static string name = "live";
        public WindowsLiveLogin wll = null;
        private string appid, secret;
        private string securityalgorithm = "wsignin1.0";

        public Live(string id, string psw)
        {
            appid = id;
            secret = psw;
        }

        public void createWLL()
        {
            wll = new WindowsLiveLogin(false);
            wll.AppId = appid;
            wll.Secret = secret;
            wll.SecurityAlgorithm = securityalgorithm;
        }

        /// <summary>
        /// ���ûش��õ������ƻ�ȡ�û�����Ϣ
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public XmlDocument getData(string token)
        {
            if (wll == null) { createWLL(); }
            string lid = wll.ProcessConsentToken(token).LocationID;
            //�����û��ĵ�ַ��
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("https://livecontacts.services.live.com/users/@L@" + lid + "/rest/LiveContacts/owner"));
            request.Method = "GET";
            request.Headers.Add("Authorization", "DelegatedToken dt=\"" + wll.ProcessConsentToken(token).DelegationToken + "\"");
            HttpWebResponse response = getResponse(request);
            XmlDocument doc = new XmlDocument();
            if (response != null)
            {
                doc.Load(response.GetResponseStream());
            }
            return doc;
        }

        /// <summary>
        /// �õ��û��ĵ�¼URL��ַ
        /// </summary>
        /// <param name="nextUrl"></param>
        /// <returns></returns>
        public override string getLoginUrl(string nextUrl)
        {
            if (wll == null)
            { 
                createWLL(); 
            }

            wll.PolicyUrl = "http://www.jeebook.com/mmm/login.ashx";
            wll.ReturnUrl = nextUrl;
            return wll.GetConsentUrl("Contacts.View");
        }

        /// <summary>
        /// �����¼��ת��Ϣ
        /// </summary>
        /// <param name="page"></param>
        public override void parseHandle(HttpContext page)
        {
            if (wll == null) { createWLL(); }
            string action = page.Request["action"];
            switch (action)
            {
                case "delauth":
                    //����û���ַ��XML����
                    XmlDocument doc = getData(page.Request["ConsentToken"]);
                    //��ȡ�û��ʺ�
                    XmlNode node = doc.SelectSingleNode("Owner/WindowsLiveID");
                    string account = node != null ? node.InnerText : "";
                    //��ȡ�û����ǳ�
                    node = doc.SelectSingleNode("Owner/Profiles/Personal/DisplayName");
                    string nickName = node != null ? node.InnerText : "";
                    //�����û���Ϣ��Cookie
                    setUserInfo(account, nickName, name);
                    break;
                case "login"://����Live�ӿ�Ҫ����֧�ֵ����ͣ�ϵͳ֮��û������ʹ����������
                    WindowsLiveLogin.User user = wll.ProcessLogin(page.Request.Form);//��URL����֮�н������û��ĵ�¼��Ϣ
                    setUserInfo(user.Id, user.Id, name);//�����user.IDʵ�����Ѿ����û���E-mail
                    page.Response.Redirect(wll.GetConsentUrl("Contacts.View", user.Token));
                    break;
                case "logout"://����Live�ӿ�Ҫ����֧�ֵ����ͣ�ϵͳ֮��û������ʹ����������
                    clearCookie();
                    break;
                case "clearcookie"://����Live�ӿ�Ҫ����֧�ֵ����ͣ�ϵͳ֮��û������ʹ����������
                default:
                    clearCookie();
                    string type;
                    byte[] content;
                    wll.GetClearCookieResponse(out type, out content);
                    page.Response.ContentType = type;
                    page.Response.OutputStream.Write(content, 0, content.Length);
                    break;

            }
        }
    }
}
