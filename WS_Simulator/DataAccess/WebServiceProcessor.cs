using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using WebServiceStudio;

namespace WS_Simulator.DataAccess
{
    public static class WebServiceProcessor
    {
        public static TreeView TreeMethods { get; private set; } = new TreeView();
        public static TreeView TreeInput { get; private set; } = new TreeView();

        private static Wsdl _wsdl = new Wsdl();

        static WebServiceProcessor()
        {
            WSSWebRequestCreate.RegisterPrefixes();
            SetupAssemblyResolver();
        }

        public async static Task Reset(string wSAddress)
        {
            _wsdl.Reset();
            _wsdl.Paths.Add(wSAddress);

            // Gernerate WSDL file according to current web address
            await Task.Run(() => _wsdl.Generate());
        }

        private static void SetupAssemblyResolver()
        {
            ResolveEventHandler handler = new ResolveEventHandler(OnAssemblyResolve);
            AppDomain.CurrentDomain.AssemblyResolve += handler;
        }
        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly proxyAssembly = _wsdl.ProxyAssembly;
            if ((proxyAssembly != null) && (proxyAssembly.GetName().ToString() == args.Name))
            {
                return proxyAssembly;
            }
            return null;
        }

        // When web service address change, reinitial method list
        public static void FillInvokeTab()
        {
            Assembly proxyAssembly = _wsdl.ProxyAssembly;
            if (proxyAssembly != null)
            {
                TreeMethods.Nodes.Clear();
                foreach (System.Type type in proxyAssembly.GetTypes())
                {
                    if (TreeNodeProperty.IsWebService(type))
                    {
                        TreeNode node = TreeMethods.Nodes.Add(type.Name);

                        HttpWebClientProtocol proxy = (HttpWebClientProtocol)Activator.CreateInstance(type);
                        ProxyProperty property = new ProxyProperty(proxy);
                        property.RecreateSubtree(null);

                        node.Tag = property.TreeNode;

                        proxy.Credentials = CredentialCache.DefaultCredentials;
                        if (proxy is SoapHttpClientProtocol protocol2)
                        {
                            protocol2.CookieContainer = new CookieContainer();
                            protocol2.AllowAutoRedirect = true;
                        }
                        foreach (MethodInfo info in type.GetMethods())
                        {
                            if (TreeNodeProperty.IsWebMethod(info))
                            {
                                node.Nodes.Add(info.Name).Tag = info;
                            }
                        }
                    }
                }
            }
        }
        public static void InvokeWebMethod(int totalCount, Action<string> updateReplyMessage)
        {
            string replyHeader = "";
            string replyMessage = "";

            try
            {
                MethodProperty currentMethodProperty = GetCurrentMethodProperty();
                if (currentMethodProperty != null)
                {
                    HttpWebClientProtocol proxy = currentMethodProperty.GetProxyProperty().GetProxy();
                    RequestProperties properties = new RequestProperties(proxy);
                    replyMessage = "<Reply>";
                    MethodInfo method = currentMethodProperty.GetMethod();
                    System.Type declaringType = method.DeclaringType;
                    //for (int tempInfoCount = 0; tempInfoCount < _testClient.TotalCount; tempInfoCount++)
                    for (int tempInfoCount = 0; tempInfoCount < totalCount; tempInfoCount++)
                    {
                        try
                        {

                            WSSWebRequest.RequestTrace = properties;
                            object[] parameters = currentMethodProperty.ReadChildren() as object[];
                            object result = method.Invoke(proxy, BindingFlags.Public, null, parameters, null);
                            //MethodProperty property2 = new MethodProperty(currentMethodProperty.GetProxyProperty(), method, result, parameters);
                        }
                        catch (Exception err)
                        {
                            replyMessage += err.Message + Environment.NewLine;
                        }
                        finally
                        {
                            WSSWebRequest.RequestTrace = null;
                            //richRequest.Text = properties.requestPayLoad;
                            //replyMessage = properties.responsePayLoad;
                            if (properties != null && !string.IsNullOrEmpty(properties.responsePayLoad))
                            {
                                replyHeader = GetReplyHeader(properties.responsePayLoad);
                                if (replyHeader.Contains("ResponseCode: 200 (OK)"))
                                {
                                    replyMessage += GetReplyBody(properties.responsePayLoad) + Environment.NewLine;
                                }
                            }
                            else
                            {
                                replyMessage += "For some reason, no reponse." + Environment.NewLine;
                            }
                        }
                    }
                    replyMessage += "</Reply>";
                }
                else
                {
                    replyMessage += "currentMethodProperty = null, can not get current Method Property";
                }
            }
            catch (Exception err)
            {
                replyMessage += "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                updateReplyMessage?.Invoke(replyMessage);
            }
        }

        private static string GetReplyHeader(string inReply)
        {
            string outHeader;
            List<string> striparr = inReply.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();

            foreach (string tempStr in striparr)
            {
                outHeader = tempStr;
                return outHeader;
            }

            return "Get Header Fail!";
        }

        private static string GetReplyBody(string inReply)
        {
            string outBody = "";
            bool isBody = false;
            List<string> striparr = inReply.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();

            foreach (string tempStr in striparr)
            {
                if (isBody)
                {
                    outBody += tempStr;
                }
                else
                {
                    if (tempStr == "")
                    {
                        isBody = true;
                    }
                }
            }
            if (outBody != "")
            {
                outBody = outBody.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
                outBody = outBody.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
                return outBody;
            }

            return "Get Body Fail!";
        }

        private static MethodProperty GetCurrentMethodProperty()
        {
            if ((TreeInput.Nodes == null) || (TreeInput.Nodes.Count == 0))
            {
                throw new Exception("Select a web method to execute");
            }
            TreeNode node = TreeInput.Nodes[0];
            if (!(node.Tag is MethodProperty tag))
            {
                throw new Exception("Select a method to execute");
            }
            return tag;
        }

        public static void SetInputNode(string methodName)
        {
            TreeNode currentNode = new TreeNode();
            currentNode = FindCurrentNode(TreeMethods.Nodes, methodName);
            if (currentNode != null && (currentNode.Tag is MethodInfo))
            {
                MethodInfo tag = currentNode.Tag as MethodInfo;
                TreeInput.Nodes.Clear();
                MethodProperty property = new MethodProperty(GetProxyPropertyFromNode(currentNode), tag);
                property.RecreateSubtree(null);
                TreeInput.Nodes.Add(property.TreeNode);
            }
        }

        private static TreeNode FindCurrentNode(TreeNodeCollection inNodeCollection, string nodeName)
        {
            foreach (TreeNode node in inNodeCollection)
            {
                if (node.Text == nodeName)
                {
                    return node;
                }
                else if (node.Nodes.Count != 0)
                {
                    return FindCurrentNode(node.Nodes, nodeName);
                }
            }

            return null;
        }

        private static ProxyProperty GetProxyPropertyFromNode(TreeNode treeNode)
        {
            while (treeNode.Parent != null)
            {
                treeNode = treeNode.Parent;
            }
            if (treeNode.Tag is TreeNode tag)
            {
                return (tag.Tag as ProxyProperty);
            }
            return null;
        }
    }
}
