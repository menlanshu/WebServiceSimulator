namespace WebServiceStudio
{
    using System.IO;

    internal class NoCloseMemoryStream : MemoryStream
    {
        public override void Close()
        {
        }
    }
}

