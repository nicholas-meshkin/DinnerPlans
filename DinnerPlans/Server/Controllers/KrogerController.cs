using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DinnerPlans.Server.Core.IServices;
using DinnerPlans.Server.Core.ErrorHandling;
using DinnerPlans.Shared.DTOs;
using System.Net;

namespace DinnerPlans.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KrogerController : ControllerBase
    {
        private readonly IKrogerApiService _krogerApiService;
        private readonly ILogger<KrogerController> _logger;

        public KrogerController(IKrogerApiService krogerApiService, ILogger<KrogerController> logger)
        {
            _krogerApiService = krogerApiService;
            _logger = logger;
        }

        [HttpPost("productSearch")]
        [ProducesResponseType(typeof(KrogerProductSearchResponseDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ProductSearch(KrogerProductSearchRequestDto dto)
        {
            try
            {
                var respDto = await _krogerApiService.ProductSearch(dto);
                return respDto == null ? NotFound() : Ok(respDto);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpGet("locationSearch/{zip}/{radius}")]
        [ProducesResponseType(typeof(KrogerProductSearchResponseDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> LocationSearch(string zip, int radius)
        {
            try
            {
                var respDto = await _krogerApiService.LocationSearch(zip, radius);
                return respDto == null ? NotFound() : Ok(respDto);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }
    }
}
