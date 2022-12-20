using AirStack.Core.Connection;
using AirStack.Core.Services.Validation;
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
        [Route("/ping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get()
        {
            try
            {
                CancellationTokenSource t = new CancellationTokenSource();

                using (var con = _sql.Connect())
                {
                    var task = Task.Run(() =>
                    {
                        con.Open();
                        con.Close();
                    }, t.Token);

                    if (Task.WaitAll(new[] { task }, TimeSpan.FromSeconds(3)) == false)
                    {
                        con.Dispose();
                        t.Cancel();
                        return Problem("Chyba při spojení s databází!");
                    }

                    return Ok("pong");
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return Problem("Chyba při spojení s databází!");
            }
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

            return Ok();
        }
    }
}
