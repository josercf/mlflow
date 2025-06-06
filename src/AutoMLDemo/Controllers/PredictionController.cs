using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using AutoMLDemo.Services;

namespace AutoMLDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictionController : ControllerBase
    {
        private static ITransformer? _model;
        private static PredictionEngine<RatingInput, RatingPrediction>? _engine;
        private readonly IMLService _mlService;
        private readonly ILogger<PredictionController> _logger;

        public PredictionController(IMLService mlService, ILogger<PredictionController> logger)
        {
            _mlService = mlService;
            _logger = logger;
        }

        [HttpPost("predict")]
        public async Task<ActionResult<PredictionResponse>> Predict([FromBody] PredictionRequest request)
        {
            try
            {
                // Inicializar modelo se necessário (lazy loading)
                if (_model == null || _engine == null)
                {
                    _logger.LogInformation("Inicializando modelo pela primeira vez...");
                    (_model, _engine) = await _mlService.InitializeAsync();
                }

                var result = _mlService.Predict(request, _model, _engine);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer predição");
                return StatusCode(500, new { error = "Erro interno do servidor", details = ex.Message });
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                model_loaded = _model != null 
            });
        }

        [HttpGet("metrics-summary")]
        public ActionResult GetMetricsSummary()
        {
            return Ok(new
            {
                total_predictions = (long)MetricsRegistry.PredictionCounter.Value,
                model_r2 = MetricsRegistry.R2Gauge.Value,
                model_rmse = MetricsRegistry.RmseGauge.Value,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
