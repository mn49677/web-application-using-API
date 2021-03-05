using System;
using Google.OrTools.ConstraintSolver;
using ORTools;
namespace ORToolsWebApp
{
    public class ORToolsVprSolver
    {
        public Assignment Calculate(DataModel model)
        {
            DataModel data = model;

            // ------------------------------------------------ Routing manager 
            RoutingIndexManager manager = new RoutingIndexManager(
                data.TimeMatrix.GetLength(0),
                data.VehicleNumber,
                data.Depot);

            // ------------------------------------------------ Routing model 
            RoutingModel routing = new RoutingModel(manager);

            // ------------------------------------------------ Transit callback 
            int transitCallbackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) => {
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    return data.TimeMatrix[fromNode, toNode];
                }
            );

            // ------------------------------------------------ Slanje veličina udaljenosti modelu 
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            // ------------------------------------------------ Capacity Constraint
            int demandCallbackIndex = routing.RegisterUnaryTransitCallback(
                (long fromIndex) => {
                    var fromNode = manager.IndexToNode(fromIndex);
                    return data.Demands[fromNode];
                }
            );

            //ovdje ce zapravo bit pozivi ovisno o kojem vozilu se radi
            routing.AddDimensionWithVehicleCapacity(
            demandCallbackIndex, 0,  // pocetni 0
            data.VehicleCapacities,   // maksimalni kapacitet
            true,
            "Capacity");

            //ogranicenje udaljenosti- pickups and deliveries
            routing.AddDimension(transitCallbackIndex, 0, 3000, true, "Distance");
            RoutingDimension distanceDimension = routing.GetMutableDimension("Distance");
            distanceDimension.SetGlobalSpanCostCoefficient(100);

            Solver solver = routing.solver();
            for (int i = 0; i < data.PickupsDeliveries.GetLength(0); i++)
            {
                long pickupIndex = manager.NodeToIndex(data.PickupsDeliveries[i][0]);
                long deliveryIndex = manager.NodeToIndex(data.PickupsDeliveries[i][1]);
                routing.AddPickupAndDelivery(pickupIndex, deliveryIndex);
                solver.Add(solver.MakeEquality(
                      routing.VehicleVar(pickupIndex),
                      routing.VehicleVar(deliveryIndex)));
                solver.Add(solver.MakeLessOrEqual(
                      distanceDimension.CumulVar(pickupIndex),
                      distanceDimension.CumulVar(deliveryIndex)));
            }

            int transitTimeCallbackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) => {
                    // Convert from routing variable Index to distance matrix NodeIndex.
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    return data.TimeMatrix[fromNode, toNode];
                }
            );

            routing.AddDimension(
            transitTimeCallbackIndex, // transit callback
            30, // allow waiting time
            30, // vehicle maximum capacities
            false,  // start cumul to zero
            "Time");

            RoutingDimension timeDimension = routing.GetMutableDimension("Time");

            //dodano time window za svaku lokaciju 
            for (int i = 1; i < data.TimeWindows.GetLength(0); ++i)
            {
                long index = manager.NodeToIndex(i);
                timeDimension.CumulVar(index).SetRange(
                    data.TimeWindows[i, 0],
                    data.TimeWindows[i, 1]);
            }
            // Add time window constraints for each vehicle start node.
            for (int i = 0; i < data.VehicleNumber; ++i)
            {
                long index = routing.Start(i);
                timeDimension.CumulVar(index).SetRange(
                    data.TimeWindows[0, 0],
                    data.TimeWindows[0, 1]);
            }
            for (int i = 0; i < data.VehicleNumber; ++i)
            {
                routing.AddVariableMinimizedByFinalizer(
                    timeDimension.CumulVar(routing.Start(i)));
                routing.AddVariableMinimizedByFinalizer(
                    timeDimension.CumulVar(routing.End(i)));
            }

            // Setting first solution heuristic.
            RoutingSearchParameters searchParameters =
              operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy =
              FirstSolutionStrategy.Types.Value.PathCheapestArc;
            //searchParameters.TimeLimit = new Google.Protobuf.WellKnownTypes.Duration { Seconds = 300 };

            // Solve the problem.
            Assignment solution = routing.SolveWithParameters(searchParameters);
            PrintSolution(data, routing, manager, solution);
            return solution;
        }



        // ----------------------------------------------------------- PRINT function

        static void PrintSolution(
              in DataModel data,
              in RoutingModel routing,
              in RoutingIndexManager manager,
              in Assignment solution)
        {
            RoutingDimension timeDimension = routing.GetMutableDimension("Time");
            // Inspect solution.
            long totalTime = 0;
            for (int i = 0; i < data.VehicleNumber; ++i)
            {
                Console.WriteLine("Route for Vehicle {0}:", i);
                var index = routing.Start(i);
                while (routing.IsEnd(index) == false)
                {
                    var timeVar = timeDimension.CumulVar(index);
                    Console.Write("{0} Time({1},{2}) -> ",
                        manager.IndexToNode(index),
                        solution.Min(timeVar),
                        solution.Max(timeVar));
                    index = solution.Value(routing.NextVar(index));
                }
                var endTimeVar = timeDimension.CumulVar(index);
                Console.WriteLine("{0} Time({1},{2})",
                    manager.IndexToNode(index),
                    solution.Min(endTimeVar),
                    solution.Max(endTimeVar));
                Console.WriteLine("Time of the route: {0}min", solution.Min(endTimeVar));
                totalTime += solution.Min(endTimeVar);
            }
            Console.WriteLine("Total time of all routes: {0}min", totalTime);
        }

    }
}