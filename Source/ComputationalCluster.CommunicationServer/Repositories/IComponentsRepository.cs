using ComputationalCluster.CommunicationServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Repositories
{
    public interface IComponentsRepository
    {
        void Add(Component component);
        Component GetById(ulong componentId);
        void UpdateLastStatusTimestamp(ulong componentId);
    }
}
