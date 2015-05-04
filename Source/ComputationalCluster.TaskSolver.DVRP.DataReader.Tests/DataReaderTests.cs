using System;
using System.IO;
using ComputationalCluster.TaskSolver.DVRP.DataReader;
using NUnit.Framework;

namespace ComputationalCluster.TaskSolver.DVRP.Tests
{
    [TestFixture]
    public class DataReaderTests
    {
        [TestCase("01.txt")]
        [TestCase("02.txt")]
        [TestCase("03.txt")]
        public void ParsesProperFilesWithNoErrors(string filename)
        {
            var data = File.ReadAllText("SampleData/" + filename);

            var reader = new Reader();

            reader.Parse(data);
        }

        [TestCase("bad01.txt")]
        public void BadFilesFailMiserably(string filename)
        {
            var data = File.ReadAllText("SampleData/" + filename);

            var reader = new Reader();

            Assert.Throws<ArgumentException>(() => reader.Parse(data));
        }

        [TestCase("simple.txt")]
        public void ParsesSimpleDataProperly(string filename)
        {
            var data = File.ReadAllText("SampleData/" + filename);

            var reader = new Reader();

            var parsed = reader.Parse(data);

            Assert.AreEqual(parsed.Depots.Count, 1);
            Assert.AreEqual(parsed.Depots[0].X, 0);
            Assert.AreEqual(parsed.Depots[0].Y, 0);
            Assert.AreEqual(parsed.Depots[0].Starts, 0);
            Assert.AreEqual(parsed.Depots[0].Ends, 560);

            Assert.AreEqual(parsed.NumVehicles, 8);
            Assert.AreEqual(parsed.VehicleCapacity, 100);
            Assert.AreEqual(parsed.VehicleSpeed, 1);

            Assert.AreEqual(parsed.Pickups.Count, 8);
            Assert.AreEqual(parsed.Pickups[0].X, -39);
            Assert.AreEqual(parsed.Pickups[0].Y, 97);
            Assert.AreEqual(parsed.Pickups[0].AvailableAfter, 276);
            Assert.AreEqual(parsed.Pickups[0].Size, -29);
            Assert.AreEqual(parsed.Pickups[0].UnloadTime, 20);
        }
    }
}
