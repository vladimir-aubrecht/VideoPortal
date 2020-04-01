using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class VideoControl : System.Web.UI.UserControl
{
    String url;
    public String Url
    {
        get { return url; }
        set { this.url = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}