using System.Threading.Tasks;
using Microsoft.ML;
using AutoMLDemo.Models;
using AutoMLDemo.DTOs;

namespace AutoMLDemo.Services
{
    public interface IMLService
    {
        Task<(ITransformer model, PredictionEngine<RatingInput, RatingPrediction> engine)> InitializeAsync();
        PredictionResponse Predict(PredictionRequest request, ITransformer model, PredictionEngine<RatingInput, RatingPrediction> engine);
    }
}
