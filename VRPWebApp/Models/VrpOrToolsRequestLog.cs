using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VRPWebApp.Models
{
    public class VrpOrToolsRequestLog
    {
        [ForeignKey("VrpToolsResponseLog")]
        [Key]
        public int Id { get; set; }
        public string Locations { get; set; }
        public string DistanceMatrix { get; set; }
        public int VehicleNumber { get; set; } // JSON format
     //   public string PickupsDeliveries { get; set; } // JSON format
        public string TimeWindows { get; set; } // JSON format
        public string Depots { get; set; }
     //   public string Demands { get; set; } // JSON format
        public int VehicleLoadTime { get; set; } // vrijeme punjenja vozila
        public long VehicleUnloadTime { get; set; } // vrijeme praznjenja vozila
        public int DepotCapacity; // broj vozila koje se mogu puniti u isto vrijeme
        public string VehicleLocationUnloadTime { get; set; } // service time
        public string DemandsByType { get; set; }

        //npr [0] banane, [0][1]  - kapacitet vozila 1 za banane, [0][2] - kapacitet vozila 2 za banane
        public string VehicleCapacitiesByType { get; set; }
        //for multiple starting points - depots
        public string Starts { get; set; }
        //for multiple ending points 
        public string Ends { get; set; }

        // Other parameters
        public string Configuration { get; set; }
        //public int AlternativeAlgorithmMode { get; set; } // Alternative algorithm mode
        //public int TimeLimit { get; set; }
        //public int SolutionNumber { get; set; }
        //public int DataIndex { get; set; }

        // Request data
        public DateTime DateTimeOfRequest { get; set; }

        // Response class
    }
}