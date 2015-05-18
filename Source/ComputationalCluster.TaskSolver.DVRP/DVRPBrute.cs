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

        public float CalculateRequiredTimeDFS(Node currentLocation, ICollection<Pickup> assignedPickups, int pickupsDone,
            float usedCapacity, float time, float travel, float bestTravel)
        {
            if (travel > bestTravel) // worse solution already
                return float.MaxValue;

            if (currentLocation is Pickup)
            {
                var pickup = (Pickup)currentLocation;

                if (currentLocation.Visited 
                    //|| pickup.AvailableAfter > time 
                    || usedCapacity + pickup.Size > _commonData.VehicleCapacity)
                    return float.MaxValue;
                currentLocation.Visited = true;

                usedCapacity += pickup.Size;
                time = Math.Max(time, pickup.AvailableAfter) + pickup.UnloadTime;
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
                    return travel;
                }

                usedCapacity = 0; // unload
            }

            float minimumTotalDistance = bestTravel;
            Node goThrough = null;
            bool anySolutionFound = false;

            if (pickupsDone < _commonData.Pickups.Count)
            {
                foreach (var pickup in assignedPickups)
                {
                    if (pickup == currentLocation)
                        continue;

                    float travelDistance = EuclideanDistance(currentLocation, pickup);
                    float travelTime = travelDistance / _commonData.VehicleSpeed;
                    var foundDistance = CalculateRequiredTimeDFS(pickup, assignedPickups, pickupsDone, usedCapacity,
                        time + travelTime, travel + travelDistance, minimumTotalDistance);

                    if (foundDistance != float.MaxValue)
                        anySolutionFound = true;

                    if (foundDistance < minimumTotalDistance)
                    {
                        goThrough = pickup;
                        minimumTotalDistance = foundDistance;
                    }
                }
            }

            if (!anySolutionFound)
            {
                foreach (var depot in _commonData.Depots)
                {
                    if (depot == currentLocation)
                        continue;

                    float travelDistance = EuclideanDistance(currentLocation, depot);
                    float travelTime = travelDistance / _commonData.VehicleSpeed;
                    var foundDistance = CalculateRequiredTimeDFS(depot, assignedPickups, pickupsDone, usedCapacity,
                        time + travelTime, travel + travelDistance, minimumTotalDistance);

                    if (foundDistance < minimumTotalDistance)
                    {
                        goThrough = depot;
                        minimumTotalDistance = foundDistance;
                    }
                }
            }

            currentLocation.Visited = false;

            if (goThrough != null)
            {
                currentLocation.NextOnPath.Push(goThrough);
            }

            return minimumTotalDistance;
        }

        public float CalculateRequiredDistance(ICollection<Pickup> assignedPickups, float bestTravel = float.MaxValue)
        {
            // todo support for multiple starting locations
            return CalculateRequiredTimeDFS(_commonData.Depots[0], assignedPickups, 0, 0, 0, 0, bestTravel);
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

            return true;
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

        // won't work, cycles possible
        private List<int> RecreateWay(Node node)
        {
            var path = new List<int>();

            do
            {
                path.Add(node.Id);
                node.Visited = true;
                node = node.NextOnPath.Count > 0 ? node.NextOnPath.Pop() : null;
            }
            while (node != null && (node is Depot || !node.Visited));

            foreach (var pickup in _commonData.Pickups)
            {
                pickup.NextOnPath.Clear();
            }
            foreach (var depo in _commonData.Depots)
            {
                depo.NextOnPath.Clear();
            }

            return path;
        }


        private void ApplyCutoff()
        {
            _commonData.Pickups
                .Where(p => p.AvailableAfter >= _commonData.Depots.Max(t => t.Ends) / 2)
                .ToList().ForEach(p => p.AvailableAfter = 0);
        }

        public float IterateBetweenSetPartitions(DVRPRange range)
        {
            int[][] routes = null;
            return IterateBetweenSetPartitions(range, out routes);
        }

        public float IterateBetweenSetPartitions(DVRPRange range, out int[][] routes)
        {
            int[] current = range.Start;
            float bestSolution = float.MaxValue;

            var bestPaths = new List<List<int>>();
            var paths = new List<List<int>>();

            ApplyCutoff();

            do
            {
                float requiredDistance = 0.0f;
                paths.Clear();

                foreach (var pickup in _commonData.Pickups)
                {
                    pickup.NextOnPath.Clear();
                    pickup.Visited = false;
                }
                foreach (var depo in _commonData.Depots)
                {
                    depo.NextOnPath.Clear();
                    depo.Visited = false;
                }

                for (int vehicle = 0; vehicle < _commonData.NumVehicles; ++vehicle)
                {
                    var pickups = BuildPickupsForVehicle(vehicle, current);
                    var vehicleDistance = CalculateRequiredDistance(pickups, bestSolution);

                    if (vehicleDistance == float.MaxValue)
                    {
                        requiredDistance = float.MaxValue;
                        break; // because it's impossible to do it
                    }

                    requiredDistance += vehicleDistance;

                    if (requiredDistance >= bestSolution)
                    {
                        requiredDistance = float.MaxValue;
                        break; // we have better solution already found
                    }

                    paths.Add(RecreateWay(_commonData.Depots[0]));
                }

                if (requiredDistance < bestSolution)
                {
                    bestSolution = requiredDistance;
                    bestPaths = paths.ToList();
                }
            }
            while (MoveNext(current, _commonData.NumVehicles, range.End));

            routes = bestPaths.Select(t => t.ToArray()).ToArray();
            return bestSolution;
        }
    }
}
