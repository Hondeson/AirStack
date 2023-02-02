using AirStack.API.Helper;
using AirStack.Core.Model;
using AirStack.Core.Model.API;
using AirStack.Core.Service;
using AirStack.Core.Service.Validation;
using Azure.Identity;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AirStack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class ItemController : ControllerBase
    {
        private readonly IItemProvider _itemSvc;
        private readonly IItemHistoryProvider _histSvc;
        private readonly IItemValidationService _itemValidationSvc;
        private readonly IItemDTOProvider _itemDTOSvc;
        private readonly ILogger _logger;
        public ItemController(
            ILogger<ItemController> logger,
            IItemProvider itemSvc,
            IItemHistoryProvider histSvc,
            IItemValidationService itemValidationSvc,
            IItemDTOProvider itemDTOSvc)
        {
            _logger = logger;
            _itemSvc = itemSvc;
            _histSvc = histSvc;
            _itemValidationSvc = itemValidationSvc;
            _itemDTOSvc = itemDTOSvc;
        }

        /// <summary>
        /// Vrací seznam vyfiltrovaných airbagů
        /// </summary>
        /// <param name="offset">Odsun filtrovaných airbagů o, slouží pro stránkování</param>
        /// <param name="fetch">Počet kolik airbagů chci vrátit</param>
        /// <param name="statusEnum">Flag stavů airbagů</param>
        /// <param name="codeLike">Filtr kódu airbagu</param>
        /// <param name="parentCodeLike">Filtr kódu výrobku na němž se airbag nachází</param>
        /// <param name="productionFrom">Vstup do produkce OD</param>
        /// <param name="productionTo">Vstup do produkce DO</param>
        /// <param name="dispatchedFrom">Expedice OD</param>
        /// <param name="dispatchedTo">Expedice DO</param>
        /// <param name="testsFrom">Testy OD</param>
        /// <param name="testsTo">Testy DO</param>
        /// <param name="complaintFrom">Reklamace zákazníka OD</param>
        /// <param name="complaintTo">Reklamace zákazníka DO</param>
        /// <param name="complaintSuplFrom">Reklamace dodavateli OD</param>
        /// <param name="complaintSuplTo">Reklamace dodavateli OD</param>
        /// <returns></returns>
        /// <response code="200">Seznam airbagů</response>
        /// <response code="204">Seznam airbagů obshauje 0 položek</response>
        /// <response code="500">Chyba</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<GetItemDTO>> Get(
            [Required] long offset, [Required] long fetch,
            StatusFilterEnum? statusEnum,
            string codeLike, string parentCodeLike,
            DateTimeOffset? productionFrom, DateTimeOffset? productionTo,
            DateTimeOffset? dispatchedFrom, DateTimeOffset? dispatchedTo,
            DateTimeOffset? testsFrom, DateTimeOffset? testsTo,
            DateTimeOffset? complaintFrom, DateTimeOffset? complaintTo,
            DateTimeOffset? complaintSuplFrom, DateTimeOffset? complaintSuplTo)
        {
            try
            {
                var enumFilterList = FilterEnumHelper.GetMainEnumListFromFilter<StatusEnum, StatusFilterEnum>(statusEnum);

                var (prodFrom, prodTo) = FormatDates(productionFrom, productionTo);
                var (dispFrom, dispTo) = FormatDates(dispatchedFrom, dispatchedTo);
                var (tesFrom, tesTo) = FormatDates(testsFrom, testsTo);
                var (compFrom, compTo) = FormatDates(complaintFrom, complaintTo);
                var (compSplFrom, compSplTo) = FormatDates(complaintSuplFrom, complaintSuplTo);

                var items = _itemDTOSvc.Get(
                    offset, fetch,
                    enumFilterList,
                    codeLike, parentCodeLike,
                    prodFrom, prodTo,
                    dispFrom, dispTo,
                    tesFrom, tesTo,
                    compFrom, compTo,
                    compSplFrom, compSplTo);

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem("Chyba při dotazu na databázi!");
            }
        }

        /// <summary>
        /// Vrací počet všech airbagů splňující filter, slouží pro stránkování
        /// </summary>
        /// <param name="statusEnum">Flag stavů airbagů</param>
        /// <param name="codeLike">Filtr kódu airbagu</param>
        /// <param name="parentCodeLike">Filtr kódu výrobku na němž se airbag nachází</param>
        /// <param name="productionFrom">Vstup do produkce OD</param>
        /// <param name="productionTo">Vstup do produkce DO</param>
        /// <param name="dispatchedFrom">Expedice OD</param>
        /// <param name="dispatchedTo">Expedice DO</param>
        /// <param name="testsFrom">Testy OD</param>
        /// <param name="testsTo">Testy DO</param>
        /// <param name="complaintFrom">Reklamace zákazníka OD</param>
        /// <param name="complaintTo">Reklamace zákazníka DO</param>
        /// <param name="complaintSuplFrom">Reklamace dodavateli OD</param>
        /// <param name="complaintSuplTo">Reklamace dodavateli OD</param>
        /// <returns></returns>
        /// <response code="200">Vrací počet airbagů splňující filter, int64</response>
        /// <response code="500">Chyba</response>
        [HttpGet]
        [Route("GetItemCount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<long> GetItemCount(
            StatusFilterEnum? statusEnum,
            string codeLike, string parentCodeLike,
            DateTimeOffset? productionFrom, DateTimeOffset? productionTo,
            DateTimeOffset? dispatchedFrom, DateTimeOffset? dispatchedTo,
            DateTimeOffset? testsFrom, DateTimeOffset? testsTo,
            DateTimeOffset? complaintFrom, DateTimeOffset? complaintTo,
            DateTimeOffset? complaintSuplFrom, DateTimeOffset? complaintSuplTo)
        {
            try
            {
                var enumFilterList = FilterEnumHelper.GetMainEnumListFromFilter<StatusEnum, StatusFilterEnum>(statusEnum);

                var (prodFrom, prodTo) = FormatDates(productionFrom, productionTo);
                var (dispFrom, dispTo) = FormatDates(dispatchedFrom, dispatchedTo);
                var (tesFrom, tesTo) = FormatDates(testsFrom, testsTo);
                var (compFrom, compTo) = FormatDates(complaintFrom, complaintTo);
                var (compSplFrom, compSplTo) = FormatDates(complaintSuplFrom, complaintSuplTo);

                var count = _itemDTOSvc.GetCount(
                    enumFilterList,
                    codeLike, parentCodeLike,
                    prodFrom, prodTo,
                    dispFrom, dispTo,
                    tesFrom, tesTo,
                    compFrom, compTo,
                    compSplFrom, compSplTo);

                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem("Chyba při dotazu na databázi!");
            }
        }

        /// <summary>
        /// Slouží pro stáhnutí vyfiltrovaných airbagů do .csv
        /// </summary>
        /// <param name="statusEnum">Flag stavů airbagů</param>
        /// <param name="codeLike">Filtr kódu airbagu</param>
        /// <param name="parentCodeLike">Filtr kódu výrobku na němž se airbag nachází</param>
        /// <param name="productionFrom">Vstup do produkce OD</param>
        /// <param name="productionTo">Vstup do produkce DO</param>
        /// <param name="dispatchedFrom">Expedice OD</param>
        /// <param name="dispatchedTo">Expedice DO</param>
        /// <param name="testsFrom">Testy OD</param>
        /// <param name="testsTo">Testy DO</param>
        /// <param name="complaintFrom">Reklamace zákazníka OD</param>
        /// <param name="complaintTo">Reklamace zákazníka DO</param>
        /// <param name="complaintSuplFrom">Reklamace dodavateli OD</param>
        /// <param name="complaintSuplTo">Reklamace dodavateli OD</param>
        /// <response code="200">OK, vrací application/octet-stream</response>
        /// <response code="204">Vrací application/octet-stream, .csv je prázdný</response>
        /// <response code="500">Chyba</response>
        [HttpGet]
        [Route("GetFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetFile(
            StatusFilterEnum? statusEnum,
            string codeLike, string parentCodeLike,
            DateTimeOffset? productionFrom, DateTimeOffset? productionTo,
            DateTimeOffset? dispatchedFrom, DateTimeOffset? dispatchedTo,
            DateTimeOffset? testsFrom, DateTimeOffset? testsTo,
            DateTimeOffset? complaintFrom, DateTimeOffset? complaintTo,
            DateTimeOffset? complaintSuplFrom, DateTimeOffset? complaintSuplTo)
        {
            var dir = Path.Combine(AppContext.BaseDirectory, "Exports");
            var filePath = Path.Combine(dir, "ItemExport.csv");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            try
            {
                var enumFilterList = FilterEnumHelper.GetMainEnumListFromFilter<StatusEnum, StatusFilterEnum>(statusEnum);

                var (prodFrom, prodTo) = FormatDates(productionFrom, productionTo);
                var (dispFrom, dispTo) = FormatDates(dispatchedFrom, dispatchedTo);
                var (tesFrom, tesTo) = FormatDates(testsFrom, testsTo);
                var (compFrom, compTo) = FormatDates(complaintFrom, complaintTo);
                var (compSplFrom, compSplTo) = FormatDates(complaintSuplFrom, complaintSuplTo);

                var items = _itemDTOSvc.Get(
                    -1, -1,
                    enumFilterList,
                    codeLike, parentCodeLike,
                    prodFrom, prodTo,
                    dispFrom, dispTo,
                    tesFrom, tesTo,
                    compFrom, compTo,
                    compSplFrom, compSplTo);

                using (var writer = new StreamWriter(filePath))
                {
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(items);
                    }
                }

                return File(new FileStream(filePath, FileMode.Open), "application/octet-stream", "ItemExport.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError("Request params: {0}, {1}", productionFrom, productionTo);
                _logger.LogError(ex.Message);
                return Problem("Chyba při exportu souboru!");
            }
        }

        (DateTime? fromDate, DateTime? toDate) FormatDates(DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            if (fromDate is null && toDate is null)
                return (null, null);
            else if (fromDate is null && toDate.HasValue)
                fromDate = DateTimeOffset.UtcNow.AddYears(-90);
            else if (fromDate.HasValue && toDate is null)
                toDate = DateTimeOffset.UtcNow;

            return (fromDate.Value.ToLocalTime().DateTime, toDate.Value.ToLocalTime().DateTime);
        }

        /// <summary>
        /// Vrátí konkrétní airbag.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="404">ID airbagu nenalezeno</response>
        /// <response code="500">Chyba</response>
        [HttpGet("{id}", Name = "Get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<GetItemDTO> Get(long id)
        {
            try
            {
                ItemModel item = _itemSvc.Get(id);

                if (item == null)
                    return NotFound();

                var itemHist = _histSvc.GetByItemId(item.ID);
                var respItem = new GetItemDTO(item, itemHist);

                return Ok(respItem);
            }
            catch (Exception ex)
            {
                _logger.LogError("Request params: {0}", id);
                _logger.LogError(ex.Message);
                return Problem();
            }
        }

        /// <summary>
        /// Vytvoří nový airbag
        /// </summary>
        /// <param name="code"></param>
        /// <response code="201">Vrací nově vytvořený item</response>
        /// <response code="400">Kód nemůže být prázdný string</response>
        /// <response code="409">Pokud kód již existuje</response>
        /// <response code="500">Chyba</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromBody] string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return BadRequest();

                var item = new ItemModel() { Code = code };

                if (_itemValidationSvc.IsItemCodeValid(code) == false)
                    return ValidationProblem($"Kód neodpovídá definovanému regexu!");

                if (_itemSvc.GetByCode(item.Code) != null)
                    return Conflict($"Kód již existuje!");

                bool result = _itemSvc.Create(item);

                if (result == false)
                    return Problem("Vytvoření kódu se nezdařilo!");

                var histObj = new ItemHistoryModel() { ItemID = item.ID, StatusID = new StatusModel(StatusEnum.Production).ID, CreatedAt = DateTime.Now };
                result = _histSvc.Create(histObj);

                if (result == false)
                {
                    _itemSvc.Delete(item.ID);
                    return Problem();
                }

                return CreatedAtRoute("Get", new { id = item.ID }, item);
            }
            catch (Exception ex)
            {
                _logger.LogError("Request params: {0}", code);
                _logger.LogError(ex.Message);
                return Problem("Chyba na serveru!");
            }
        }

        /// <summary>
        /// Vytvoří nový záznam do historie airbagu, čímž dojde ke změně aktuálního stavu. Pokud je stav požadovaný
        /// Tests, Dispatched, Complaint, tak očekává SN dílu, jinak čeká SN airbagu
        /// </summary>
        /// <param name="itemToUpdate"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="404">Pokud kód dílu nebo airbagu není nalezen</response>
        /// <response code="500">Chyba</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Put([FromBody] UpdateItemDTO itemToUpdate)
        {
            bool result = false;

            StatusEnum reqStatus = itemToUpdate.ActualStatus;
            ItemModel item = CreateItemFromCodeByReuqestStatus(itemToUpdate.Code, reqStatus);

            var itemHistory = new ItemHistoryModel() { StatusID = (long)reqStatus };
            itemHistory.CreatedAt = DateTime.Now;

            try
            {
                if (!string.IsNullOrEmpty(item.ParentCode))
                {
                    item = _itemSvc.GetByParentCode(item.ParentCode);
                    if (item == null)
                        return NotFound("Na dílu se nenechází kód airbagu!");
                }
                else
                {
                    item = _itemSvc.GetByCode(item.Code);
                    if (item == null)
                        return NotFound("Kód airbagu nenalezen v systému!");
                }

                result = _itemSvc.Update(item);
                itemHistory.ItemID = item.ID;

                if (result == false)
                    return Problem("Změna stavu položky se nezdařila!");

                result = _histSvc.Create(itemHistory);

                if (result == false)
                    return Problem("Vytvoření záznamu se nezdařilo!");
            }
            catch (Exception ex)
            {
                var json = JsonSerializer.Serialize(itemToUpdate);
                _logger.LogError("Request params: {0}", json);
                _logger.LogError(ex.Message);
            }

            if (result == false)
                return Problem("Chyba na serveru!");

            return Ok();
        }

        /// <summary>
        /// Během stavů Tests, Dispatched, Complaint se skenuje ParentCode, je tedy potřeba item takhle vytvořit
        /// </summary>
        private ItemModel CreateItemFromCodeByReuqestStatus(string code, StatusEnum status)
        {
            switch (status)
            {
                case StatusEnum.Tests:
                case StatusEnum.Dispatched:
                case StatusEnum.Complaint:
                    return new ItemModel() { ParentCode = code };
                default:
                    return new ItemModel() { Code = code };
            }
        }

        /// <summary>
        /// Vrací regulární výrazy definované pro airbag
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Vrací seznam regexů pro airbag</response>
        /// <response code="204">Není definován žádný regex</response>
        /// <response code="500">Chyba</response>
        [HttpGet("CodeRegexes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<string>> GetCodeRegexes()
        {
            try
            {
                List<string> regexList = _itemValidationSvc.GetRegexes().Select(x => x.ToString()).ToList();

                if (regexList.Count == 0)
                    return NoContent();

                return Ok(regexList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }
    }
}