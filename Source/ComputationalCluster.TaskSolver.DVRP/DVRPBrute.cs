using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.TaskSolver.DVRP
{
    internal class DVRPBrute
    {
        DVRPCommonData _commonData;

        public DVRPBrute(DVRPCommonData commonData)
        {
            _commonData = commonData;
        }

        public int CalculateRequiredTime(ICollection<Pickup> assignedPickups, int usedCapacity = 0, int time = 0)
        {
            int minimumServingTime = int.MaxValue;
            return minimumServingTime;
        }

        public bool MoveNext(int[] set, int numVehicles, int[] maxSet)
        {
            int move = 1, x;
            for (int i = set.Length - 1; i >= 0; --i)
            {
                x = set[i] + move;
                move = x / numVehicles;
                set[i] = x % numVehicles;
            }

            if (move == 1) // overflow
                return false;

            for (int i = 0; i < set.Length; ++i)
            {
                if (set[i] > maxSet[i])
                    return false;
                if (set[i] < maxSet[i])
                    return true;
            }

            return false;
        }

        public IList<Pickup> BuildPickupsForVehicle(int vehicle, int[] partitioning)
        {
            var pickups = new List<Pickup>();

            for (int i = 0; i < partitioning.Length; ++i)
            {
                if (partitioning[i] == vehicle)
                {
                    pickups.Add(_commonData.Pickups[i]);
                }
            }

            return pickups;
        }

        public int IterateBetweenSetPartitions(DVRPRange range)
        {
            int[] current = range.Start;
            int bestSolution = int.MaxValue;

            do
            {
                int totalRequiredTime = 0;

                for (int vehicle = 0; vehicle < _commonData.NumVehicles; ++vehicle)
                {
                    var pickups = BuildPickupsForVehicle(vehicle, current);
                    var vehicleTime = CalculateRequiredTime(pickups);
                    if (vehicleTime == int.MaxValue) // impossible to solve
                        totalRequiredTime = int.MaxValue;
                    
                    if (totalRequiredTime >= bestSolution)
                        break;
                }

                if (totalRequiredTime < bestSolution)
                {
                    bestSolution = totalRequiredTime;
                }
            }
            while (MoveNext(current, _commonData.NumVehicles, range.End));

            return bestSolution;
        }
    }
}
