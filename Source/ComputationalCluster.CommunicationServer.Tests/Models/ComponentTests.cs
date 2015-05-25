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
    class ComponentTests
    {
        [Test]
        public void AddingTaskAssignsItToComponent()
        {
            var task = new Problem();
            var component = new Component();

            component.AddTask(task);

            Assert.AreSame(component, task.AssignedTo);
        }

        [Test]
        public void RemovingTaskUnassingsItFromComponent()
        {
            var task = new Problem();
            var component = new Component();
            task.AssignedTo = component;

            component.RemoveTask(task);

            Assert.AreEqual(null, task.AssignedTo);
        }

        [Test]
        public void ClearAndRepeatResetsTasksState()
        {
            var task = new Problem();
            var component = new Component();
            component.AddTask(task);
            task.IsAwaiting = false;
            component.ClearAndRepeatTasks();

            Assert.AreEqual(true, task.IsAwaiting);
            Assert.AreEqual(null, task.AssignedTo);
        }
    }
}
