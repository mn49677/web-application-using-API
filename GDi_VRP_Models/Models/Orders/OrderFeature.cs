using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDi_VRP_Models.Models.Orders
{
    /// <summary>
    /// An order can represent a delivery (for example, furniture delivery), 
    /// a pickup (such as an airport shuttle bus picking up a passenger), 
    /// or some type of service or inspection (a tree trimming job or building inspection, for instance).
    /// </summary>
    public class OrderFeature
    {
        /// <summary>
        /// Order geometry
        /// 
        /// </summary>
        [JsonProperty("geometry")]
        public OrderGeometry geometry { get; set; }

        /// <summary>
        /// Order Attributes
        /// </summary>
        [JsonProperty("attributes")]
        public OrderAttributes attributes { get; set; }
    }
}
