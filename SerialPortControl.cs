using System;
using System.IO.Ports;
using ViscaLib.Data;

namespace ViscaLib
{
    public class SerialPortSource:ISource
    {
        public SerialPort Port { get; set; }
        private readonly bool _useTimeout;

        public SerialPortSource(SerialPort port)
        {
            Port = port;
        }

        public SerialPortSource(string comPort,bool useTimeout)
        {
            Port = new SerialPort(comPort)
            {
                BaudRate = 9600, DataBits = 8, Parity = Parity.None, Handshake = Handshake.None, ReadTimeout = 2000
            };
            _useTimeout = useTimeout;
        }

        public void Connect()
        {
            Port.Open();
        }

        public void Disconnect()
        {
            Port.Close();
        }

        public void Send(object data)
        {
            if(data is byte[] rawBytes)
                Port.Write(rawBytes, 0, rawBytes.Length);
        }


        public CameraResponseModel GetResponse()
        {
            int oldTimeout = Port.ReadTimeout;
            if (!_useTimeout)
            {
                Port.ReadTimeout = SerialPort.InfiniteTimeout;
            }

            var response = ReadFromBuffer(oldTimeout);
            Port.ReadTimeout = oldTimeout;
            if (response.Response.Count < 3)
            {
                return new CameraResponseModel { ResponseType = ViscaCodes.ResponseError, Response = response.Response};
            }

            response.ResponseType = response.Response[1] & 0xF0;
            return response;
        }
        private CameraResponseModel ReadFromBuffer(int oldTimeout)
        {
            CameraResponseModel response = new CameraResponseModel();
            while (true)
            {
                try
                {
                    response.Response.Add(Port.ReadByte());
                }
                catch (TimeoutException)
                {
                    Port.ReadTimeout = oldTimeout;
                    return new CameraResponseModel {ResponseType = ViscaCodes.ResponseTimeout};
                }

                if (response.Response[^1] == ViscaCodes.Terminator)
                {
                    break;
                }
            }

            return response;

        }
    }

}
