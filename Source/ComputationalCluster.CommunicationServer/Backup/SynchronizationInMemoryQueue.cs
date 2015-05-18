using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Core;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using log4net;

namespace ComputationalCluster.CommunicationServer.Backup
{
    public class SynchronizationInMemoryQueue : ISynchronizationQueue
    {
        private readonly ILog _log;
        private readonly Queue<IMessage> _messageQueue;
        private readonly IComponentsRepository _componentRepository;
        private bool _isInitialized;

        public SynchronizationInMemoryQueue(ILog log,IComponentsRepository componentsRepository)
        {
            _log = log;
            _componentRepository = componentsRepository;
            _messageQueue = new Queue<IMessage>();
            _isInitialized = false;
        }

        public void Enqueue(IMessage message)
        {
            if (_isInitialized)
            {
                _log.InfoFormat("Enqueue message: {0}", message);
                _messageQueue.Enqueue(message);
            }
        }

        public IMessage Dequeue()
        {
            if (_isInitialized)
            {
                var message = _messageQueue.Dequeue();
                _log.InfoFormat("Dequeue message: {0}", message);
                return message;
            }
            return null;
        }

        public void InitializeSync()
        {
            _log.Info("InitializeSync");
            foreach (var component in _componentRepository.GetAll())
            {
                var registerMessage = new Register
                {
                    DeregisterSpecified = false,
                    Id = component.Id,
                    Type = component.Type,
                    ParallelThreads = (byte) component.MaxThreads,
                    SolvableProblems = component.SolvableProblems.Select(x => x.Name).ToArray()
                };
                Enqueue(registerMessage);
            }
            _isInitialized = true;
        }


        public ICollection<IMessage> DequeueAll()
        {
            lock (_messageQueue)
            {
                var result = _messageQueue.ToList();
                _messageQueue.Clear();
                return result;
            }
        }

    }
}
