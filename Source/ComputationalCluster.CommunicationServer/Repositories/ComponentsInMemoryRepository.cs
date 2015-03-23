using ComputationalCluster.CommunicationServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Repositories
{
    public class ComponentsInMemoryRepository : IComponentsRepository
    {
        private Dictionary<ulong, Component> _componentDictionary;

        public ComponentsInMemoryRepository()
        {
            _componentDictionary = new Dictionary<ulong, Component>();
        }

        public void Add(Component component)
        {
            _componentDictionary.Add(component.Id, component);
        }

        public Component GetById(ulong componentId)
        {
            return _componentDictionary.FirstOrDefault(t => componentId == t.Key).Value;
        }

        public void UpdateLastStatusTimestamp(ulong componentId)
        {
            var component = GetById(componentId);
            component.LastStatusTimestamp = DateTime.Now; // TODO: poprawka na timeprovider, po merge
        }
    }
}
