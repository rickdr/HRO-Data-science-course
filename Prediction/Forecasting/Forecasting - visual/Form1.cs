using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Forecasting___visual
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void LoadData(DataTable dataSet, int totalPeriods)
        {
            Debug.WriteLine(Chartline);
            for (int i = 1; i <= totalPeriods; i++)
            {
                var row = dataSet.Rows[i];
                Chartline.Series["Demand"].Points.AddXY
                    ((double)row["t"], (double)row["Demand"]);
            }
            for (int i = totalPeriods+1; i < totalPeriods+12; i++)
            {
                var row = dataSet.Rows[i];
                Chartline.Series["ForeCast"].Points.AddXY
                    ((double)row["t"], (double)row["Forecast"]);
            }
            Chartline.Series["Demand"].ChartType =
                SeriesChartType.FastLine;
            Chartline.Series["Demand"].Color = Color.Blue;

            Chartline.Series["ForeCast"].ChartType =
                SeriesChartType.FastLine;
            Chartline.Series["ForeCast"].Color = Color.Red;
        }
    }
}