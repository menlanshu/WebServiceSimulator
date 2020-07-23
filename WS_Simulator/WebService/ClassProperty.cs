namespace WebServiceStudio
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows.Forms;

    internal class ClassProperty : TreeNodeProperty
    {
        private bool isNull;
        private object oValue;

        public ClassProperty(Type[] possibleTypes, string name, object val) : base(possibleTypes, name)
        {
            isNull = false;
            oValue = val;
            isNull = oValue == null;
        }

        protected override void CreateChildren()
        {
            TreeNode.Nodes.Clear();
            if (OkayToCreateChildren())
            {
                foreach (PropertyInfo pInfo in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    object val = pInfo.GetValue(oValue, null);
                    if ((val == null) && IsInput())
                    {
                        val = CreateNewInstance(pInfo.PropertyType);
                    }
                    CreateTreeNodeProperty(GetIncludedTypes(pInfo.PropertyType), pInfo.Name, val).RecreateSubtree(TreeNode);
                }
                foreach (FieldInfo fInfo2 in Type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    object obj3 = fInfo2.GetValue(oValue);

                    if ((obj3 == null) && IsInput())
                    {
                        obj3 = CreateNewInstance(fInfo2.FieldType);
                    }
                    CreateTreeNodeProperty(GetIncludedTypes(fInfo2.FieldType), fInfo2.Name, obj3).RecreateSubtree(TreeNode);
                }
            }
        }

        protected virtual bool OkayToCreateChildren()
        {
            if (IsInternalType(Type))
            {
                return false;
            }
            if (IsDeepNesting(this))
            {
                InternalValue = null;
            }
            if (InternalValue == null)
            {
                return false;
            }
            return true;
        }

        public override object ReadChildren(bool update = false)
        {
            object internalValue = InternalValue;
            if (internalValue == null)
            {
                return null;
            }
            int num = 0;
            foreach (PropertyInfo pInfo in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                TreeNode node = TreeNode.Nodes[num++];
                if (node.Tag is TreeNodeProperty tag)
                {
                    pInfo.SetValue(internalValue, tag.ReadChildren(update), null);
                }
            }
            foreach (FieldInfo fInfo in Type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                TreeNode node = TreeNode.Nodes[num++];
                if (node.Tag is TreeNodeProperty property2)
                {
                    fInfo.SetValue(internalValue, property2.ReadChildren(update), BindingFlags.Public, null, null);
                }
            }
            return internalValue;
        }

        public virtual object ToObject()
        {
            return InternalValue;
        }

        public override string ToString()
        {
            return (GetTypeList()[0].Name + " " + Name + (IsNull ? " = null" : ""));
        }

        internal object InternalValue
        {
            get
            {
                return (isNull ? null : oValue);
            }
            set
            {
                oValue = value;
                isNull = value == null;
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        public bool IsNull
        {
            get
            {
                return isNull;
            }
            set
            {
                if (isNull != value)
                {
                    if (!value)
                    {
                        if (oValue == null)
                        {
                            oValue = CreateNewInstance(Type);
                        }
                        if (oValue == null)
                        {
                            MessageBox.Show("Not able to create an instance of " + Type.FullName);
                            value = true;
                        }
                    }
                    else
                    {
                        ReadChildren();
                    }
                    isNull = value;
                    CreateChildren();
                    TreeNode.Text = ToString();
                }
            }
        }

        public override Type Type
        {
            get
            {
                return ((InternalValue != null) ? InternalValue.GetType() : base.Type);
            }
            set
            {
                try
                {
                    if (Type != value)
                    {
                        InternalValue = CreateNewInstance(value);
                    }
                }
                catch
                {
                    InternalValue = null;
                }
            }
        }
    }
}

