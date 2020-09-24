﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WS_Simulator.Models
{
    public class Node
    {
        public int Id { get; set; }
        public string TreeNodeName { get; set; }
        [NotMapped]
        public TreeNode TreeNodeValue { get; set; }
        public string NodeFullPath { get; set; }
        public string TreeNodeMessage { get; set; }
        public string TreeNodeReplyMessage { get; set; }

        public int? MotherNodeId { get; set; }
        public Node MotherNode { get; set; }
        public TreeNodeType TreeNodeType { get; set; }
        [NotMapped]
        public TestNodeType TreeNodeTestType { get; set; } = TestNodeType.NORMAL;
        [NotMapped]
        public SourceNodeType TreeNodeSourceType { get; set; }
        public int? RepositoryId { get; set; }
        public TestRepository Repository { get; set; }

        public FileType FileType {
            get {
                string PostFix = TreeNodeName.Substring(TreeNodeName.LastIndexOf('.')+1).ToUpper();
                switch(PostFix)
                {
                    case "SQL":
                        return FileType.SQL;
                    case "XML":
                        return FileType.XML;
                    case "TXT":
                        return FileType.TEXT;
                    default:
                        return FileType.OTHER;
                }
            }
        }

        public Node(TreeNodeType treeNodeType, TreeNode treeNodeValue, Node motherNode, string fullPath)
        {
            TreeNodeType = treeNodeType;
            TreeNodeValue = treeNodeValue;
            TreeNodeName = treeNodeValue?.Text;

            MotherNode = motherNode;

            NodeFullPath = fullPath;
        }

        public Node()
        {
        }

        public virtual void SaveReplyResult(string currentNodeReplyMessage) { throw new NotImplementedException(); }
        public virtual string GetCurrentMessage(bool updateControl) { throw new NotImplementedException(); }
        public virtual void UpdateCurrentMessage(string requestMessage) { throw new NotImplementedException(); }
        public virtual void RenameFile(string newFileName) { throw new NotImplementedException(); }
        public virtual void DeleteFile() { throw new NotImplementedException(); }

            //public string GetFullPathOfFile(string rootPath)
            //{
            //    string outputPath = "";
            //    switch (this.TreeNodeType)
            //    {
            //        case TreeNodeType.Root:
            //            outputPath = $"{rootPath}";
            //            break;
            //        case TreeNodeType.Directory:
            //            outputPath = $@"{rootPath}\{NodeFullPath.Substring(8)}";
            //            break;
            //        case TreeNodeType.File:
            //            outputPath = $@"{rootPath}\{NodeFullPath.Substring(8)}\{TreeNodeName}";
            //            break;
            //        default:
            //            break;
            //    }

            //    return outputPath;
            //}
        }
}
