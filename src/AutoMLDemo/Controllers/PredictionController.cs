using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMLDemo.Services;
using AutoMLDemo.DTOs;
using Microsoft.ML;
using AutoMLDemo.Models;

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
                if (_model == null || _engine == null)
                {
                    _logger.LogInformation("Inicializando modelo pela primeira vez...");
                    (_model, _engine) = await _mlService.InitializeAsync();
                }

                var result = _mlService.Predict(request, _model!, _engine!);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer predi\u00e7\u00e3o");
                return StatusCode(500, new { error = "Erro interno do servidor", details = ex.Message });
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
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
                total_predictions = MLService.PredictionCounter.Value,
                model_r2 = MLService.R2Gauge.Value,
                model_rmse = MLService.RmseGauge.Value,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
