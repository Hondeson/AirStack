using AirStack.Core.Model;
using AirStack.Core.Model.API;
using AirStack.Core.Services;
using AirStack.Core.Services.Validation;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //time param: 2022-11-21T23:59:59
        public ActionResult<GetItemDTOList> Get(long offset, long fetch)
        {
            try
            {
                var listObj = _itemDTOSvc.Get(offset, fetch);
                return Ok(listObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem("Chyba při dotazu na databázi!");
            }
        }

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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromBody] ItemModel item)
        {
            try
            {
                if (_itemValidationSvc.IsItemCodeValid(item.Code) == false)
                    return ValidationProblem($"Kód neodpovídá definovanému regexu!");

                if (_itemSvc.Get(item.Code) != null)
                    return Conflict($"Kód již existuje!");

                bool result = _itemSvc.Create(item);

                if (result == false)
                    return Problem();

                var histObj = new ItemHistoryModel() { ItemID = item.ID, StatusID = new StatusModel(StatusEnum.Production).ID, CreatedAt = DateTime.Now };
                result = _histSvc.Create(histObj);

                if (result == false)
                    return Problem();
            }
            catch (Exception ex)
            {
                var json = JsonSerializer.Serialize(item);
                _logger.LogError("Request params: {0}", json);
                _logger.LogError(ex.Message);
                return Problem("Chyba na serveru!");
            }

            return CreatedAtRoute("Get", new { id = item.ID }, item);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Put([FromBody] UpdateItemDTO itemToUpdate)
        {
            bool result = false;
            ItemModel item = itemToUpdate.Item;
            ItemHistoryModel itemHistory = itemToUpdate.History;
            itemHistory.CreatedAt = DateTime.Now;

            try
            {
                if (_itemSvc.Get(item.ID) == null)
                    return NotFound(item.Code);

                result = _itemSvc.Update(item);

                if (result == false)
                    return Problem("Chyba na serveru! Update položky se nezdařil!");

                if (CanCreateHistoryRecordForItem(item, itemHistory) == false)
                    return Ok();

                result = _histSvc.Create(itemHistory);

                if (result == false)
                    return Problem("Chyba na serveru! Vytvoření záznamu se nezdařilo!");
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

        bool CanCreateHistoryRecordForItem(ItemModel item, ItemHistoryModel itemHistory)
        {
            if (itemHistory is null)
                return false;

            var itemsHist = _histSvc.GetByItemId(item.ID);
            if (itemsHist.Count == 0 || itemsHist.Any(x => x.StatusID == itemHistory.StatusID))
                return false;

            return true;
        }

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
