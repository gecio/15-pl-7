using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.TaskSolver.DVRP.DataReader
{
    public static class Reader
    {
        private State _state = State.EXPECTING_KEY;
        private List<Key> keys = new List<Key> {
            new Key("VRPTEST", true, DataType.SPECIAL, DataFormat.SPECIAL),
            new Key("NAME", false, DataType.STRING, DataFormat.SINGLE_LINE),
            new Key("NUM_VISITS", true, DataType.INT, DataFormat.SINGLE_LINE),
            new Key("NUM_DEPOTS", false, DataType.INT, DataFormat.SINGLE_LINE, 1),
            new Key("NUM_VEHICLES", true, DataType.INT, DataFormat.SINGLE_LINE),
            new Key("NUM_CAPACITIES", false, DataType.INT, DataFormat.SINGLE_LINE, 1),
            new Key("NUM_LOCATIONS", false, DataType.INT, DataFormat.SINGLE_LINE), //compute default
            new Key("CAPACITIES", false, DataType.REAL, DataFormat.SINGLE_LINE, 0.0),
            new Key("SPEED", false, DataType.REAL, DataFormat.SINGLE_LINE, 1.0),
            new Key("MAX_TIME", false, DataType.REAL, DataFormat.SINGLE_LINE, double.PositiveInfinity),
            new Key("EDGE_WEIGHT_TYPE", false, DataType.ENUM, DataFormat.SINGLE_LINE), //Enum
            new Key("EDGE_WEIGHT_FORMAT", false, DataType.ENUM, DataFormat.SINGLE_LINE), //Enum
            new Key("OBJECTIVE", false, DataType.ENUM, DataFormat.SINGLE_LINE), //Enum
            new Key("DATA_SECTION", true, DataType.SPECIAL, DataFormat.SPECIAL),
            new Key("DEPOTS", true, DataType.INT, DataFormat.SPECIAL),
            new Key("DEMAND_SECTION", true, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("LOCATION_COORD_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("VISIT_LOCATION_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("DEPOT_LOCATION_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("VEHICLE_CAPACITY_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("VEHICLE_SPEED_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("VEHICLE_COST_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("EDGE_WEIGHT_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("TIME_WINDOW_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("DEPOT_TIME_WINDOW_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("VEH_TIME_WINDOW_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("DURATION_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("TIME_AVAIL_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("DURATION_BY_VEH_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("VISIT_COMPAT_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("DEPOT_COMPAT_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("VEH_COMPAT_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("VEH_DEPOT_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("ORDER_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("OPTIONAL_VISIT_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("VISIT_AVAIL_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE),
            new Key("EOF", false, DataType.SPECIAL, DataFormat.SPECIAL)
        };

        public static void Parse(string data)
        {
            foreach (var line in data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {

            }

            throw new ArgumentException();
        }
    }

    private enum State
    {
        EXPECTING_KEY,
        EXPECTING_VALUE,
        EXPECTING_ANY
    }
}
