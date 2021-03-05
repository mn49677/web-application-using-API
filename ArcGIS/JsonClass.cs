using ArcGIS.Response;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ArcGIS.Request
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    [DataContract]
    public class Geometry
    {
        [DataMember]
        public double x { get; set; }
        [DataMember]
        public double y { get; set; }

    }
    [DataContract]
    public class Attributes
    {
        [DataMember]
        public string Name { get; set; }

    }
    [DataContract]
    public class Feature
    {
        [DataMember]
        public Geometry geometry { get; set; }
        [DataMember]
        public Attributes attributes { get; set; }

    }

    [DataContract]
    public class Root
    {
        [DataMember]
        public List<Feature> features { get; set; }
        [DataMember]
        public SpatialReference spatialReference { get; set; }

    }

    [DataContract]
    public class SpatialReference
    {
        public int wkid { get; set; }
        public int latestWkid { get; set; }

    }
}
