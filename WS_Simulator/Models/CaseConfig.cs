using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_Simulator.Models
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class XTestCase
    {

        private string caseNameField;

        private string dataDirField;

        private string setupSqlsFileField;

        private XTestCaseStepDetails[] stepDetailsField;

        /// <remarks/>
        public string CaseName
        {
            get
            {
                return this.caseNameField;
            }
            set
            {
                this.caseNameField = value;
            }
        }

        /// <remarks/>
        public string DataDir
        {
            get
            {
                return this.dataDirField;
            }
            set
            {
                this.dataDirField = value;
            }
        }

        /// <remarks/>
        public string SetupSqlsFile
        {
            get
            {
                return this.setupSqlsFileField;
            }
            set
            {
                this.setupSqlsFileField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("StepDetails")]
        public XTestCaseStepDetails[] StepDetails
        {
            get
            {
                return this.stepDetailsField;
            }
            set
            {
                this.stepDetailsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class XTestCaseStepDetails
    {

        private int stepField;

        private bool waitingField;

        private int waitingTimeField;

        private string stepTypeField;

        private string inputFileField;

        private string outputFileField;

        private string sQLFileField;

        private string resultFileField;

        /// <remarks/>
        public int Step
        {
            get
            {
                return this.stepField;
            }
            set
            {
                this.stepField = value;
            }
        }

        /// <remarks/>
        public bool Waiting
        {
            get
            {
                return this.waitingField;
            }
            set
            {
                this.waitingField = value;
            }
        }

        /// <remarks/>
        public int WaitingTime
        {
            get
            {
                return this.waitingTimeField;
            }
            set
            {
                this.waitingTimeField = value;
            }
        }

        /// <remarks/>
        public string StepType
        {
            get
            {
                return this.stepTypeField;
            }
            set
            {
                this.stepTypeField = value;
            }
        }

        /// <remarks/>
        public string InputFile
        {
            get
            {
                return this.inputFileField;
            }
            set
            {
                this.inputFileField = value;
            }
        }

        /// <remarks/>
        public string OutputFile
        {
            get
            {
                return this.outputFileField;
            }
            set
            {
                this.outputFileField = value;
            }
        }

        /// <remarks/>
        public string SQLFile
        {
            get
            {
                return this.sQLFileField;
            }
            set
            {
                this.sQLFileField = value;
            }
        }

        /// <remarks/>
        public string ResultFile
        {
            get
            {
                return this.resultFileField;
            }
            set
            {
                this.resultFileField = value;
            }
        }
    }


}
