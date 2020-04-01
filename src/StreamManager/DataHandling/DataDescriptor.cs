using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Golem2.Manager.DataHandling
{
    public struct QueryParams
    {
        public static readonly String Path = "path";
        public static readonly String Width = "width";
        public static readonly String Height = "height";
    }

    public class DataDescriptor
    {
        public enum DataType { None, Image, Video }

        private HttpContext context;

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public String Path
        {
            get
            {
                return context.Request.QueryString[QueryParams.Path];
            }
        }

        public bool IsPath
        {
            get
            {
                return !String.IsNullOrEmpty(context.Request.QueryString[QueryParams.Path]);
            }
        }

        public DataType Type
        {
            get
            {
                if (IsDataTypeImage())
                    return DataType.Image;

                if (IsDataTypeVideo())
                    return DataType.Video;

                return DataType.None;
            }
        }


        public DataDescriptor(HttpContext context)
        {
            Width = 320;
            Height = 240;

            this.context = context;

            SetResolution();
        }

        private void SetResolution()
        {
            int width = GetWidth();
            int height = GetHeight();

            if (width > 0 && height > 0)
            {
                this.Width = width;
                this.Height = height;
            }
        }

        private int GetWidth()
        {
            int width = -1;

            if (!String.IsNullOrEmpty(context.Request.QueryString[QueryParams.Width]))
            {
                String widthString = context.Request.QueryString[QueryParams.Width];
                Int32.TryParse(widthString, out width);
            }

            return width;
        }

        private int GetHeight()
        {
            int height = -1;

            if (!String.IsNullOrEmpty(context.Request.QueryString[QueryParams.Height]))
            {
                String widthString = context.Request.QueryString[QueryParams.Height];
                Int32.TryParse(widthString, out height);
            }

            return height;
        }

        private bool IsDataTypeImage()
        {
            String path = this.Path;

            if (path.EndsWith("jpg"))
                return true;

            if (path.EndsWith("png"))
                return true;

            if (path.EndsWith("bmp"))
                return true;

            return false;
        }

        private bool IsDataTypeVideo()
        {
            String path = this.Path;

            if (path.EndsWith("mkv"))
                return true;

            if (path.EndsWith("avi"))
                return true;

            if (path.EndsWith("mp4"))
                return true;

            return false;
        }
    }
}
