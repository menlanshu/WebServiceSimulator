using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WS_Simulator.DataAccess
{
    public static class FileProcessor
    {
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

                    updateAfterReadFile(tempFileStr);

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

            FileStream stream = System.IO.File.OpenWrite(fileName);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(contents);
            writer.Flush();
            stream.SetLength(stream.Position);
            stream.Close();
        }
    }
}
