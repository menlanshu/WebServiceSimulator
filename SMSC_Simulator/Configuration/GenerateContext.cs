using System;
using System.Configuration;

namespace SMSC_Simulator
{
    public class GenerateContext : ConfigurationSection
    {
        [ConfigurationProperty("GenerateContext", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(GenerateContextSection),
            CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap, RemoveItemName = "remove")]
        public GenerateContextSection ContextGroupList
        {
            get
            {
                return (GenerateContextSection)base["GenerateContext"];
            }

            set
            {
                base["GenerateContext"] = value;
            }
        }
    }

    public class GenerateContextSection : ConfigurationElementCollection
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ContextGroup)element).Name;
        }

        protected override ConfigurationElement CreateNewElement()
        {

            return new ContextGroup();
        }

        public ContextGroup this[int i]
        {
            get
            {
                return (ContextGroup)base.BaseGet(i);
            }
        }

        public ContextGroup this[string key]
        {
            get
            {
                return (ContextGroup)base.BaseGet(key);
            }
        }
    }

    public class ContextGroup : ConfigurationElement
    {
        private int currentCount;

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
            set
            {
                base["name"] = value;
            }
        }

        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        public string Type
        {
            get
            {
                return (string)base["type"];
            }
            set
            {
                base["type"] = value;
            }
        }


        [ConfigurationProperty("count", IsRequired = true, IsKey = true)]
        public int Count
        {
            get
            {
                return base["count"].ToString() =="" ? 0 : (int)base["count"];
            }
            set
            {
                base["count"] = value;
            }
        }

        [ConfigurationProperty("mode", IsRequired = true, IsKey = true)]
        public string Mode
        {
            get
            {
                return (string)base["mode"];
            }
            set
            {
                base["mode"] = value;
            }
        }

        public int CurrentCount
        {
            get
            {
                return currentCount;
            }
            set
            {
                currentCount = value;
            }
        }

    }
}
