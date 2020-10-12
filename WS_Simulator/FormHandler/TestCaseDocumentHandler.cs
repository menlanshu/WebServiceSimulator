using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using WS_Simulator.Models;

namespace WS_Simulator.FormHandler
{
    public enum FileCase
    {
        CASECONFIG,
        TESTCASE
    }
    public static class TestCaseDocumentHandler
    {
        private static readonly string _resultPostFix = "_Result.txt";

        private static readonly string _eventName = "Event";
        private static readonly string _executeName = "Execute";
        private static readonly string _sqlName = "SQL";

        public static void GenerateTestCaseFile(Node directoryNode)
        {
            string directoryPath = ((FileNode)directoryNode).DirectoryPath;

            StringBuilder fileContent = new StringBuilder();
            StringBuilder expectResult = new StringBuilder();

            expectResult.AppendLine("Expected Results:");

            fileContent.AppendLine("Initial:");
            fileContent.AppendLine("1. Clean up & re-configuration of the contexts via S1_SQL000_Initialize.sql");
            fileContent.AppendLine("2. Clean up the horizon data related to test case via S0_Horizon_Initialize.sql");
            fileContent.AppendLine("");

            //Test Procedure:
            fileContent.AppendLine("Test Procedure:");

            DirectoryInfo directory = new DirectoryInfo(directoryPath);
            var fileList = directory.GetFiles().ToList();
            fileList.OrderBy(x => x.Name);

            int step = 0;
            int expectStep = 0;
            foreach (var file in fileList)
            {
                if (
                    (file.Name.Substring(file.Name.LastIndexOf(".") + 1).ToUpper().Equals("XML") ||
                    file.Name.Substring(file.Name.LastIndexOf(".") + 1).ToUpper().Equals("SQL")
                    ) &&
                    int.TryParse(file.Name.Substring(0, 1), out int tempInt))
                {
                    step++;
                    switch (file.Name.Substring(file.Name.LastIndexOf(".") + 1).ToUpper())
                    {
                        case "SQL":
                            fileContent.AppendLine($"{step}. Execute SQL file \" { file.Name } \" ");
                            break;
                        case "XML":
                            fileContent.AppendLine($"{step}. Generate the event \" { file.Name } \" ");
                            break;
                    }

                    string outputfileName = file.Name.Substring(0, file.Name.LastIndexOf(".")) + _resultPostFix;
                    if (fileList.Any(x => x.Name == outputfileName))
                    {
                        expectStep++;
                        expectResult.AppendLine($"{expectStep}. Step {step}: expect result as {outputfileName}");
                    }
                }
            }

            fileContent.AppendLine();

            string fullFileContext = fileContent.ToString() + expectResult.ToString();

            //WriteToFile(GetFileNamePath(FileCase.TESTCASE, directoryPath), fullFileContext);

            SimulatorFormHandler.AddCurrentNodeToDir(
                GetFileName(FileCase.TESTCASE), directoryNode, fullFileContext);
        }

        private static void WriteToFile(string filePath, string fileContent)
        {
            File.WriteAllText(filePath, fileContent);
        }

        public static void AddZeroToFileName(string directoryPath)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryPath);

            foreach (var file in directory.GetFiles())
            {
                if (int.TryParse(file.Name.Substring(0, 1), out int tempInt))
                {
                    File.Move(file.Name, $"0{file.Name}");
                }
            }

        }
        public static void RenameFileFollowSequence(TreeNode directoryNode, string directoryPath)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryPath);
            string pattern = @"^\d+";
            Regex rgx = new Regex(pattern);

            int step = 0;
            string newFileName;

            List<string> alreadyHandled = new List<string>();
            var nodeList = directoryNode.Nodes.OfType<TreeNode>().ToList();

            foreach (TreeNode node in nodeList)
            {
                if (rgx.Match(node.Text).Success)
                {
                    if (!alreadyHandled.Contains(node.Text))
                    {
                        newFileName = rgx.Replace(node.Text, step.ToString("00"));
                        File.Move($@"{directoryPath}\{node.Text}", $@"{directoryPath}\{newFileName}");
                        alreadyHandled.Add(node.Text);

                        string outputfileName = node.Text.Substring(0, node.Text.LastIndexOf(".")) + _resultPostFix;
                        TreeNode outputNode = nodeList.FirstOrDefault(x => x.Text == outputfileName);
                        if (outputNode != null)
                        {
                            newFileName = rgx.Replace(outputNode.Text, step.ToString("00"));
                            File.Move($@"{directoryPath}\{outputNode.Text}", $@"{directoryPath}\{newFileName}");
                            alreadyHandled.Add(outputNode.Text);
                        }

                        step++;
                    }
                }
            }

        }

        public static string AddNumberBeforeFileName(string currentFileName, out bool needRename)
        {
            needRename = false;
            string pattern = @"^\d+";
            string newFileName = currentFileName;
            Regex rgx = new Regex(pattern);

            if (!rgx.Match(currentFileName).Success)
            {
                needRename = true;
                newFileName = $"0_{currentFileName}";
            }

            return newFileName;
        }

        public static void GenerateCaseConfigFile(Node directoryNode)
        {
            string errDesc = "";

            XTestCase xTestCase = GetCurrentFolderXTestCase(directoryNode);

            // if sql file without result file show error
            if(!xTestCase.CheckTestCaseAvailable(out errDesc))
            {
                MessageBox.Show(errDesc, "Generate Case Config Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string fileContent = ConvertInstanceToString<XTestCase>(xTestCase);

            SimulatorFormHandler.AddCurrentNodeToDir(
                GetFileName(FileCase.CASECONFIG), directoryNode, fileContent);
        }

        private static XTestCase GetCurrentFolderXTestCase(Node directoryNode)
        {
            string directoryPath = ((FileNode)directoryNode).DirectoryPath;

            XTestCase result = new XTestCase();

            DirectoryInfo directory = new DirectoryInfo(directoryPath);

            result.CaseName = directory.Name;
            result.DataDir = $@"{directory.Name}\";
            result.SetupSqlsFile = "S1_SQL000_Initialize.sql";

            List<XTestCaseStepDetails> stepDetail = new List<XTestCaseStepDetails>();

            int step = 1;

            XTestCaseStepDetails xTestCaseStep0 = new XTestCaseStepDetails();
            xTestCaseStep0.Step = step;
            xTestCaseStep0.Waiting = true;
            xTestCaseStep0.StepType = _sqlName;
            xTestCaseStep0.InputFile = "";
            xTestCaseStep0.OutputFile = "";
            xTestCaseStep0.SQLFile = "S0_Horizon_Initialize.sql";
            xTestCaseStep0.ResultFile = "S0_Horizon_Initialize_result.txt";

            stepDetail.Add(xTestCaseStep0);

            var fileList = directory.GetFiles().ToList();
            fileList.OrderBy(x => x.Name);

            foreach (var file in fileList)
            {
                if ((file.Name.Substring(file.Name.LastIndexOf(".") + 1).ToUpper().Equals("XML") ||
                    file.Name.Substring(file.Name.LastIndexOf(".") + 1).ToUpper().Equals("SQL")
                    ) &&
                    int.TryParse(file.Name.Substring(0, 1), out int tempInt))
                {
                    step++;

                    string outputfileName = file.Name.Substring(0, file.Name.LastIndexOf(".")) + _resultPostFix;
                    if (!fileList.Any(x => x.Name == outputfileName))
                    {
                        outputfileName = "";
                    }
                    XTestCaseStepDetails xTestCaseStep = new XTestCaseStepDetails();
                    if (file.Name.Substring(file.Name.LastIndexOf(".") + 1).ToUpper().Equals("XML"))
                    {
                        xTestCaseStep.Step = step;
                        xTestCaseStep.Waiting = true;
                        xTestCaseStep.StepType = outputfileName == "" ? _executeName : _eventName;
                        xTestCaseStep.InputFile = file.Name;
                        xTestCaseStep.OutputFile = outputfileName;
                        xTestCaseStep.SQLFile = "";
                        xTestCaseStep.ResultFile = "";
                    }
                    else
                    {
                        xTestCaseStep.Step = step;
                        xTestCaseStep.Waiting = true;
                        xTestCaseStep.StepType = _sqlName;
                        xTestCaseStep.InputFile = "";

                        xTestCaseStep.OutputFile = "";
                        xTestCaseStep.SQLFile = file.Name;
                        xTestCaseStep.ResultFile = outputfileName;
                    }

                    stepDetail.Add(xTestCaseStep);
                }
            }

            result.StepDetails = stepDetail.ToArray();


            return result;
        }

        private static bool CheckTestCaseAvailable(this XTestCase xTestCase, out string errDesc)
        {
            bool result = true;
            errDesc = "";

            if (xTestCase.StepDetails.ToList().Any(x => x.SQLFile != "" && x.ResultFile == ""))
            {
                result = false;
                errDesc = "SQL file has no result file!";
            }

            return result;
        }

        private static string GetFileNamePath(FileCase nameCase, string directoryPath)
        {
            switch (nameCase)
            {
                case FileCase.CASECONFIG:
                    return $@"{directoryPath}\CaseConfig.xml";
                case FileCase.TESTCASE:
                    return $@"{directoryPath}\TestCase.txt";
                default:
                    throw new Exception("Do not support other condition!");
            }
        }

        public static string GetFileName(FileCase nameCase)
        {
            switch (nameCase)
            {
                case FileCase.CASECONFIG:
                    return $@"CaseConfig.xml";
                case FileCase.TESTCASE:
                    return $@"TestCase.txt";
                default:
                    throw new Exception("Do not support other condition!");
            }
        }


        public static string ConvertInstanceToString<T>(T instance)
        {
            // Create an instance of the XmlSerializer class;
            // specify the type of object to serialize.
            XmlSerializer serializer =
            new XmlSerializer(instance.GetType());

            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, instance);
                return stringWriter.ToString();
            }
        }

    }
}
