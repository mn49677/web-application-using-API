using OpenRouteService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AlternativeAlgorithm
{
    public class AlternateAlgorithmJ
    {
        private NodeJ[] Nodes { get; }        
        public AlternateAlgorithmJ(NodeJ[] nodes)
        {
            this.Nodes = nodes;
        }
        public double[,] GetHaversineMatrix(double[,] locations)
        {
            double[,] hMatrix = HaversineDistanceMatrix(locations);
            return hMatrix;
        }

        public double[,] ChangeDistanceMatrix(int K, int M, double konstanta, double p, double[,] locations)
        {
            
            //stvori matricu s haversine udaljenostima
            double[,] distanceMatrix = HaversineDistanceMatrix(locations);

            //values točke su indeksi označenih točaka povezanih s key točkom
            Dictionary<int, int[]> selected = new Dictionary<int, int[]>();

            //M najbližih
            Dictionary<int, List<int>> closestPairs = new Dictionary<int, List<int>>();
            closestPairs = GetMValues(distanceMatrix, M);
            
           // var l = closestPairs;

            for (int i = 0; i < distanceMatrix.GetLength(0); ++i)
            {      
                //dohvati indekse od M najblizih
                List<int> j = closestPairs.GetValueOrDefault(i);
                int zadnjiEl = j.Last();

                //ako je najdalji element blizak, označi sve (Slucaj 1)
                if (distanceMatrix[i,zadnjiEl] < K)
                {
                    selected.Add(i, j.ToArray());
                }
                else //slucaj2
                {                  
                    List<int> bliski = new List<int>();
                    List<int> potencijalnoBliski = new List<int>();

                    foreach (var v in closestPairs.GetValueOrDefault(i))
                    {                        
                        if (distanceMatrix[i,zadnjiEl]*konstanta > distanceMatrix[i,v])
                        {
                            bliski.Add(v);                 
                        }
                        else
                        {
                            potencijalnoBliski.Add(v);
                        }
                    }

                   if(potencijalnoBliski != null)
                   {
                        for(int k = 0; k < potencijalnoBliski.Count-1; ++k)
                        {
                            if (GetMaxRatio(i, k, potencijalnoBliski, distanceMatrix, p))
                            {
                                // provjeri je li tocka vec oznacena
                                if (NijeOznacena(i, potencijalnoBliski[k], selected))
                                {
                                    bliski.Add(potencijalnoBliski[k]);
                                    
                                }
                                if (NijeOznacena(i,  potencijalnoBliski[k+1], selected))
                                {
                                    bliski.Add(potencijalnoBliski[k + 1]);
                                }

                            }
                        }
                        selected.Add(i, bliski.ToArray());
                    }
                   
                }
               
            }
            ChangeDistance(distanceMatrix, selected);
            return distanceMatrix;
        }


        //napravi matricu haversine udaljenosti
        public double[,] HaversineDistanceMatrix(double[,] locations)
        {
            // 
            int N = locations.GetLength(0);
            double[,] distanceMatrix = new double[N, N];

            for (int row = 0; row < N; ++row)
            {
                distanceMatrix[row, row] = 0;
                for (int rowElement = row + 1; rowElement < N; ++rowElement)
                {
                    // Simetrična matrica 
                    double distance = Calculate(locations[row, 0], locations[row, 1], locations[rowElement, 0], locations[rowElement, 1]);
                    distanceMatrix[row, rowElement] = (long)distance;
                    distanceMatrix[rowElement, row] = (long)distance;
                }
            }
            return distanceMatrix;
        }
        public static double Calculate(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6372.8; // In kilometers
            var dLat = toRadians(lat2 - lat1);
            var dLon = toRadians(lon2 - lon1);
            lat1 = toRadians(lat1);
            lat2 = toRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Asin(Math.Sqrt(a));
            return R * c * 1000; // u metrima
        }
        private static double toRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }


        //treba vratit indekse od M najmanjih vrijednosti za svaki stupac
        public Dictionary<int, List<int>> GetMValues(double[,] distanceMatrix, int M)
        {
            Dictionary<int, List<int>> linked = new Dictionary<int, List<int>>();

           // Dictionary<int, int[]> dict = new Dictionary<int, int[]>();
            for (int i = 0; i < distanceMatrix.GetLength(0); ++i)
            {
                Dictionary<int, double> dictionaryByColumn = new Dictionary<int, double>();
                dictionaryByColumn = GetColumn(distanceMatrix, i);
                List<int> listaPovezanih = new List<int>();
                var orderedLista = dictionaryByColumn.OrderBy(k => k.Value);
                //poredaj po udaljenostima, uzmi M najbližih
                int j = 0;
                foreach (var item in orderedLista)
                {
                    if (j < M) {

                        if (item.Value != 0)
                        {
                            listaPovezanih.Add(item.Key);
                            ++j;
                        }
                    }
                    
                }

                linked.Add(i, listaPovezanih);
            }
            return linked;
        }

        //vraća svaki stupac kao dictionary, key:indeks, value:vrijednost
        private Dictionary<int, double> GetColumn(double[,] distanceMatrix, int column)
        {
            Dictionary<int, double> dict = new Dictionary<int, double>();
            for (int i = 0; i < distanceMatrix.GetLength(0); ++i)
            {
                dict.Add(i, distanceMatrix[i, column]);
            }
            return dict;
        }

        //provjeri je li tocka vec oznacena
        public bool NijeOznacena (int i, int j, Dictionary<int, int[]> selected) 
        {
            foreach(var item in selected)
            {
                if(item.Key != i)
                {
                    var val = item.Value.ToArray();
                    if (val.Contains(j))
                    {
                        return false;
                    }
                }
                      
            }
            return true;
        }

        //provjera omjera
        public bool GetMaxRatio(int i, int k, List<int> potencijalnoBlizu, double[,] distanceMatrix, double p)
        {
            
                double o1 = distanceMatrix[i, potencijalnoBlizu[k]] / distanceMatrix[i, potencijalnoBlizu[k + 1]];
                double o2 = distanceMatrix[i, potencijalnoBlizu[k +1]] / distanceMatrix[i, potencijalnoBlizu[k]];


                if (o1 > 1 && o1 < 1 + p)
                {
                    return true;
                }
                else if (o2 > 1 && o2 < 1 + p)
                {
                    return true;
                }
                return false;
            

        }

        //pozvati ors i promijeniti udaljenost 
        public async void ChangeDistance(double[,] distanceMatrix, Dictionary<int, int[]> selectedPairs)
        {
            
            foreach(var item in selectedPairs)
            {
                bool prvi = true;
                double min = 0;
                //double dmin = distanceMatrix[item.Key, item.Value[0]];

                foreach (int value in item.Value)
                {          
                             
                    if (prvi)
                    {
                        DataModelORS openRouteService = new DataModelORS(new double[,] { { Nodes[value].Longitude, Nodes[value].Latitude }, { Nodes[item.Key].Longitude, Nodes[item.Key].Latitude } });
                        min = (await openRouteService.DistanceMatrix())[0, 1];      
                        if(min<distanceMatrix[item.Key, value])
                        {
                            distanceMatrix[item.Key, value] = min;
                            prvi = false;
                        }
                       
                        
                    }
                    else
                    {
                        if(distanceMatrix[item.Key, value] < min)
                        {
                            DataModelORS openRouteService = new DataModelORS(new double[,] { { Nodes[value].Longitude, Nodes[value].Latitude }, { Nodes[item.Key].Longitude, Nodes[item.Key].Latitude } });
                            min = (await openRouteService.DistanceMatrix())[0, 1];
                            distanceMatrix[item.Key, value] = min;                           

                        }
                    }
                    
                }                
            }
            
        }


    }

    public class NodeJ
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
