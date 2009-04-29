using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Xml;
using System.Collections.Specialized;

namespace OpenAuth
{
    public abstract class Base
    {
        public static string cookieName = "OpenAuthCookie";
        public static string cookieDomain = "jeebook.com";

        public static string cookieAccount = "account";
        public static string cookieNickname = "nickname";
        public static string cookieService = "service";

        //清除Cookie
        public static void clearCookie()
        {
            HttpCookie loginCookie = new HttpCookie(cookieName);
            // loginCookie.Expires = DateTime.Now.AddYears(-10);
            loginCookie.Domain = cookieDomain;
            loginCookie.HttpOnly = false;
            HttpContext.Current.Response.Cookies.Add(loginCookie);
        }

        //将用户信息加入到Cookie
        public static void setUserInfo(string account, string name, string service)
        {
            HttpCookie loginCookie = new HttpCookie(cookieName);
            loginCookie.Domain = cookieDomain;
            loginCookie.Values.Add(cookieAccount, Convert.ToBase64String(Encoding.UTF8.GetBytes(account)));
            loginCookie.Values.Add(cookieNickname, Convert.ToBase64String(Encoding.UTF8.GetBytes(name)));
            loginCookie.Values.Add(cookieService, Convert.ToBase64String(Encoding.UTF8.GetBytes(service)));
            //loginCookie.Expires = DateTime.Now.AddYears(1);
            HttpContext.Current.Response.Cookies.Add(loginCookie);
        }

        //从Cookie之中获取用户信息
        public static NameValueCollection getUserInfo()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                NameValueCollection userInfo = new NameValueCollection();
                foreach (string key in cookie.Values)
                {
                    userInfo.Add(key, Encoding.UTF8.GetString(Convert.FromBase64String(cookie.Values[key])));
                }
                return userInfo.HasKeys() ? userInfo : null;
            }
            return null;
        }

        public static string getUserName() {
            NameValueCollection nv = getUserInfo();
            if (null == nv)
                return "";

            return nv[cookieNickname];
        }

        public static string getService()
        {
            NameValueCollection nv = getUserInfo();
            if (null == nv)
                return "";

            return nv[cookieService];
        }

        public HttpWebResponse getResponse(HttpWebRequest request)
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
        public abstract string getLoginUrl(string nextUrl);
        public abstract void parseHandle( HttpContext page);
    }
}
