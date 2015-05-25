using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.NetModule;

namespace ComputationalCluster.CommunicationServer.Backup.Consumers
{
    public class SolveRequestBackupConsumer : IMessageConsumer<SolveRequest>
    {
        private IMessageConsumer<SolveRequest> _solveRequestConsumer;

        public SolveRequestBackupConsumer(IMessageConsumer<SolveRequest> solveRequestMessageConsumer)
        {
            _solveRequestConsumer = solveRequestMessageConsumer;
        }

        public ICollection<IMessage> Consume(SolveRequest message, ConnectionInfo connectionInfo = null)
        {
            _solveRequestConsumer.Consume(message, connectionInfo);
            return new List<IMessage>();
        }

        public ICollection<IMessage> Consume(IMessage message, ConnectionInfo connectionInfo = null)
        {
            _solveRequestConsumer.Consume(message, connectionInfo);
            return new List<IMessage>();
        }
    }
}
