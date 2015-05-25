using ComputationalCluster.Common;
using log4net;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ComputationalCluster.NetModule
{
    /// <summary>
    /// Interfejs warstwy abstrakcji dla połączeń TCP/IP.
    /// </summary>
    public interface INetServer
    {
        void Start();
        void Stop();
    }

    /// <summary>
    /// Warstwa abstrakcji dla połączeń TCP/IP, umożliwia wygodne rejestrowanie konsumentów dla danych
    /// wiadomości, transformacji oraz zwracania odpowiedzi. Sterowanie konsumentami odbywa się poprzez
    /// poprawne skonfigurowanie odbiorcy wiadomości.
    /// </summary>
    public class NetServer : INetServer
    {
        public static readonly char ETB = (char)23;

        private readonly IMessageReceiver _messageReceiver;
        private readonly Encoding _encoding;
        private readonly IConfigProvider _configProvider;
        private readonly ILog _log;

        private TcpListener _tcpListener;
        private Thread _listeningThread;

        private volatile bool _shouldStop;

        private ManualResetEvent _tcpClientConnected = new ManualResetEvent(false); // thread signal

        public NetServer(IMessageReceiver messageReceiver, Encoding encoding, IConfigProvider configProvider,
            ILog log)
        {
            _messageReceiver = messageReceiver;
            _encoding        = encoding;
            _configProvider  = configProvider;
            _log             = log;
        }

        public void Start()
        {
            _tcpListener = new TcpListener(IPAddress.Any, _configProvider.Port);
            _listeningThread = new Thread(new ThreadStart(ListenForConnections));

            _shouldStop = false;
            _listeningThread.Start();

            _log.Info("Started.");
        }

        public void Stop()
        {
            _shouldStop = true;
            _tcpClientConnected.Set(); // break waiting for connection
            _listeningThread.Join();
            _tcpListener.Stop();

            _log.Info("Stopped.");
        }

        private void ListenForConnections()
        {
            _tcpListener.Start();
            
            while (!_shouldStop)
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
                
                _log.InfoFormat("Connected: {0}", tcpClient.Client.AddressFamily.ToString());

                _tcpClientConnected.Set(); // run waiting for next connection

                var stream = tcpClient.GetStream();

                var requestBuffer = stream.ReadBuffered(0);
                var request = _encoding.GetString(requestBuffer, 0, requestBuffer.Length);

                var connectionInfo = new ConnectionInfo
                {
                    IpAddress = (tcpClient.Client.RemoteEndPoint as IPEndPoint).Address,
                    Port = (tcpClient.Client.RemoteEndPoint as IPEndPoint).Port
                };

                var response = _messageReceiver.Dispatch(request,connectionInfo);

                byte[] responseBuffer = _encoding.GetBytes(response);
                
                stream.WriteBuffered(responseBuffer, 0, responseBuffer.Length);

                tcpClient.Client.Shutdown(SocketShutdown.Send);
                tcpClient.Close();
            }
            catch(ObjectDisposedException ex)
            {
                _log.ErrorFormat("Error in HandleIncomingConnection, StackTrace: {9}", ex.StackTrace.ToString());
            }
        }

    }
}
