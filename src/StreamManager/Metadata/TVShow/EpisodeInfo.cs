using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Golem2.Manager.Metadata.Util;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Golem2.Manager.TVShow.Metadata
{
    public class EpisodeInfo
    {
        public ImageDescriptor EpisodeImage
        {
            get;
            private set;
        }

        public String EpisodeNumber
        {
            get;
            private set;
        }

        public String EpisodeName
        {
            get;
            private set;
        }

        public String SeasonNumber
        {
            get;
            private set;
        }

        public String EpisodeOverview
        {
            get;
            private set;
        }

        public String[] SubtitleLanguages
        {
            get;
            private set;
        }

        public String Filename
        {
            get;
            private set;
        }

        public EpisodeInfo(String filename, ImageDescriptor episodeImage, String episodeNumber, String episodeName, String seasonNumber, String episodeOverview, String[] subtitleLanguages)
        {
            this.EpisodeImage = episodeImage;
            this.EpisodeNumber = episodeNumber;
            this.EpisodeName = episodeName;
            this.SeasonNumber = seasonNumber;
            this.EpisodeOverview = episodeOverview;
            this.Filename = filename;
            this.SubtitleLanguages = subtitleLanguages;
        }

        public EpisodeInfo(String xmlFilename)
        {
            XmlDataDocument doc = new XmlDataDocument();
            doc.Load(xmlFilename);

            string[] subtitleLanguages = DetectSubtitleLanguages(xmlFilename);

            XmlElement item = doc["Item"];

            String imageFilename = String.Format("{0}.jpg", item["id"].InnerText);
            String episodeNumber = item["EpisodeNumber"].InnerText;
            String episodeName = item["EpisodeName"].InnerText;
            String seasonNumber = item["SeasonNumber"].InnerText;
            String episodeOverview = item["Overview"].InnerText;
            String filename = Path.GetFileNameWithoutExtension(xmlFilename);

            int width = 120;
            int height = 160;

            String imageFilePath = String.Format("{0}\\{1}", Path.GetFullPath(xmlFilename), imageFilename);
            try
            {
                if (File.Exists(imageFilePath))
                {
                    using (Image img = Image.FromFile(imageFilePath))
                    {
                        width = img.Width;
                        height = img.Height;
                    }
                }
            }
            catch (OutOfMemoryException ex)
            {
                //dosla pamet, nevadi, pouziji se defaultni rozmery, pravdepodobne poskozeny obrazek ...
            }
            
            this.Filename = filename;
            this.EpisodeImage = new ImageDescriptor(imageFilename, width, height);
            this.EpisodeNumber = episodeNumber;
            this.EpisodeName = episodeName;
            this.SeasonNumber = seasonNumber;
            this.EpisodeOverview = episodeOverview;
            this.SubtitleLanguages = subtitleLanguages;
        }

        private string[] DetectSubtitleLanguages(String xmlFilename)
        {
            Regex regEx = new Regex("(?<SubtitleLanguage>[a-zA-Z]{2,3}Sub)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection matches = regEx.Matches(xmlFilename);

            String[] output = new String[matches.Count];
            int i = 0;
            foreach (Match m in matches)
            {
                output[i] = m.Groups["SubtitleLanguage"].Value.ToLower().Replace("sub", String.Empty);
                i++;
            }

            return output;
        }
    }
}
