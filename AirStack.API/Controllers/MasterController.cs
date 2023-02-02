using AirStack.Core.Connection;
using AirStack.Core.Service.Validation;
using Microsoft.AspNetCore.Mvc;

namespace AirStack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class MasterController : ControllerBase
    {
        readonly ILogger<MasterController> _log;
        readonly IItemValidationService _itemValidationSvc;
        public MasterController(ILogger<MasterController> log, ISqlAdapter sql, IItemValidationService itemValidationSvc)
        {
            _log = log;
            _itemValidationSvc = itemValidationSvc;
        }

        /// <summary>
        /// Znovunačtení regexů
        /// </summary>
        /// <returns></returns>
        /// <response code="500">OK, došlo k znovunačtení regexů</response>
        /// <response code="500">Chyba</response>
        [HttpGet]
        [Route("/reload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Reload()
        {
            try
            {
                _itemValidationSvc.Load();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return Problem("Chyba při načítání regexů!");
            }

            return Ok("Reload proběhl úspěšně");
        }
    }
}
