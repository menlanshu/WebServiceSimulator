using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_Simulator.Models
{
    public class TestRepository
    {
        public int Id { get; set; }
        public string RepositoryName { get; set; }
        public List<Node> TestNodeList { get; set; }
    }
}
