using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComputationalCluster.NetModule
{
    public interface INetServer
    {
        void Start();
        void Stop();
    }

    /// <summary>
    /// </summary>
    public class NetServer : INetServer
    {
        private readonly int _port = 3000;

        private readonly IMessageReceiver _messageReceiver;
        private readonly Encoding _encoding;

        private TcpListener _tcpListener;
        private Thread _listeningThread;

        public NetServer(IMessageReceiver messageReceiver, Encoding encoding)
        {
            _messageReceiver = messageReceiver;
            _encoding        = encoding;
        }

        public void Start()
        {
            _tcpListener = new TcpListener(IPAddress.Any, _port);
            _listeningThread = new Thread(new ThreadStart(ListenForConnections));

            _listeningThread.Start();
        }

        public void Stop()
        {
            _tcpListener.Stop();
        }

        private void ListenForConnections()
        {
            _tcpListener.Start();
            
            while (true)
            {
                // todo: poczytać o BeginTcpAcceptClient i przerobić na rozwiązanie nieblokujące!
                var client = _tcpListener.AcceptTcpClient();
                var clientThread = new Thread(new ParameterizedThreadStart(HandleIncomingConnection));
                clientThread.Start(client);
            }
        }

        private void HandleIncomingConnection(object tcpClientObject)
        {
            var tcpClient = (TcpClient)tcpClientObject;
            var stream = tcpClient.GetStream();

            var requestBuffer = new byte[4096*4];

            var bytesRead = stream.Read(requestBuffer, 0, requestBuffer.Length);
            var request = _encoding.GetString(requestBuffer, 0, bytesRead);

            var response = _messageReceiver.Dispatch(request);

            byte[] responseBuffer = _encoding.GetBytes(response);
            stream.Write(responseBuffer, 0, responseBuffer.Length);

            tcpClient.Close();
        }
    }
}
