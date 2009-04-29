using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Specialized;
using ExtremeSwank.Authentication.OpenID;
using ExtremeSwank.Authentication.OpenID.Plugins.Extensions;

namespace OpenAuth.OpenID
{
    public class GS : Base
    {
        public static string name = "openid";

        public override string getLoginUrl(string nextUrl)//返回登录地址
        {
            OpenIDConsumer openid = new OpenIDConsumer(new NameValueCollection(), null, null);

            SimpleRegistration sr = new SimpleRegistration(openid);
            sr.AddRequiredFields(SimpleRegistrationFields.Nickname);//设置附加的字段列表

            openid.ReturnURL = nextUrl;
            openid.Identity = HttpContext.Current.Request["openid_url"];
            return openid.BeginAuth(false, false);//获得并返回登录地址
        }

        public override void parseHandle(HttpContext page)//回转内容处理函数
        {
            OpenIDConsumer openid = new OpenIDConsumer(page.Request.QueryString, null, null);
            switch (openid.RequestedMode)
            {
                case RequestedMode.IdResolution:
                    if (openid.Validate())
                    {
                        OpenIDUser user = openid.RetrieveUser();
                        string account = user.Identity;
                        if (user.ExtensionData.ContainsKey(SimpleRegistrationFields.Email))
                        {
                            account = user.ExtensionData[SimpleRegistrationFields.Email];
                        }
                        string nickName;//虽然设置了附加的字段类型，可是服务器未必支持，因此还是要判断该字段是不是存在
                        if (user.ExtensionData.ContainsKey(SimpleRegistrationFields.Nickname))
                        {
                            nickName = user.ExtensionData[SimpleRegistrationFields.Nickname];
                        }
                        else
                        {
                            nickName = account;
                        }
                        setUserInfo(account, nickName, name);
                    }
                    break;
            }
        }
    }
}
