using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.TaskSolver.DVRP.DataReader;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ComputationalCluster.TaskSolver.DVRP
{
    public class DVRPTaskSolver : UCCTaskSolver.TaskSolver
    {
        private DVRPBrute _dvrpBrute;
        private DVRPCommonData _commonData;

        public DVRPTaskSolver(byte[] problemData)
            : base(problemData)
        {
            var data = Encoding.UTF8.GetString(problemData);
            var reader = new Reader();
            try
            {
                _commonData = reader.Parse(data);
                _dvrpBrute = new DVRPBrute(_commonData);
            }
            catch (ArgumentException)
            {
                State = TaskSolverState.Error;
            }
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            var result = new List<int[]>();

            ulong counts = (ulong)Math.Pow(_commonData.NumVehicles, _commonData.Pickups.Count);
            ulong forOne = Math.Max(counts / (ulong)threadCount, 1);
            for (ulong i = 0; i < counts; i += forOne)
            {
                ulong from = i;

                int[] input = new int[_commonData.Pickups.Count];
                for (int j = _commonData.Pickups.Count - 1; j >= 0; --j)
                {
                    input[j] = (int)(from % (ulong)_commonData.NumVehicles);
                    from = from / (ulong)_commonData.NumVehicles;
                }
                result.Add(input);
            }

            var ending = new int[_commonData.Pickups.Count];
            for (int i = 0; i < _commonData.Pickups.Count; ++i)
                ending[i] = _commonData.NumVehicles - 1;
            result.Add(ending);

            List<DVRPRange> parameters = new List<DVRPRange>();

            for (int i = 0; i < threadCount; i++)
            {
                var dvrpRange = new DVRPRange
                {
                    Start = result[i],
                    End = result[i + 1]
                };
                parameters.Add(dvrpRange);
            }

            List<byte[]> parsedParameters = new List<byte[]>();
            foreach (var param in parameters)
            {
                parsedParameters.Add(BinarySerializer(param));
            }
            return parsedParameters.ToArray();
        }

        public override byte[] MergeSolution(byte[][] solutions)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            List<float> partialSolutions = new List<float>();
            foreach (var partialSolution in solutions)
            {
                partialSolutions.Add((float)BinaryDeserializer(partialSolution));
            }
            return Encoding.UTF8.GetBytes(String.Format("result: {0}", partialSolutions.Min()));
        }

        public override string Name
        {
            get { return "DVRPTaskSolver"; }
        }

        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
        {
            DVRPRange range = BinaryDeserializer(partialData) as DVRPRange;
            var result = _dvrpBrute.IterateBetweenSetPartitions(range);
            byte[] resultBytes = BinarySerializer(result);
            return resultBytes;
        }

        private byte[] BinarySerializer(object obj)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            byte[] resultBytes;
            using (var ms = new MemoryStream())
            {
                binaryFormatter.Serialize(ms, obj);
                resultBytes = ms.GetBuffer();
            }
            return resultBytes;
        }

        private object BinaryDeserializer(byte[] data)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (var ms = new MemoryStream(data))
            {
                return binaryFormatter.Deserialize(ms);
            }
        }

    }
}
