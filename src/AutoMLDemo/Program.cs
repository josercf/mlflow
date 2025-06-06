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
    }

    public class MovieRatingPrediction
    {
        public float Score { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MetricServer.Start(port: 5000);

            var mlContext = new MLContext(seed: 0);

            string datasetPath = "dataset/movies.csv";
            IDataView data = mlContext.Data.LoadFromTextFile<MovieRating>(
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

            Console.WriteLine("Iniciando experimento AutoML para regressao...");
            var result = experiment.Execute(trainData: split.TrainSet, labelColumnName: "Label");
            Console.WriteLine($"Melhor modelo: {result.BestRun.TrainerName}");
            Console.WriteLine($"R\u00b2 treino: {result.BestRun.ValidationMetrics.RSquared:F4}, RMSE treino: {result.BestRun.ValidationMetrics.RootMeanSquaredError:F4}");

            var model = result.BestRun.Model;
            var predictions = model.Transform(split.TestSet);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label");

            Console.WriteLine($"R\u00b2 teste: {metrics.RSquared:F4}, RMSE teste: {metrics.RootMeanSquaredError:F4}");

            MetricServer.SetMetrics(r2: metrics.RSquared, rmse: metrics.RootMeanSquaredError);

            var predictionEngine = mlContext.Model.CreatePredictionEngine<MovieRating, MovieRatingPrediction>(model);
            var sample = new MovieRating { UserId = 1, MovieId = 10 };
            var prediction = predictionEngine.Predict(sample);
            Console.WriteLine($"Predicao para usuario 1 no filme 10: {prediction.Score:F4}");

            Console.WriteLine("Pressione qualquer tecla para encerrar...");
            Console.ReadKey();
        }
    }
}
