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

        public override string getLoginUrl(string nextUrl)//���ص�¼��ַ
        {
            OpenIDConsumer openid = new OpenIDConsumer(new NameValueCollection(), null, null);

            SimpleRegistration sr = new SimpleRegistration(openid);
            sr.AddRequiredFields(SimpleRegistrationFields.Nickname);//���ø��ӵ��ֶ��б�

            openid.ReturnURL = nextUrl;
            openid.Identity = HttpContext.Current.Request["openid_url"];
            return openid.BeginAuth(false, false);//��ò����ص�¼��ַ
        }

        public override void parseHandle(HttpContext page)//��ת���ݴ�����
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
                        string nickName;//��Ȼ�����˸��ӵ��ֶ����ͣ����Ƿ�����δ��֧�֣���˻���Ҫ�жϸ��ֶ��ǲ��Ǵ���
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
