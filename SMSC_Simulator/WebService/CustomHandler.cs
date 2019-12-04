namespace WebServiceStudio
{
    using System.ComponentModel;
    using System.Xml.Serialization;

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CustomHandler
    {
        public override string ToString()
        {
            return (Handler + "{" + TypeName + "}");
        }

        [XmlAttribute]
        public string Handler { get; set; }

        [XmlAttribute]
        public string TypeName { get; set; }
    }
}

