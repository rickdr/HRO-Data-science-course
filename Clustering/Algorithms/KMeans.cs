using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Clustering.Algorithms
{
    internal class KMeans
    {
        private readonly int ClusterAmount;

        public KMeans(int k)
        {
            ClusterAmount = k;
        }

        /// <summary>
        /// This method creates all initial clusters/centroids at random.
        /// </summary>
        /// <returns>Centroid locations for 4 cluster in 32 dimensions</returns>
        public DataTable CreateClusters()
        {
            var rand = new Random();
            var clusterLocations = new DataTable();

            clusterLocations.Columns.Add("Offer");
            for (var i = 1; i <= ClusterAmount; i++)
                clusterLocations.Columns.Add("Cluster " + i);
            
            // We need to create a centroid position for each dimension, in this case there are 32 dimensions.
            for (var i = 1; i <= 32; i++)
            {
                var row = clusterLocations.NewRow();
                row[0] = i;

                // Generate random centroid position for each cluster
                for (var j = 1; j <= ClusterAmount; j++)
                    row[j] = rand.NextDouble();

                clusterLocations.Rows.Add(row);
            }
            return clusterLocations;
        }

        /// <summary>
        /// This method updates centroids for k clusters in all dimensions.
        /// </summary>
        /// <param name="pivot">Binary data of purchases per offer</param>
        /// <param name="distancesTable">Distances from each customer to each cluster and assigned cluster</param>
        /// <returns>A DataTable with the new updated centroids</returns>
        public DataTable UpdateCentroids(DataTable pivot, DataTable distancesTable)
        {
            DataTable clusterLocations = new DataTable();
            clusterLocations.Columns.Add("Offer");
            for (int i = 1; i <= ClusterAmount; i++)
                clusterLocations.Columns.Add("Cluster " + i);

            int offerCounter = 0;
            foreach (var offer in pivot.AsEnumerable())
            {
                DataRow row = clusterLocations.NewRow();
                float[] locations = new float[ClusterAmount];
                for (int cluster = 1; cluster <= ClusterAmount; cluster++)
                {
                    //Get all customers assigned to cluster k
                    List<DataRow> assignedUserDistances =
                        distancesTable.AsEnumerable()
                            .Where(s => s.Field<string>(distancesTable.Columns.Count - 1) == cluster.ToString())
                            .ToList();

                    //Get only the names of customers that are assigned to cluster k
                    var names = assignedUserDistances.AsEnumerable().Select(s => s.Field<string>("Customer"));
                    float totalBought = 0;

                    foreach (var name in names)
                        if (offer.Field<string>(name).Equals("1"))
                            totalBought++;

                    if (names.Count() == 0) continue;

                    // New centroid position is the number of purchases for the offer for cluster k, 
                    // divided by the total amount of customers assigned to the cluster.
                    row[cluster] = totalBought/names.Count();
                }
                offerCounter++;
                row[0] = offerCounter;
                clusterLocations.Rows.Add(row);
            }
            return clusterLocations;
        }

        private DataTable CreateDataTable (int length)
        {
            var table = new DataTable();
            table.Columns.Add("Customer");
            for (var i = 0; i < length; i++)
                table.Columns.Add("Cluster " + (i + 1));

            table.Columns.Add("Assigned Cluster");
            return table;
        }

        /// <summary>
        ///     Calculate the distance between every customer and the cluster locations.
        /// </summary>
        /// <param name="pivot">DataTable containing the purchases for every customer</param>
        /// <param name="clusterLocations">The locations of the cluster</param>
        public DataTable CalculateDistanceBetween (DataTable pivot, DataTable clusterLocations)
        {
            DataTable distancesTable = CreateDataTable(ClusterAmount);

            //Loop through all customers
            for (int i = 1; i < pivot.Columns.Count; i++)
            {
                //Retrieve the Column with the purchases made by this customer
                List<string> purchases = pivot.AsEnumerable().Select(s => s.Field<String>(pivot.Columns[i])).ToList();

                DataRow newDistancerow = distancesTable.NewRow();
                newDistancerow[0] = pivot.Columns[i];

                double smallestClusterDistance = 100;
                int assignedCluster = 0;

                // Loop through all clusters, j is the current cluster
                for (int j = 0; j < ClusterAmount; j++)
                {
                    double clusterDistance = 0;

                    int counter = 0;
                    foreach (var purchase in purchases)
                    {
                        DataRow clusterLocationForOffer = clusterLocations.Rows[counter];

                        //Euclidian Sum
                        int purchaseValue = purchase.Equals("1") ? 1 : 0;
                        var clusterPosition = clusterLocationForOffer[j + 1];

                        var clusterOffer = clusterLocationForOffer[j + 1] == DBNull.Value ? 0 : clusterLocationForOffer[j + 1];
                        double k = purchaseValue - float.Parse(clusterOffer.ToString());
                        clusterDistance += Math.Pow(k, 2);
                        counter++;
                    }
                    //Euclidian Squareroot
                    clusterDistance = Math.Sqrt(clusterDistance);

                    newDistancerow[j + 1] = clusterDistance;

                    //Assign to nearest cluster
                    if (clusterDistance < smallestClusterDistance)
                    {
                        smallestClusterDistance = clusterDistance;
                        assignedCluster = j + 1;
                    }
                }
                newDistancerow[distancesTable.Columns.Count - 1] = assignedCluster;
                distancesTable.Rows.Add(newDistancerow);
            }
            return distancesTable;
        }

        public float CalculateTotalDistance (DataTable distancesTable)
        {
            float totalDistance = 0;
            for (var i = 1; i <= ClusterAmount; i++)
            {
                //Select all Customers assigned to cluster k
                var distances =
                    distancesTable.AsEnumerable()
                        .Where(s => s.Field<String>(distancesTable.Columns.Count - 1) == i.ToString())
                        .ToList();
                foreach (var distance in distances)
                {
                    totalDistance += float.Parse(distance[i].ToString());
                }
            }
            return totalDistance;
        }

        /// <summary>
        /// Method to calculate the SSE (Sum Of Squared Errors)
        /// </summary>
        /// <param name="pivot">Binary purchase data</param>
        /// <param name="clusterLocations">Locations of centroid in k clusters for all dimensions</param>
        /// <param name="distancesTable">Distances from customers to clusters</param>
        /// <returns>SSE (Sum of Squared Errors)</returns>
        public float CalculateSSE(DataTable pivot, DataTable clusterLocations, DataTable distancesTable)
        {
            float sse = 0;
            for (var cluster = 1; cluster <= ClusterAmount; cluster++)
            {
                // Get the names of all customers that are assigned to the current cluster
                var assignedUserDistances =
                    distancesTable.AsEnumerable()
                        .Where(s => s.Field<string>(distancesTable.Columns.Count - 1) == cluster.ToString())
                        .ToList();
                var names = assignedUserDistances.AsEnumerable().Select(s => s.Field<string>("Customer"));

                // Loop through all records (offers).
                for (var i = 0; i < pivot.Rows.Count; i++)
                {
                    var offer = pivot.Rows[i];
                    var clusterLocation = float.Parse(clusterLocations.Rows[i][cluster].ToString());
                    foreach (var name in names)
                    {
                        float customerPosition = offer.Field<string>(name).Equals("1") ? 1 : 0;
                            
                        sse += (float) Math.Pow(clusterLocation - customerPosition, 2);
                    }
                }
            }
            return sse;
        }
    }
}