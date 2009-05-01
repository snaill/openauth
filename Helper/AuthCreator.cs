using System;
using System.Collections.Generic;
using System.Web;

namespace OpenAuth.Helper
{
    /// <summary>
    /// Summary description for Creator
    /// </summary>
    public class Creator : System.Configuration.IConfigurationSectionHandler
    {
        public System.Collections.ArrayList Services = new System.Collections.ArrayList();
        public static Base Create(string service, string account)
        {
            Creator  c = (Creator)System.Web.Configuration.WebConfigurationManager.GetSection("openAuth");
            for (int i = 0; i < c.Services.Count; i++)
            {
                System.Xml.XmlNode node = (System.Xml.XmlNode)c.Services[i];
                if (node.Attributes["name"].Value != service)
                    continue;

                object[] param = new object[] { node, account };
                return (Base)System.Type.GetType(node.Attributes["type"].Value).GetConstructor(System.Type.GetTypeArray(param)).Invoke(param); ;
            }

            return null;
        }

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            System.Xml.XmlNodeList list = section.SelectNodes("services/service");
            for (int i = 0; i < list.Count; i++)
               Services.Add( list[i].Clone() );

            return this;
        }
    }
}