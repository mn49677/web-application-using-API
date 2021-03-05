using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDi_VRP_Models.Models.Orders
{
    /// <summary>
    /// Specify each attribute as a key-value pair where the key is the name of a given field, 
    /// and the value is the attribute value for the corresponding field.
    /// </summary>
    public class OrderAttributes
    {
        public OrderAttributes()
        {
            //AssignmentRule = (int)OrderAssigmentRule.Override;
        }

        /// <summary>
        /// The name of the order. The name must be unique. If the name is left null, a name is automatically generated at solve time.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The descriptive information about the order. This can contain any textual information for the order and has no restrictions for uniqueness.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// This property specifies the amount of time that will be spent at the network location when the route visits it.
        /// A zero or null value indicates that the network location requires no service time.
        /// The unit for this field value is specified by the time_units parameter.
        /// </summary>
       /// !----------------- used in data mapping : vehiclelocation unload time -----------------!
        public double? ServiceTime { get; set; }
        /// <summary>
        /// The beginning time of the first time window for the network location. This field can contain a null value; a null value indicates no beginning time.
        /// </summary>
        /// !-------------- used for data mapper : time window------------!
        public long? TimeWindowStart1 { get; set; }
        /// <summary>
        /// The ending time of the first window for the network location. This field can contain a null value; a null value indicates no ending time.
        /// 
        /// A time window only states when a vehicle can arrive at an order; it doesn't state when the service time must be completed. To account for service time and departure before the time window ends, subtract ServiceTime from the <see cref="TimeWindowEnd1"/> field.
        /// </summary>
        /// !-------------------used for data mapper : time window ------------!
        public long? TimeWindowEnd1 { get; set; }
       
        /// <summary>
        /// A time window is considered violated if the arrival time occurs after the time window has ended. This field specifies the maximum allowable violation time for the first time window of the order.
        /// </summary>
        public long? MaxViolationTime1 { get; set; }
        
        /// <summary>
        /// The size of the delivery. You can specify size in any dimension, such as weight, volume, or quantity. You can even specify multiple dimensions, for example, weight and volume.
        /// 
        /// Make sure that <see cref="RouteAttributes.Capacities"/> for Routes and <see cref="OrderAttributes.DeliveryQuantities"/> and <see cref="RouteAttributes.PickupQuantities"/> for Orders are specified in the same manner.
        /// </summary>
        public string DeliveryQuantities { get; set; }
        /// <summary>
        /// The size of the pickup. You can specify size in any dimension, such as weight, volume, or quantity. You can even specify multiple dimensions, for example, weight and volume.
        /// 
        /// This field is analogous to the <see cref="DeliveryQuantities"/> field of Orders.
        /// </summary>
        /// ! ------------------- used in data mapper : demands --------------!
        public string PickupQuantities { get; set; }
       
        /// <summary>
        /// A space-separated string containing the names of the specialties required by the order. A null value indicates that the order doesn't require specialties.
        /// 
        /// Spelling must match <see cref="RouteAttributes.SpecialtyNames"/> exactly.
        /// </summary>
        public string SpecialtyNames { get; set; }

       
        /// <summary>
        /// Specifies the rule for assigning the order to a route. 
        /// See <see cref="OrderAssigmentRule"/>
        /// </summary>
        //public int? AssignmentRule { get; set; }

        /// <summary>
        /// The name of the route to which the order is assigned.
        /// </summary>
        //public string RouteName { get; set; }
       
        /// <summary>
        /// This indicates the sequence of the order on its assigned route.
        /// A null value can only occur together with a null RouteName value.
        /// </summary>
        //public int? Sequence { get; set; }

    }
}
