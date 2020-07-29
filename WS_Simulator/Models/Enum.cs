using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_Simulator.Models
{
   public enum ConfigType
    {
        CONN,
        APP
    }

    public enum TestNodeType
    {
        NORMAL,
        START,
        END
    }

    public enum SourceNodeType
    {
        DB,
        LOCAL
    }

    public enum TreeNodeType
    {
        Root,
        Directory,
        File
    }

    public enum TestStatus
    {
        WaitForSend,
        Sending,
        Sended
    }
}
