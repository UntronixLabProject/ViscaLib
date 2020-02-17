using ViscaLib.Data;

namespace ViscaLib
{
    public interface ISource
    {
        void Connect();
        void Disconnect();
        void Send(object data);
        CameraResponseModel GetResponse();
    }
}
