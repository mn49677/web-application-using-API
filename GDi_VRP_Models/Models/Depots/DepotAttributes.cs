using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDi_VRP_Models.Models.Depots
{
    /// <summary>
    /// Specify each attribute as a key-value pair where the key is the name of a given field, and the value is the attribute value for the corresponding field.
    /// </summary>
    public class DepotAttributes
    {
        /// <summary>
        /// The name of the depot. The <see cref="RouteAttributes.StartDepotName"/> and <see cref="RouteAttributes.EndDepotName"/> on routes reference the names you specify here. 
        /// It is also referenced by the route renewals, when used.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The descriptive information about the depot location. This can contain any textual information and has no restrictions for uniqueness.
        /// </summary>
        public string Description { get; set; }
    }
}
