using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WS_Simulator.Models;

namespace WS_Simulator.Interface
{
    public interface ISaveSingleFieToDB
    {
        bool RequestSaveFile(Node newNode);
    }
}
