namespace WebServiceStudio
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    internal class ArrayProperty : ClassProperty
    {
        public ArrayProperty(Type[] possibleTypes, string name, Array val) : base(possibleTypes, name, val)
        {
        }

        protected override void CreateChildren()
        {
            TreeNode.Nodes.Clear();
            if (OkayToCreateChildren())
            {
                Type elementType = Type.GetElementType();
                int length = Length;
                for (int i = 0; i < length; i++)
                {
                    object val = ArrayValue.GetValue(i);
                    if ((val == null) && IsInput())
                    {
                        val = CreateNewInstance(elementType);
                    }
                    CreateTreeNodeProperty(GetIncludedTypes(elementType), Name + "_" + i.ToString(), val).RecreateSubtree(TreeNode);
                }
            }
        }

        public override object ReadChildren(bool update = false)
        {
            Array arrayValue = ArrayValue;
            if (arrayValue == null)
            {
                return null;
            }
            int num = 0;
            for (int i = 0; i < arrayValue.Length; i++)
            {
                TreeNode node = TreeNode.Nodes[num++];
                if (node.Tag is TreeNodeProperty tag)
                {
                    arrayValue.SetValue(tag.ReadChildren(), i);
                }
            }
            return arrayValue;
        }

        private Array ArrayValue
        {
            get
            {
                return (InternalValue as Array);
            }
            set
            {
                InternalValue = value;
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        public virtual int Length
        {
            get
            {
                return ((ArrayValue != null) ? ArrayValue.Length : 0);
            }
            set
            {
                int length = Length;
                int num2 = value;
                Array destinationArray = Array.CreateInstance(Type.GetElementType(), num2);
                if (ArrayValue != null)
                {
                    Array.Copy(ArrayValue, destinationArray, Math.Min(num2, length));
                }
                ArrayValue = destinationArray;
                TreeNode.Text = ToString();
                CreateChildren();
            }
        }
    }
}

