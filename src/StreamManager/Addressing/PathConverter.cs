using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Golem2.Manager.Addressing.Util;

namespace Golem2.Manager.Addressing
{
    public class PathConverter
    {
        static PathConverter instance;

        Dictionary<String, String> nameToPhysicalTable = new Dictionary<string, string>();
        Dictionary<String, String> physicalToNameTable = new Dictionary<string, string>();
        Dictionary<String, List<String>> mergeTable = new Dictionary<string, List<string>>();

        public static PathConverter GetInstance()
        {
            if (instance == null)
                instance = new PathConverter();

            return instance;
        }

        private PathConverter()
        {
            Settings.SettingsManager settings = Settings.SettingsManager.GetInstance();

            this.Clear();

            foreach (var library in settings.Libraries)
            {
                this.AddLibrary(library.LibraryName, library.LibraryMapFolder);
            }

            foreach (var mergeLibrary in settings.LibrariesMergeList)
            {
                foreach (var lib in mergeLibrary.MergeLibrariesName)
                {
                    this.MergeLibraries(lib, mergeLibrary.LibraryName);
                }
            }
        }

        public void AddLibrary(String virtualName, String physicalPath)
        {
            if (!nameToPhysicalTable.ContainsKey(virtualName))
            {
                nameToPhysicalTable.Add(virtualName, physicalPath);
                physicalToNameTable.Add(physicalPath, virtualName);
            }
        }
        public void MergeLibraries(String mergedLibrary, String intoLibrary)
        {
            if (!mergeTable.ContainsKey(intoLibrary))
                mergeTable.Add(intoLibrary, new List<String>());

            mergeTable[intoLibrary].Add(mergedLibrary);
        }
        public void Clear()
        {
            nameToPhysicalTable.Clear();
            physicalToNameTable.Clear();
            mergeTable.Clear();
        }

        public bool IsValidPath(string virtualPath)
        {
            if (String.IsNullOrEmpty(virtualPath))
                return false;

            bool valid = true;

            valid &= virtualPath[0] == '/' ;

            if (!valid)
                return valid;

            string physicalPath = ConvertToPhysicalPath(virtualPath);

            valid &= (Directory.Exists(physicalPath) || File.Exists(physicalPath));

            return valid;
        }

        public String GetParentDirectory(String virtualPath)
        {
            int slashIndex = virtualPath.LastIndexOf("/");

            if (slashIndex == -1 || String.IsNullOrEmpty(virtualPath))
                return String.Empty;

            return virtualPath.Substring(0, slashIndex);
        }

        public String CorrectPath(String virtualPath)
        {
            if (String.IsNullOrEmpty(virtualPath))
                return null;

            string physicalPathToLibrary = ConvertToPhysicalPath(virtualPath);

            virtualPath = virtualPath.Remove(0, 1);
            int index = virtualPath.IndexOf('/');

            string libraryName = virtualPath;

            if (index != -1)
                libraryName = virtualPath.Substring(0, index);

            if (mergeTable.ContainsKey(libraryName))
            {
                if (!Directory.Exists(physicalPathToLibrary) && !File.Exists(physicalPathToLibrary))
                {
                    foreach (var lib in mergeTable[libraryName])
                    {
                        if (virtualPath != String.Empty)
                            physicalPathToLibrary = String.Format("{0}\\{1}", nameToPhysicalTable[lib], virtualPath.Replace('/', '\\').Replace(libraryName, String.Empty));

                        if (Directory.Exists(physicalPathToLibrary) || File.Exists(physicalPathToLibrary))
                            return String.Format("/{0}", virtualPath.Replace(libraryName, lib));
                    }
                }
            }

            return null;
        }

        public String ConvertToVirtualPath(String physicalPath)
        {
            String virtualPath = "/";

            if (!String.IsNullOrEmpty(physicalPath))
            {
                if (physicalPath[physicalPath.Length - 1] == '\\')
                    physicalPath = physicalPath.Substring(0, physicalPath.Length - 1);

                foreach (var mapRecord in physicalToNameTable)
                {
                    if (physicalPath.StartsWith(mapRecord.Key))
                    {
                        virtualPath = String.Format("{0}{1}", virtualPath, mapRecord.Value);

                        physicalPath = physicalPath.Remove(0, mapRecord.Key.Length);
                        physicalPath.Replace('\\', '/');

                        if (!String.IsNullOrEmpty(physicalPath))
                            virtualPath = String.Format("{0}\\{1}", virtualPath, physicalPath);

                        return virtualPath;
                    }
                }
            }

            return virtualPath;
        }
        public String ConvertToPhysicalPath(String virtualPath)
        {
            virtualPath = virtualPath.Remove(0, 1);
            int index = virtualPath.IndexOf('/');

            string libraryName = virtualPath;

            if (index != -1)
            {
                libraryName = virtualPath.Substring(0, index);
                virtualPath = virtualPath.Replace('/', '\\');
            }

            virtualPath = virtualPath.Remove(0, libraryName.Length);

            string physicalPathToLibrary = nameToPhysicalTable[libraryName];
            
            if (virtualPath != String.Empty)
                physicalPathToLibrary = String.Format("{0}\\{1}", physicalPathToLibrary, virtualPath);

            return physicalPathToLibrary;
        }

        public List<String> ConvertToVirtualPath(List<String> physicalPaths)
        {
            List<String> virtualPaths = (from q in physicalPaths select ConvertToVirtualPath(q)).ToList();
            return virtualPaths;
        }
        public List<String> ConvertToPhysicalPath(List<String> virtualPaths)
        {
            List<String> physicalPaths = (from q in virtualPaths select ConvertToPhysicalPath(q)).ToList();
            return physicalPaths;
        }

        public List<String> GetFoldersPhysicalPathInVirtual(String virtualPath)
        {
            string physicalPath = ConvertToPhysicalPath(virtualPath);

            return GetFoldersPhysicalPathInPhysical(physicalPath);
        }
        public List<String> GetFoldersVirtualPathInVirtual(String virtualPath)
        {
            return ConvertToVirtualPath(GetFoldersPhysicalPathInVirtual(virtualPath));
        }
        
        public List<String> GetFilesPhysicalPathInVirtual(String virtualPath)
        {
            string physicalPath = ConvertToPhysicalPath(virtualPath);

            return GetFilesPhysicalPathInPhysical(physicalPath);
        }
        public List<String> GetFilesVirtualPathInVirtual(String virtualPath)
        {
            return ConvertToVirtualPath(GetFilesPhysicalPathInVirtual(virtualPath));
        }

        public List<String> GetFoldersPhysicalPathInPhysical(String physicalPath)
        {
            List<String> physicalPaths = new List<string>();

            physicalPaths.AddRange(Directory.GetDirectories(physicalPath));

            String libraryName = ConvertToVirtualPath(physicalPath);
            if (libraryName[0] == '/')
            {
                libraryName = libraryName.Remove(0, 1);

                if (libraryName.IndexOf('/') == -1)
                {
                    if (mergeTable.ContainsKey(libraryName))
                    {
                        foreach (var lib in mergeTable[libraryName])
                        {
                            physicalPaths.AddRange(Directory.GetDirectories(ConvertToPhysicalPath("/" + lib)));
                        }
                    }
                }
                
            }

            return physicalPaths;
        }
        public List<String> GetFoldersVirtualPathInPhysical(String physicalPath)
        {
            return ConvertToVirtualPath(GetFoldersPhysicalPathInPhysical(physicalPath));
        }

        public List<String> GetFilesPhysicalPathInPhysical(String physicalPath)
        {
            List<String> physicalPaths = new List<string>();

            physicalPaths.AddRange(Directory.GetFiles(physicalPath));

            return physicalPaths;

        }
        public List<String> GetFilesVirtualPathInPhysical(String physicalPath)
        {
            return ConvertToVirtualPath(GetFilesPhysicalPathInPhysical(physicalPath));
        }

        public List<T> SortByNumeric<T>(List<T> files)
        {
            NumericComparer nc = new NumericComparer();
            T[] temp = files.ToArray();
            Array.Sort(temp, nc);
            return temp.ToList();
        }
    }
}
