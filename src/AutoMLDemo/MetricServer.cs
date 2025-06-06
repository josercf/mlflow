using System;
using Prometheus;

namespace AutoMLDemo
{
    public static class MetricServer
    {
        private static readonly Gauge _r2Gauge   = Metrics.CreateGauge("model_r2",   "R-squared do modelo (.NET)");
        private static readonly Gauge _rmseGauge = Metrics.CreateGauge("model_rmse", "RMSE do modelo (.NET)");

        public static void Start(int port = 5000)
        {
            // Aqui referenciamos explicitamente o tipo Prometheus.MetricServer
            var server = new Prometheus.MetricServer(port);
            server.Start();
        }

        public static void SetMetrics(double r2, double rmse)
        {
            _r2Gauge.Set(r2);
            _rmseGauge.Set(rmse);
        }
    }
}