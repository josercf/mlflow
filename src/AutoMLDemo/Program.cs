using System;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;

namespace AutoMLDemo
{
    public class MovieRating
    {
        [LoadColumn(0)] public float UserId { get; set; }
        [LoadColumn(1)] public float MovieId { get; set; }
        [LoadColumn(2)] public float Label { get; set; }
        // Inclua outras colunas de atributos do dataset conforme necessidade
    }

    class Program
    {
        static void Main(string[] args)
        {
            // 1. Iniciar MetricServer para expor métricas Prometheus
            MetricServer.Start(port: 5000);

            // 2. Criar MLContext
            var mlContext = new MLContext(seed: 0);

            // 3. Carregar dados
            string datasetPath = "dataset/movies.csv"; // ajuste conforme localização real do CSV
            IDataView data = mlContext.Data.LoadFromTextFile<MovieRating>(
                path: datasetPath,
                hasHeader: true,
                separatorChar: ',');

            // 4. Dividir em treino e teste
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

            // 5. Configurar experimento AutoML para regressão
            var experimentSettings = new RegressionExperimentSettings
            {
                MaxExperimentTimeInSeconds = 60,
                OptimizingMetric = RegressionMetric.RSquared
            };
            var experiment = mlContext.Auto().CreateRegressionExperiment(experimentSettings);

            Console.WriteLine("Iniciando experimento AutoML para regressão...");
            var result = experiment.Execute(trainData: split.TrainSet, labelColumnName: "Label");
            Console.WriteLine($"Melhor modelo encontrado: {result.BestRun.TrainerName}");
            Console.WriteLine($"Métricas no conjunto de treino: R² = {result.BestRun.ValidationMetrics.RSquared:F4}, RMSE = {result.BestRun.ValidationMetrics.RootMeanSquaredError:F4}");

            // 6. Avaliar melhor modelo no conjunto de teste
            var model = result.BestRun.Model;
            var predictions = model.Transform(split.TestSet);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label");

            Console.WriteLine($"Avaliação no conjunto de teste: R² = {metrics.RSquared:F4}, RMSE = {metrics.RootMeanSquaredError:F4}");

            // 7. Atualizar métricas Prometheus
            MetricServer.SetMetrics(r2: metrics.RSquared, rmse: metrics.RootMeanSquaredError);

            // 8. Fazer uma predição de exemplo
            var predictionEngine = mlContext.Model.CreatePredictionEngine<MovieRating, MovieRatingPrediction>(model);
            var sample = new MovieRating { UserId = 1, MovieId = 10 };
            var prediction = predictionEngine.Predict(sample);
            Console.WriteLine($"Predição de nota para usuário 1 no filme 10: {prediction.Score:F4}");

            Console.WriteLine("Pressione qualquer tecla para encerrar...");
            Console.ReadKey();
        }
    }

    public class MovieRatingPrediction
    {
        public float Score { get; set; }
    }
}
