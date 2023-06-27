using FileTree.Infrastructure;
using FileTree.Service;
using FileTree.View;
using HtmlAgilityPack;
using System;
using System.Drawing;
using System.IO;


namespace FileTree
{

    internal class Program
    {
        static private readonly string _path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
        static void Main(string[] args)
        {
            FileSelection fileSelection = new FileSelection(_path);
            fileSelection.CreateTree(_path);

            HtmlDocument doc = new HtmlDocument();

            Layout layout = new Layout(doc);
            doc.LoadHtml(Layout.BaseHtml);

            //Добавление стилей
            var styleNode = doc.CreateElement("style");

            Bitmap icon = ShellIcon.GetSmallIcon(fileSelection.Directories[0].PATH).ToBitmap();
            ImageConverter converter = new ImageConverter();
            var img = (byte[])converter.ConvertTo(icon, typeof(byte[]));

            styleNode.InnerHtml = layout.Style + "\r\n.root summary::before{background  : url('" + $"data:application/octet-stream;base64,{Convert.ToBase64String(img)}')}}";
            layout.SetStyle(styleNode);

            //Запись дерева файлов в Html
            var root = doc.GetElementbyId("filesRoot");
            layout.CreateTree(fileSelection, fileSelection.PATH, root);

            FileStatistics statistics = new FileStatistics(fileSelection.Files);

            layout.Save(_path + "/doc.html");
        }
    }
}
