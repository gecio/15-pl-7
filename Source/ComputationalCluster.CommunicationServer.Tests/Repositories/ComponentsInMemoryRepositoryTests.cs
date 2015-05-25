using ComputationalCluster.Common;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Models;
using ComputationalCluster.CommunicationServer.Repositories;
using log4net;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Tests.Repositories
{
    [TestFixture]
    class ComponentsInMemoryRepositoryTests
    {
        private Mock<ILog> _logMock = new Mock<ILog>();
        private ComponentsInMemoryRepository repository;
        private Component component;

        [SetUp]
        public void SetUp()
        {
            component = new Component
            {
                SolvableProblems = new List<ProblemDefinition>() {new ProblemDefinition() {Name = "Test"}}
            };
            var problemDefinitionsRepository = new Mock<IProblemDefinitionsRepository>();
            repository = new ComponentsInMemoryRepository(problemDefinitionsRepository.Object, new TimeProviderUtcNow(), _logMock.Object);
        }

        [Test]
        public void RegisteringCompontentAssignsId()
        {
            repository.Register(component);

            Assert.AreNotEqual(0, component.Id);
        }

        [Test]
        public void DeregisteringCompontentRemovesComponent()
        {
            var id = repository.Register(component);

            Assert.AreEqual(component, repository.GetById(id));
            
            repository.Deregister(component.Id);

            Assert.IsNull(repository.GetById(id));
        }

        [Test]
        public void UpdateLastStatusUpdatesLastStatus()
        {
            var id = repository.Register(component);

            var beforeTime = component.LastStatusTimestamp;

            repository.UpdateLastStatusTimestamp(id);

            Assert.AreNotEqual(beforeTime, component.LastStatusTimestamp);
        }
    }
}
