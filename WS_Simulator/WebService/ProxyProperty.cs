namespace WebServiceStudio
{
    using System;
    using System.Reflection;
    using System.Web.Services.Protocols;
    using System.Windows.Forms;

    internal class ProxyProperty : TreeNodeProperty
    {
        private readonly HttpWebClientProtocol proxy;
        private readonly ProxyProperties proxyProperties;

        public ProxyProperty(HttpWebClientProtocol proxy) : base(new System.Type[] { typeof(ProxyProperties) }, "Proxy")
        {
            this.proxy = proxy;
            proxyProperties = new ProxyProperties(proxy);
        }

        protected override void CreateChildren()
        {
            TreeNode.Nodes.Clear();
            foreach (PropertyInfo info in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                object val = info.GetValue(proxyProperties, null);
                TreeNodeProperty.CreateTreeNodeProperty(GetIncludedTypes(info.PropertyType), info.Name, val).RecreateSubtree(TreeNode);
            }
        }

        public HttpWebClientProtocol GetProxy()
        {
            ((ProxyProperties) ReadChildren()).UpdateProxy(proxy);
            return proxy;
        }

        public override object ReadChildren(bool update = false)
        {
            object proxyProperties = this.proxyProperties;
            if (proxyProperties == null)
            {
                return null;
            }
            int num = 0;
            foreach (PropertyInfo info in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                TreeNode node = TreeNode.Nodes[num++];
                if (node.Tag is TreeNodeProperty tag)
                {
                    info.SetValue(proxyProperties, tag.ReadChildren(), null);
                }
            }
            return proxyProperties;
        }
    }
}

