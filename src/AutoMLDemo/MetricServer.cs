using Prometheus;

namespace AutoMLDemo
{
    public static class MetricServer
    {
        private static readonly Gauge R2Gauge = Metrics.CreateGauge("model_r2", "R-squared do modelo (.NET)");
        private static readonly Gauge RmseGauge = Metrics.CreateGauge("model_rmse", "RMSE do modelo (.NET)");

        public static void Start(int port = 5000)
        {
            var server = new KestrelMetricServer(port: port);
            server.Start();
        }

        public static void SetMetrics(double r2, double rmse)
        {
            R2Gauge.Set(r2);
            RmseGauge.Set(rmse);
        }
    }
}
