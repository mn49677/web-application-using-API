using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ArcGIS.Response
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class SpatialReference
    {
        public int wkid { get; set; }
        public int latestWkid { get; set; }

    }

    public class Field
    {
        public string name { get; set; }
        public string type { get; set; }
        public string alias { get; set; }
        public int? length { get; set; }

    }
    [DataContract(Name = "attributes")]
    public class attributes
    {
        [DataMember]
        public int ObjectID { get; set; }
        public int DestinationRank { get; set; }
        [DataMember]
        public double Total_Time { get; set; }
        [DataMember]
        public double Total_Distance { get; set; }
        public int OriginOID { get; set; }
        public string OriginName { get; set; }
        public int DestinationOID { get; set; }
        [DataMember]
        public string DestinationName { get; set; }
        public int Shape_Length { get; set; }

    }
    [DataContract(Name = "features")]
    public class Featuree
    {
        [DataMember(Name = "attributes")]
        public attributes attributes { get; set; }

    }

    public class Value
    {
        public string displayFieldName { get; set; }
        public string geometryType { get; set; }
        public SpatialReference spatialReference { get; set; }
        public List<Field> fields { get; set; }
        public List<Featuree> features { get; set; }
        public bool exceededTransferLimit { get; set; }

    }

    public class RootJson
    {
        public string paramName { get; set; }
        public string dataType { get; set; }
        public Value value { get; set; }

    }



}

