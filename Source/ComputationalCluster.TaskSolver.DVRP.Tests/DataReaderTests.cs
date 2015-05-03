using System;
using System.IO;
using System.Text;
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
    }
}
