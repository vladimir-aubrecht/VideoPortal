<%@ WebHandler Language="C#" Class="GetFile" %>

using System;
using System.Web;
using System.IO;
using System.Drawing;
using Golem2.Manager.Addressing;
using Golem2.Manager.DataHandling;
using Golem2.Manager.Settings;
using Golem2.Manager.Streams;
using System.Diagnostics;

public class GetFile : IHttpHandler 
{
    private HttpContext context;
    
    private void SendError(String message)
    {
        context.Response.ContentType = "text/html";
        context.Response.Write(String.Format("<p>{0}</p>", message));
    }
    
    public void ProcessRequest (HttpContext context) 
    {
        this.context = context;
        
        context.Response.Clear();
        context.Response.Buffer = true;

        DataDescriptor desc = new DataDescriptor(context);
        
        if (!desc.IsPath)
        {
            SendError("Need a valid path");
            return;    
        }
        
        Stream stream = null;
        switch (desc.Type)
        {
            case DataDescriptor.DataType.Image:
                    context.Response.ContentType = "image/jpeg";
                    stream = GetImage(desc);
                break;
                
            case DataDescriptor.DataType.Video:
                    context.Response.ContentType = "video/x-matroska";
                    stream = GetVideo(desc);                
                break;
        }

        if (stream != null)
        {
            Helper.ReadWriteStream(stream, context.Response.OutputStream);

            stream.Close();
            stream.Dispose();
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

    
    private Stream GetVideo(DataDescriptor desc)
    {
        PathConverter pathConverter = PathConverter.GetInstance();
        String physicalPath = pathConverter.ConvertToPhysicalPath(desc.Path);
        
        if (File.Exists(physicalPath))
        {
            Stream s = null;

            try
            {
                String name = Path.GetFileName(physicalPath);

                String cachePhysicalPath = String.Format(@"C:\inetpub\wwwroot\Cache\{0}.mkv", name);

                if (!File.Exists(cachePhysicalPath))
                {
                    string ffmpegURL = SettingsManager.GetInstance().VirtualFFMpegPath; 
                    ffmpegURL = context.Server.MapPath(ffmpegURL);

                    VideoProcesor video = new VideoProcesor(physicalPath, ffmpegURL);
                    throw new Exception(video.CreateParam(cachePhysicalPath).ToString());
                    video.ConvertVideo(video.CreateParam(cachePhysicalPath));
                    
                    //return video.GetStream(physicalPath);
                }

                //s = new StreamReader(cachePhysicalPath).BaseStream;
                //context.Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", name));
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "text/html";
                context.Response.Write(ex.Message);
            }
            
            return s;
        }

        return null;
    }

    private Stream GetImage(DataDescriptor desc)
    {
        SettingsManager settings = SettingsManager.GetInstance();
        
        String cachePhysicalPath = String.Format("{0}\\", context.Server.MapPath(settings.VirtualTempPath));
        PathConverter pathConverter = PathConverter.GetInstance();


        String physicalPath = String.Empty;
        
        if (pathConverter.IsValidPath(desc.Path))
            physicalPath = pathConverter.ConvertToPhysicalPath(desc.Path);

        String filename = settings.GenerateUniqueNameForTempPath(physicalPath);
        filename = String.Format("{0}{1}", cachePhysicalPath, filename);

        Image thumbnail = ImageProcesor.GetThumbnail(physicalPath, filename, desc);

        return Helper.GetStream(thumbnail);
    }

}