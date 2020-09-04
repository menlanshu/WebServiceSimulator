using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WS_Simulator.Models;

namespace WS_Simulator.FormHandler
{
    public class TestCaseDocumentHandler
    {
        private static readonly string _resultPostFix = "_Result.txt";

        private static readonly string _eventName = "Event";
        private static readonly string _executeName = "Execute";
        private static readonly string _sqlName = "SQL";

        public enum FileType
        {
            CASECONFIG,
            TESTCASE
        }
        public static void GenerateTestCaseFile(string directoryPath)
        {
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

            WriteToFile(GetFileName(FileType.TESTCASE, directoryPath), fileContent.ToString() + expectResult.ToString());

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

        public static void GenerateCaseConfigFile(string directoryPath)
        {
            XTestCase xTestCase = GetCurrentFolderXTestCase(directoryPath);
            SaveInstanceToFIle(xTestCase, GetFileName(FileType.CASECONFIG, directoryPath));
        }

        private static XTestCase GetCurrentFolderXTestCase(string directoryPath)
        {
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

        private static string GetFileName(FileType nameCase, string directoryPath)
        {
            switch (nameCase)
            {
                case FileType.CASECONFIG:
                    return $@"{directoryPath}\CaseConfig.xml";
                case FileType.TESTCASE:
                    return $@"{directoryPath}\TestCase.txt";
                default:
                    throw new Exception("Do not support other condition!");
            }
        }


        public static void SaveInstanceToFIle(XTestCase instance, string fileName)
        {
            // Create an instance of the XmlSerializer class;
            // specify the type of object to serialize.
            XmlSerializer serializer =
            new XmlSerializer(typeof(XTestCase));

            using (TextWriter textWriter = new StreamWriter(fileName))
            {
                serializer.Serialize(textWriter, instance);
            }
        }

    }
}
