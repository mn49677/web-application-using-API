using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Configuration;
using ArcGIS;

namespace AlternativeAlgorithm
{
    public class Solvers
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        // public VRPWebApp.Enums.AlgorithmType Mode {get;set;}
        /// <summary>
        /// Mode uses Haversine distance only in calculating DistanceMatrix
        /// </summary>
        public static readonly int MODE_HAVERSINE = 0;
        /// <summary>
        /// Mode uses both Manhattan and ORS and is faster than ORS
        /// </summary>
        public static readonly int MODE_HYBRID = 1;
        /// <summary>
        /// Mode uses OpenRouteService for every distance calculated in DistanceMatrix
        /// </summary>
        public static readonly int MODE_ORS = 2;

        // Attributes
        private Node[] Nodes;
        private List<Node> O = new List<Node>();
        public Solvers(Node[] Nodes)
        {
            this.Nodes = Nodes;
        }

        // Main methods
        /// <summary>
        /// Calculate distance matrix
        /// </summary>
        /// <param name="mode">Mode can return Manhattan distances (MODE_MANHATTAN), hybrid mode distances (MODE_HYBRID)
        /// and distances calculated by OpenRouteService algorithm for time and distance (MODE_ORS)</param>
        /// <param name="M">App.config -> Number of nodes taken for alternative algorithm check when calling hybrid mode</param>
        /// <param name="K">App.config -> Distance limit for nodes from iteration node taken for alternative algorithm calculation</param>
        /// <param name="k">App.config -> Constant which is multiplied with maximum distance of a node to set maximum distance from current node</param>
        /// <param name="p">App.config -> Fixed constant for similarity between two nodes in terms of distance from iteration node</param>
        public async Task<long[,]> CalculateDistanceMatrix(int mode)
        {
            int M = Int32.Parse(ConfigurationManager.AppSettings["M"]);
            double K = Double.Parse(ConfigurationManager.AppSettings["K"]);
            double k = Double.Parse(ConfigurationManager.AppSettings["k"]);
            double p = Double.Parse(ConfigurationManager.AppSettings["p"]);

            double[,] locations = new double[Nodes.Length,2];
            for (int i = 0; i < Nodes.Length; ++i) 
            {
                locations[i, 0] = Nodes[i].Latitude;
                locations[i, 1] = Nodes[i].Longitude;
            }
            switch (mode)
            {
                case 0:
                    return Haversine.Calculate(locations);
                case 1:
                    return await HybridMatrix(M, K, k, p);
                case 2:
                    RouteMatrix routeMatrix = new RouteMatrix(locations);
                    return await routeMatrix.CostMatrix(RouteMatrix.TIME_MATRIX);
                    //DataModelORS model = new DataModelORS(locations);
                    //return await model.DistanceMatrix();
                default:
                    return null;
            }
        }
        /// <summary>
        /// Hybrid version of distance matrix, uses Haversine and MapQuest API
        /// </summary>
        /// <param name="M">Changes maximum of M nodes closest to iteration node. Changes at max N*M distances.</param>
        /// <param name="K">Maximum distance from iteration node.</param>
        /// <param name="k">Maximum distance for nodes inside K distance, default initialize to 0.5</param>
        /// <param name="p">Constant less than 1 which is used to compare two nodes, default initialize to 0.2</param>
        /// <returns>long[,] distance matrix</returns>
        public async Task<long[,]> HybridMatrix(int? M, double? K, double? k, double? p)
        {
            long[,] distanceMatrix = Haversine.Calculate(GetMatrixFromNodes(Nodes));
            int N = Nodes.Length;
            for(int i = 0; i < N; ++i)
            {
                O = new List<Node>();
                long[] distances = GetRow(distanceMatrix, i);
                List<Node> nodes = Nodes.ToList<Node>();

                // Dodavanje udaljenosti od trenutnog čvora
                foreach (var node in nodes) node.DistanceFromCurrentNode = distances[node.Index];
                nodes = nodes.Where(n => n.Index >= i).ToList<Node>();
                // Sortiranje liste čvorova
                nodes.Sort((a, b) => a.DistanceFromCurrentNode.CompareTo(b.DistanceFromCurrentNode));
                // Uzimanje M čvorova
                nodes.Remove(nodes.Single(n => n.Index == i));
                nodes = nodes.GetRange(0, (int)M);
                //SLUČAJ 1 - Za sve vrijedi da su manji od K odnosno maksimalna udaljenost je manja od K
                if (nodes.All<Node>(n => n.DistanceFromCurrentNode < K))
                {
                    O.AddRange(nodes);
                    distanceMatrix = await UpdateMatrix(distanceMatrix, O, i);
                    continue;
                }
                // SLUČAJ 2: Ako nisu:
                // Račun za maksimalnu vrijednost (udaljenost)
                long max = 0;
                List<Node> nodesB2 = new List<Node>();
                foreach(var node in nodes) if (node.DistanceFromCurrentNode > max) max = node.DistanceFromCurrentNode;

                foreach (var node in nodes)
                    if (node.DistanceFromCurrentNode < k * max) O.Add(node);
                    else nodesB2.Add(node);

                for(int j = 0; j < nodesB2.Count - 1; ++j)
                {
                    double b = GetMaxReciprocally(nodesB2[j].DistanceFromCurrentNode, nodesB2[j+1].DistanceFromCurrentNode);
                    if (b < (1 + p))
                    {
                        if(!O.Contains(nodesB2[j])) O.Add(nodesB2[j]);
                        O.Add(nodesB2[j+1]);
                    }
                }
                // Sada imamo ispunjenu listu O označenih primjera
                distanceMatrix = await UpdateMatrix(distanceMatrix, O, i);
            }
            return distanceMatrix;
        }
        public async Task<long[,]> CalculateTimeMatrix(double speed, int mode)
        {
            long[,] matrix = await CalculateDistanceMatrix(mode);
            int N = matrix.GetLength(0);
            for (int i = 0; i < N; ++i)
                for(int j = 0; j < N; ++j)
                    matrix[i, j] = (long)(matrix[i,j] / speed);
            return matrix;
        }
        // Help methods
        private long[] GetRow(long[,] matrix, int row)
        {
            int length = matrix.GetLength(1);
            long[] ret = new long[length];
            for (int i = 0; i < length; ++i)
            {
                ret[i] = matrix[row, i];
            }
            return ret;
        }
        private double GetMaxReciprocally(long a, long b)
        {
            if (a < b) return (double) (b / a);
            else return a / b;
        }
        private double[,] GetMatrixFromNodes(Node[] nodes)
        {
            int N = nodes.Length;
            double[,] locations = new double[N, 2];
            for (int i = 0; i < N; ++i)
            {
                locations[i, 0] = nodes[i].Latitude;
                locations[i, 1] = nodes[i].Longitude;
            }
            return locations;
        }
        private async Task<long[,]> UpdateMatrix(long[,] distanceMatrix, List<Node> O, int i)
        {
            // Sada imamo ispunjenu listu O označenih primjera
            long d_min = int.MaxValue;
            List<Node> nodes = new List<Node>(O);
            foreach (var node in O)
            {
                if (O.First<Node>() == node || node.DistanceFromCurrentNode < d_min)
                {
                    //DataModelORS openRouteService = new DataModelORS(new double[,] { { node.Longitude, node.Latitude }, { Nodes[i].Longitude, Nodes[i].Latitude } });
                    RouteMatrix matrix = new RouteMatrix(new double[,] { { node.Latitude, node.Longitude }, { Nodes[i].Longitude, Nodes[i].Latitude } } );

                    try
                    {
                        nodes.Where<Node>(n => n.Index == node.Index).First<Node>().DistanceFromCurrentNode = (await matrix.CostMatrix(RouteMatrix.DISTANCE_MATRIX))[0, 1];
                        d_min = node.DistanceFromCurrentNode;
                    } catch(Exception ex)
                    {
                        Logger.Error("NullReferenceException in distance matrix ArcGIS calculation. Probably invalid key. Exception: " + ex);
                        Logger.Info("Distances from node" + " are calculated with Haversine.");
                    }
                }
            }
            // Sada su updateane sve vrijednosti udaljenosti od trenutnog čvora, potrebno je updateati matricu!
            foreach (var node in nodes)
            {
                distanceMatrix[i, node.Index] = node.DistanceFromCurrentNode;
                distanceMatrix[node.Index, i] = node.DistanceFromCurrentNode;
            }
            return distanceMatrix;
        }
    }
}
