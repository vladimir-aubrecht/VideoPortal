using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Golem2.Manager.Metadata.Util
{
    public class ImageDescriptor
    {
        public String ImagePhysicalPath
        {
            get;
            set;
        }

        public String ImageVirtualPath
        {
            get
            {
                return Addressing.PathConverter.GetInstance().ConvertToVirtualPath(this.ImagePhysicalPath);
            }
        }

        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        Settings.SettingsManager settings = Settings.SettingsManager.GetInstance();

        public ImageDescriptor(String imagePath, int width, int height)
        {
            ImagePhysicalPath = imagePath;
            Width = width;
            Height = height;

            if (!settings.ForceDefaultImagesSize)
            {
                try
                {
                    using (Image img = Image.FromFile(imagePath))
                    {
                        this.Width = img.Width;
                        this.Height = img.Height;
                    }
                }
                catch
                {
                    /* Image is corrupted or file is missing, use default value*/
                }
            }

        }

    }
}
