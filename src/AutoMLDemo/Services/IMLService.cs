using Microsoft.ML;

namespace AutoMLDemo
{
    // 5. Serviço para ML.NET
    public interface IMLService
    {
        Task<(ITransformer model, PredictionEngine<RatingInput, RatingPrediction> engine)> InitializeAsync();
        PredictionResponse Predict(PredictionRequest request, ITransformer model, PredictionEngine<RatingInput, RatingPrediction> engine);
    }
}