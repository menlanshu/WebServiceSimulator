﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace WS_Simulator.DataAccess
{
    public static class XMLProcessor
    {
        private static string DefaultValue = "NA";
        private static char[] ConfigDelimeter = (";").ToCharArray();
        private const string DispatchFilePath = @".\DispatcherFormat.xml";
        public static readonly string DefaultXmlStr = $"<defaultValue>{DefaultValue}</defaultValue>";


        private static Hashtable _normalCollection;
        private static Hashtable _batchCollection;
        private static Hashtable _methodMapping;
        private static Hashtable _dispatcherConfig;
        public static string GetVlaueByPath(string path, string requestMessage, bool isBatch, bool getInnerXml = false)
        {
            XmlDocument testXmlDoc = new XmlDocument();

            string pathValue = DefaultXmlStr;

            if (requestMessage == "") return pathValue;

            try
            {

                testXmlDoc.LoadXml(RemoveAllNamespaces(requestMessage));

                if (testXmlDoc.SelectSingleNode(path) != null)
                {
                    if (testXmlDoc.SelectSingleNode(path) != null)
                    {
                        if (testXmlDoc.SelectSingleNode(path).InnerText != testXmlDoc.SelectSingleNode(path).InnerXml)
                        {
                            XmlNodeList tempNodeList = testXmlDoc.SelectNodes(path);
                            //if (tempNodeList.Count > 1 && !isBatch)
                            //{
                            //    pathValue = tempNodeList[sendIndex] == null ?
                            //        $"<defaultValue>{DefaultValue}</defaultValue>" : tempNodeList[sendIndex].OuterXml;
                            //}
                            if (tempNodeList.Count == 1)
                            {
                                pathValue = testXmlDoc.SelectSingleNode(path) == null ?
                                    DefaultXmlStr :
                                    (getInnerXml ? testXmlDoc.SelectSingleNode(path).InnerXml : testXmlDoc.SelectSingleNode(path).OuterXml);
                            }
                            //else if (sendIndex < totalCount)
                            //{
                            //    pathValue = tempNodeList[sendIndex] == null ?
                            //        $"<defaultValue>{DefaultValue}</defaultValue>" : tempNodeList[sendIndex].OuterXml;
                            //}
                        }
                        else
                        {
                            pathValue = testXmlDoc.SelectSingleNode(path) == null ?
                                DefaultXmlStr : testXmlDoc.SelectSingleNode(path).InnerText;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                pathValue = $"<Exception>{err.Message}</Exception>";
            }

            return pathValue;
        }

        public static string GetValueByPathList(string pathConfig, string requestMessage)
        {
            List<string> pathList = pathConfig.Split(ConfigDelimeter).ToList();
            string result = DefaultXmlStr;

            foreach(var path in pathList)
            {
                result = GetVlaueByPath(path, requestMessage, false, true);
                if (result != DefaultXmlStr)
                {
                    return result;
                }
            }

            return result;
        }

        //Implemented based on interface, not part of algorithm
        public static string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

            return xmlDocumentWithoutNs.ToString();
        }

        //Core recursion function
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                if (!string.IsNullOrEmpty(xmlDocument.Value))
                {
                    xElement.Value = xmlDocument.Value;
                }

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }

        public static bool IsE3EventInfo(string requestMessage, out int dispatcherConfig, out string errDesc)
        {
            string path = "";
            string fullPathStr = "";

            errDesc = "";
            dispatcherConfig = 1;

            try
            {
                if (requestMessage == "")
                {
                    errDesc = "Request Message is empty!";
                    return false;
                }


                fullPathStr = ConfigurationManager.ConnectionStrings["E3TestRootName"] == null ? "" :
                        ConfigurationManager.ConnectionStrings["E3TestRootName"].ConnectionString;


                if (fullPathStr != "")
                {
                    for (int configLocaion = 0; configLocaion < fullPathStr.Split(ConfigDelimeter).Count(); configLocaion++)
                    {
                        path = "";
                        path = fullPathStr.Split(ConfigDelimeter)[configLocaion];
                        if (path != "")
                        {
                            XmlDocument testXmlDoc = new XmlDocument();
                            testXmlDoc.LoadXml(requestMessage);
                            if (testXmlDoc == null)
                            {
                                errDesc = "Load request message to XML fail";
                                return false;
                            }

                            if (testXmlDoc.SelectSingleNode(path) != null)
                            {
                                dispatcherConfig = dispatcherConfig + configLocaion;

                                if (UpdateCollectionConfig(dispatcherConfig, out errDesc))
                                {
                                    return false;
                                }

                                return true;
                            }else
                            {
                                errDesc += $"Can not find path in current message {path}\n";
                            }
                        }
                        else
                        {
                            errDesc = "Empty path configuration for dispatch convert";
                        }
                    }
                }
            }
            catch (Exception err)
            {
                errDesc = $"Excepetion happen in {System.Reflection.MethodBase.GetCurrentMethod().Name} : {err.Message}";
            }

            return false;
        }

        private static bool UpdateCollectionConfig(int configLocation, out string errDesc)
        {
            errDesc = "";

            try
            {
                _normalCollection = (Hashtable)ConfigurationManager.GetSection("dispatcherMapping" + configLocation.ToString());
                _batchCollection = (Hashtable)ConfigurationManager.GetSection("batchDispatcherMapping" + configLocation.ToString());
                _methodMapping = (Hashtable)ConfigurationManager.GetSection("dispatcherMethodMapping");
                _dispatcherConfig = (Hashtable)ConfigurationManager.GetSection("dispatchConfig");

                if (_normalCollection == null || _batchCollection == null ||
                    _methodMapping == null || _dispatcherConfig == null)
                {
                    errDesc = "The config for transfer message to dispatcher is not enough!";
                    return false;
                }

            }
            catch (Exception err)
            {
                errDesc = $"Exception happen when read dispatch configuration : {err.Message} {Environment.NewLine}" +
                    $"Check Configuration of {"dispatcherMapping" + configLocation.ToString()} {Environment.NewLine}" +
                    $"{"batchDispatcherMapping" + configLocation.ToString()} {Environment.NewLine}" +
                    $"{"dispatcherMethodMapping"} {Environment.NewLine}" +
                    $"{"dispatchConfig"}";
                return false;
            }
            return true;
        }

        public static bool ChangeToDispatcherMessage(string nodeName, ref string requestMessage, out string errDesc)
        {
            bool result = false;
            errDesc = "";

            XmlDocument xDoc = new XmlDocument();
            XmlDocument xRequestDoc = new XmlDocument();
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter xtw = null;

            try
            {
                if (File.Exists(DispatchFilePath) && requestMessage != "")
                {
                    xDoc.Load(DispatchFilePath);
                    xRequestDoc.LoadXml(requestMessage);
                    XmlNodeList nodeList = xDoc.SelectNodes("/");

                    XmlNamespaceManager xnm = new XmlNamespaceManager(xRequestDoc.NameTable);
                    xnm.AddNamespace("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
                    xnm.AddNamespace("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

                    ReplaceDispatcher(nodeList, xRequestDoc, nodeName);

                    xtw = new XmlTextWriter(sw);
                    xtw.Formatting = Formatting.Indented;
                    xtw.Indentation = 8;
                    xtw.IndentChar = ' ';
                    xDoc.WriteTo(xtw);
                    //xDoc.Save(xmlPath);

                    result = true;
                }
                else
                {
                    errDesc = "Can't find DispatcherFormat.xml in running folder!";
                }
            }
            catch (Exception err)
            {
                errDesc = $"Excepetion happen in { System.Reflection.MethodBase.GetCurrentMethod().Name} : { err.Message }";
            }
            finally
            {
                if (xtw != null) xtw.Close();
            }

            requestMessage = sb.ToString();

            return result;
        }

        private static void ReplaceDispatcher(XmlNodeList nodeList, XmlDocument requestDoc, string nodeName)
        {
            string autoChangeMe = "";
            foreach (XmlNode tempNode in nodeList)
            {
                if (tempNode.NodeType != XmlNodeType.Text && tempNode.NodeType != XmlNodeType.Document)
                {
                    string tempPath = GetNodePath(tempNode);

                    if (_dispatcherConfig.ContainsKey("AutoChange"))
                    {
                        if (_dispatcherConfig["AutoChange"].ToString() == tempPath)
                        {
                            autoChangeMe = GetAutoChangeName(nodeName);
                            if (autoChangeMe != "") tempNode.InnerText = autoChangeMe;
                        }
                    }

                    if (_dispatcherConfig.ContainsKey("IsBatch") && nodeName.Contains(_dispatcherConfig["IsBatch"].ToString()))
                    {
                        if (_batchCollection.ContainsKey(tempPath))
                        {
                            if (tempNode.ChildNodes.Count == 1 && tempNode.InnerText == tempNode.InnerXml)
                            {
                                if (requestDoc.SelectSingleNode(_batchCollection[tempPath].ToString()) != null)
                                {
                                    tempNode.InnerText = requestDoc.SelectSingleNode(_batchCollection[tempPath].ToString()).InnerText;
                                }
                            }
                            else
                            {
                                if (requestDoc.SelectSingleNode(_batchCollection[tempPath].ToString()) != null)
                                {
                                    tempNode.InnerXml = requestDoc.SelectSingleNode(_batchCollection[tempPath].ToString()).InnerXml;
                                }
                            }
                            continue;
                        }
                    }
                    else
                    {
                        if (_normalCollection.ContainsKey(tempPath))
                        {
                            if (tempNode.ChildNodes.Count == 1)
                            {
                                if (requestDoc.SelectSingleNode(_normalCollection[tempPath].ToString()) != null)
                                {
                                    tempNode.InnerText = requestDoc.SelectSingleNode(_normalCollection[tempPath].ToString()).InnerText;
                                }
                            }
                            else
                            {
                                if (requestDoc.SelectSingleNode(_normalCollection[tempPath].ToString()) != null)
                                {
                                    tempNode.InnerXml = requestDoc.SelectSingleNode(_normalCollection[tempPath].ToString()).InnerXml;
                                }
                            }
                            continue;
                        }
                    }
                }

                if (tempNode.ChildNodes.Count > 0)
                {
                    ReplaceDispatcher(tempNode.ChildNodes, requestDoc, nodeName);
                }
            }
        }

        private static string GetAutoChangeName(string nodeName)
        {
            string tempStr = "";

            foreach (var tempVar in _methodMapping.Keys)
            {
                if (nodeName.Contains(tempVar.ToString()))
                {
                    tempStr = _methodMapping[tempVar.ToString()].ToString();
                }
            }

            return tempStr;
        }

        private static string GetNodePath(XmlNode node)
        {
            string nodePath = node.Name;
            while (node.ParentNode != null && node.ParentNode.NodeType != XmlNodeType.Document
                && node.ParentNode.NodeType != XmlNodeType.XmlDeclaration
                && node.ParentNode.NodeType != XmlNodeType.Text)
            {
                node = node.ParentNode;
                nodePath = node.Name + "/" + nodePath;
            }
            nodePath = "/" + nodePath;
            return nodePath;
        }

        public static string FormatXml(string sUnformattedXml)
        {
            XmlDocument xd = new XmlDocument();
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter xtw = null;
            try
            {
                xd.LoadXml(sUnformattedXml);
                xtw = new XmlTextWriter(sw);
                xtw.Formatting = Formatting.Indented;
                xtw.Indentation = 8;
                xtw.IndentChar = ' ';
                xd.WriteTo(xtw);
            }
            finally
            {
                if (xtw != null)
                    xtw.Close();
            }
            return sb.ToString();
        }

        public static string RestoreXml(string xmlStrInWebService)
        {
            string s = xmlStrInWebService;
            while (s.Contains("&lt;") || s.Contains("&gt;") ||
                s.Contains("&apos;") || s.Contains("&aquot;") || s.Contains("&amp;"))
            {
                s = s.Replace("&lt;", "<");
                s = s.Replace("&gt;", ">");
                s = s.Replace("&apos;", "'");
                s = s.Replace("&aquot;", "\"");
                s = s.Replace("&amp;", "&");
            }
            return s;
        }

        public static string ToDataFromWebService(string xml)
        {
            string s = xml;
            s = s.Replace("&", "&amp;");
            s = s.Replace("\"", "&aquot;");
            s = s.Replace("'", "&apos;");
            s = s.Replace(">", "&gt;");
            s = s.Replace("<", "&lt;");
            return s;
        }
    }
}
