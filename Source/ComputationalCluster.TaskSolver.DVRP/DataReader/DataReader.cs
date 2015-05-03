using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComputationalCluster.TaskSolver.DVRP.DataReader
{
    public class Reader
    {
        private State _state = State.EXPECTING_KEY;

        public Dictionary<string, Key> Keys = new Dictionary<string, Key>
        {
            {"VRPTEST", new Key("VRPTEST", true, DataType.NONE, DataFormat.PASSABLE)},
            {"NAME", new Key("NAME", false, DataType.STRING, DataFormat.SINGLE_LINE)},
            {"NUM_VISITS", new Key("NUM_VISITS", true, DataType.INT, DataFormat.SINGLE_LINE)},
            {"NUM_DEPOTS", new Key("NUM_DEPOTS", false, DataType.INT, DataFormat.SINGLE_LINE, 1)},
            {"NUM_VEHICLES", new Key("NUM_VEHICLES", true, DataType.INT, DataFormat.SINGLE_LINE)},
            {"NUM_CAPACITIES", new Key("NUM_CAPACITIES", false, DataType.INT, DataFormat.SINGLE_LINE, 1)},
            {"NUM_LOCATIONS", new Key("NUM_LOCATIONS", false, DataType.INT, DataFormat.SINGLE_LINE)}, //compute default
            {"CAPACITIES", new Key("CAPACITIES", false, DataType.REAL, DataFormat.SINGLE_LINE, 0.0)},
            {"SPEED", new Key("SPEED", false, DataType.REAL, DataFormat.SINGLE_LINE, 1.0)},
            {"MAX_TIME", new Key("MAX_TIME", false, DataType.REAL, DataFormat.SINGLE_LINE, double.PositiveInfinity)},
            {"EDGE_WEIGHT_TYPE", new Key("EDGE_WEIGHT_TYPE", false, DataType.ENUM, DataFormat.SINGLE_LINE)}, //Enum
            {"EDGE_WEIGHT_FORMAT", new Key("EDGE_WEIGHT_FORMAT", false, DataType.ENUM, DataFormat.SINGLE_LINE)}, //Enum
            {"OBJECTIVE", new Key("OBJECTIVE", false, DataType.ENUM, DataFormat.SINGLE_LINE)}, //Enum
            {"DATA_SECTION", new Key("DATA_SECTION", true, DataType.NONE, DataFormat.PASSABLE)},
            {"DEPOTS", new Key("DEPOTS", true, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"DEMAND_SECTION", new Key("DEMAND_SECTION", true, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"LOCATION_COORD_SECTION", new Key("LOCATION_COORD_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"VISIT_LOCATION_SECTION", new Key("VISIT_LOCATION_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"DEPOT_LOCATION_SECTION", new Key("DEPOT_LOCATION_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"VEHICLE_CAPACITY_SECTION", new Key("VEHICLE_CAPACITY_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"VEHICLE_SPEED_SECTION", new Key("VEHICLE_SPEED_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"VEHICLE_WEIGHT_SECTION", new Key("VEHICLE_COST_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"EDGE_WEIGHT_SECTION", new Key("EDGE_WEIGHT_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"TIME_WINDOW_SECTION", new Key("TIME_WINDOW_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"DEPOT_TIME_WINDOW_SECTION", new Key("DEPOT_TIME_WINDOW_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"VEH_TIME_WINDOW_SECTION", new Key("VEH_TIME_WINDOW_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"DURATION_SECTION", new Key("DURATION_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"TIME_AVAIL_SECTION", new Key("TIME_AVAIL_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"DURATION_BY_VEH_SECTION", new Key("DURATION_BY_VEH_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"VISIT_COMPAT_SECTION", new Key("VISIT_COMPAT_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"DEPOT_COMPAT_SECTION", new Key("DEPOT_COMPAT_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"VEH_COMPAT_SECTION", new Key("VEH_COMPAT_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"VEH_DEPOT_SECTION", new Key("VEH_DEPOT_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"ORDER_SECTION", new Key("ORDER_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"OPTIONAL_VISIT_SECTION", new Key("OPTIONAL_VISIT_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"VISIT_AVAIL_SECTION", new Key("VISIT_AVAIL_SECTION", false, DataType.LIST_NUM, DataFormat.MULTI_LINE)},
            {"TYPE", new Key("TYPE", false, DataType.ENUM, DataFormat.SINGLE_LINE)},
            {"COMMENT", new Key("COMMENT", false, DataType.STRING, DataFormat.SINGLE_LINE)},
            {"EOF", new Key("EOF", false, DataType.NONE, DataFormat.PASSABLE)}
        };

        public void Parse(string data)
        {
            Key multilineHeaderKey = null;
            foreach (var line in data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(s => s + "\n"))
            {
                var regex = new Regex(@"(?<!.)[A-Z_]+(?=[: \n])");
                var key = regex.Match(line);
                if (key.Success)
                {
                    if (_state == State.EXPECTING_VALUE)
                    {
                        throw new ArgumentException("Was expecting value but got key instead");
                    }
                    multilineHeaderKey = null;
                    if (key.Groups.Count > 1)
                    {
                        throw new ArgumentException("Config data seems invalid");
                    }

                    Key relatedKey;
                    if (Keys.TryGetValue(key.ToString(), out relatedKey))
                    {
                        relatedKey.Found = true;

                        //Header for multiline or single line
                        switch (relatedKey.DataFormat)
                        {
                            case DataFormat.SINGLE_LINE:
                                regex = new Regex(@"(?<=(:{1}\W)).+");
                                var value = regex.Match(line);
                                if (value.Success)
                                {
                                    relatedKey.Value = value.ToString();
                                }
                                else
                                {
                                    throw new ArgumentException("Can't parse single line key " + key);
                                }
                                _state = State.EXPECTING_KEY;
                                break;
                            case DataFormat.MULTI_LINE:
                                multilineHeaderKey = relatedKey;
                                multilineHeaderKey.Value = new List<string>();
                                _state = State.EXPECTING_VALUE;
                                break;
                            case DataFormat.PASSABLE:
                                _state = State.EXPECTING_KEY;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Key " + key + " does not seem parseable");
                    }
                }
                else
                {
                    if (_state == State.EXPECTING_KEY)
                    {
                        throw new ArgumentException("Was expecting key but got value instead");
                    }
                    if (multilineHeaderKey == null)
                    {
                        throw new ArgumentException("Got data without proper header. Config is invalid.");
                    }
                    //Multi line data
                    multilineHeaderKey.Value.Add(line);
                    _state = State.EXPECTING_ANY;
                }
            }

            if (Keys.Any(pair => pair.Value.Required && !pair.Value.Found))
            {
                throw new ArgumentException("Some required keys were not found. Config data is invalid.");
            }
        }

        private enum State
        {
            EXPECTING_KEY,
            EXPECTING_VALUE,
            EXPECTING_ANY
        }
    }
}
