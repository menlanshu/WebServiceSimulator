using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WS_Simulator.Models;

namespace WS_Simulator.FormHandler
{
    public static class SimulatorFormHandler
    {

        public static char[] ConfigDelimeter = (";").ToCharArray();

        public static async Task InitMethodNameOfWebService(WSConfig wSConfig, TreeNodeCollection treeNodes)
        {
            //comboBox.Items.Clear();
            wSConfig.WSMethodList.Clear();

            foreach (TreeNode node in treeNodes)
            {
                await AddMethoName(node, wSConfig);
            }
        }

        public static async Task AddMethoName(TreeNode inTreeNode, WSConfig wSConfig)
        {
            if (inTreeNode.Nodes.Count == 0)
            {
                wSConfig.WSMethodList.Add(inTreeNode.Text);
            }
            else
            {
                foreach (TreeNode tempNode in inTreeNode.Nodes)
                {
                    await AddMethoName(tempNode, wSConfig);
                }
            }
        }

        public static bool NeedWait(string fileName, List<string> needWaitmessageList)
        {
            if (fileName == "")
            {
                return false;
            }
            else
            {
                foreach (string tempStr in needWaitmessageList)
                {
                    if (fileName.Contains(tempStr))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
