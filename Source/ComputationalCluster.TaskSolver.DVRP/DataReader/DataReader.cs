using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComputationalCluster.TaskSolver.DVRP.DataReader
{
    internal class Reader
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
            {"CAPACITIES", new Key("CAPACITIES", false, DataType.REAL, DataFormat.SINGLE_LINE, 0.0f)},
            {"SPEED", new Key("SPEED", false, DataType.REAL, DataFormat.SINGLE_LINE, 1.0f)},
            {"MAX_TIME", new Key("MAX_TIME", false, DataType.REAL, DataFormat.SINGLE_LINE, double.PositiveInfinity)},
            {"EDGE_WEIGHT_TYPE", new Key("EDGE_WEIGHT_TYPE", false, DataType.ENUM, DataFormat.SINGLE_LINE){EnumType = typeof(EdgeWeightType)}},
            {"EDGE_WEIGHT_FORMAT", new Key("EDGE_WEIGHT_FORMAT", false, DataType.ENUM, DataFormat.SINGLE_LINE){EnumType = typeof(EdgeWeightFormat)}},
            {"OBJECTIVE", new Key("OBJECTIVE", false, DataType.ENUM, DataFormat.SINGLE_LINE){EnumType = typeof(Objective)}},
            {"DATA_SECTION", new Key("DATA_SECTION", true, DataType.NONE, DataFormat.PASSABLE)},
            {"DEPOTS", new Key("DEPOTS", true, DataType.LIST, DataFormat.MULTI_LINE)},
            {"DEMAND_SECTION", new Key("DEMAND_SECTION", true, DataType.LIST, DataFormat.MULTI_LINE)},
            {"LOCATION_COORD_SECTION", new Key("LOCATION_COORD_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"VISIT_LOCATION_SECTION", new Key("VISIT_LOCATION_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"DEPOT_LOCATION_SECTION", new Key("DEPOT_LOCATION_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"VEHICLE_CAPACITY_SECTION", new Key("VEHICLE_CAPACITY_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"VEHICLE_SPEED_SECTION", new Key("VEHICLE_SPEED_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"VEHICLE_WEIGHT_SECTION", new Key("VEHICLE_COST_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"EDGE_WEIGHT_SECTION", new Key("EDGE_WEIGHT_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"TIME_WINDOW_SECTION", new Key("TIME_WINDOW_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"DEPOT_TIME_WINDOW_SECTION", new Key("DEPOT_TIME_WINDOW_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"VEH_TIME_WINDOW_SECTION", new Key("VEH_TIME_WINDOW_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"DURATION_SECTION", new Key("DURATION_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"TIME_AVAIL_SECTION", new Key("TIME_AVAIL_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"DURATION_BY_VEH_SECTION", new Key("DURATION_BY_VEH_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"VISIT_COMPAT_SECTION", new Key("VISIT_COMPAT_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"DEPOT_COMPAT_SECTION", new Key("DEPOT_COMPAT_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"VEH_COMPAT_SECTION", new Key("VEH_COMPAT_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"VEH_DEPOT_SECTION", new Key("VEH_DEPOT_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"ORDER_SECTION", new Key("ORDER_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"OPTIONAL_VISIT_SECTION", new Key("OPTIONAL_VISIT_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"VISIT_AVAIL_SECTION", new Key("VISIT_AVAIL_SECTION", false, DataType.LIST, DataFormat.MULTI_LINE)},
            {"TYPE", new Key("TYPE", false, DataType.NONE, DataFormat.PASSABLE)},
            {"COMMENT", new Key("COMMENT", false, DataType.NONE, DataFormat.PASSABLE)},
            {"EOF", new Key("EOF", false, DataType.NONE, DataFormat.PASSABLE)}
        };

        internal DVRPCommonData Parse(string data)
        {
            Key multilineHeaderKey = null;
            foreach (var line in data.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s + "\n"))
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
                                    switch (relatedKey.DataType)
                                    {
                                        case DataType.STRING:
                                            relatedKey.Value = value.ToString();
                                            break;
                                        case DataType.ENUM:
                                            relatedKey.Value = Enum.Parse(relatedKey.EnumType, value.ToString());
                                            break;
                                        case DataType.INT:
                                            relatedKey.Value = int.Parse(value.ToString());
                                            break;
                                        case DataType.REAL:
                                            relatedKey.Value = float.Parse(value.ToString());
                                            break;
                                        case DataType.LIST:
                                            throw new ArgumentException("Single line keys can't have values of list type");
                                        case DataType.NONE:
                                            throw new ArgumentException("Single line key must have a key");
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                }
                                else
                                {
                                    throw new ArgumentException("Can't parse single line key " + key);
                                }
                                _state = State.EXPECTING_KEY;
                                break;
                            case DataFormat.MULTI_LINE:
                                multilineHeaderKey = relatedKey;
                                multilineHeaderKey.Value = new List<object>();
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
                    var trimmed = line.TrimEnd('\n');
                    var split = trimmed.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                    var values = split.ToList();
                    multilineHeaderKey.Value.Add(values);
                    _state = State.EXPECTING_ANY;
                }
            }

            if (Keys.Any(pair => pair.Value.Required && !pair.Value.Found))
            {
                throw new ArgumentException("Some required keys were not found. Config data is invalid.");
            }

            var depots = new List<Depot>();

            foreach (var depotId in Keys["DEPOTS"].Value)
            {
                var coord = Keys["LOCATION_COORD_SECTION"].Value[int.Parse(depotId[0])]; // Ohhh... So ba
                var depot = new Depot {Id = int.Parse(coord[0]), X = float.Parse(coord[1]), Y = float.Parse(coord[2])};
                if (Keys["DEPOT_TIME_WINDOW_SECTION"].Found)
                {
                    var times = Keys["DEPOT_TIME_WINDOW_SECTION"].Value[int.Parse(depotId[0])];
                    depot.Starts = float.Parse(times[1]);
                    depot.Ends = float.Parse(times[2]);
                }
                depots.Add(depot);
            }

            var pickups = new List<Pickup>();

            foreach (var demand in Keys["DEMAND_SECTION"].Value)
            {
                var visitId = int.Parse(demand[0]);
                var coord = Keys["LOCATION_COORD_SECTION"].Value[visitId]; // Probably even worse...
                var pickup = new Pickup
                {
                    Id = int.Parse(coord[0]),
                    Size = -float.Parse(demand[1]),
                    X = float.Parse(coord[1]),
                    Y = float.Parse(coord[2]),
                    UnloadTime = 0,
                    AvailableAfter = 0
                };
                if (Keys["DURATION_SECTION"].Found)
                {
                    var duration = Keys["DURATION_SECTION"].Value[visitId - 1]; // :(
                    pickup.UnloadTime = float.Parse(duration[1]);
                }
                if (Keys["TIME_AVAIL_SECTION"].Found)
                {
                    var avail = Keys["TIME_AVAIL_SECTION"].Value[visitId - 1];
                    pickup.AvailableAfter = float.Parse(avail[1]);
                }
                pickups.Add(pickup);
            }

            var speed = Keys["SPEED"].Value ?? Keys["SPEED"].DefaultValue;

            var capacity = Keys["CAPACITIES"].Value ?? Keys["CAPACITIES"].DefaultValue;

            var count = Keys["NUM_VEHICLES"].Value ?? Keys["NUM_VEHICLES"].DefaultValue;

            return new DVRPCommonData()
            {
                Depots = depots,
                Pickups = pickups,
                VehicleSpeed = speed,
                VehicleCapacity = capacity,
                NumVehicles = count
            };
        }

        private enum State
        {
            EXPECTING_KEY,
            EXPECTING_VALUE,
            EXPECTING_ANY
        }
    }
}
