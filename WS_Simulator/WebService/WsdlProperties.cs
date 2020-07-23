namespace WebServiceStudio
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WsdlProperties
    {
        private string customCodeDomProvider;
        private string domain;
        private WebServiceStudio.Language language;
        private string password;
        private WebServiceStudio.Protocol protocol;
        private string proxy;
        private string proxyBaseType;
        private string proxyDomain;
        private string proxyPassword;
        private string proxyUserName;
        private string userName;

        public string[] GetProxyBaseTypeList()
        {
            return Configuration.MasterConfig.GetProxyBaseTypes();
        }

        public override string ToString()
        {
            return "";
        }

        [RefreshProperties(RefreshProperties.All), XmlAttribute]
        public string CustomCodeDomProvider
        {
            get
            {
                return ((language == WebServiceStudio.Language.Custom) ? customCodeDomProvider : "");
            }
            set
            {
                customCodeDomProvider = value;
                if ((value != null) && (value.Length > 0))
                {
                    language = WebServiceStudio.Language.Custom;
                }
            }
        }

        [XmlAttribute]
        public string Domain
        {
            get
            {
                return domain;
            }
            set
            {
                domain = value;
            }
        }

        [XmlAttribute, RefreshProperties(RefreshProperties.All)]
        public WebServiceStudio.Language Language
        {
            get
            {
                return language;
            }
            set
            {
                language = value;
            }
        }

        [XmlAttribute]
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }

        [XmlAttribute]
        public WebServiceStudio.Protocol Protocol
        {
            get
            {
                return protocol;
            }
            set
            {
                protocol = value;
            }
        }

        [TypeConverter(typeof(ListStandardValues)), XmlAttribute]
        public string ProxyBaseType
        {
            get
            {
                return proxyBaseType;
            }
            set
            {
                proxyBaseType = value;
            }
        }

        [XmlAttribute]
        public string ProxyDomain
        {
            get
            {
                return proxyDomain;
            }
            set
            {
                proxyDomain = value;
            }
        }

        [XmlAttribute]
        public string ProxyPassword
        {
            get
            {
                return proxyPassword;
            }
            set
            {
                proxyPassword = value;
            }
        }

        [XmlAttribute]
        public string ProxyServer
        {
            get
            {
                return proxy;
            }
            set
            {
                proxy = value;
            }
        }

        [XmlAttribute]
        public string ProxyUserName
        {
            get
            {
                return proxyUserName;
            }
            set
            {
                proxyUserName = value;
            }
        }

        [XmlAttribute]
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }
        }
    }
}

