namespace WebServiceStudio
{
    using System.Web.Services.Protocols;

    public interface IAdditionalProperties
    {
        void UpdateProxy(HttpWebClientProtocol proxy);
    }
}

