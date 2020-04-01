using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Golem2.Manager.Addressing;

public partial class Serialy : System.Web.UI.Page
{
    private String baseUrl = null;
    private static String pageFilename = "Serialy.aspx";
    String parentDirectory = null;
    String path;


    protected void Page_Load(object sender, EventArgs e)
    {
        InitEvents();

        player.Visible = false;
        player1.Visible = false;

        baseUrl = GetBaseUrl();

        path = Request.QueryString["Path"];

        Golem2.Manager.Addressing.PathWalker mng = new Golem2.Manager.Addressing.PathWalker(path);

        if (mng.IsPlayable(out parentDirectory))
        {
            ShowPlayer(path.EndsWith(".mp4"));
            SetPlayerUrl(path);
            mng = new Golem2.Manager.Addressing.PathWalker(parentDirectory);
        }

        FillList(mng.GetFileAndFolders());

    }

    private void InitEvents()
    {
        lbtnParentFolder.Click += new EventHandler(lbtnParentFolder_Click);
    }

    void lbtnParentFolder_Click(object sender, EventArgs e)
    {
        String parFolder = PathConverter.GetInstance().GetParentDirectory(path);

        if (parFolder == this.parentDirectory)
            parFolder = PathConverter.GetInstance().GetParentDirectory(parFolder);

        Response.Redirect(String.Format("{0}{1}?Path={2}", baseUrl, pageFilename, parFolder));
    }

    private void SetPlayerUrl(String path)
    {
        if (path.EndsWith(".mp4") || path.EndsWith(".mkv") || path.EndsWith("avi"))
        {
            player.Url = String.Format("{0}{1}", baseUrl, path);
            player1.Url = String.Format("{0}{1}", baseUrl, path);
        }
    }

    private void ShowPlayer(bool forceHTML5)
    {
        if (Request.QueryString["Version"] == "Experimental" || forceHTML5)
        {
            player.Visible = true;
            player1.Visible = false;
        }
        else
        {
            player1.Visible = true;
            player.Visible = false;
        }
    }

    private String GetBaseUrl()
    {
        return String.Format("http://{0}/", Golem2.Manager.Settings.SettingsManager.GetInstance().Domain);
    }

    private void FillList(List<Golem2.Manager.Addressing.PathWalker.FolderInfo> folders)
    {
        double imgQuality = 2.0;
        Panel parentPanel = new Panel();

        for (int i = 0; i < folders.Count; i++)
        {
            Panel panel = new Panel();
            panel.CssClass = "FolderDiv";

            LinkButton lb = new LinkButton();

            int width = folders[i].FolderImage.Width;
            int height = folders[i].FolderImage.Height;

            String overview = folders[i].Overview;

            string name = folders[i].FolderDiplayName;
            string imagePath = folders[i].FolderImage.ImagePhysicalPath.Replace("\\", "/").Insert(0, String.Format("GetFile.ashx?path="));
            imagePath = String.Format("{0}&width={1}&height={2}", imagePath, Math.Round(width * imgQuality), Math.Round(height * imgQuality));

            String subtitlesLanguages = String.Empty;

            foreach (var sub in folders[i].SubtitleLanguages)
            {
                subtitlesLanguages = String.Format("{0},{1}", subtitlesLanguages, sub);
            }

            if (subtitlesLanguages.Length > 0)
                subtitlesLanguages = subtitlesLanguages.Remove(0, 1);

            subtitlesLanguages = String.Format("Titulky: {0}", subtitlesLanguages);

            String url = String.Format("{0}{1}/{2}", baseUrl, parentDirectory.Remove(0, 1), folders[i].FolderName);
            String header = String.Format("<div style=\"width:{1}px; max-width:{1}px; font-size: 11px; height: 14px;\"><div style=\"float: left; height: 14px; padding-left: 6px;\">{0}</div><div style=\"float: right; height: 14px; padding-right: 6px;\"><a href=\"{2}\">download</a></div></div>", subtitlesLanguages, width + 9, url);
            String body = String.Format("<img src=\"{0}\" title=\"{1}\" alt=\"{1}\" style=\"width:{2}px; height:{3}px;\" />", imagePath, overview, width, height);
            String footer = String.Format("<div style=\"width:{0}px; max-width:{0}px;\">{1}</div>", width + 9, name);

            lb.Text = String.Format("{0}{1}", body, footer);
            lb.ID = folders[i].FolderName.Replace("'", "~");
            lb.Click += new EventHandler(lb_Click);

            if (folders[i].IsFile)
                panel.Controls.Add(new Label() { Text = header });
            
            panel.Controls.Add(lb);


            parentPanel.Controls.Add(panel);
        }

        TVShowsView.Controls.Add(parentPanel);
    }

    void lb_Click(object sender, EventArgs e)
    {
        String name = ((LinkButton)sender).ID.Replace("~", "'");

        String path = parentDirectory;

        Response.Redirect(String.Format("{0}{1}?Path={2}/{3}", baseUrl, pageFilename, path, name));
    }
}