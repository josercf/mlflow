using System.ComponentModel.DataAnnotations;

namespace AutoMLDemo
{
    // 3. DTO para requisição da API
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