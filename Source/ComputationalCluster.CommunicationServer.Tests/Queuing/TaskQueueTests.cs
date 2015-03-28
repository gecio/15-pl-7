using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ComputationalCluster.CommunicationServer.Queueing;
using Moq;
using ComputationalCluster.CommunicationServer.Models;
using log4net;

namespace ComputationalCluster.CommunicationServer.Tests.Queueing
{
    [TestFixture]
    class TaskQueueTests
    {
        [Test]
        public void GetNextTask_QueueOneAwaitingAndAvailableThenDequeueOne_ShouldDequeue()
        {
            var problemDefinition = new ProblemDefinition
            {
                Id = 1,
                Name = "BigProblem",
                AvailableTaskManagers = 1,
                AvailableComputationalNodes = 0,
            };

            var queuableTaskMock = new Mock<IQueueableTask>();
            queuableTaskMock.Setup(t => t.IsAwaiting).Returns(true);
            queuableTaskMock.Setup(t => t.RequestDate).Returns(new DateTime(2014, 8, 21, 16, 30, 12));
            queuableTaskMock.Setup(t => t.ProblemDefinition).Returns(problemDefinition);

            var repository = new Mock<IQueuableTasksRepository<IQueueableTask>>();
            repository.Setup(t => t.GetQueuableTasks())
                .Returns(new List<IQueueableTask>() { queuableTaskMock.Object });

            var queue = new TaskQueue<IQueueableTask>(repository.Object, t => t.AvailableTaskManagers, 
                new Mock<ILog>().Object);

            var task = queue.GetNextTask();

            repository.Verify(t => t.GetQueuableTasks(), Times.Once);
            repository.Verify(t => t.DequeueTask(queuableTaskMock.Object), Times.Once);
            Assert.AreEqual(task, queuableTaskMock.Object);
        }
    }
}
