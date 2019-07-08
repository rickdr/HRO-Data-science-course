using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Clustering.Algorithms;
using Clustering.Reader;

namespace Clustering
{
    class Program
    {
        private static KMeans KMeansAlgorithm;
        private static int k = 5;

        public static String TransactionLocation = "../Assets/Transaction.csv";
        public static String OfferLocation = "../Assets/OfferInformation.csv";
        public static String Pivot = "../../Assets/Pivot.csv";

        public const int Transaction = 1;
        public const int Offer = 2;

        static void Main(string[] args)
        {
            FileReader reader = new FileReader();
            DataTable pivot = reader.ReadDataFromFile(Pivot);

            //Create initial centroids
            KMeansAlgorithm = new KMeans(k);
            DataTable clusterLocations = KMeansAlgorithm.CreateClusters();

            //Compute distances from customers to clusters
            DataTable distancesTable = KMeansAlgorithm.CalculateDistanceBetween(pivot, clusterLocations);

            double totalDistance = 0.0f;

            // Keep on updating centroid location and customer distances until the total distance doesn't change anymore
            while(true){
                
                clusterLocations = KMeansAlgorithm.UpdateCentroids(pivot, distancesTable);

                distancesTable = KMeansAlgorithm.CalculateDistanceBetween(pivot, clusterLocations);

                if (totalDistance == KMeansAlgorithm.CalculateTotalDistance(distancesTable))
                {
                    break;
                }
                else
                {
                    totalDistance = KMeansAlgorithm.CalculateTotalDistance(distancesTable);
                }
            }
            //printDataTable(distancesTable);

            float sseval = KMeansAlgorithm.CalculateSSE(pivot,clusterLocations,distancesTable);

            Silhouette silhouette = new Silhouette();
            DataTable customerDistances = silhouette.CalculateCustomerDistances(pivot);

            double silhoutteval = silhouette.CalculateSilhoutte(customerDistances, distancesTable, k);

            TopDeals topDeals = new TopDeals();
            List<DataTable> topDealsList = new List<DataTable>();
            for (var i = 1; i <= k; i++)
            {
                DataTable test = topDeals.CalculateTopDeals(pivot, distancesTable, i);
                topDealsList.Add(test);
            }

            Form1 form = new Form1();
            form.setDataSource(topDealsList);
            Application.EnableVisualStyles();
            Application.Run(form); // or whatever

            topDealsList.ForEach(table => printDataTable(table));

            Console.ReadKey();
        }
        
        static void printDataTable (DataTable table)
        {
            string data = string.Empty;
            StringBuilder sb = new StringBuilder();

            if (null != table && null != table.Rows)
            {
                foreach (DataRow dataRow in table.Rows)
                {
                    foreach (var item in dataRow.ItemArray)
                    {
                        sb.Append(item);
                        sb.Append(',');
                    }
                    sb.AppendLine();
                }

                data = sb.ToString();
            }
            Console.WriteLine(data);
            //return data;
        }
    }
}
