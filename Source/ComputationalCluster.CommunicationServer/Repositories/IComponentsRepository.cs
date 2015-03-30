using ComputationalCluster.CommunicationServer.Models;
using System;

namespace ComputationalCluster.CommunicationServer.Repositories
{
    public interface IComponentsRepository
    {
        ulong Register(Component component);
        void Deregister(ulong componentId);
        Component GetById(ulong componentId);
        void UpdateLastStatusTimestamp(ulong componentId);
        void RemoveInactive();
    }
}
