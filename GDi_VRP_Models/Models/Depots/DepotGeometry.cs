using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDi_VRP_Models.Models.Depots
{
    /// <summary>
    /// Specifies the depot geometry as a point containing x and y properties.
    /// </summary>
    public class DepotGeometry
    {
        /// <summary>
        /// X Coordinate
        /// </summary>
        public double x { get; set; }
        
        /// <summary>
        /// Y Coordinate
        /// </summary>
        public double y { get; set; }
    }
}
