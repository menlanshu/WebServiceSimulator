using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using WS_Simulator.Models;

namespace WS_Simulator.DataAccess
{
    public static class SQLiteDBProcessor
    {
        private static SQLiteContext _sQLiteContext = new SQLiteContext();
        static SQLiteDBProcessor()
        {
            _sQLiteContext.Database.EnsureCreated();
        }

        public static void SaveDataToDB(TestRepository testRepository)
        {
            using (_sQLiteContext = new SQLiteContext())
            {
                _sQLiteContext.TestRespositories.Add(testRepository);
                _sQLiteContext.SaveChanges();
            }
        }

        public static bool CheckRepositoryName(string name)
        {
            using (_sQLiteContext = new SQLiteContext())
            {
                return (_sQLiteContext.TestRespositories.Where(x => x.RepositoryName == name).Count() > 0);
            }
        }

        public static string GetRequestMessageOfCurrentNode(Node node)
        {
            string outputMessage = "";
            using (_sQLiteContext = new SQLiteContext())
            {
                if (node.TreeNodeType == TreeNodeType.File)
                {
                    outputMessage = _sQLiteContext.NodeList.Where(x => x.Id == node.Id).FirstOrDefault()?.TreeNodeMessage;
                }
            }

            return outputMessage;
        }

        public static Node GetResultDirectoryNodeOfCurrentFolder(Node node, string resultFolderName)
        {
            Node output = null;
            using (_sQLiteContext = new SQLiteContext())
            {
                output = _sQLiteContext.NodeList.Where(x => x.MotherNodeId == node.MotherNodeId &&
                x.TreeNodeType == TreeNodeType.Directory && x.TreeNodeName == resultFolderName)
                    .Include(x => x.MotherNode).Include(x => x.Repository).FirstOrDefault();
            }

            return output;
        }

        public static Node GetChildNodeOfCurrentDirectory(Node node, string childNodeName)
        {
            Node output = null;
            using (_sQLiteContext = new SQLiteContext())
            {
                output = _sQLiteContext.NodeList.Where(x => x.MotherNodeId == node.Id &&
                x.TreeNodeType == TreeNodeType.File && x.TreeNodeName == childNodeName).FirstOrDefault();
            }

            return output;
        }

        public static void SaveOneNode(Node node)
        {
            using (_sQLiteContext = new SQLiteContext())
            {
                node.Repository = null;
                node.MotherNodeId = node.MotherNode?.Id;
                node.MotherNode = null;
                _sQLiteContext.NodeList.Add(node);
                _sQLiteContext.SaveChanges();
            }
        }

        public static void DeleteOneNode(Node node)
        {
            using (_sQLiteContext = new SQLiteContext())
            {
                Node removeNode = _sQLiteContext.NodeList.Where(x => x.Id == node.Id).FirstOrDefault();
                _sQLiteContext.NodeList.Remove(removeNode);
                _sQLiteContext.SaveChanges();
            }
        }

        public static List<TestRepository> GetAllRepository()
        {
            List<TestRepository> outputList = new List<TestRepository>();
            using (_sQLiteContext = new SQLiteContext())
            {
                foreach (TestRepository currRepo in _sQLiteContext.TestRespositories.ToList())
                {
                    TestRepository testRepository = new TestRepository();
                    testRepository.Id = currRepo.Id;
                    testRepository.RepositoryName = currRepo.RepositoryName;
                    testRepository.TestNodeList = new List<Node>(
                        _sQLiteContext.NodeList.Where(x => x.RepositoryId == currRepo.Id)
                        .Select(
                            x =>
                            new DBNode
                            {
                                Id = x.Id,
                                TreeNodeName = x.TreeNodeName,
                                NodeFullPath = x.NodeFullPath,
                                MotherNodeId = x.MotherNodeId,
                                MotherNode = x.MotherNode,
                                TreeNodeType = x.TreeNodeType,
                                RepositoryId = x.RepositoryId,
                                Repository = x.Repository
                            }
                        ).ToList());

                    outputList.Add(testRepository);
                }
            }

            return outputList;
        }

    }
}
