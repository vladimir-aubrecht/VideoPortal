using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Golem2.Manager.Metadata.Util;
using System.Drawing;

namespace Golem2.Manager.TVShow.Metadata
{
    public class SeasonInfo
    {
        public EpisodeInfo[] Episodes
        {
            get;
            private set;
        }

        public ImageDescriptor PosterImage
        {
            get;
            private set;
        }

        public ImageDescriptor BackdropImage
        {
            get;
            private set;
        }

        Settings.SettingsManager settings = Settings.SettingsManager.GetInstance();

        public SeasonInfo(String physicalPath)
        {
            LoadSeasonImages(physicalPath);

            LoadEpisodes(physicalPath);
        }

        private void LoadSeasonImages(String physicalPath)
        {
            String backdropPath = String.Format("{0}\\backdrop.jpg", physicalPath);
            String posterPath = String.Format("{0}\\folder.jpg", physicalPath);

            this.PosterImage = new ImageDescriptor(posterPath, settings.DefaultPosterSize.Width, settings.DefaultPosterSize.Height);
            this.BackdropImage = new ImageDescriptor(backdropPath, settings.DefaultBackdropSize.Width, settings.DefaultBackdropSize.Height);
        }

        private void LoadEpisodes(String physicalPath)
        {
            physicalPath = String.Format("{0}\\metadata", physicalPath);
            List<EpisodeInfo> episodes = new List<EpisodeInfo>();

            if (Directory.Exists(physicalPath))
            {
                String[] xmlFiles = Directory.GetFiles(physicalPath, "*.xml");

                foreach (var xmlFile in xmlFiles)
                {
                    if (xmlFile.StartsWith("."))
                        continue;

                    EpisodeInfo episode = new EpisodeInfo(xmlFile);
                    episodes.Add(episode);
                }

            }

            this.Episodes = episodes.ToArray();
        }
    }
}
