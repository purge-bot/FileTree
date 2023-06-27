using System;
using System.IO;

namespace FileTree.Model
{
    public class FileModel
    {
        public DirectoryContainer Directory;

        public string Type { get { return _descriptions[^1]; } }
        public string PATH { get; set; }

        private string[] _descriptions;
        private string _name;
        public string Name
        {
            get { return _name; }
            private set
            {
                _descriptions = value.Split('.');
                string partial = "";
                for (int i = 0; i < _descriptions.Length - 1; i++)
                {
                    if (partial != _descriptions[^1])
                    {
                        partial += "." + _descriptions[i];
                    }
                }
                _name = partial.Split('\\')[^1];
            }
        }

        private long _size;

        public FileModel(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (!File.Exists(path))
                throw new FileNotFoundException();

            PATH = path;
            Name = path;
            FileInfo fileInfo = new FileInfo(PATH);
            Size = fileInfo.Length;
            MimeType = MimeTypes.MimeTypeMap.GetMimeType(Type);

        }

        public long Size
        {
            get { return _size / 1024; }
            private set { _size = value; }
        }

        public string MimeType { get; set; }
    }
}
