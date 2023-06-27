using FileTree.Infrastructure;
using FileTree.Model;
using FileTree.Service;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FileTree.View
{
    public class Layout
    {
        public HtmlDocument Document { get; private set; }
        public Layout(HtmlDocument doc)
        {
            Document = doc;
        }

        public const string BaseHtml = @"﻿<!DOCTYPE html>
<html>
    <head>
        <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC"" crossorigin=""anonymous"">
    </head>
    <body>
        <div class=""d-flex align-items-center justify-content-center container"" style=""min-height:100vh;"">
            <div class=""col border rounded"">
                <div class=""row m-2"">
                    <div class=""col-6 col-md-6"" id=""filesRoot"">  
                    </div>
                    <div class=""col-6 col-md-6"" id=""stats"">  
                    </div>
                </div>
            </div>
        </div>
    </body>
</html>";

        public string Style = @"            
            .root li{
              display      : block;
              position     : relative;
              padding-left : 35px;
            }

            .root ul{
              padding-left : 0;
            }

            .root summary{
              display : block;
              cursor  : pointer;
            }

            .root summary::marker{
              display : none;
            }

            .root li::after,
            .root summary::before{
              content       : '';
              display       : block;
              position      : absolute;
	      top           : 3px;
              left          : 16px;
              width         : 16px;
              height        : 16px;
            }";

        public void SetStyle(HtmlNode node)
        {
            var head = Document.DocumentNode.SelectSingleNode("//head");
            head.ChildNodes.Add(node);
        }
        public void AddFiles(List<FileModel> files, HtmlNode parentNode)
        {
            files = files.OrderBy(p => p.MimeType).ToList();
            foreach (var file in files)
            {
                var ul = Document.CreateElement("ul");
                var li = Document.CreateElement("li");
                if (file.Name.Count() > 20)
                {
                    li.InnerHtml = file.Name.Remove(20) + "...";
                }
                else
                {
                    li.InnerHtml = file.Name + "." + file.Type;

                }
                li.SetAttributeValue("title", file.Name + "." + file.Type);

                Bitmap icon = ShellIcon.GetSmallIcon(file.PATH).ToBitmap();
                ImageConverter converter = new ImageConverter();
                var img = (byte[])converter.ConvertTo(icon, typeof(byte[]));

                li.SetAttributeValue("style", "background: url('" + $"data:application/octet-stream;base64,{Convert.ToBase64String(img)}');height: 20px;background-repeat: no-repeat; padding-left:20px;");
                ul.AppendChild(li);
                parentNode.AppendChild(ul);
            }
        }
        public HtmlNode AddDirectory(HtmlNode parentNode, string path, FileSelection fileSelection)
        {
            var ul = Document.CreateElement("ul");
            var li = Document.CreateElement("li");
            var details = Document.CreateElement("details");
            var summary = Document.CreateElement("summary");

            //if (parentNode.Name == "body")
            if (fileSelection.PATH == path)
            {
                ul.AddClass("root");
            }
            summary.InnerHtml = path.Split('\\')[^1];
            details.AppendChild(summary);
            li.AppendChild(details);
            ul.AppendChild(li);
            parentNode.AppendChild(ul);
            return details;
        }

        public void CreateTree(FileSelection fileSelection, string path, HtmlNode parentNode)
        {
            var files = fileSelection.Directories.FirstOrDefault(p => p.PATH == path).Files;
            HtmlNode newParentNode = AddDirectory(parentNode, path, fileSelection);
            AddFiles(files, newParentNode);

            var subDirectory = fileSelection.Directories.FindAll(p => p.ParentDirectory == path);
            foreach (var dir in subDirectory)
            {
                CreateTree(fileSelection, dir.PATH, newParentNode);
            }
        }

        public void Save(string path)
        {
            Document.Save(path);
        }
    }
}
