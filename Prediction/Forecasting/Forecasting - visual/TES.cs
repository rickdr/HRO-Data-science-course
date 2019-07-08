using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecasting
{
    class TES
    {
        private readonly DataTable DataSet;
        private const int PredictionPeriod = 12;
        private double OptimalAlpha, OptimalGamma;
        private double StandardError = -1;
        private double Level = 144.42;
        private double Trend = 2.2095;
        private double Delta = 0;
        private double SSE = 0;
        private List<double> Deltas;

        public TES (DataTable dataSet, List<double> deltas)
        {
            DataSet = dataSet;
            Deltas = deltas;
            var firstRow = DataSet.Rows[0];
            firstRow["Level Estimate"] = Level;
            firstRow["Trend"] = Trend;
            firstRow["Seasonal Adjustment"] = Deltas.First();
        }

        public void Execute ()
        {
            for (float gamma = 0.01f; gamma < 1; gamma += 0.01f)
            {
                for (float alpha = 0.01f; alpha < 1; alpha += 0.01f)
                {
                    CalculateLevelEstimate(alpha, gamma, Delta);
                    CalculateSSE(alpha, gamma, Delta);
                }
            }

            CalculateLevelEstimate(OptimalAlpha, OptimalGamma, 0);
            Forecast();
        }

        private void CalculateSSE (double alpha, double gamma, double Delta)
        {
            var sse = DataSet.AsEnumerable().Sum(x => x.Field<double>("Squared Error"));
            var standardError = Math.Sqrt(sse / (DataSet.Rows.Count - 3));
            if ((StandardError == -1 || standardError < StandardError) && standardError > 0)
            {
                StandardError = standardError;
                OptimalAlpha = alpha;
                OptimalGamma = gamma;
            }
        }

        private void Forecast ()
        {
            var lastRow = DataSet.Rows[DataSet.Rows.Count - 1];
            var lastT = (double)lastRow["t"];
            var lastLevelEstimate = (double)lastRow["Level Estimate"];
            var lastTrend = (double)lastRow["Trend"];
            for (int i = 1; i <= PredictionPeriod; i++)
            {
                var newRow = DataSet.NewRow();
                newRow["t"] = lastT + i;
                newRow["Forecast"] = lastLevelEstimate + i * lastTrend;
                DataSet.Rows.Add(newRow);
            }
        }

        private void CalculateLevelEstimate (double alpha, double gamma, double delta)
        {
            for (int i = 1; i < DataSet.Rows.Count; i++)
            {
                var previousRow = DataSet.Rows[i - 1];
                var row = DataSet.Rows[i];

                double previousLevelEstimate = (double)previousRow["Level Estimate"];
                double previousTrend = (double)previousRow["Trend"];
                double previousSeasonal = (double)previousRow["Seasonal Adjustment"];
                double oneStepForecast = (previousLevelEstimate + previousTrend) * previousSeasonal;

                double foreCastError = (double)row["Demand"] - oneStepForecast;
                double levelEstimate = previousLevelEstimate + previousTrend +
                                       alpha * foreCastError / previousSeasonal;

                double trend = previousTrend + gamma * alpha * foreCastError / previousSeasonal;

                double seasonal = previousSeasonal;
                if (i < PredictionPeriod)
                {
                    seasonal = Deltas[i];
                }
                else
                {
                    previousSeasonal = Deltas[(i - PredictionPeriod)];
                    seasonal = previousSeasonal + delta * (1 - alpha) * foreCastError / (levelEstimate + Trend);
                    Deltas.Add(seasonal);
                }
                
                row["Level Estimate"] = levelEstimate;
                row["Trend"] = trend;
                row["Seasonal Adjustment"] = seasonal;
                row["Forecast"] = oneStepForecast;
                row["Forecast Error"] = foreCastError;
                row["Squared Error"] = Math.Pow(foreCastError, 2);
            }
        }
    }
}

