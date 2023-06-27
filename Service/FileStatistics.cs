using FileTree.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileTree.Service
{
    public class FileStatistics
    {
        public long TotalSize;
        public int FileNumber;
        public Dictionary<string, List<FileModel>> FilesOfType = new Dictionary<string, List<FileModel>>();
        public Dictionary<string, long> TypesOfSize = new Dictionary<string, long>();
        private List<FileModel> _files = new List<FileModel>();
        public FileStatistics(List<FileModel> files)
        {
            _files = files.ToList();
            TypeInfo();
            SizeInfo();
        }

        private void TypeInfo()
        {
            FileNumber = _files.Count();
            FilesOfType = new Dictionary<string, List<FileModel>>();
            foreach (var file in _files)
            {
                if (FilesOfType.Keys.Contains(file.MimeType))
                {
                    FilesOfType[file.MimeType].Add(file);
                }
                else
                {
                    FilesOfType.Add(file.MimeType, new List<FileModel>());
                    FilesOfType[file.MimeType].Add(file);
                }
            }
        }
        private void SizeInfo()
        {
            foreach (var item in FilesOfType)
            {
                long size = 0;
                foreach (var file in item.Value)
                {
                    size += file.Size;
                }
                TypesOfSize.Add(item.Key, size);
                TotalSize += size;
            }
        }
    }
}
