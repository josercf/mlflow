using Microsoft.ML.Data;

namespace AutoMLDemo
{
    // 1. Classe que representa cada registro de ratings.csv
    public class RatingInput
    {
        [LoadColumn(0)]
        public float UserId { get; set; }

        [LoadColumn(1)]
        public float MovieId { get; set; }

        [LoadColumn(2)]
        public float Label { get; set; }
    }
}