using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;

namespace ComputationalCluster.TaskSolver.ArithmeticProgressionSum.Tests
{
    [TestFixture]
    public class ArithmeticProgressionSumTests
    {
        [TestCase(1, 1, 4)]
        [TestCase(1, 2, 5)]
        [TestCase(2, 3, 10)]
        [TestCase(3, 7, 256)]
        public void FullSequence_CalculateSeriesSingleThreaded_CorrectAnswerNoDivisions(
            int firstMember, int difference, int amount)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(firstMember);
                writer.Write(difference);
                writer.Write(amount);

                var solver = new ArithmeticProgressionSumSolver(ms.GetBuffer());
                var partialProblems = solver.DivideProblem(1);
                var partialSolution = solver.Solve(partialProblems[0], TimeSpan.Zero);
                var mergedSolution = solver.MergeSolution(new byte[][] { partialSolution });
                var finalSolution = BitConverter.ToInt32(mergedSolution, 0);

                int expectedSum = (amount * (2*firstMember + difference * (amount-1))) / 2;

                Assert.AreEqual(1, partialProblems.Length);
                Assert.AreEqual(expectedSum, finalSolution);
            }
        }
    }
}
