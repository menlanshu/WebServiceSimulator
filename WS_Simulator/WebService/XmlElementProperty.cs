namespace WebServiceStudio
{
    using System;
    using System.Collections;
    using System.Xml;

    internal class XmlElementProperty : ClassProperty
    {
        private static readonly Type[] attrArrayType = new Type[] { typeof(XmlAttribute[]) };
        private static readonly Type[] elemArrayType = new Type[] { typeof(XmlElement[]) };
        private static readonly Type[] stringType = new Type[] { typeof(string) };

        public XmlElementProperty(Type[] possibleTypes, string name, object val) : base(possibleTypes, name, val)
        {
        }

        protected override void CreateChildren()
        {
            TreeNode.Nodes.Clear();
            if (InternalValue != null)
            {
                CreateTreeNodeProperty(stringType, "Name", XmlElement.Name).RecreateSubtree(TreeNode);
                CreateTreeNodeProperty(stringType, "NamespaceURI", XmlElement.NamespaceURI).RecreateSubtree(TreeNode);
                CreateTreeNodeProperty(stringType, "TextValue", XmlElement.InnerText).RecreateSubtree(TreeNode);
                ArrayList list1 = new ArrayList();
                ArrayList list2 = new ArrayList();
                if (XmlElement != null)
                {
                    for (XmlNode node = XmlElement.FirstChild; node != null; node = node.NextSibling)
                    {
                        if (node.NodeType == XmlNodeType.Element)
                        {
                            list1.Add(node);
                        }
                    }
                    foreach (XmlAttribute attribute in XmlElement.Attributes)
                    {
                        if ((attribute.Name != "xmlns") && !attribute.Name.StartsWith("xmlns:"))
                        {
                            list2.Add(attribute);
                        }
                    }
                }
                XmlAttribute[] val = ((list2.Count == 0) && !IsInput()) ? null : (list2.ToArray(typeof(XmlAttribute)) as XmlAttribute[]);
                XmlElement[] elementArray = ((list1.Count == 0) && !IsInput()) ? null : (list1.ToArray(typeof(XmlElement)) as XmlElement[]);
                CreateTreeNodeProperty(attrArrayType, "Attributes", val).RecreateSubtree(TreeNode);
                CreateTreeNodeProperty(elemArrayType, "SubElements", elementArray).RecreateSubtree(TreeNode);
            }
        }

        public XmlDocument GetXmlDocument()
        {
            XmlElementProperty property = null;
            if (GetParent() is ArrayProperty parent)
            {
                property = parent.GetParent() as XmlElementProperty;
            }
            if (property == null)
            {
                return XmlElement.OwnerDocument;
            }
            return property.GetXmlDocument();
        }

        public override object ReadChildren(bool update = false)
        {
            XmlElement element3;
            if (InternalValue == null)
            {
                return null;
            }
            string qualifiedName = ((TreeNodeProperty) TreeNode.Nodes[0].Tag).ReadChildren().ToString();
            string namespaceURI = ((TreeNodeProperty) TreeNode.Nodes[1].Tag).ReadChildren().ToString();
            string str3 = ((TreeNodeProperty) TreeNode.Nodes[2].Tag).ReadChildren().ToString();
            XmlAttribute[] attributeArray = (XmlAttribute[]) ((TreeNodeProperty) TreeNode.Nodes[3].Tag).ReadChildren();
            XmlElement[] elementArray = (XmlElement[]) ((TreeNodeProperty) TreeNode.Nodes[4].Tag).ReadChildren();
            XmlElement element = GetXmlDocument().CreateElement(qualifiedName, namespaceURI);
            if (attributeArray != null)
            {
                foreach (XmlAttribute attribute in attributeArray)
                {
                    element.SetAttributeNode(attribute);
                }
            }
            element.InnerText = str3;
            if (elementArray != null)
            {
                foreach (XmlElement element2 in elementArray)
                {
                    element.AppendChild(element2);
                }
            }
            XmlElement = element3 = element;
            return element3;
        }

        private XmlElement XmlElement
        {
            get
            {
                return (InternalValue as XmlElement);
            }
            set
            {
                InternalValue = value;
            }
        }
    }
}

