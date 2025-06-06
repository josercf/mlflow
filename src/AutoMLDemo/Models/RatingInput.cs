using Microsoft.ML.Data;

namespace AutoMLDemo.Models
{
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
