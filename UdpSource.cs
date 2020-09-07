using System;
using System.Net.Sockets; 
using System.Net;
using ViscaLib.Data;
using System.Linq;

namespace ViscaLib
{
    public class UdpSource:ISource
    {
        private UdpClient _sender;
        private IPEndPoint _ip;
        private ulong _commands;
        private bool _connected;
        public UdpClient Sender 
        { 
            get
            {
                return _sender;
            } 
            set 
            {
                if(value == null)
                {
                    throw new ArgumentNullException("Udp client cannot be null");
                }
                _sender = value;
            } 
        }
        public IPEndPoint Ip 
        {
             get 
             {
                return _ip;
             }
             set
             {

                 if(value == null)
                 {
                     throw new ArgumentNullException("Address cannot be null");
                 }
                 _ip = value;
             } 
        }
        public UdpSource(UdpClient sender,string address,int port)
        {
            Sender = sender;
            Ip = new IPEndPoint(address: IPAddress.Parse(address),port: port);
            _commands = 0;
            _connected = true;
        }
        public UdpSource(string address,int port)
        {
            Sender = new UdpClient();
            Ip = new IPEndPoint(address: IPAddress.Parse(address),port: port);
            _connected = true;
            _commands = 0;
        }
        public UdpSource(IPEndPoint ip)
        {
            Sender = new UdpClient();
            Ip = ip;
            _commands = 0;
            _connected = true;
        }
        public void Connect()
        {
            if(!_connected)
            {
                Sender.Connect(Ip);
            }
        }

        protected virtual void Disconnect(bool stayOpen)
        {
            Sender.Client.Disconnect(stayOpen);
            _connected = false;
        }

        public void Disconnect()
        {
            Disconnect(true);
        }

        private byte[] PreProcess(byte[] arr)
        {
            int length = arr.Length;
            byte[] bytes = BitConverter.GetBytes(_commands);
            byte[] request = new byte[length + 8];
            request[0] = 0x01;
            request[1] = 0x00;
            if(arr[1] == ViscaCodes.Inquiry)
            {
                request[1] = 0x10;
            }
            request[2] = (byte)(length >> 8);
            request[3] = (byte)length;
            bytes.CopyTo(request,4);
            arr.CopyTo(request,8); 
            for(int idx = 0;idx < request.Length; ++idx)
            {
                Console.Write(request[idx].ToString() + " ");
            }
            Console.WriteLine();
            return request;
        }

        public void Send(object data)
        {
            if(data is byte[] rawBytes)
            {
                var request = PreProcess(rawBytes);
                Sender.Send(request,request.Length,Ip);
                _commands++;
            }
            else
            {
                throw new System.ArgumentException("You must send a camera command");
            }
        }

        public CameraResponseModel GetResponse()
        {
            var response = new CameraResponseModel();
            var buffer = Sender.Receive(ref _ip);
            byte[] arr = buffer.Skip(8).ToArray();
            if(buffer.Length < 3)
            {
                return new CameraResponseModel { ResponseType = ViscaCodes.ResponseError,Response = arr.Select(b => (int)b).ToList() };
            }
            response.ResponseType = arr[1] & 0xF0;
            response.Response = arr.Select(b => (int)b).ToList();
            return response;
        }

    }

}