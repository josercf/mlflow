using System;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using Prometheus;

namespace AutoMLDemo
{
    // 1. Classe que representa cada registro de ratings.csv
    //    Coluna 0 → userId (float), Coluna 1 → movieId (float), Coluna 2 → Label (float rating), 
    //    Coluna 3 (timestamp) vamos ignorar para este experimento.
    public class RatingInput
    {
        [LoadColumn(0)]
        public float UserId { get; set; }

        [LoadColumn(1)]
        public float MovieId { get; set; }

        [LoadColumn(2)]
        public float Label { get; set; }
    }

    // 2. Classe para armazenar a predição
    public class RatingPrediction
    {
        public float Score { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // 1. Iniciar servidor de métricas Prometheus
            //    Observe que usamos o tipo completo para não confundir com nossa classe MetricServer
            var metricServer = new Prometheus.MetricServer(port: 5000);
            metricServer.Start();

            // 2. Criar MLContext
            var mlContext = new MLContext(seed: 0);

            // 3. Definir caminho do CSV (será copiado para bin/.../dataset/ratings.csv)
            string datasetPath = "src/AutoMLDemo/dataset/ratings.csv";  
            Console.WriteLine($"Carregando dados de: {datasetPath}");

            // 4. Carregar o arquivo ratings.csv
            IDataView data = mlContext.Data.LoadFromTextFile<RatingInput>(
                path: datasetPath,
                hasHeader: true,
                separatorChar: ',');

            // 5. Dividir em treino e teste (80% treino, 20% teste)
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

            // 6. Configurar experimento AutoML para REGRESSÃO
            var experimentSettings = new RegressionExperimentSettings
            {
                MaxExperimentTimeInSeconds = 60,
                OptimizingMetric = RegressionMetric.RSquared
            };
            var experiment = mlContext.Auto().CreateRegressionExperiment(experimentSettings);

            Console.WriteLine("Iniciando experimento AutoML para regressão...");
            var result = experiment.Execute(
                trainData: split.TrainSet, 
                labelColumnName: nameof(RatingInput.Label));

            Console.WriteLine($"Melhor modelo encontrado: {result.BestRun.TrainerName}");
            Console.WriteLine($"Métricas no treino: R² = {result.BestRun.ValidationMetrics.RSquared:F4}, " +
                              $"RMSE = {result.BestRun.ValidationMetrics.RootMeanSquaredError:F4}");

            // 7. Avaliar o melhor modelo no conjunto de teste
            var model = result.BestRun.Model;
            var predictions = model.Transform(split.TestSet);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label");

            Console.WriteLine($"Avaliação no teste: R² = {metrics.RSquared:F4}, RMSE = {metrics.RootMeanSquaredError:F4}");

            // 8. Expor métricas para o Prometheus
            var r2Gauge = Metrics.CreateGauge("model_r2", "R-squared do modelo (.NET)");
            var rmseGauge = Metrics.CreateGauge("model_rmse", "RMSE do modelo (.NET)");
            r2Gauge.Set(metrics.RSquared);
            rmseGauge.Set(metrics.RootMeanSquaredError);

            // 9. Fazer uma predição de exemplo para ver se tudo funciona
            var predictionEngine = mlContext.Model.CreatePredictionEngine<RatingInput, RatingPrediction>(model);
            var sample = new RatingInput { UserId = 1, MovieId = 10 };
            var pred = predictionEngine.Predict(sample);
            Console.WriteLine($"Predição de rating para userId=1, movieId=10: {pred.Score:F4}");

            Console.WriteLine("Pressione qualquer tecla para encerrar...");
            Console.ReadKey();
        }
    }
}