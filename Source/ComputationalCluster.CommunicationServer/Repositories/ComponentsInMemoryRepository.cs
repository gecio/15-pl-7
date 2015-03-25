using ComputationalCluster.Common;
using ComputationalCluster.CommunicationServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputationalCluster.CommunicationServer.Repositories
{
    public class ComponentsInMemoryRepository : IComponentsRepository
    {
        private readonly ITimeProvider _timeProvider;

        private ulong _nextValidGuid = 1;
        private Dictionary<ulong, Component> _componentDictionary;

        public ComponentsInMemoryRepository(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;

            _componentDictionary = new Dictionary<ulong, Component>();
        }

        public ulong Register(Component component)
        {
            component.Id = _nextValidGuid++;

            _componentDictionary.Add(component.Id, component);

            return component.Id;
        }

        public Component GetById(ulong componentId)
        {
            return _componentDictionary.FirstOrDefault(t => componentId == t.Key).Value;
        }

        public void UpdateLastStatusTimestamp(ulong componentId)
        {
            var component = GetById(componentId);
            if (component == null)
            {
                return;
            }

            component.LastStatusTimestamp = _timeProvider.Now;
        }

        public void RemoveInactive()
        {
            var timeout = new TimeSpan(0, 0, 30); // TODO: config?
            var minimalTime = _timeProvider.Now.Subtract(timeout);

            _componentDictionary = _componentDictionary
                .Where(t => t.Value.LastStatusTimestamp >= minimalTime)
                .ToDictionary(t => t.Key, t => t.Value);
        }
    }
}
