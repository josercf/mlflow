using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using Microsoft.Extensions.Logging;
using Prometheus;


namespace AutoMLDemo.Services
{
    public class MLService : IMLService
    {
        private readonly ILogger<MLService> _logger;

        public MLService(ILogger<MLService> logger)
        {
            _logger = logger;
        }

        public async Task<(ITransformer model, PredictionEngine<RatingInput, RatingPrediction> engine)> InitializeAsync()
        {
            _logger.LogInformation("Inicializando modelo ML.NET...");

            var mlContext = new MLContext(seed: 0);
            string datasetPath = "src/AutoMLDemo/dataset/ratings.csv";

            // Carregar e treinar modelo
            IDataView data = mlContext.Data.LoadFromTextFile<RatingInput>(
                path: datasetPath,
                hasHeader: true,
                separatorChar: ',');

            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

            var experimentSettings = new RegressionExperimentSettings
            {
                MaxExperimentTimeInSeconds = 60,
                OptimizingMetric = RegressionMetric.RSquared
            };

            var experiment = mlContext.Auto().CreateRegressionExperiment(experimentSettings);
            
            _logger.LogInformation("Executando experimento AutoML...");
            var result = experiment.Execute(
                trainData: split.TrainSet, 
                labelColumnName: nameof(RatingInput.Label));

            var model = result.BestRun.Model;
            var predictions = model.Transform(split.TestSet);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label");

            // Atualizar métricas do modelo
            MetricsRegistry.R2Gauge.Set(metrics.RSquared);
            MetricsRegistry.RmseGauge.Set(metrics.RootMeanSquaredError);

            var predictionEngine = mlContext.Model.CreatePredictionEngine<RatingInput, RatingPrediction>(model);

            _logger.LogInformation($"Modelo treinado - R²: {metrics.RSquared:F4}, RMSE: {metrics.RootMeanSquaredError:F4}");

            return (model, predictionEngine);
        }

        public PredictionResponse Predict(PredictionRequest request, ITransformer model, PredictionEngine<RatingInput, RatingPrediction> engine)
        {
            DateTime startTime = DateTime.UtcNow;
            
            using (MetricsRegistry.PredictionLatency.NewTimer())
            {
                var sample = new RatingInput 
                { 
                    UserId = request.UserId, 
                    MovieId = request.MovieId 
                };

                var prediction = engine.Predict(sample);
                MetricsRegistry.PredictionCounter.Inc();

                // Calcular faixa de rating e atualizar métrica
                string ratingRange = GetRatingRange(prediction.Score);
                UpdateAccuracyMetrics(ratingRange);

                double latencyMs = (DateTime.UtcNow - startTime).TotalMilliseconds;

                _logger.LogInformation($"Predição - User: {request.UserId}, Movie: {request.MovieId}, Rating: {prediction.Score:F2}, Latência: {latencyMs:F2}ms");

                return new PredictionResponse
                {
                    UserId = request.UserId,
                    MovieId = request.MovieId,
                    PredictedRating = prediction.Score,
                    RatingRange = ratingRange,
                    LatencyMs = latencyMs,
                    TotalPredictions = (long)MetricsRegistry.PredictionCounter.Value
                };
            }
        }

        private string GetRatingRange(float rating)
        {
            return rating switch
            {
                >= 4.0f => "high",      // 4.0-5.0
                >= 3.0f => "medium",    // 3.0-3.9
                >= 2.0f => "low",       // 2.0-2.9
                _ => "very_low"         // 0.0-1.9
            };
        }

        private void UpdateAccuracyMetrics(string range)
        {
            // Simular atualização de acurácia baseada em feedback histórico
            var accuracy = range switch
            {
                "high" => 0.85 + (Random.Shared.NextDouble() * 0.1),      // 85-95%
                "medium" => 0.75 + (Random.Shared.NextDouble() * 0.15),   // 75-90%
                "low" => 0.65 + (Random.Shared.NextDouble() * 0.2),       // 65-85%
                "very_low" => 0.55 + (Random.Shared.NextDouble() * 0.25), // 55-80%
                _ => 0.7
            };
            
            MetricsRegistry.AccuracyByRange.WithLabels(range).Set(accuracy);
        }
    }
}
