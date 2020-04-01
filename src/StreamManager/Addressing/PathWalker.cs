using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Golem2.Manager.Metadata.Util;
using System.IO;
using Golem2.Manager.TVShow.Metadata;

namespace Golem2.Manager.Addressing
{
    public class PathWalker
    {
        public class FolderInfo
        {
            public bool IsFile
            {
                get;
                private set;
            }

            public String PhysicalPath
            {
                get;
                private set;
            }

            public String VirtualPath
            {
                get;
                private set;
            }

            public String FolderName
            {
                get;
                private set;
            }

            public String FolderDiplayName
            {
                get;
                internal set;
            }

            public String Overview
            {
                get;
                internal set;
            }

            public ImageDescriptor FolderImage
            {
                get;
                internal set;
            }
            public ImageDescriptor BackdropImage
            {
                get;
                internal set;
            }

            public String[] SubtitleLanguages
            {
                get;
                internal set;
            }

            public FolderInfo(String physicalPath, bool IsFile)
            {
                PathConverter pathConverter = PathConverter.GetInstance();

                this.SubtitleLanguages = new String[0];
                this.IsFile = IsFile;
                this.PhysicalPath = physicalPath;
                this.VirtualPath = pathConverter.ConvertToVirtualPath(physicalPath);
                this.FolderName = System.IO.Path.GetFileName(physicalPath);
                this.FolderDiplayName = this.FolderName;

                String folderImagePath = String.Format("{0}\\{1}", this.PhysicalPath, "folder.jpg");

                int width = 120;
                int height = 160;

                if (IsFile)
                {
                    width = 200;
                    height = 112;
                }

                if (File.Exists(folderImagePath))
                    this.FolderImage = new ImageDescriptor(String.Format("{0}/folder.jpg", this.VirtualPath), width, height);
                else
                    this.FolderImage = new ImageDescriptor("Resources/unknown.jpg", width, height);

                folderImagePath = String.Format("{0}\\{1}", this.PhysicalPath, "backdrop.jpg");

                if (File.Exists(folderImagePath))
                    this.BackdropImage = new ImageDescriptor(String.Format("{0}/backdrop.jpg", this.VirtualPath), width, height);
                else
                    this.BackdropImage = new ImageDescriptor("Resources/unknown.jpg", width, height);


            }

            public override string ToString()
            {
                return FolderDiplayName;
            }

        }


        static Settings.SettingsManager settings = Settings.SettingsManager.GetInstance();

        public String VirtualAddress
        {
            get
            {
                if (String.IsNullOrEmpty(virtualAddress))
                    return String.Format("/{0}", settings.DefaultLibrary);

                return this.virtualAddress;
            }
        }

        String virtualAddress = String.Empty;

        public PathWalker(String virtualAddress)
        {
            this.virtualAddress = virtualAddress;
        }

        public bool IsPlayable(out String parentVirtualDirectory)
        {
            PathConverter pathConverter = PathConverter.GetInstance();
            if (!pathConverter.IsValidPath(this.virtualAddress))
                this.virtualAddress = pathConverter.CorrectPath(this.virtualAddress);

            if (this.virtualAddress == null)
                this.virtualAddress = String.Format("/{0}", settings.DefaultLibrary);

            bool valid = settings.PlayFilter.Test(this.virtualAddress);

            if (valid)
            {
                parentVirtualDirectory = pathConverter.GetParentDirectory(this.virtualAddress);
            }
            else
            {
                parentVirtualDirectory = this.virtualAddress;
            }

            return valid;
        }

        public List<FolderInfo> GetFileAndFolders()
        {
            List<FolderInfo> data = GetFolders();
            data.AddRange(GetFiles());

            return data;
        }

        public List<FolderInfo> GetFolders()
        {
            List<FolderInfo> folders = new List<FolderInfo>();

            PathConverter pathConverter = PathConverter.GetInstance();

            if (!pathConverter.IsValidPath(this.virtualAddress))
                this.virtualAddress = pathConverter.CorrectPath(this.virtualAddress);

            List<string> folderPaths = pathConverter.GetFoldersPhysicalPathInVirtual(this.virtualAddress);

            foreach (var path in folderPaths)
            {
                if (settings.FolderFilter.Test(path))
                {
                    FolderInfo folderInfo = new FolderInfo(path, false);
                    folders.Add(folderInfo);
                }
            }

            folders = pathConverter.SortByNumeric<FolderInfo>(folders);

            return folders;
        }

        public List<FolderInfo> GetFiles()
        {
            List<FolderInfo> folders = new List<FolderInfo>();

            PathConverter pathConverter = PathConverter.GetInstance();
            List<string> folderPaths = pathConverter.GetFilesPhysicalPathInVirtual(this.virtualAddress);

            folderPaths = pathConverter.SortByNumeric(folderPaths);

            String physicalPath = pathConverter.ConvertToPhysicalPath(this.virtualAddress);

            SeasonInfo parser = new SeasonInfo(physicalPath);

            foreach (var path in folderPaths)
            {
                if (settings.FileFilter.Test(path))
                {
                    FolderInfo folderInfo = new FolderInfo(path, true);

                    foreach (var episode in parser.Episodes)
                    {
                        if (Path.GetFileNameWithoutExtension(folderInfo.FolderName) == episode.Filename)
                        {
                            string imgVirtPath = String.Format("{0}/metadata/{1}", this.virtualAddress, episode.EpisodeImage.ImagePhysicalPath);

                            String imgPhysPath = pathConverter.ConvertToPhysicalPath(imgVirtPath);

                            if (!File.Exists(imgPhysPath))
                            {
                                imgVirtPath = "Resources/unknown.jpg";
                            }

                            folderInfo.FolderImage.ImagePhysicalPath = imgVirtPath;
                            folderInfo.FolderDiplayName = String.Format("{0}) {1}", episode.EpisodeNumber, episode.EpisodeName);
                            folderInfo.Overview = episode.EpisodeOverview;
                            folderInfo.SubtitleLanguages = episode.SubtitleLanguages;
                            break;
                        }
                    }

                    folders.Add(folderInfo);
                }
            }

            return folders;
        }

    }
}
