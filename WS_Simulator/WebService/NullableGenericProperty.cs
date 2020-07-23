﻿namespace WebServiceStudio
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;

    internal class NullableGenericProperty : ClassProperty
    {
		public NullableGenericProperty(Type[] possibleTypes, string name, object val)	: base(possibleTypes, name, val)
        {
					//isNull = false;
					//val = val;
					//isNull = val == null;
					if (possibleTypes.Length == 2)
					{				
						if (possibleTypes[1].FullName == "System.Int32")
						{
							//possibleTypes[0] = new Nullable  <int>();
						}
						possibleTypes[1] = null;
					}
        }

        protected override void CreateChildren()
        {
        }

        public override object ReadChildren(bool update = false)
        {
            return Value;
        }

        public override string ToString()
        {
            string str = ToString();
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

