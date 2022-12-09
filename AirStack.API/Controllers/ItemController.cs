using AirStack.API.DTO;
using AirStack.Core.Model;
using AirStack.Core.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AirStack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        readonly IItemProvider _itemSvc;
        readonly IItemHistoryProvider _histSvc;
        readonly IStatusProvider _statSvc;
        readonly ILogger _logger;
        public ItemController(ILogger<ItemController> logger, IItemProvider itemSvc, IItemHistoryProvider histSvc, IStatusProvider statSvc)
        {
            _logger = logger;
            _itemSvc = itemSvc;
            _histSvc = histSvc;
            _statSvc = statSvc;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //time param: 2022-11-21T23:59:59
        public ActionResult<List<ItemModel>> Get(DateTime from, DateTime to)
        {
            if (from > to)
                return BadRequest();

            return NoContent();
        }

        [HttpGet("{id}", Name = "Get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<GetItemDTO> Get(long id)
        {
            try
            {
                var item = _itemSvc.Get(id);

                if (item == null)
                    return NotFound();

                var itemHist = _histSvc.GetByItemId(item.ID)
                    .Select(x => new GetItemHistoryModel()
                    {
                        ID = x.ID,
                        Status = new StatusModel(x.ID),
                        CreatedAt = x.CreatedAt
                    }).ToList();

                GetItemDTO respItem = new() { Item = item, HistoryList = itemHist };

                return Ok(respItem);
            }
            catch (Exception ex)
            {
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
            //TODO: validace kódu na regex

            try
            {
                if (_itemSvc.Get(item.Code) != null)
                    return Conflict(item.Code);

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
                _logger.LogError(ex.Message);
                return Problem();
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

            try
            {
                if (_itemSvc.Get(item.ID) == null)
                    return NotFound(item.Code);

                result = _itemSvc.Update(item);

                if (result == false)
                    return Problem("Item could not be updated!");

                if (CanCreateHistoryRecordForItem(item, itemHistory) == false)
                    return Ok();

                result = _histSvc.Create(itemHistory);

                if (result == false)
                    return Problem("History record could not be created!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            if (result == false)
                return Problem();

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
    }
}
