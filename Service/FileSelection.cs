using FileTree.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileTree.Service
{
    public class FileSelection
    {
        public List<DirectoryContainer> Directories { get; private set; } = new List<DirectoryContainer>();
        public List<FileModel> Files { get; private set; } = new List<FileModel>();
        public string PATH { get; private set; }
        public DirectoryContainer RootDirectory { get; set; }

        public int MaxLevel;
        public FileSelection(string rootPath, bool getSubdirectories = false)
        {
            PATH = rootPath;
            RootDirectory = new DirectoryContainer() { PATH = rootPath, Level = 0 };
            Directories.Add(RootDirectory);
            MaxLevel = 0;
        }

        public void GetFiles(string directoryPath, DirectoryContainer dir)
        {
            string[] files = Directory.GetFiles(directoryPath);
            foreach (var item in files)
            {
                FileInfo file = new FileInfo(item);
                FileModel fileModel = new FileModel(file.FullName) { Directory = dir };
                Files.Add(fileModel);
                dir.Files.Add(fileModel);

            }
        }

        public List<DirectoryContainer> GetDirectories(string directoryPath, DirectoryContainer storage)
        {
            var directories = new DirectoryInfo(directoryPath).GetDirectories();
            List<DirectoryContainer> InnerDirectories = new List<DirectoryContainer>();
            foreach (var item in directories)
            {
                DirectoryContainer dir = new DirectoryContainer() { PATH = item.FullName, ParentDirectory = item.Parent.FullName, Level = Directories.FirstOrDefault(p => p.PATH == item.Parent.FullName).Level + 1 };
                Directories.Add(dir);
                InnerDirectories.Add(dir);
                if (MaxLevel < dir.Level)
                    MaxLevel++;
            }
            return InnerDirectories;
        }

        public void CreateTree(string dir)
        {
            GetFiles(dir, RootDirectory);
            List<DirectoryContainer> InnerDirectories = GetDirectories(dir, RootDirectory);
            foreach (var item in InnerDirectories)
            {
                CreateTree(item.PATH, item);
            }
        }
        private void CreateTree(string dir, DirectoryContainer storage)
        {
            GetFiles(dir, storage);
            List<DirectoryContainer> InnerDirectories = GetDirectories(dir, storage);
            foreach (var item in InnerDirectories)
            {
                CreateTree(item.PATH, item);
            }
        }
    }
}
