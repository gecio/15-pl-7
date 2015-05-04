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

        private float EuclideanDistance(Node a, Node b)
        {
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        public float CalculateRequiredTimeDFS(Node currentLocation, ICollection<Pickup> assignedPickups, int pickupsDone = 0,
            int usedCapacity = 0, float time = 0)
        {
            if (currentLocation is Pickup)
            {
                var pickup = (Pickup)currentLocation;

                if (currentLocation.Visited 
                    || pickup.AvailableAfter > time 
                    || usedCapacity + pickup.Size > _commonData.VehicleCapacity)
                    return float.MaxValue;
                currentLocation.Visited = true;
;
                usedCapacity += pickup.Size;
                time += pickup.UnloadTime;
                ++pickupsDone;
            }
            else if (currentLocation is Depot)
            {
                var depo = (Depot)currentLocation;
                if (depo.Starts > time || time > depo.Ends)
                    return float.MaxValue;

                if (pickupsDone == assignedPickups.Count)
                {
                    currentLocation.Visited = false;
                    return time;
                }

                // unloading time?
                usedCapacity = 0; // unload
            }

            float minimumServingTime = float.MaxValue;

            if (pickupsDone < _commonData.Pickups.Count)
            {
                foreach (var pickup in assignedPickups)
                {
                    if (pickup == currentLocation)
                        continue;

                    float travelDistance = EuclideanDistance(currentLocation, pickup);
                    float travelTime = travelDistance / _commonData.VehicleSpeed;
                    var foundTime = CalculateRequiredTimeDFS(pickup, assignedPickups, pickupsDone, usedCapacity, time + travelTime);

                    if (foundTime < minimumServingTime)
                    {
                        minimumServingTime = foundTime;
                    }
                }
            }

            foreach (var depot in _commonData.Depots)
            {
                if (depot == currentLocation)
                    continue;

                float travelDistance = EuclideanDistance(currentLocation, depot);
                float travelTime = travelDistance / _commonData.VehicleSpeed;
                var foundTime = CalculateRequiredTimeDFS(depot, assignedPickups, pickupsDone, usedCapacity, time + travelTime);

                if (foundTime < minimumServingTime)
                {
                    minimumServingTime = foundTime;
                }
            }

            currentLocation.Visited = false;
            return minimumServingTime;
        }

        public float CalculateRequiredTime(ICollection<Pickup> assignedPickups)
        {
            // todo support for multiple starting locations
            return CalculateRequiredTimeDFS(_commonData.Depots[0], assignedPickups, 0);
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

        public float IterateBetweenSetPartitions(DVRPRange range)
        {
            int[] current = range.Start;
            float bestSolution = float.MaxValue;

            do
            {
                float requiredTime = float.MinValue;

                for (int vehicle = 0; vehicle < _commonData.NumVehicles; ++vehicle)
                {
                    var pickups = BuildPickupsForVehicle(vehicle, current);
                    var vehicleTime = CalculateRequiredTime(pickups);
                    requiredTime = Math.Max(requiredTime, vehicleTime);

                    if (requiredTime == float.MaxValue)
                        break;
                }

                if (requiredTime < bestSolution)
                {
                    bestSolution = requiredTime;
                }
            }
            while (MoveNext(current, _commonData.NumVehicles, range.End));

            return bestSolution;
        }
    }
}
