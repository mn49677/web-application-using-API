using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDi_VRP_Models.Models.Depots
{
    /// <summary>
    /// Use this parameter to specify a location that a vehicle departs from at the beginning of its workday and returns to at the end of the workday. 
    /// Vehicles are loaded (for deliveries) or unloaded (for pickups) at depots at the start of the route.
    /// </summary>
    public class DepotFeature
    {
        /// <summary>
        /// Depot geometry
        /// </summary>
        public DepotGeometry geometry { get; set; }
        
        /// <summary>
        /// Depot Attributes
        /// </summary>
        public DepotAttributes attributes { get; set; }
    }
}
