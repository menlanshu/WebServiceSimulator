namespace WebServiceStudio
{
    using System;
    using System.Windows.Forms;

    public class XmlTreeNode : TreeNode
    {
        private int endPos;
        private int startPos;

        public XmlTreeNode(string text, int startPos) : base(text)
        {
            this.startPos = startPos;
        }

        public int EndPosition
        {
            get
            {
                return endPos;
            }
            set
            {
                endPos = value;
            }
        }

        public int StartPosition
        {
            get
            {
                return startPos;
            }
            set
            {
                startPos = value;
            }
        }
    }
}

