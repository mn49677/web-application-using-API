using Google.OrTools.ConstraintSolver;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using System.Xml;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Generic;
using System;

namespace ORTools
{
    public class VRPSolver
    {
        readonly DataModel DataModel;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public VRPSolver(DataModel DataModel)
        {
            this.DataModel = DataModel;
        }

        /// <summary>
        /// Solve Vehicle Routing Problem
        /// </summary>
        /// <param name="solutionLimit">Limits the number of solutions for VRP</param>
        /// <param name="timeLimitInSeconds">Limits search time for VRP solution</param>
        /// <returns>VRP Result which has solution for problem. VRPResult class implemented here.</returns>
        /// <exception cref=""
        public VRPResult Calculate(int solutionLimit, int timeLimitInSeconds, int slack_waiting_time, bool time, bool capacity, bool depot, int solutionStrategy)
        {
            // ---------------------------------------------------------------------------------
            // ------ Postavljanje -------------------------------------------------------------
            RoutingIndexManager manager = new RoutingIndexManager(
                DataModel.TimeMatrix.GetLength(0),
                DataModel.VehicleNumber,
                DataModel.Depot);
            RoutingModel routing = new RoutingModel(manager);

            //for multiple starting / ending points

            /*RoutingIndexManager manager = new RoutingIndexManager(
                data.DistanceMatrix.GetLength(0),
                data.VehicleNumber,
                data.Starts,
                data.Ends);
            */

            // ---------------------------------------------------------------------------------
            // ------ Indexi za ograničenja ----------------------------------------------------
            //travel time + service time
            int totalTransitTimeCallbackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) =>
                {
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    return DataModel.TimeMatrix[fromNode, toNode] + DataModel.VehicleLocationUnloadTime[fromNode];
                }
            );

            routing.SetArcCostEvaluatorOfAllVehicles(totalTransitTimeCallbackIndex);

            /*int demandCallbackIndex = routing.RegisterUnaryTransitCallback(
                (long fromIndex) =>
                {
                    var fromNode = manager.IndexToNode(fromIndex);
                    return DataModel.Demands[fromNode];
                }); */

            int demandType0CallbackIndex = routing.RegisterUnaryTransitCallback(
               (long fromIndex) =>
               {
                   var fromNode = manager.IndexToNode(fromIndex);
                   return DataModel.DemandsByType[fromNode][0];
               });

            int demandType1CallbackIndex = routing.RegisterUnaryTransitCallback(
                (long fromIndex) =>
                {
                    var fromNode = manager.IndexToNode(fromIndex);
                    return DataModel.DemandsByType[fromNode][1];
                });


            int demandTotalCallbackIndex = routing.RegisterUnaryTransitCallback(
                (long fromIndex) =>
                {
                    var fromNode = manager.IndexToNode(fromIndex);
                    return demandType0CallbackIndex + demandType1CallbackIndex;
                });

            //skillovi

            for (int i = 1; i < DataModel.DemandsByType.Length; i++)
            {
                //provjeriti podudarnost paleta i LDC

                var index = manager.NodeToIndex(i);
                bool allowedLDC = false;
                List<int> allowedVehicles = new List<int>();
                //prepostavka da su vrijednosti sortirane
                for (int j = 0; j < DataModel.VehicleNumber; j++)
                {
                    //provjera za LDC
                    if (DataModel.SpecialtyNamesLocations[i][DataModel.SpecialtyNamesLocations[i].Length - 1]
                       .Equals(DataModel.SpecialtyNamesVehicles[j][DataModel.SpecialtyNamesVehicles[j].Length - 1])) {
                        allowedLDC = true;
                    }
                    try
                    {
                        if (
                            Convert.ToInt32(DataModel.SpecialtyNamesVehicles[j][DataModel.SpecialtyNamesVehicles[j].Length - 2]) >
                            Convert.ToInt32(DataModel.SpecialtyNamesLocations[i][0]))
                        {
                            if (allowedLDC)
                            {
                                var vehicleIndex = manager.NodeToIndex(j);
                                allowedVehicles.Add(Convert.ToInt32(vehicleIndex));
                                allowedLDC = false;
                            }
                        }
                    } catch (FormatException exc)
                    {
                        continue;
                    }

                }
                routing.SetAllowedVehiclesForIndex(allowedVehicles.ToArray(), index);
                allowedVehicles.Clear();

            }


            // ---------------------------------------------------------------------------------
            // ------ Dimenzije za ograničenja -------------------------------------------------
            if (capacity)
            {
                routing.AddDimensionWithVehicleCapacity(
                   demandType0CallbackIndex,
                   slack_max: 0,
                   DataModel.VehicleCapacitiesByType[0],
                   true,
                   "Capacity0");
                routing.AddDimensionWithVehicleCapacity(
                   demandType1CallbackIndex,
                   slack_max: 0,
                   DataModel.VehicleCapacitiesByType[1],
                   true,
                   "Capacity1");
            }

            //Allow to drop nodes.
            long penalty = (long)FindPenalty(DataModel.TimeMatrix);
            for (int i = 1; i < DataModel.TimeMatrix.GetLength(0); ++i)
            {
                routing.AddDisjunction(
                    new long[] { manager.NodeToIndex(i) }, penalty);
            }

            routing.AddDimension(
                totalTransitTimeCallbackIndex,
                30000000000000,
                30000000000000,
                false,
                "Time");

           

            if (time) {
                RoutingDimension timeDimension = routing.GetMutableDimension("Time");
                for (int i = 1; i < DataModel.TimeWindows.GetLength(0); ++i)
                {
                    long index = manager.NodeToIndex(i);
                    timeDimension.CumulVar(index).SetRange(
                        DataModel.TimeWindows[i, 0],
                        DataModel.TimeWindows[i, 1]);
                }
                // Add maximum working time for each vehicle start node.
                for (int i = 0; i < DataModel.VehicleNumber; ++i)
                {
                    long index = routing.Start(i);

                    /*timeDimension.CumulVar(index).SetMax(
                        (long)DataModel.VehicleWorkingHours[i]);*/

                    timeDimension.CumulVar(index).SetRange(
                      DataModel.VehicleStartingTimes[i, 0],
                      DataModel.VehicleStartingTimes[i, 1]);
                    routing.AddToAssignment(timeDimension.SlackVar(index));
                   
                }
               routing.AddDimension(totalTransitTimeCallbackIndex, 0, 550, true, "MaxTime");
                /* for (int i = 0; i < DataModel.VehicleNumber; ++i)
                 {
                     long index = routing.Start(i);
                     timeDimension.CumulVar(index).SetRange(
                         DataModel.TimeWindows[0, 0],
                         DataModel.TimeWindows[0, 1]);
                 }*/
                if (depot)
                {
                    // Add resource constraints at the depot.
                    Solver solver = routing.solver();
                      IntervalVar[] intervals = new IntervalVar[DataModel.VehicleNumber * 2];
                for (int i = 0; i < DataModel.VehicleNumber; ++i)
                {
                    // Add load duration at start of routes
                    intervals[2 * i] = solver.MakeFixedDurationIntervalVar(
                          timeDimension.CumulVar(routing.Start(i)), DataModel.VehicleLoadTime,
                          "depot_interval");
                    // Add unload duration at end of routes.
                    intervals[2 * i + 1] = solver.MakeFixedDurationIntervalVar(
                          timeDimension.CumulVar(routing.End(i)), DataModel.VehicleUnloadTime,
                          "depot_interval");
                }
                
                    long[] depot_usage = Enumerable.Repeat<long>(1, intervals.Length).ToArray();
                    solver.Add(solver.MakeCumulative(intervals, depot_usage,
                          DataModel.DepotCapacity, "depot"));

                    for (int i = 0; i < DataModel.VehicleNumber; ++i)
                    {
                        routing.AddVariableMinimizedByFinalizer(
                            timeDimension.CumulVar(routing.Start(i)));
                        routing.AddVariableMinimizedByFinalizer(
                            timeDimension.CumulVar(routing.End(i)));
                    }
                }

            }

            // ---------------------------------------------------------------------------------
            // ------ Računanje rješenja transit-------------------------------------------------------
            // Izračunaj Solution
            RoutingSearchParameters searchParameters =
              operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy =
              FirstSolutionStrategy.Types.Value.PathCheapestArc;
            searchParameters.TimeLimit = new Duration { Seconds = timeLimitInSeconds };
            searchParameters.SolutionLimit = solutionLimit;
            searchParameters.FirstSolutionStrategy = GetSolutionStrategyType(solutionStrategy);
            // Solve the problem.
            Assignment solution = routing.SolveWithParameters(searchParameters);
            //if (routing.Equals(null) || manager.Equals(null) || solution.Equals(null)) throw new NullReferenceException("Solution not found, check if solution possible.");
            Logger.Info("OR-tools VRP request");

            
            
            return new VRPResult
            {
                Routing = routing,
                Manager = manager,
                Solution = solution
            };
            
        }
        double FindPenalty(long[,] a)
        {
            int N = a.GetLength(0);
            long sum = 0;

            // total sum calculation of matrix 
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    sum += a[i, j];

            return (double)sum / 2; // (N * N);
        }
        /*
                    <option value="0">Automatic</option>
                    <option value="1">Path cheapest arc</option>
                    <option value="2">Path most constrained arc</option>
                    <option value="3">Evaluator strategy</option>
                    <option value="4">Savings</option>
                    <option value="5">Sweep</option>
                    <option value="6">Christofides</option>
                    <option value="7">All unperformed</option>
                    <option value="8">Best insertion</option>
                    <option value="9">Parallel cheapest insertion</option>
                    <option value="10">Local cheapest arc</option>
                    <option value="11">Global cheapest insertion</option>
                    <option value="12">Local cheapest insertion</option>
                    <option value="13">First unbound minimal value</option>
         */
        private FirstSolutionStrategy.Types.Value GetSolutionStrategyType(int type) {
            switch(type)
            {
                case 0:
                    return FirstSolutionStrategy.Types.Value.Automatic;
                case 1:
                    return FirstSolutionStrategy.Types.Value.PathCheapestArc;
                case 2:
                    return FirstSolutionStrategy.Types.Value.PathMostConstrainedArc;
                case 3:
                    return FirstSolutionStrategy.Types.Value.EvaluatorStrategy;
                case 4:
                    return FirstSolutionStrategy.Types.Value.Savings;
                case 5:
                    return FirstSolutionStrategy.Types.Value.Sweep;
                case 6:
                    return FirstSolutionStrategy.Types.Value.Christofides;
                case 7:
                    return FirstSolutionStrategy.Types.Value.AllUnperformed;
                case 8:
                    return FirstSolutionStrategy.Types.Value.BestInsertion;
                case 9:
                    return FirstSolutionStrategy.Types.Value.ParallelCheapestInsertion;
                case 10:
                    return FirstSolutionStrategy.Types.Value.LocalCheapestArc;
                case 11:
                    return FirstSolutionStrategy.Types.Value.GlobalCheapestArc;
                case 12:
                    return FirstSolutionStrategy.Types.Value.LocalCheapestInsertion;
                case 13:
                    return FirstSolutionStrategy.Types.Value.FirstUnboundMinValue;
                default:
                    return FirstSolutionStrategy.Types.Value.Automatic;
            }
        }
    }
}