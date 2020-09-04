using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WS_Simulator.Models;

namespace WS_Simulator.DataAccess
{
    public static class FileProcessor
    {
        public static string GetFullPath(string rootPath, string nodeFullPath)
        {
            return rootPath + nodeFullPath.Substring(8);
        }

        public static string GetFullDirectoryPath(string rootPath, string dirNodeFullPath)
        {
            return $"{rootPath}{GetRelativeFolderPath(dirNodeFullPath)}";
        }

        public static string GetRelativeFolderPath(string nodeFullPath)
        {
            string relativeFilePah = nodeFullPath.Substring(8);
            string relativeFolderPath = relativeFilePah.Substring(0, relativeFilePah.LastIndexOf("\\") + 1);

            return relativeFolderPath;
        }
        public static void LoadMessageForAllNodes(List<Node> nodeList, string rootPath)
        {
            foreach(Node node in nodeList)
            {
                string filePath = rootPath + node.TreeNodeValue.FullPath.Substring(8);
                node.TreeNodeMessage = node.GetCurrentMessage(false);
                node.TreeNodeMessage = ReadFile(filePath, null);
            }
        }

        public static string ReadFile(string filePath, Action<string> updateAfterReadFile)
        {
            FileStream tempFileStream = null;
            StreamReader tempReader = null;
            string tempFileStr = "";

            try
            {

                if (File.Exists(filePath))
                {

                    tempFileStream = new FileStream(filePath, FileMode.Open);
                    tempReader = new StreamReader(tempFileStream);
                    tempFileStr = tempReader.ReadToEnd();

                    updateAfterReadFile?.Invoke(tempFileStr);

                }
            }
            catch (Exception err)
            {
                throw new Exception($"Exception happen in ReadFileStream : {err.Message}");
            }
            finally
            {
                if (tempFileStream != null) tempFileStream.Close();
                if (tempReader != null) tempReader.Close();
            }

            return tempFileStr;
        }

        public static void SaveFile(string fileName, string contents)
        {

            try { 
                FileStream stream = System.IO.File.OpenWrite(fileName);
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(contents);
                writer.Flush();
                stream.SetLength(stream.Position);
                stream.Close();
            }catch(Exception err)
            {
                // TODO - May be write log or show exception in reply box
                throw err;
            }
        }

        public static void MoveFile(string oldFileName, string newFileName)
        {
            File.Move(oldFileName, newFileName);
        }
    }
}
