using System;
using System.Collections.Generic;
using ORTools;
using System.Linq;
using ArcGIS;

namespace VRPWebApp.ViewModels.Vrp
{
    public class ResultViewModel
    {
        public ResultViewModel()
        {
            this.Routes = new List<Route>();
        }
        public ICollection<Route> Routes { get; set; }

        public void FillWithResults(VRPResult result, double[,] locations, DataModel dataModel)
        {
            // ispunjava: rute (lokacije, id, ime rute), dodaje lokacije za rute (x, y, StopID)
            for (int i = 0; i < dataModel.VehicleNumber; ++i)
            {

                this.Routes.Add(new Route
                {
                    Locations = new List<Location>(),
                    Id = i,
                    RouteName = "Route " + i,

                });
                var index = result.Routing.Start(i);
                int j; Location loc;
                while (result.Routing.IsEnd(index) == false)
                {

                    j = result.Manager.IndexToNode(index);
                    loc = new Location
                    {
                        X = locations[j, 1],
                        Y = locations[j, 0],
                        StopID = j,
                        LocationName = dataModel.LocationNames[j],
                        Volume = (int)dataModel.DemandsByType[j][1],
                        Weight = (int)dataModel.DemandsByType[j][0],
                        ServiceTime = dataModel.VehicleLocationUnloadTime[j]
                    };
                    var timeVar = result.Routing.GetMutableDimension("Time").CumulVar(index);
                    var t = result.Solution.Min(timeVar);
                    loc.ArrivalTime = t.ToString();

                    this.Routes.Where(r => r.Id == i).FirstOrDefault().Locations.Add(loc);
                    index = result.Solution.Value(result.Routing.NextVar(index));
                }
                foreach (Route r in Routes)
                {
                    long distance = 0;
                    long travelTime = 0;
                    
                    int stops = r.Locations.Count;
                    double serviceTime = 0;
                    Location[] locationsArray = r.Locations.ToArray<Location>();
                    int length = locationsArray.Length;
                    for (int l = 0; l < length - 1; ++l)
                    {
                        RouteMatrix arcgisCostMatrixSolver = new RouteMatrix(
                            new double[,] { { locationsArray[l].Y, locationsArray[l].X },
                                             { locationsArray[l+1].Y, locationsArray[l+1].X } });
                        try
                        {
                            //distance += (await arcgisCostMatrixSolver.CostMatrix(RouteMatrix.DISTANCE_MATRIX))[0, 1];
                            distance += dataModel.TimeMatrix[locationsArray[l].StopID, locationsArray[l + 1].StopID] * 833;

                            
                        }
                        catch (NullReferenceException ex)
                        {
                            //distance += dataModel.TimeMatrix[locationsArray[l].StopID, locationsArray[l + 1].StopID]; 
                        }
                        try
                        {
                            //travelTime += (await arcgisCostMatrixSolver.CostMatrix(RouteMatrix.TIME_MATRIX))[0, 1];
                            travelTime += dataModel.TimeMatrix[locationsArray[l].StopID, locationsArray[l + 1].StopID];
                        }
                        catch (NullReferenceException ex)
                        {
                            //travelTime += dataModel.TimeMatrix[locationsArray[l].StopID, locationsArray[l + 1].StopID];
                        }
                    }
                    
                    int vehicleId = r.Id;
                    long volume = dataModel.VehicleCapacitiesByType[0][vehicleId];
                    long weight = dataModel.VehicleCapacitiesByType[1][vehicleId];
                    for(int m = 0; m < r.Locations.Count; m++)
                    {
                        serviceTime += r.Locations.ElementAt(m).ServiceTime;
                    }
                    // Passing values to View Model
                    r.TravelTime = travelTime;
                    r.Stops = stops;
                    r.Distance = (int)distance; // long u int!!!
                    r.Volume = (int)volume;
                    r.Weight = (int)weight;
                    r.ServiceTime = serviceTime;
                    
                }
            }
        }
    }
}
