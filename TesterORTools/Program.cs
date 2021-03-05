using System;
using Google.OrTools.ConstraintSolver;
using ORTools;

namespace TesterORTools
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            DataModel model = DataModel.GetMockDataS();
            VRPSolver solver = new VRPSolver(model);
            var result = solver.Calculate(100, 100, 0);
            PrintSolution(model, result.Routing, result.Manager, result.Solution);
        }

        public static void PrintSolution(
              in DataModel data,
              in RoutingModel routing,
              in RoutingIndexManager manager,
              in Assignment solution)
        {
            RoutingDimension timeDimension = routing.GetMutableDimension("Time");
            // Inspect solution.
            long totalDistance = 0;
            long totalLoad = 0;
            for (int i = 0; i < data.VehicleNumber; ++i)
            {
                Console.WriteLine("Route for Vehicle {0}:", i);
                long routeDistance = 0;
                long routeLoad = 0;
                var index = routing.Start(i);
                while (routing.IsEnd(index) == false)
                {
                    long nodeIndex = manager.IndexToNode(index);
                    routeLoad =  routeLoad + data.DemandsByType[nodeIndex][0]+ data.DemandsByType[nodeIndex][1];
                    Console.Write("{0} Load({1}) -> ", nodeIndex, routeLoad);
                    var previousIndex = index;
                    index = solution.Value(routing.NextVar(index));
                    routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
                }
                Console.WriteLine("{0}", manager.IndexToNode((int)index));
                Console.WriteLine("Distance of the route: {0}m", routeDistance);
                totalDistance += routeDistance;
                totalLoad += routeLoad;
            }
            Console.WriteLine("Total distance of all routes: {0}m", totalDistance);
            Console.WriteLine("Total load of all routes: {0}m", totalLoad);
        }
            
           
        
    }
}
