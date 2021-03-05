using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlternativeAlgorithm;
using MapQuest;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using ORTools;
using VRPWebApp.Enums;
using VRPWebApp.Models;
using VRPWebApp.ViewModels.Vrp;

namespace VRPWebApp.Controllers
{
    public class VrpController : Controller
    {
        private readonly VrpContext _context;
        public VrpController(VrpContext context)
        {
            _context = context;
        }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

     
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Result(int AlgorithmMode, string timeLimit, string solutionLimit, bool capacity, bool time, bool depo, int solutionStrategy, int loadTime, int unloadTime, int depotCapacity)
        {
            //pomocni objekt kako bi mogla napraviti json                      

            ConfigSpecification spec = new ConfigSpecification
            {
                AlgorithmMode = (AlgorithmType)AlgorithmMode,
                TimeLimit = timeLimit,
                SolutionLimit = solutionLimit,
                DataIndex = "1",
                //  DateTimeRequest = DateTime.Now --> ovo je iz  VrpOrToolsRequestLog modela
            };

            string[] names = { "input_orders", "input_routes", "depots", "1" };

            GetDataModel dmodel = new GetDataModel();

            dmodel.GenerateDataModel(names, "1");


            //priprema za poziv solvera (AA)
            Node[] nodes = new Node[dmodel.dataModel.Locations.GetLength(0)];
            for (int i = 0; i < dmodel.dataModel.Locations.GetLength(0); ++i)
            {
                nodes[i] = new Node();
                nodes[i].Latitude = dmodel.dataModel.Locations[i, 1];
                nodes[i].Longitude = dmodel.dataModel.Locations[i, 0];
                nodes[i].Index = i;
            }
            Solvers solvers = new Solvers(nodes);
            // ArcGIS Cost matrix solver
            //RouteMatrix matrix = new RouteMatrix(dmodel.dataModel.Locations);
            //pozovi algoritam ovisno o odabranom modelu
            var timeMatrix = await solvers.CalculateTimeMatrix(833, (int)AlgorithmMode);
            //var timeMatrix = await matrix.CostMatrix(RouteMatrix.TIME_MATRIX);
            dmodel.dataModel.TimeMatrix = timeMatrix;
            var spectojson = JsonConvert.SerializeObject(spec);

            //spremi novi request u bazu
            VrpOrToolsRequestLog request = new VrpOrToolsRequestLog
            {
                Locations = JsonConvert.SerializeObject(dmodel.dataModel.Locations),
                DistanceMatrix = JsonConvert.SerializeObject(timeMatrix),
                VehicleNumber = dmodel.dataModel.VehicleNumber,
                TimeWindows = JsonConvert.SerializeObject(dmodel.dataModel.TimeWindows),
                Depots = JsonConvert.SerializeObject(dmodel.dataModel.Depot),
                VehicleLoadTime = loadTime,
                VehicleUnloadTime = unloadTime,
                DepotCapacity = depotCapacity,
                VehicleLocationUnloadTime = JsonConvert.SerializeObject(dmodel.dataModel.VehicleLocationUnloadTime),
                DemandsByType = JsonConvert.SerializeObject(dmodel.dataModel.DemandsByType),
                VehicleCapacitiesByType = JsonConvert.SerializeObject(dmodel.dataModel.VehicleCapacitiesByType),
                Starts = JsonConvert.SerializeObject(dmodel.dataModel),
                Ends = JsonConvert.SerializeObject(dmodel.dataModel),
                Configuration = JsonConvert.SerializeObject(spec),
                DateTimeOfRequest = DateTime.Now
            };

            _context.Add(request);
            await _context.SaveChangesAsync();

            //poziv VRP solvera:
            VRPSolver solve = new VRPSolver(dmodel.dataModel);
            var result = solve.Calculate(!String.IsNullOrEmpty(solutionLimit) ? Int32.Parse(solutionLimit): 0, !String.IsNullOrEmpty(timeLimit) ? Convert.ToInt32(timeLimit): 0, 30000, time, capacity, depo, solutionStrategy);

            VrpOrToolsResponseLog response = new VrpOrToolsResponseLog
            {
                DateTimeOfResponse = DateTime.Now,
                Response = JsonConvert.SerializeObject(result),
                requestLog = request
            };
            _context.Add(response);
            await _context.SaveChangesAsync();


            ResultViewModel rvmResult = new ResultViewModel();
            rvmResult.FillWithResults(result, dmodel.dataModel.Locations, dmodel.dataModel);
            return View(rvmResult);
        }
    }
}
