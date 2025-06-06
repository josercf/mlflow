using Prometheus;

namespace AutoMLDemo
{
    public static class MetricsRegistry
    {
        // Métricas de predição
        public static readonly Histogram PredictionLatency = Metrics.CreateHistogram(
            "prediction_latency_seconds", 
            "Tempo para fazer uma predição");
            
        public static readonly Counter PredictionCounter = Metrics.CreateCounter(
            "predictions_total", 
            "Total de predições realizadas");
            
        public static readonly Gauge AccuracyByRange = Metrics.CreateGauge(
            "accuracy_by_rating_range", 
            "Acurácia por faixa de rating", 
            "range");

        // Métricas do modelo
        public static readonly Gauge R2Gauge = Metrics.CreateGauge("model_r2", "R-squared do modelo (.NET)");
        public static readonly Gauge RmseGauge = Metrics.CreateGauge("model_rmse", "RMSE do modelo (.NET)");
    }
}
