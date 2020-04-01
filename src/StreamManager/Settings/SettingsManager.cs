using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Configuration;

namespace Golem2.Manager.Settings
{
    public class SettingsManager
    {
        internal class LibraryMapingPair
        {
            public String LibraryName
            {
                get;
                private set;
            }

            public String LibraryMapFolder
            {
                get;
                private set;
            }

            public LibraryMapingPair(String libraryName, String libraryMapFolder)
            {
                this.LibraryName = libraryName;
                this.LibraryMapFolder = libraryMapFolder;
            }
        }
        internal class LibraryMergePair
        {
            public String LibraryName
            {
                get;
                private set;
            }

            public String[] MergeLibrariesName
            {
                get;
                private set;
            }

            public LibraryMergePair(String libraryName, params String[] mergeLibrariesName)
            {
                this.LibraryName = libraryName;
                this.MergeLibrariesName = mergeLibrariesName;
            }
        }
        
        public String VirtualTempPath
        {
            get;
            private set;
        }
        public String VirtualFFMpegPath
        {
            get;
            private set;
        }
        public Image UnknownImage
        {
            get
            {
                return global::Manager.Properties.Resources.unknown;
            }
        }

        public String DefaultLibrary
        {
            get;
            private set;
        }

        public String Domain
        {
            get;
            private set;
        }

        public bool ForceDefaultImagesSize
        {
            get;
            private set;
        }

        public Size DefaultPosterSize
        {
            get;
            private set;
        }
        public Size DefaultBackdropSize
        {
            get;
            private set;
        }
        public Size DefaultThumbnailSize
        {
            get;
            private set;
        }

        internal CommonFilter FolderFilter
        {
            get { return folderFilter; }
        }
        internal CommonFilter FileFilter
        {
            get { return fileFilter; }
        }
        internal CommonFilter PlayFilter
        {
            get { return playFilter; }
        }

        private CommonFilter folderFilter = new CommonFilter(@"\$.+", @".*\.tmp", "System Volume Information", "metadata", "Subtitles");
        private CommonFilter fileFilter = new CommonFilter(@".+\.config", @".+\.srt", @".+\.jpg", @".*\.wc", @".*\.DS_Store", @".*\.db", @".+\.xml", @".*\.ico", @".*\.sub", @".*\.nfo", @".*\.ogm");
        private CommonFilter playFilter = new CommonFilter(true, @".+\.mkv", @".+\.avi", @".+\.mp4");

        internal LibraryMapingPair[] Libraries
        {
            get;
            private set;
        }

        internal LibraryMergePair[] LibrariesMergeList
        {
            get;
            private set;
        }

        private static SettingsManager instance;

        public static SettingsManager GetInstance()
        {
            if (instance == null)
                instance = new SettingsManager();

            return instance;
        }

        private SettingsManager()
        {
            this.VirtualTempPath = "~/Cache";
            this.VirtualFFMpegPath = "~/Bin/ffmpeg.exe";

            this.DefaultPosterSize = new Size(120, 160);
            this.DefaultBackdropSize = new Size(120, 160);
            this.DefaultThumbnailSize = new Size(200, 112);

            this.ForceDefaultImagesSize = true;

            var settings = ConfigurationSettings.AppSettings;

            List<LibraryMapingPair> libraries = new List<LibraryMapingPair>();
            List<LibraryMergePair> mergePairs = new List<LibraryMergePair>();
            for (int i = 0; i < settings.Count; i++)
            {
                var key = settings.GetKey(i);

                if (key.StartsWith("libraryName:"))
                {
                    key = key.Replace("libraryName:", String.Empty);

                    var value = settings[i];

                    var paths = value.Split(';');

                    List<String> librariesNames = new List<string>();
                    for (int j = 0; j < paths.Length; j++)
                    {
                        librariesNames.Add(String.Format("{0}{1}", key, (j == 0) ? String.Empty : j.ToString()));
                        libraries.Add(new LibraryMapingPair(librariesNames[j], paths[j]));
                    }

                    librariesNames.RemoveAt(0);
                    mergePairs.Add(new LibraryMergePair(key, librariesNames.ToArray()));
                }
                else if (key.StartsWith("defaultLibrary:"))
                {
                    key = key.Replace("defaultLibrary:", String.Empty);

                    this.DefaultLibrary = key;
                }
                else if (key.StartsWith("domain:"))
                {
                    key = key.Replace("domain:", String.Empty);
                    this.Domain = key;
                }
            }

            this.Libraries = libraries.ToArray();
            this.LibrariesMergeList = mergePairs.ToArray();

        }

        public String GenerateUniqueNameForTempPath(String originalName)
        {
            originalName = originalName.Replace("\\", "-").Replace(":", "-");

            return originalName;
        }
    }
}
