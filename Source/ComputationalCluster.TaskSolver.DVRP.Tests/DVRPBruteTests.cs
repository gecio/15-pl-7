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
                    new Depot { Id = 0, X = 0, Y = 0, Starts = 0, Ends = 10000 },
                },
                Pickups = new List<Pickup>()
                {
                    new Pickup { Id = 1, X = 0, Y = 2, Size = 1 },
                    new Pickup { Id = 2, X = 0, Y = 3, Size = 1 },
                    new Pickup { Id = 3, X = 0, Y = 4, Size = 1 },
                    new Pickup { Id = 4, X = 0, Y = -2, Size = 1 },
                    new Pickup { Id = 5, X = 0, Y = -3, Size = 1 },
                    new Pickup { Id = 6, X = 0, Y = -4, Size = 1 },
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

            Assert.AreEqual(16.0f, result, 0.005f);
        }

        [Test]
        public void Solve_SimpleStaticCaseTwoSmallVehicles()
        {
            var commonData = new DVRPCommonData
            {
                Depots = new List<Depot>()
                {
                    new Depot { Id = 0, X = 0, Y = 0, Starts = 0, Ends = 20 },
                },
                Pickups = new List<Pickup>()
                {
                    new Pickup { Id = 1, X = 0, Y = 2, Size = 1 },
                    new Pickup { Id = 2, X = 0, Y = 3, Size = 1 },
                    new Pickup { Id = 3, X = 0, Y = 4, Size = 1 },
                    new Pickup { Id = 4, X = 0, Y = -2, Size = 1 },
                    new Pickup { Id = 5, X = 0, Y = -3, Size = 1 },
                    new Pickup { Id = 6, X = 0, Y = -4, Size = 1 },
                },
                NumVehicles = 2,
                VehicleCapacity = 1,
                VehicleSpeed = 1,
            };

            var dvrp = new DVRPBrute(commonData);
            var range = new DVRPRange
            {
                Start = new int[] { 0, 0, 0, 0, 0, 0 },
                End = new int[] { 1, 1, 1, 1, 1, 1 },
            };

            var result = dvrp.IterateBetweenSetPartitions(range);

            Assert.AreEqual(36.0f, result, 0.005f);
        }

        [Test]
        public void Solve_OkuleleMegatest()
        {
            var commonData = new DVRPCommonData
            {
                Depots = new List<Depot>()
                {
                    new Depot { X = 0, Y = 0, Starts = 0, Ends = 560 },
                },
                Pickups = new List<Pickup>()
                {
                    new Pickup { Id = 1, X = -39, Y =  97, Size = 29, AvailableAfter = 276, UnloadTime = 20 },
                    new Pickup { Id = 2, X =  34, Y = -45, Size = 21, AvailableAfter = 451, UnloadTime = 20 },
                    new Pickup { Id = 3, X =  77, Y = -20, Size = 28, AvailableAfter = 171, UnloadTime = 20 },
                    new Pickup { Id = 4, X = -34, Y = -99, Size = 20, AvailableAfter = 365, UnloadTime = 20 },
                    new Pickup { Id = 5, X =  75, Y = -43, Size = 8, AvailableAfter = 479, UnloadTime = 20 },
                    new Pickup { Id = 6, X =  87, Y = -66, Size = 31, AvailableAfter = 546, UnloadTime = 20 },
                    new Pickup { Id = 7, X = -55, Y =  86, Size = 13, AvailableAfter = 376, UnloadTime = 20 },
                    new Pickup { Id = 8, X = -93, Y =  -3, Size = 29, AvailableAfter = 289, UnloadTime = 20 },
                },
                NumVehicles = 8,
                VehicleCapacity = 100,
                VehicleSpeed = 1,
            };

            var dvrp = new DVRPBrute(commonData);
            var range = new DVRPRange
            {
                //Start = new int[] { 0, 1, 1, 0, 1, 0, 1, 1 },
                //End = new int[]   { 0, 1, 1, 0, 1, 1, 0, 0 },
                Start = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                End = new int[] { 7, 7, 7, 7, 7, 7, 7, 7 },
            };
            // 0 0 0 1 0 0 1 1
            var result = dvrp.IterateBetweenSetPartitions(range);

            var finalResult = ((int)((result+0.005f) * 100)) / 100.0f;
            Assert.AreEqual(680.09f, finalResult, 0.005f);

            //Assert.Fail();
        }
    }
}
