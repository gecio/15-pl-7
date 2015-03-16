﻿using Autofac;
using ComputationalCluster.Common;
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
        private readonly IMessageReceiver _messageReceiver;
        private readonly Encoding _encoding;
        private readonly IConfigProvider _configProvider;

        private TcpListener _tcpListener;
        private Thread _listeningThread;

        private volatile bool _shoudStop;

        private ManualResetEvent _tcpClientConnected = new ManualResetEvent(false); // thread signal

        public NetServer(IMessageReceiver messageReceiver, Encoding encoding, IConfigProvider configProvider)
        {
            _messageReceiver = messageReceiver;
            _encoding        = encoding;
            _configProvider = configProvider;
        }

        public void Start()
        {
            _tcpListener = new TcpListener(IPAddress.Any, _configProvider.Port);
            _listeningThread = new Thread(new ThreadStart(ListenForConnections));

            _shoudStop = false;
            _listeningThread.Start();
        }

        public void Stop()
        {
            _shoudStop = true;
            _tcpClientConnected.Set(); // break waiting for connection
            _listeningThread.Join();
            _tcpListener.Stop();
        }

        private void ListenForConnections()
        {
            _tcpListener.Start();
            
            while (!_shoudStop)
            {
                _tcpClientConnected.Reset();
                _tcpListener.BeginAcceptTcpClient(new AsyncCallback(HandleIncomingConnection), _tcpListener);
                _tcpClientConnected.WaitOne(); // wait for connection
            }
        }

        private void HandleIncomingConnection(IAsyncResult asyncResult)
        {
            try
            {
                var listener = (TcpListener)asyncResult.AsyncState;
                var tcpClient = (TcpClient)listener.EndAcceptTcpClient(asyncResult);

                _tcpClientConnected.Set(); // run waiting for next connection

                var stream = tcpClient.GetStream();

                var requestBuffer = stream.ReadBuffered(0);
                var request = _encoding.GetString(requestBuffer, 0, requestBuffer.Length);

                var response = _messageReceiver.Dispatch(request);

                byte[] responseBuffer = _encoding.GetBytes(response);
                stream.WriteBuffered(responseBuffer, 0, responseBuffer.Length);

                tcpClient.Close();
            }
            catch(ObjectDisposedException ex)
            {
                //todo: connection closed, logi
            }
        }

    }
}
