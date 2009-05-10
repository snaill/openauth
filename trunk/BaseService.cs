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
//        public const string cookieName = "OpenAuth";
        public static string cookieUserID = "UserID";
        public static string cookieService = "Service";

        //protected static HttpCookie GetCookie()
        //{
        //    HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
        //    if (null != cookie)
        //        return cookie;

        //    cookie = new HttpCookie(cookieName);
        //    cookie.Domain = "www.jeebook.com";
        //    cookie.Values.Add(cookieService, Convert.ToBase64String(Encoding.UTF8.GetBytes("unknown")));
        //    HttpContext.Current.Response.Cookies.Add(cookie);
        //    return cookie;
        //}

        public static bool IsToken()
        {
            return Token.UserID != "";
        }

        public static string UserID
        {
            get
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieUserID];
                if ( null == cookie )
                    return "";

                return Encoding.UTF8.GetString(Convert.FromBase64String(cookie.Value));
            }
            set
            {
                if (value == null || value == "")
                    return;
            //    GetCookie().Values.Set(cookieUserID, Convert.ToBase64String(Encoding.UTF8.GetBytes(value)));
                HttpCookie cookie = new HttpCookie(cookieUserID, Convert.ToBase64String(Encoding.UTF8.GetBytes(value)));
                cookie.Domain = "www.jeebook.com";
                cookie.Expires = DateTime.Now.AddMinutes(30);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        public static string Service
        {
            get
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieService];
                if (null == cookie)
                    return "";

                return Encoding.UTF8.GetString(Convert.FromBase64String(cookie.Value));
            }
            set
            {
                if (value == null || value == "")
                    return;
 //               GetCookie().Values.Set(cookieService, Convert.ToBase64String(Encoding.UTF8.GetBytes(value)));
                HttpCookie cookie = new HttpCookie(cookieService, Convert.ToBase64String(Encoding.UTF8.GetBytes(value)));
                cookie.Domain = "www.jeebook.com";
                cookie.Expires = DateTime.Now.AddMinutes(30);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        //Çå³ýCookie
        public static void clear()
        {
            UserID = "";
            Service = "";
            //HttpCookie cookie = new HttpCookie(cookieName);
            //cookie.Domain = HttpContext.Current.Request.Url.Host;
            //cookie.HttpOnly = false;
            //HttpContext.Current.Response.Cookies.Add(cookie);
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
