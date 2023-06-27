using System.Collections.Generic;

namespace FileTree.Model
{
    public class DirectoryContainer
    {
        public string PATH;
        public string ParentDirectory;
        public List<FileModel> Files = new List<FileModel>();
        public int Level;
    }
}
