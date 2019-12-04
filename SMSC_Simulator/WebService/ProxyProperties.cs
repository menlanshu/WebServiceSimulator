﻿namespace WebServiceStudio
{
    using System;
    using System.Collections;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Web.Services.Protocols;

    internal class ProxyProperties
    {
        private static Hashtable proxyTypeHandlers;
        private static readonly string typeNotFoundMessage = "ProxyPropertiesType {0} specified in WebServiceStudio.exe.options is not found";

        public ProxyProperties()
        {
        }

        public ProxyProperties(HttpWebClientProtocol proxy)
        {
            Timeout = proxy.Timeout;
            AllowAutoRedirect = proxy.AllowAutoRedirect;
            PreAuthenticate = proxy.PreAuthenticate;
            if (proxy.CookieContainer == null)
            {
                UseCookieContainer = true;
            }
            Server = new ServerProperties
            {
                Url = proxy.Url
            };
            SetCredentialValues(proxy.Credentials, new Uri(Server.Url), out Server.UseDefaultCredentials, out Server.UserNameForBasicAuth, out Server.PasswordForBasicAuth);
            if (proxy.Proxy is WebProxy proxy2)
            {
                HttpProxy = new ServerProperties
                {
                    Url = proxy2.Address.ToString()
                };
                SetCredentialValues(proxy2.Credentials, new Uri(HttpProxy.Url), out HttpProxy.UseDefaultCredentials, out HttpProxy.UserNameForBasicAuth, out HttpProxy.PasswordForBasicAuth);
            }
            InitAdditionalProperties(proxy);
        }

        private void InitAdditionalProperties(HttpWebClientProtocol proxy)
        {
            if (proxyTypeHandlers == null)
            {
                proxyTypeHandlers = new Hashtable();
                CustomHandler[] proxyProperties = Configuration.MasterConfig.ProxyProperties;
                if ((proxyProperties != null) && (proxyProperties.Length > 0))
                {
                    foreach (CustomHandler handler in proxyProperties)
                    {
                        string typeName = handler.TypeName;
                        string str2 = handler.Handler;
                        if (((typeName != null) && (typeName.Length != 0)) && ((str2 != null) && (str2.Length != 0)))
                        {
                            Type key = Type.GetType(typeName);
                            if (key == null)
                            {
                                //MainForm.ShowMessage(this, MessageType.Warning, string.Format(typeNotFoundMessage, typeName));
                            }
                            else
                            {
                                Type type = Type.GetType(str2);
                                if (type == null)
                                {
                                    //MainForm.ShowMessage(this, MessageType.Warning, string.Format(typeNotFoundMessage, str2));
                                }
                                else
                                {
                                    proxyTypeHandlers.Add(key, type);
                                }
                            }
                        }
                    }
                }
            }
            for (Type type3 = proxy.GetType(); type3 != typeof(object); type3 = type3.BaseType)
            {
                Type type4 = proxyTypeHandlers[type3] as Type;
                if (type4 != null)
                {
                    AdditionalProperties = (IAdditionalProperties) Activator.CreateInstance(type4, new object[] { proxy });
                    break;
                }
            }
        }

        private ICredentials ReadCredentials(ICredentials credentials, Uri uri, bool useDefaultCredentials, string userName, string password)
        {
            if ((credentials != null) && !(credentials is CredentialCache))
            {
                return credentials;
            }
            if (!(credentials is CredentialCache cache))
            {
                cache = new CredentialCache();
            }
            if (useDefaultCredentials)
            {
                cache.Add(uri, "NTLM", (NetworkCredential) CredentialCache.DefaultCredentials);
            }
            else
            {
                cache.Remove(uri, "NTLM");
            }
            if ((((userName != null) && (userName.Length > 0)) || ((password != null) && (password.Length > 0))) && (cache.GetCredential(uri, "Basic") == null))
            {
                NetworkCredential cred = new NetworkCredential("", "");
                cache.Add(uri, "Basic", cred);
            }
            return cache;
        }

        private void SetCredentialValues(ICredentials credentials, Uri uri, out bool useDefaultCredentials, out string userName, out string password)
        {
            useDefaultCredentials = false;
            userName = "";
            password = "";
            if (((credentials == null) || (credentials is CredentialCache)) && (credentials != null))
            {
                NetworkCredential credential = null;
                if (credentials is CredentialCache cache)
                {
                    if (CredentialCache.DefaultCredentials == cache.GetCredential(uri, "NTLM"))
                    {
                        useDefaultCredentials = true;
                    }
                    credential = cache.GetCredential(uri, "Basic");
                }
                else if (credentials == CredentialCache.DefaultCredentials)
                {
                    useDefaultCredentials = true;
                }
                else
                {
                    credential = credentials as NetworkCredential;
                }
                if (credential != null)
                {
                    userName = credential.UserName;
                    password = credential.Password;
                }
            }
        }

        public void UpdateProxy(HttpWebClientProtocol proxy)
        {
            proxy.Timeout = Timeout;
            proxy.AllowAutoRedirect = AllowAutoRedirect;
            proxy.PreAuthenticate = PreAuthenticate;
            if (UseCookieContainer)
            {
                if (proxy.CookieContainer == null)
                {
                    proxy.CookieContainer = new CookieContainer();
                }
            }
            else
            {
                proxy.CookieContainer = null;
            }
            proxy.Url = Server.Url;
            proxy.Credentials = ReadCredentials(proxy.Credentials, new Uri(Server.Url), Server.UseDefaultCredentials, Server.UserNameForBasicAuth, Server.PasswordForBasicAuth);
            if (((HttpProxy != null) && (HttpProxy.Url != null)) && (HttpProxy.Url.Length > 0))
            {
                Uri uri = new Uri(HttpProxy.Url);
                if (proxy.Proxy == null)
                {
                    proxy.Proxy = new WebProxy();
                }
                WebProxy proxy2 = proxy.Proxy as WebProxy;
                proxy2.Address = uri;
                proxy2.Credentials = ReadCredentials(proxy2.Credentials, uri, Server.UseDefaultCredentials, Server.UserNameForBasicAuth, Server.PasswordForBasicAuth);
            }
            if (AdditionalProperties != null)
            {
                AdditionalProperties.UpdateProxy(proxy);
            }
        }

        public IAdditionalProperties AdditionalProperties { get; set; }

        public bool AllowAutoRedirect { get; set; }

        public ServerProperties HttpProxy { get; set; }

        public bool PreAuthenticate { get; set; }

        public ServerProperties Server { get; set; }

        public int Timeout { get; set; }

        public bool UseCookieContainer { get; set; }

        public class ServerProperties
        {
            public string PasswordForBasicAuth;
            public string Url;
            public bool UseDefaultCredentials;
            public string UserNameForBasicAuth;
        }
    }
}

