using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenAuth.OpenSocial
{
    public class Xiaonei : Default
    {
        protected override string  getLoginUrl()
        {
            return "http://apps.xiaonei.com/passport/login.html";
        }
    }
}
