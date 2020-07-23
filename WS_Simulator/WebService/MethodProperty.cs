namespace WebServiceStudio
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Web.Services.Protocols;
    using System.Windows.Forms;

    internal class MethodProperty : TreeNodeProperty
    {
        private readonly bool isIn;
        private MethodInfo method;
        private object[] paramValues;
        private ProxyProperty proxyProperty;
        private object result;

        public MethodProperty(ProxyProperty proxyProperty, MethodInfo method) : base(new System.Type[] { method.ReturnType }, method.Name)
        {
            this.proxyProperty = proxyProperty;
            this.method = method;
            isIn = true;
        }

        public MethodProperty(ProxyProperty proxyProperty, MethodInfo method, object result, object[] paramValues) : base(new System.Type[] { method.ReturnType }, method.Name)
        {
            this.proxyProperty = proxyProperty;
            this.method = method;
            isIn = false;
            this.result = result;
            this.paramValues = paramValues;
        }

        private void AddBody()
        {
            TreeNode parentNode = TreeNode.Nodes.Add("Body");
            if (!isIn && (method.ReturnType != typeof(void)))
            {
                Type type = (result != null) ? result.GetType() : method.ReturnType;
                CreateTreeNodeProperty(new Type[] { type }, "result", result).RecreateSubtree(parentNode);
            }
            ParameterInfo[] parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                if ((!isIn && (parameters[i].IsOut || parameters[i].ParameterType.IsByRef)) || (isIn && !parameters[i].IsOut))
                {
                    Type parameterType = parameters[i].ParameterType;
                    if (parameterType.IsByRef)
                    {
                        parameterType = parameterType.GetElementType();
                    }
                    object val = (paramValues != null) ? paramValues[i] : (isIn ? CreateNewInstance(parameterType) : null);
                    CreateTreeNodeProperty(GetIncludedTypes(parameterType), parameters[i].Name, val).RecreateSubtree(parentNode);
                }
            }
            parentNode.ExpandAll();
        }

        private void AddHeaders()
        {
            TreeNode parentNode = TreeNode.Nodes.Add("Headers");
            FieldInfo[] soapHeaders = GetSoapHeaders(method, isIn);
            HttpWebClientProtocol proxy = proxyProperty.GetProxy();
            foreach (FieldInfo info in soapHeaders)
            {
                object val = (proxy != null) ? info.GetValue(proxy) : null;
                CreateTreeNodeProperty(GetIncludedTypes(info.FieldType), info.Name, val).RecreateSubtree(parentNode);
            }
            parentNode.ExpandAll();
        }

        protected override void CreateChildren()
        {
            AddHeaders();
            AddBody();
        }

        protected override MethodInfo GetCurrentMethod()
        {
            return method;
        }

        protected override object GetCurrentProxy()
        {
            return proxyProperty.GetProxy();
        }

        public MethodInfo GetMethod()
        {
            return method;
        }

        public ProxyProperty GetProxyProperty()
        {
            return proxyProperty;
        }

        public static FieldInfo[] GetSoapHeaders(MethodInfo method, bool isIn)
        {
            System.Type declaringType = method.DeclaringType;
            SoapHeaderAttribute[] customAttributes = (SoapHeaderAttribute[]) method.GetCustomAttributes(typeof(SoapHeaderAttribute), true);
            ArrayList list = new ArrayList();
            for (int i = 0; i < customAttributes.Length; i++)
            {
                SoapHeaderAttribute attribute = customAttributes[i];
                if (((attribute.Direction == SoapHeaderDirection.InOut) || (isIn && (attribute.Direction == SoapHeaderDirection.In))) || (!isIn && (attribute.Direction == SoapHeaderDirection.Out)))
                {
                    FieldInfo field = declaringType.GetField(attribute.MemberName);
                    list.Add(field);
                }
            }
            return (FieldInfo[]) list.ToArray(typeof(FieldInfo));
        }

        protected override bool IsInput()
        {
            return isIn;
        }

        private void ReadBody()
        {
            TreeNode node = TreeNode.Nodes[1];
            ParameterInfo[] parameters = method.GetParameters();
            paramValues = new object[parameters.Length];
            int index = 0;
            int num2 = 0;
            while (index < paramValues.Length)
            {
                ParameterInfo info = parameters[index];
                if (!info.IsOut)
                {
                    TreeNode node2 = node.Nodes[num2++];
                    if (node2.Tag is TreeNodeProperty tag)
                    {
                        paramValues[index] = tag.ReadChildren(true);
                    }
                }
                index++;
            }
        }

        public override object ReadChildren(bool update = false)
        {
            ReadHeaders();
            ReadBody();
            return paramValues;
        }

        private void ReadHeaders()
        {
            TreeNode node = TreeNode.Nodes[0];
            System.Type declaringType = method.DeclaringType;
            HttpWebClientProtocol proxy = proxyProperty.GetProxy();
            foreach (TreeNode node2 in node.Nodes)
            {
                if (node2.Tag is ClassProperty tag)
                {
                    declaringType.GetField(tag.Name).SetValue(proxy, tag.ReadChildren());
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

