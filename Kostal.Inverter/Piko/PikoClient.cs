using Kostal.Inverter.Contract;
using Kostal.Inverter.Internal;
using System;
using System.IO;
using System.Net.Sockets;

namespace Kostal.Inverter.Piko
{
    public class PikoClient : IDisposable
    {
        private string _host;
        private int _port;
        private Socket _socket;

        public PikoClient()
            : base()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.ReceiveTimeout = 5000;
            _socket.SendTimeout = 5000;
        }

        public PikoClient(string host)
            : this()
        {
            Connect(host);
        }

        public void Connect(string host)
        {
            _host = host;
            _port = 81;

            _socket.Connect(_host, _port);
        }

        public DeviceInfo GetDeviceInfo(byte address)
        {
            var info = new DeviceInfo();

            SendCommand(address, 0x44);

            using (var reader = ReadResponse())
            {
                info.Name = new string(reader.ReadChars(15)).TrimEnd((char)0x00);
            }

            SendCommand(address, 0x90);

            using (var reader = ReadResponse())
            {
                info.Model = new string(reader.ReadChars(16)).TrimEnd((char)0x00);
            }

            SendCommand(address, 0x50);

            using (var reader = ReadResponse())
            {
                info.Serial = new string(reader.ReadChars(13)).TrimEnd((char)0x00);
            }

            return info;
        }

        public InverterState GetStatus(byte address)
        {
            SendCommand(address, 0x57);

            using (var reader = ReadResponse())
            {
                return (InverterState)reader.ReadByte();
            }
        }

        public double GetDailyKwh(byte address)
        {
            SendCommand(address, 0x9d);

            using (var reader = ReadResponse())
            {
                return reader.ReadUInt32() / 1000.0;
            }
        }

        public double GetTotalKwh(byte address)
        {
            SendCommand(address, 0x45);

            using (var reader = ReadResponse())
            {
                return reader.ReadUInt32() / 1000.0;
            }
        }

        private void SendCommand(byte address, byte command)
        {
            byte[] bytes = BuildCommand(address, command);
            _socket.Send(bytes);
        }

        private BinaryReader ReadResponse()
        {
            var readBytes = new byte[1024];
            var numBytes = _socket.Receive(readBytes);
            Array.Resize(ref readBytes, numBytes);

            var stream = new MemoryStream(readBytes);
            var reader = new BinaryReader(stream);
            stream.Seek(5, SeekOrigin.Begin);
            return reader;
        }

        private byte[] BuildCommand(byte address, byte command)
        {
            ChecksumByteList bytes = new ChecksumByteList();
            bytes.Add(0x62);
            bytes.Add(address);
            bytes.Add(0x03);
            bytes.Add(address);
            bytes.Add(0x00);
            bytes.Add(command);
            bytes.AddChecksum();
            bytes.Add(0x00);

            return bytes.ToArray();
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_socket != null)
                {
                    try
                    {
                        _socket.Shutdown(SocketShutdown.Both);
                    }
                    finally
                    {
                        _socket.Close();
                        _socket = null;
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PikoClient()
        {
            Dispose(false);
        }
    }
}
