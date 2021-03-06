﻿namespace WebServiceStudio
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Configuration;
    using System.Xml;
    using System.Windows.Forms;

    internal class NullablePrimitiveProperty : ClassProperty
    {
        public static Func<string, string> GetNodeValue;

        public NullablePrimitiveProperty(Type[] possibleTypes, string name, object val) : base(possibleTypes, name, val)
        {
        }

        protected override void CreateChildren()
        {
        }

        public override object ReadChildren(bool update = false)
        {
            if (update)
            {
                Value = GetNodeValue(TreeNode.Text);
            }
            return Value;
        }

        public override string ToString()
        {
            string str = base.ToString();
            if (Value == null)
            {
                return str;
            }
            return (str + " = " + Value.ToString());
        }

        [RefreshProperties(RefreshProperties.All), Editor(typeof(DynamicEditor), typeof(UITypeEditor)), TypeConverter(typeof(DynamicConverter))]
        public object Value
        {
            get
            {
                return InternalValue;
            }
            set
            {
                InternalValue = value;
                TreeNode.Text = ToString();
            }
        }
    }
}

