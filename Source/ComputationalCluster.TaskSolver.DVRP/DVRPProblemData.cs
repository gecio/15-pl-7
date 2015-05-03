using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.TaskSolver.DVRP
{
    internal class Node
    {
        public int X { get; set; }
        public int Y { get; set; }

        // NON-SERIALIZABLE ELEMENTS! (for dfs)
        public bool Visited { get; set; }
    }

    internal class Depot : Node
    {
        public float Starts { get; set; }
        public float Ends { get; set; }

        public Depot()
        {
        }
    }

    internal class Pickup : Node
    {
        public float AvailableAfter { get; set; }
        public int UnloadTime { get; set; }
        public int Size { get; set; }

        public Pickup()
        {
        }
    }

    internal class DVRPRange
    {
        public int[] Start { get; set; }
        public int[] End { get; set; }

        public DVRPRange()
        {
        }
    }

    internal class DVRPCommonData
    {
        public IList<Depot> Depots { get; set; }
        public IList<Pickup> Pickups { get; set; }
        public int VehicleSpeed { get; set; }
        public int VehicleCapacity { get; set; }
        public int NumVehicles { get; set; }

        public DVRPCommonData()
        {
        }
    }
}
