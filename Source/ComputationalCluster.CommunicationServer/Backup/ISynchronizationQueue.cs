using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.NetModule;

namespace ComputationalCluster.CommunicationServer.Backup
{
    public interface ISynchronizationQueue
    {
        void Enqueue(IMessage message);
        IMessage Dequeue();
        void InitializeSync();
        ICollection<IMessage> DequeueAll();
    }
}
