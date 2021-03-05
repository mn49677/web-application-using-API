using System;
using System.Collections.Generic;
using System.Text;
using GDi_VRP_Models.Models;
using GDi_VRP_Models.Models.Routes;
using GDi_VRP_Models.Models.Orders;
using GDi_VRP_Models.Models.Options;
using GDi_VRP_Models.Models.Depots;
using GDi_VRP_Models;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.Design;
using Google.Protobuf.WellKnownTypes;

namespace ORTools
{
    public class DataMapper_GDiVRP : IORToolsMapper<SchedulerVRPRequestModel>
    {
        public DataModel Map(SchedulerVRPRequestModel model)
        {
            
            DataModel dataModel = new DataModel()
            {
                VehicleNumber = model.Routes.Count,
                TimeWindows = model.Orders.Select(x => new[,]
                 {
                    { x.attributes.TimeWindowStart1.GetValueOrDefault(),
                        x.attributes.TimeWindowEnd1.GetValueOrDefault() },
                }).To2DDateTimeArray<long>(),
                VehicleLocationUnloadTime = model.Orders.Select(x => Convert.ToInt64(x.attributes.ServiceTime.GetValueOrDefault())).ToArray(),
                DemandsByType = model.Orders.Select(x => x.attributes.DeliveryQuantities).ToArray().ToJaggedArray<long>(),
                VehicleCapacitiesByType = model.Routes.Select(x => x.attributes.Capacities).ToArray().ToJaggedArrayByType<long>(),
                SpecialtyNamesVehicles = model.Routes.Select(x => x.attributes.SpecialtyNames).ToArray().ToJaggedArrayString<long>(),
                SpecialtyNamesLocations = model.Orders.Select(x => x.attributes.SpecialtyNames).ToArray().ToJaggedArrayString<long>(),
                Locations = model.Orders.Select(x => new[,]
                {
                    { x.geometry.x, x.geometry.y },
                }).To2DArray<Double>(),
                LocationNames = model.Orders.Select(x => x.attributes.Name).ToArray(),
                VehicleCapacities = model.Routes.Select(x => x.attributes.Capacities).ToArray().ToSumLong<long>(),
               // DepotCapacity = 10000000,
                VehicleWorkingHours = model.Routes.Select(x => x.attributes.MaxTotalTime.GetValueOrDefault()).ToArray(),
                VehicleStartingTimes = model.Routes.Select(x => new[,]
                {
                    { x.attributes.EarliestStartTime,
                    x.attributes.LatestStartTime }
                }).To2DDateTimeArray<long>()
            };

            return dataModel;
        }
    }

    //pomocne metode za mapiranje
    public static class LINQExtensions
    {
        //finds min starting time
        public static DateTime GetMinimumStartingTime(IEnumerable<long[,]> source)
        {
            int length = source.First().Length;
            DateTime[] earliestStartDateTimes = new DateTime[source.Count()];
            for (int i = 0; i < source.Count(); i++)
            {
                var array = source.ElementAt(i);
                for (int j = 0; j < length; j++)
                {
                    var time = array[0, j];
                    time = time / 1000;
                    System.DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dateTime = dateTime.AddSeconds(time).ToLocalTime();
                    if (j == 0)
                    {
                        earliestStartDateTimes[i] = dateTime;
                    }
                }
            }
            // nadi najmanju vrijednost ----> referentna tocka
            var minimumStartTime = earliestStartDateTimes.Min();
            return minimumStartTime;
        }

        public static long[,] To2DDateTimeArray<Int64>(this System.Collections.Generic.IEnumerable<long[,]> source)
        {
           
            int length = source.First().Length;
            long[,] ret = new long[source.Count(), length];
            DateTime[,] startTimes = new DateTime[source.Count(), length];
            
            for (int i = 0; i < source.Count(); i++)
            {
                var array = source.ElementAt(i);
                for(int j = 0; j< length; j++)
                {
                    var time = array[0, j];
                    time = time / 1000;
                    System.DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dateTime = dateTime.AddSeconds(time).ToLocalTime();
                    startTimes[i, j] = dateTime;
                }
            }
            // nadi najmanju vrijednost ----> referentna tocka
            var minimumStartTime = GetMinimumStartingTime(source);

            //od svakog time windowa, oduzeti referentnu tocku

            for(int i = 0; i< source.Count(); i++)
            {
                var array = source.ElementAt(i);
                for(int j = 0; j < length; j++)
                {
                    ret[i,j] = Convert.ToInt64((startTimes[i, j] - minimumStartTime).TotalMinutes);
                }

            }

            return ret;
        }

        public static double[,] To2DArray<Double>(this System.Collections.Generic.IEnumerable<double[,]> source)
        {
            int length = source.First().Length;
            double[,] ret = new double[source.Count(), length];
            for (int i = 0; i < source.Count(); i++)
            {
                var array = source.ElementAt(i);
                for (int j = 0; j < length; j++)
                {
                    ret[i, j] = array[0, j];
                }
            }
            return ret;
        }

        public static long[][] ToJaggedArray<Int64>(this string[] source)
        {
            String[] type;
            long[][] jaggedArray =new long[source.Length][];
            for(int i = 0; i < source.Length; i++)
            {
                type = source[i].Split(" ");
                jaggedArray[i] = new long[type.Length];
                for (int j = 0; j< type.Length; j++)
                    try
                    {
                        jaggedArray[i][j] = Convert.ToInt64(type[j].Trim());
                    }
                    catch (FormatException exc)
                    {
                        continue;
                    }
            }

            return jaggedArray;
        }

        // polje u kojem su retci razlicite vrste kapaciteta, a stupci su vozila sa kapacitetima za te vrste
        public static long[][] ToJaggedArrayByType<Int64>(this string[] source)
        {
            String[] type;
            type = source[0].Split(" ");
            long[][] jaggedArray = new long[type.Length][];
            for (int k = 0; k < type.Length; k++)
            {
                jaggedArray[k] = new long[source.Length];
            }
            //iterira po broju vozila - 97
            for (int i = 0; i < source.Length; i++)
            {
                type = source[i].Split(" ");
                for (int j = 0; j < type.Length; j++)
                    try
                    {
                        jaggedArray[j][i] = Convert.ToInt64(type[j].Trim());
                    }
                    catch (FormatException exc)
                    {
                        continue;
                    }
            }
            return jaggedArray;
        }

        //specialty names za lokacije, npr : "10 12 14 CREW LDC_Zagreb"
        //problem : neke lokacije imaju " 12 LDC_Zagreb"
        public static string[][] ToJaggedArrayString<Int64>(this string[] source)
        {
            String[] type;
            Boolean hasCrew = false;
            string[][] jaggedArray = new string[source.Length][];
            for (int i = 0; i < source.Length; i++)
            {
                type = source[i].Trim().Split(" ");
                type = type.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                jaggedArray[i] = new string[type.Length];
                for (int j = 0; j < type.Length; j++)
                {

                    if (type[j].Equals("CREW"))
                    {
                        Array.Resize(ref jaggedArray[i], jaggedArray[i].Length - 1);
                        hasCrew = true;
                        continue;
                    }
                    else
                    {
                        if (hasCrew)
                        {
                            jaggedArray[i][j - 1] = type[j].Trim();
                        }
                        else
                        {
                            jaggedArray[i][j] = type[j].Trim();
                        }
                    }
                }
                hasCrew = false;
               
            }

            return jaggedArray;
        }

        //ukupni kapacitet vozila - zbraja kapacitete po tipovima
        public static long[] ToSumLong<Int64>(this string[] source)
        {
            long[] sumOfCapacities = new long[source.Count()];
            string[] capacities = new string[2];
            for (int i = 0; i < source.Count(); i++)
            {
                capacities = source[i].Split(" ");
                for (int j = 0; j < capacities.Length; j++)
                {
                    sumOfCapacities[i] += Convert.ToInt32(capacities[j]);
                }
            }
            return sumOfCapacities;
        }
    }
   }


