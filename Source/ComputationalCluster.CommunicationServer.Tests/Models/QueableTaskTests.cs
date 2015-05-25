using ComputationalCluster.CommunicationServer.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Tests.Models
{            
    [TestFixture]
    class QueableTaskTests
    {
        [Test]
        public void AssignedToGetsSetAndIsReturned()
        {
            var task = new Problem();
            var component = new Component();
            task.AssignedTo = component;

            Assert.AreSame(component, task.AssignedTo);
        }

        [Test]
        public void SettingAssignedToAddsTaskToComponent()
        {
            var task = new Problem();
            var component = new Mock<Component>();
            task.AssignedTo = component.Object;

            component.Verify(mock => mock.AddTask(It.Is<QueueableTask>(it => it == task)), Times.Exactly(1));
        }

        [Test]
        public void OverridingAssignedToRemovesTaskFromComponent()
        {
            var task = new Problem();
            var old_component = new Mock<Component>();
            var new_component = new Mock<Component>();
            task.AssignedTo = old_component.Object;
            task.AssignedTo = new_component.Object;

            old_component.Verify(mock => mock.AddTask(It.Is<QueueableTask>(it => it == task)), Times.Exactly(1));
            old_component.Verify(mock => mock.RemoveTask(It.Is<QueueableTask>(it => it == task)), Times.Exactly(1));
            new_component.Verify(mock => mock.AddTask(It.Is<QueueableTask>(it => it == task)), Times.Exactly(1));
        }
    }
}
