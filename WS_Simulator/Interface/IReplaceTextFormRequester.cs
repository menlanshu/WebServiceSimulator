using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS_Simulator.Models;

namespace WS_Simulator.Interface
{
    public interface IReplaceTextFormRequester
    {
        (bool okay, string errDesc) ReplaceTextFileInfo(Node folderNode, string oldText = "", string newText = "");
        (bool okay, string errDesc) ReplaceTextFileName(Node folderNode, string oldText = "", string newText = "");
    }
}
