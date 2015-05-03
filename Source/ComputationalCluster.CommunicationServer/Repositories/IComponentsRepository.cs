using ComputationalCluster.CommunicationServer.Models;
using System;
using System.Collections.Generic;

namespace ComputationalCluster.CommunicationServer.Repositories
{
    public interface IComponentsRepository
    {
        ulong Register(Component component);
        void Deregister(ulong componentId);
        Component GetById(ulong componentId);
        void UpdateLastStatusTimestamp(ulong componentId);
        void RemoveInactive();
        IEnumerable<Component> GetAll();
        Component GetBackupServer();
    }
}
