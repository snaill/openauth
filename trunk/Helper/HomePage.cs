using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenAuth.Helper
{
    public partial class HomePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ( Token.IsToken() )
                Response.Redirect("index.html");

            Response.Write("<div style=\"text-align:center\">" + Token.UserID + " | <a href=\"login.ashx?action=logout\">Sign out</a></div>");
        }
    }
}
