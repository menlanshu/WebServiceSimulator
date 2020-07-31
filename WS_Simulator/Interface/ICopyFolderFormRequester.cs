using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS_Simulator.Models;

namespace WS_Simulator.Interface
{
    public interface ICopyFolderFormRequester
    {
        (bool okay, string errDesc) SaveFolderToTreeNode(Node motherNode, Node folderNode, 
            string newFolderName, string oldText = "", string newText = "");
    }
}
