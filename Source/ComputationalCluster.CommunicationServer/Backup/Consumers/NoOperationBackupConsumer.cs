using ComputationalCluster.Communication.Messages;
using ComputationalCluster.NetModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Backup.Consumers
{
    public class NoOperationBackupConsumer : IMessageConsumer<NoOperation>
    {
        public ICollection<IMessage> Consume(NoOperation message, ConnectionInfo connectionInfo = null)
        {
            return new List<IMessage>();
        }

        public ICollection<IMessage> Consume(IMessage message, ConnectionInfo connectionInfo = null)
        {
            return new List<IMessage>();
        }
    }
}
