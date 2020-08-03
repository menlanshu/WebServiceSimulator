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
    public abstract class Node
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

        public abstract void SaveReplyResult(string currentNodeReplyMessage);

        public abstract string GetCurrentMessage(bool updateControl);
        public abstract void UpdateCurrentMessage(string requestMessage);

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
