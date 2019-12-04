namespace WebServiceStudio
{
    using System;
    using System.Xml;

    internal class XmlAttributeProperty : ClassProperty
    {
        private static readonly Type[] stringType = new Type[] { typeof(string) };

        public XmlAttributeProperty(Type[] possibleTypes, string name, object val) : base(possibleTypes, name, val)
        {
        }

        protected override void CreateChildren()
        {
            TreeNode.Nodes.Clear();
            if (InternalValue != null)
            {
                CreateTreeNodeProperty(stringType, "Name", XmlAttribute.Name).RecreateSubtree(TreeNode);
                CreateTreeNodeProperty(stringType, "NamespaceURI", XmlAttribute.NamespaceURI).RecreateSubtree(TreeNode);
                CreateTreeNodeProperty(stringType, "Value", XmlAttribute.Value).RecreateSubtree(TreeNode);
            }
        }

        public XmlDocument GetXmlDocument()
        {
            XmlElementProperty property2 = null;
            if (GetParent() is ArrayProperty parent)
            {
                property2 = parent.GetParent() as XmlElementProperty;
            }
            if (property2 == null)
            {
                return XmlAttribute.OwnerDocument;
            }
            return property2.GetXmlDocument();
        }

        public override object ReadChildren(bool update = false)
        {
            if (InternalValue == null)
            {
                return null;
            }
            string qualifiedName = ((TreeNodeProperty) TreeNode.Nodes[0].Tag).ReadChildren().ToString();
            string namespaceURI = ((TreeNodeProperty) TreeNode.Nodes[1].Tag).ReadChildren().ToString();
            string str3 = ((TreeNodeProperty) TreeNode.Nodes[2].Tag).ReadChildren().ToString();
            XmlAttribute = GetXmlDocument().CreateAttribute(qualifiedName, namespaceURI);
            XmlAttribute.Value = str3;
            return XmlAttribute;
        }

        private XmlAttribute XmlAttribute
        {
            get
            {
                return (InternalValue as XmlAttribute);
            }
            set
            {
                InternalValue = value;
            }
        }
    }
}

