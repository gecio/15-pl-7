using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.TaskSolver.DVRP.Tests
{
    [TestFixture]
    public class DVRPBruteTests
    {
        [Test]
        public void MoveNext_MoveOneInRangeNoShift_Ok()
        {
            var dvrp = new DVRPBrute(null);

            var current = new int[] { 0, 0, 0 };
            var end = new int[] { 2, 0, 0 };
            var exists = dvrp.MoveNext(current, 4, end);

            Assert.AreEqual(new int[] { 0, 0, 1 }, current);
            Assert.IsTrue(exists);
        }

        [Test]
        public void MoveNext_MoveOneInRangeWithShift_Ok()
        {
            var dvrp = new DVRPBrute(null);

            var current = new int[] { 0, 3, 3 };
            var end = new int[] { 3, 3, 3 };
            var exists = dvrp.MoveNext(current, 4, end);

            Assert.AreEqual(new int[] { 1, 0, 0 }, current);
            Assert.IsTrue(exists);
        }

        [Test]
        public void MoveNext_MoveOneToOutOfRangeWithShift_Finish()
        {
            var dvrp = new DVRPBrute(null);

            var current = new int[] { 0, 3, 3 };
            var end = new int[] { 0, 3, 3 };
            var exists = dvrp.MoveNext(current, 4, end);

            Assert.AreEqual(new int[] { 1, 0, 0 }, current);
            Assert.IsFalse(exists);
        }

        [Test]
        public void MoveNext_MoveOneToOutOfRangeWithShiftAndOverflow_Finish()
        {
            var dvrp = new DVRPBrute(null);

            var current = new int[] { 3, 3, 3 };
            var end = new int[] { 3, 3, 3 };
            var exists = dvrp.MoveNext(current, 4, end);

            Assert.AreEqual(new int[] { 0, 0, 0 }, current);
            Assert.IsFalse(exists);
        }
    }
}
