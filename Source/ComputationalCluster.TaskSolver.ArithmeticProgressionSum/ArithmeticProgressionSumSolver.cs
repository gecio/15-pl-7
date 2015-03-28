using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.TaskSolver.ArithmeticProgressionSum
{
    public class ArithmeticProgressionSumSolver : UCCTaskSolver.TaskSolver
    {
        public ArithmeticProgressionSumSolver() : base(null)
        {
        }

        public ArithmeticProgressionSumSolver(byte[] problemData) : base(problemData)
        {
        }

        public byte[] ProblemData
        {
            get { return _problemData; }
            set { _problemData = value; }
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            var partialProblemsData = new List<byte[]>();

            int firstMember;
            int membersAmount;
            int commonDifference;

            using (var ms = new MemoryStream(_problemData))
            using (var reader = new BinaryReader(ms))
            {
                firstMember = reader.ReadInt32();
                commonDifference = reader.ReadInt32();
                membersAmount = reader.ReadInt32();
            }

            int membersPerThread = (int)Math.Ceiling(membersAmount / (double)threadCount);

            int membersAssigned = 0;
            int currentFirstMember = firstMember;

            for(int thread = 0; thread < threadCount; ++thread)
            {
                int membersToAssign = Math.Min(membersAmount - membersAssigned, membersPerThread);

                using (var ms = new MemoryStream())
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(currentFirstMember);
                    writer.Write(commonDifference);
                    writer.Write(membersToAssign);

                    partialProblemsData.Add(ms.GetBuffer());
                }

                currentFirstMember += membersToAssign * commonDifference;
                membersAssigned += membersToAssign;
            }

            return partialProblemsData.ToArray();
        }

        public override byte[] MergeSolution(byte[][] solutions)
        {
            int mergedSolution = 0;

            foreach (var solution in solutions)
            {
                using (var ms = new MemoryStream(solution))
                using (var reader = new BinaryReader(ms))
                {
                    int partialSum = reader.ReadInt32();
                    mergedSolution += partialSum;
                }
            }

            return BitConverter.GetBytes(mergedSolution);
        }

        public override string Name
        {
            get { return "Arithmetic progression sum"; }
        }

        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
        {
            using (var ms = new MemoryStream(partialData))
            using (var reader = new BinaryReader(ms))
            {
                int firstMember = reader.ReadInt32();
                int commonDifference = reader.ReadInt32();
                int membersAmount = reader.ReadInt32();
                int partialSum = 0;

                for (int i = 0; i < membersAmount; ++i)
                {
                    partialSum += firstMember + i * commonDifference;
                }

                return BitConverter.GetBytes(partialSum);
            }
        }
    }
}
