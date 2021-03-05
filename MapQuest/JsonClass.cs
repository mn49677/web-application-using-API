using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MapQuest
{
    [DataContract]
    public class LatLng
    {
        [DataMember]
        public double lat { get; set; }
        [DataMember]
        public double lng { get; set; }

    }
    [DataContract]
    public class Location
    {
        [DataMember(Name = "latLng")]
        public LatLng LatLng { get; set; }

    }

    [DataContract]
    public class Root
    {
        [DataMember(Name = "locations")]
        public List<Location> Locations { get; set; }

    }


}
