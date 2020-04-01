using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Golem2.Manager.Streams
{
    public static class Helper
    {
        public static void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            if (readStream.CanSeek)
                readStream.Position = 0;

            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }

        public static Stream GetStream(Image img)
        {
            if (img == null)
                return null;
        
            MemoryStream memStream = new MemoryStream();
            img.Save(memStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            img.Dispose();
            return memStream;
        }

    }
}
