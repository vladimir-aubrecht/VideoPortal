using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace Golem2.Manager.DataHandling
{
    public class VideoProcesor
    {
        public struct Params
        {
            public String PathToOriginalVideo;
            public String PathToConvertedVideo;
            public int Width;
            public int Height;

            public override string ToString()
            {
                return String.Format("-acodec copy -scodec copy -i \"{0}\" -s {1}x{2} {3}", PathToOriginalVideo, Width, Height, PathToConvertedVideo);
            }
        }
        
        Process process = new Process();
        Params defaultParam = new Params();

        public VideoProcesor(String pathToVideo, String pathToFFMpeg)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(pathToFFMpeg));
            process.StartInfo.FileName = Path.GetFileName(pathToFFMpeg);
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = directoryInfo.FullName;
            process.StartInfo.RedirectStandardOutput = true;
            
            this.defaultParam.PathToOriginalVideo = pathToVideo;
            this.defaultParam.Width = 640;
            this.defaultParam.Height = 480;
        }

        public Params CreateParam(String convertedPath)
        {
            return new Params() { Width = defaultParam.Width, Height = defaultParam.Height, PathToOriginalVideo = defaultParam.PathToOriginalVideo, PathToConvertedVideo = convertedPath };
        }

        public void ConvertVideo(Params param)
        {
            process.StartInfo.Arguments = param.ToString();
            process.Start();
        }
    }

}
