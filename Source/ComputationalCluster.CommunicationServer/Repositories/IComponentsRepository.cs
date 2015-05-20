using ComputationalCluster.CommunicationServer.Models;
using System;
using System.Collections.Generic;
using ComputationalCluster.NetModule;

namespace ComputationalCluster.CommunicationServer.Repositories
{
    public interface IComponentsRepository
    {
        ulong Register(Component component);
        IMessage Deregister(ulong componentId);
        Component GetById(ulong componentId);
        void UpdateLastStatusTimestamp(ulong componentId);
        IMessage[] RemoveInactive();
        IEnumerable<Component> GetAll();
        Component GetBackupServer();
    }
}
