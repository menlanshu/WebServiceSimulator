namespace WebServiceStudio
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;

    internal class PrimitiveProperty : TreeNodeProperty
    {
        private object val;

        public PrimitiveProperty(string name, object val) : base(new Type[] { val.GetType() }, name)
        {
            this.val = val;
        }

        public override object ReadChildren(bool update = false)
        {
            return Value;
        }

        public override string ToString()
        {
            return string.Concat(new object[] { Type.Name, " ", Name, " = ", Value });
        }

        [Editor(typeof(DynamicEditor), typeof(UITypeEditor)), TypeConverter(typeof(DynamicConverter))]
        public object Value
        {
            get
            {
                return val;
            }
            set
            {
                val = value;
                TreeNode.Text = ToString();
            }
        }
    }
}

