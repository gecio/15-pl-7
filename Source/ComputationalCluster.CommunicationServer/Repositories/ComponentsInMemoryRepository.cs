using ComputationalCluster.Common;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputationalCluster.CommunicationServer.Repositories
{
    public class ComponentsInMemoryRepository : IComponentsRepository
    {
        private readonly IProblemDefinitionsRepository _problemDefinitionsRepository;
        private readonly ITimeProvider _timeProvider;
        private readonly ILog _log;

        private ulong _nextValidGuid = 1;
        private Dictionary<ulong, Component> _componentDictionary;
        
        public ComponentsInMemoryRepository(IProblemDefinitionsRepository problemDefinitionsRepository,
            ITimeProvider timeProvider, ILog log)
        {
            _problemDefinitionsRepository = problemDefinitionsRepository;
            _timeProvider                 = timeProvider;
            _log                          = log;

            _componentDictionary = new Dictionary<ulong, Component>();
        }

        public ulong Register(Component component)
        {
            if (component.Id == 0)
            {
                component.Id = _nextValidGuid++;
            }
            else
            {
                _nextValidGuid = Math.Max(_nextValidGuid, component.Id) + 1;
            }

            var solvableProblems = new List<ProblemDefinition>();
            foreach (var problem in component.SolvableProblems ?? new List<ProblemDefinition>())
            {
                var def = _problemDefinitionsRepository.FindByName(problem.Name);

                if (def == null)
                {
                    def = new ProblemDefinition
                    {
                        Name = problem.Name,
                        AvailableTaskManagers = 0,
                        AvailableComputationalNodes = 0,
                    };
                    _problemDefinitionsRepository.Add(def);
                }

                switch (component.Type)
                {
                    case RegisterType.ComputationalNode:
                        def.AvailableComputationalNodes += component.MaxThreads;
                        break;
                    case RegisterType.TaskManager:
                        def.AvailableTaskManagers += component.MaxThreads;
                        break;
                }

                solvableProblems.Add(def);
            }
            component.SolvableProblems = solvableProblems;
            _componentDictionary.Add(component.Id, component);

            return component.Id;
        }

        public void Deregister(ulong componentId)
        {
            var component = GetById(componentId);

            if (component == null)
            {
                _log.WarnFormat("Attempted to deregister unexisting component. (Id={0})", componentId);
                return;
            }

            _log.InfoFormat("Deregistering component. (Id={0})", componentId);

            foreach(var problemDefiniton in component.SolvableProblems)
            {
                switch (component.Type)
                {
                    case RegisterType.ComputationalNode:
                        problemDefiniton.AvailableComputationalNodes -= component.MaxThreads;
                        break;
                    case RegisterType.TaskManager:
                        problemDefiniton.AvailableTaskManagers -= component.MaxThreads;
                        break;
                }
            }

            _componentDictionary.Remove(componentId);
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

            var timedOutComponents = _componentDictionary
                .Where(t => t.Value.LastStatusTimestamp < minimalTime).ToList();

            foreach (var component in timedOutComponents)
            {
                _log.InfoFormat("Component timed out. (Id={0})", component.Key);
                component.Value.ClearAndRepeatTasks();
                Deregister(component.Key);
            }
        }

        public IEnumerable<Component> GetAll()
        {
            return _componentDictionary.Values;
        }

        public Component GetBackupServer()
        {
            return _componentDictionary.Values.FirstOrDefault(x => x.Type == RegisterType.CommunicationServer);
        }
    }
}
