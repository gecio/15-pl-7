using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.NetModule
{
    public interface INetClient
    {
        IMessage Send(IMessage message);
    }

    public class NetClient : INetClient
    {
        private readonly int _port = 3000; // todo: refactor

        private readonly IMessageTranslator _messageTranslator;
        private readonly Encoding _encoding;

        public NetClient(IMessageTranslator translator, Encoding encoding)
        {
            _messageTranslator = translator;
            _encoding          = encoding;
        }

        public IMessage Send(IMessage message)
        {
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _port);

            var client = new TcpClient();
            client.Connect(serverEndPoint);
            var stream = client.GetStream();
            var request = _messageTranslator.Stringify(message);
            byte[] encodedRequest = _encoding.GetBytes(request + NetServer.ETB);
            stream.WriteBuffered(encodedRequest, 0, encodedRequest.Length);

            byte[] encodedResponse = stream.ReadBuffered(0);
            var response = _encoding.GetString(encodedResponse, 0, encodedResponse.Length);
            client.Close();

            var responseMessage = _messageTranslator.CreateObject(response);
            return responseMessage;
        }

    }
}
