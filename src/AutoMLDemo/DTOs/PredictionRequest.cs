using System.ComponentModel.DataAnnotations;

namespace AutoMLDemo.DTOs
{
    public class PredictionRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId deve ser maior que 0")]
        public float UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MovieId deve ser maior que 0")]
        public float MovieId { get; set; }
    }
}
