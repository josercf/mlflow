namespace AutoMLDemo.DTOs
{
    public class PredictionResponse
    {
        public float UserId { get; set; }
        public float MovieId { get; set; }
        public float PredictedRating { get; set; }
        public string RatingRange { get; set; } = string.Empty;
        public double LatencyMs { get; set; }
        public long TotalPredictions { get; set; }
    }
}
