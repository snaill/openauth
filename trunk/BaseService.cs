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
    public class Token
    {
        public const string cookieName = "OpenAuth";
        public static string cookieUserID = "UserID";
        public static string cookieService = "Service";

        protected static HttpCookie GetCookie()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (null != cookie)
                return cookie;

            cookie = new HttpCookie(cookieName);
            cookie.Domain = HttpContext.Current.Request.Url.Host;
            cookie.Values.Add(cookieService, Convert.ToBase64String(Encoding.UTF8.GetBytes("unknown")));
            HttpContext.Current.Response.Cookies.Add(cookie);
            return cookie;
        }

        public static bool IsToken()
        {
            return Token.UserID != "";
        }

        public static string UserID
        {
            get
            {
                string account = GetCookie().Values[cookieUserID];
                if ( null == account )
                    return "";

                return Encoding.UTF8.GetString( Convert.FromBase64String(account));
            }
            set
            {
                if (value == null || value == "")
                    return;
                GetCookie().Values.Set(cookieUserID, Convert.ToBase64String(Encoding.UTF8.GetBytes(value)));
            }
        }

        public static string Service
        {
            get
            {
                string service = GetCookie().Values[cookieService];
                if (null == service)
                    return "";

                return Encoding.UTF8.GetString(Convert.FromBase64String(service));
            }
            set
            {
                if (value == null || value == "")
                    return;
                GetCookie().Values.Set(cookieService, Convert.ToBase64String(Encoding.UTF8.GetBytes(value)));
            }
        }

        //Çå³ýCookie
        public static void clear()
        {
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Domain = HttpContext.Current.Request.Url.Host;
            cookie.HttpOnly = false;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }

    public abstract class Base
    {
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
