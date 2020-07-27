using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_Simulator.Interface
{
    public interface ISaveToDBFormRequester
    {
        (bool, string) SaveRepositoryToDB(string name);
    }
}
