using GDi_VRP_Models.Models.Depots;
using GDi_VRP_Models.Models.Options;
using GDi_VRP_Models.Models.Orders;
using GDi_VRP_Models.Models.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDi_VRP_Models.Models
{
    /// <summary>
    /// Scheduler VRP Request/Submit model 
    /// </summary>
    public class SchedulerVRPRequestModel
    {
        public SchedulerVRPRequestModel()
        {

            var vrpOptionsBuilder = new VRPOptionsBuilder();

            this.Params = vrpOptionsBuilder.SubmitJobParams;

            this.Orders = new List<OrderFeature>();
            this.Depots = new List<DepotFeature>();
            this.Routes = new List<RouteFeature>();
        }
        /// <summary>
        /// Job Options Parameters
        /// </summary>
        public SubmitJobParams Params { get; set; }

        /// <summary>
        /// Orders (Locations/Tasks)
        /// </summary>
        public List<OrderFeature> Orders { get; set; }

        /// <summary>
        /// Depots
        /// </summary>
        public List<DepotFeature> Depots { get; set; }

        /// <summary>
        /// Vehicles (+ Drivers)
        /// 
        /// </summary>
        /// number of vehicles
        public List<RouteFeature> Routes { get; set; }


    }
}
