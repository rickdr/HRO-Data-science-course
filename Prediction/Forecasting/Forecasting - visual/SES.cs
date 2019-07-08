using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecasting
{
    class SES
    {
        private DataTable DataSet;
        private double StandardError = -1;
        private const int PredictionPeriod = 12;
        private double Alpha = 0.5;
        private double OptimalAlpha;

        public SES(DataTable dataSet)
        {
            DataSet = dataSet;
        }

        public void Execute()
        {
            CalculateAverage();
            for (Alpha = 0; Alpha < 1; Alpha += 0.02f)
            {
                CalculateLevelEstimate(Alpha);
                CalculateSSE(Alpha);
            }
            CalculateLevelEstimate(OptimalAlpha);

            Forecast();
        }

        private void CalculateSSE(double alpha)
        {
            var sse = DataSet.AsEnumerable().Sum(x => x.Field<double>("Squared Error"));
            var standardError = Math.Sqrt(sse/(DataSet.Rows.Count - 2));
            if (StandardError == -1 || standardError < StandardError)
            {
                StandardError = standardError;
                OptimalAlpha = alpha;
            }
        }

        private void Forecast()
        {
            var lastLevelEstimate = DataSet.Rows[DataSet.Rows.Count - 1]["Level Estimate"];
            var lastT = Convert.ToInt32(DataSet.Rows[DataSet.Rows.Count - 1]["t"]);
            for (int i = 0; i < PredictionPeriod; i++)
            {
                var row = DataSet.NewRow();
                row["t"] = lastT + i;
                row["Forecast"] = lastLevelEstimate;
                DataSet.Rows.Add(row);
            }
        }

        private void CalculateLevelEstimate(double alpha)
        {
            for (int i = 1; i < DataSet.Rows.Count; i++)
            {

                var row = DataSet.Rows[i];
                var oneStepForecast = Convert.ToDouble(DataSet.Rows[i - 1]["Level Estimate"]);
                var foreCastError = Convert.ToDouble(DataSet.Rows[i]["Demand"]) - oneStepForecast;
                var levelEstimate = oneStepForecast + alpha*foreCastError;

                row["Forecast"] = oneStepForecast;
                row["Forecast Error"] = foreCastError;
                row["Level Estimate"] = levelEstimate;
                row["Squared Error"] = Math.Pow(foreCastError, 2);
            }
        }

        private void CalculateAverage()
        {
            int total = 0;
            for (int i = 1; i <= PredictionPeriod; i++)
            {
                total += Convert.ToInt32(DataSet.Rows[i]["Demand"]);
            }
            DataSet.Rows[0]["Level Estimate"]=total/PredictionPeriod;
        }
    }
}
