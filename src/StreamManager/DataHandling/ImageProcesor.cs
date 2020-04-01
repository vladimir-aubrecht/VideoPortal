using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Golem2.Manager.DataHandling;
using System.Drawing;
using Golem2.Manager.DataHandling.Util;
using System.IO;

namespace Golem2.Manager.DataHandling
{
    public class ImageProcesor
    {
        public static Image CreateThumbnail(String sourcePath, String destinationPath, DataDescriptor desc)
        {
            Image thumbnail = null;

            if (File.Exists(sourcePath))
            {
                using (Image img = (Image)Bitmap.FromFile(sourcePath))
                {
                    thumbnail = PixelTransform.SaveBitmap(PixelTransform.Interpolate(PixelTransform.LoadBitmap(img), desc.Width, desc.Height));
                    thumbnail.Save(destinationPath);
                }
            }

            return thumbnail;
        }

        public static Image GetThumbnail(String sourcePath, String tempPath, DataDescriptor desc)
        {
            Image thumbnail = null;

            try
            {
                if (!File.Exists(sourcePath))
                    thumbnail = Settings.SettingsManager.GetInstance().UnknownImage;

                else if (!File.Exists(tempPath))
                {
                    thumbnail = ImageProcesor.CreateThumbnail(sourcePath, tempPath, desc);
                }
                else
                    thumbnail = (Image)Bitmap.FromFile(tempPath);
            }
            catch
            {
                thumbnail = Settings.SettingsManager.GetInstance().UnknownImage;
            }

            return thumbnail;
        }
    }
}
