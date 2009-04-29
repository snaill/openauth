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
        /// 采用回传得到的令牌获取用户的信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public XmlDocument getData(string token)
        {
            if (wll == null) { createWLL(); }
            string lid = wll.ProcessConsentToken(token).LocationID;
            //访问用户的地址本
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
        /// 得到用户的登录URL地址
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
        /// 处理登录回转信息
        /// </summary>
        /// <param name="page"></param>
        public override void parseHandle(HttpContext page)
        {
            if (wll == null) { createWLL(); }
            string action = page.Request["action"];
            switch (action)
            {
                case "delauth":
                    //获得用户地址本XML数据
                    XmlDocument doc = getData(page.Request["ConsentToken"]);
                    //获取用户帐号
                    XmlNode node = doc.SelectSingleNode("Owner/WindowsLiveID");
                    string account = node != null ? node.InnerText : "";
                    //获取用户的昵称
                    node = doc.SelectSingleNode("Owner/Profiles/Personal/DisplayName");
                    string nickName = node != null ? node.InnerText : "";
                    //设置用户信息到Cookie
                    setUserInfo(account, nickName, name);
                    break;
                case "login"://这是Live接口要求定义支持的类型，系统之中没有主动使用这种请求
                    WindowsLiveLogin.User user = wll.ProcessLogin(page.Request.Form);//从URL参数之中解析出用户的登录信息
                    setUserInfo(user.Id, user.Id, name);//这里的user.ID实际上已经是用户的E-mail
                    page.Response.Redirect(wll.GetConsentUrl("Contacts.View", user.Token));
                    break;
                case "logout"://这是Live接口要求定义支持的类型，系统之中没有主动使用这种请求
                    clearCookie();
                    break;
                case "clearcookie"://这是Live接口要求定义支持的类型，系统之中没有主动使用这种请求
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
