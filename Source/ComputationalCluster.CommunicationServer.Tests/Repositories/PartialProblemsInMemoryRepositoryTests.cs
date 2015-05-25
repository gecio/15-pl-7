using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.CommunicationServer.Models;
using ComputationalCluster.CommunicationServer.Repositories;
using log4net;
using Moq;
using NUnit.Framework;

namespace ComputationalCluster.CommunicationServer.Tests.Repositories
{
    [TestFixture]
    class PartialProblemsInMemoryRepositoryTests
    {
        private PartialProblemsInMemoryRepository repository;
        private OrderedPartialProblem problem;
        private ProblemDefinition problemDefinition;

        [SetUp]
        public void SetUp()
        {
            problemDefinition = new ProblemDefinition() {AvailableComputationalNodes = 0, AvailableTaskManagers = 1}; 
            problem = new OrderedPartialProblem()
            {
                Id = 1,
                ProblemDefinition = problemDefinition,
                AssignedTo = new Component()
            };
            var problemDefinitionsRepository = new Mock<IProblemDefinitionsRepository>();
            repository = new PartialProblemsInMemoryRepository(problemDefinitionsRepository.Object);
        }

        [Test]
        public void AddingProblemSavesItToRepository()
        {
            var tid = repository.Add(problem);

            Assert.AreEqual(problem, repository.Find(problem.Id, tid));
        }

        [Test]
        public void RepositoryReturnsFinishedProblem()
        {
            repository.Add(problem);

            Assert.IsNull(repository.GetFinishedProblem(new[] { problemDefinition }));

            problem.Done = true;
            problem.AssignedTo = null;

            Assert.IsNotNull(repository.GetFinishedProblem(new[] { problemDefinition }));
        }


        [Test]
        public void RepositoryRemovesFinishedProblems()
        {
            repository.Add(problem);

            Assert.IsNotNull(problem.AssignedTo);
            repository.RemoveFinishedProblems(1);

            Assert.IsNull(problem.AssignedTo);
        }


        [Test]
        public void RepositoryReturnsQueuableTasks()
        {
            repository.Add(problem);

            Assert.IsEmpty(repository.GetQueuableTasks());

            problem.ProblemDefinition.AvailableComputationalNodes = 1;

            Assert.IsNotEmpty(repository.GetQueuableTasks());
        }



    }
}
