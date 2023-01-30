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
        readonly ISqlAdapter _sql;
        readonly IItemValidationService _itemValidationSvc;
        public MasterController(ILogger<MasterController> log, ISqlAdapter sql, IItemValidationService itemValidationSvc)
        {
            _log = log;
            _sql = sql;
            _itemValidationSvc = itemValidationSvc;
        }

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
