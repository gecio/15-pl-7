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

        [Test]
        public void Solve_SimpleStaticCaseOneVehicle()
        {
            var commonData = new DVRPCommonData
            {
                Depots = new List<Depot>()
                {
                    new Depot { X = 0, Y = 0, Starts = 0, Ends = 10000 },
                },
                Pickups = new List<Pickup>()
                {
                    new Pickup { X = -1, Y = 3, Size = 1 },
                    new Pickup { X = 0, Y = 3, Size = 1 },
                    new Pickup { X = 1, Y = 3, Size = 1 },
                },
                NumVehicles = 1,
                VehicleCapacity = 10,
                VehicleSpeed = 1,
            };

            var dvrp = new DVRPBrute(commonData);
            var range = new DVRPRange
            {
                Start = new int[] { 0, 0, 0 },
                End = new int[] { 0, 0, 0 },
            };

            var result = dvrp.IterateBetweenSetPartitions(range);

            Assert.AreEqual(8.324f, result, 0.005f);
        }

        [Test]
        public void Solve_SimpleStaticCaseTwoVehicles()
        {
            var commonData = new DVRPCommonData
            {
                Depots = new List<Depot>()
                {
                    new Depot { X = 0, Y = 0, Starts = 0, Ends = 10000 },
                },
                Pickups = new List<Pickup>()
                {
                    new Pickup { X = 0, Y = 2, Size = 1 },
                    new Pickup { X = 0, Y = 3, Size = 1 },
                    new Pickup { X = 0, Y = 4, Size = 1 },
                    new Pickup { X = 0, Y = -2, Size = 1 },
                    new Pickup { X = 0, Y = -3, Size = 1 },
                    new Pickup { X = 0, Y = -4, Size = 1 },
                },
                NumVehicles = 2,
                VehicleCapacity = 10,
                VehicleSpeed = 1,
            };

            var dvrp = new DVRPBrute(commonData);
            var range = new DVRPRange
            {
                Start = new int[] { 0, 0, 0, 0, 0, 0 },
                End = new int[] { 1, 1, 1, 1, 1, 1 },
            };

            var result = dvrp.IterateBetweenSetPartitions(range);

            Assert.AreEqual(8.0f, result, 0.005f);
        }
    }
}
