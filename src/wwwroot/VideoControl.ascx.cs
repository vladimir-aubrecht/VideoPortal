using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class VideoControl : System.Web.UI.UserControl
{
    public String Url
    {
        get;
        set;
    }

    public String VideoType
    {
        get
        {
            if (Url.EndsWith(".mp4"))
            {
                return "video/mp4; codecs=amp4v.20.8, mp4a.40.2";
            }
            else if (Url.EndsWith(".mkv"))
            {
                return "\"video/x-matroska; codecs=avc1.64001E, mp4a.40.2\"";
            }
            else if (Url.EndsWith(".avi"))
            {
                
            }

            return String.Empty;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}