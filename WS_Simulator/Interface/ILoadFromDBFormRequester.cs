using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS_Simulator.Models;

namespace WS_Simulator.Interface
{
    public interface ILoadFromDBFormRequester
    {
        void CovertTestRepositoryToTree(TestRepository testRepository);
    }
}
