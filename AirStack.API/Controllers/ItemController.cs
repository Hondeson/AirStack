﻿using AirStack.API.Helper;
using AirStack.Core.Model;
using AirStack.Core.Model.API;
using AirStack.Core.Service;
using AirStack.Core.Service.Validation;
using Azure.Identity;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //time param: 2022-11-21T23:59:59
        public ActionResult<List<GetItemDTO>> Get(long offset, long fetch, StatusFilterEnum? statusEnum, DateTimeOffset? productionFrom, DateTimeOffset? productionTo)
        {
            try
            {
                var (prodFrom, prodTo) = FormatDates(productionFrom, productionTo);
                var enumFilterList = FilterEnumHelper.GetMainEnumListFromFilter<StatusEnum, StatusFilterEnum>(statusEnum);

                var itemList = _itemDTOSvc.Get(
                    offset, fetch,
                    enumFilterList,
                    prodFrom, prodTo);

                for (int i = 0; i < itemList.Count; i++)
                    itemList[i].LoadActualStatus();

                return Ok(itemList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem("Chyba při dotazu na databázi!");
            }
        }

        [HttpGet]
        [Route("GetItemCount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<long> GetItemCount(StatusFilterEnum? statusEnum, DateTimeOffset? productionFrom, DateTimeOffset? productionTo)
        {
            try
            {
                var (prodFrom, prodTo) = FormatDates(productionFrom, productionTo);
                var enumFilterList = FilterEnumHelper.GetMainEnumListFromFilter<StatusEnum, StatusFilterEnum>(statusEnum);

                var count = _itemDTOSvc.GetCount(enumFilterList, prodFrom, prodTo);

                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem("Chyba při dotazu na databázi!");
            }
        }

        [HttpGet]
        [Route("GetFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetFile(StatusFilterEnum? statusEnum, DateTimeOffset? productionFrom, DateTimeOffset? productionTo)
        {
            var dir = Path.Combine(AppContext.BaseDirectory, "Exports");
            var filePath = Path.Combine(dir, "ItemExport.csv");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            try
            {
                var (prodFrom, prodTo) = FormatDates(productionFrom, productionTo);
                var enumFilterList = FilterEnumHelper.GetMainEnumListFromFilter<StatusEnum, StatusFilterEnum>(statusEnum);

                var items = _itemDTOSvc.Get(-1, -1, enumFilterList, prodFrom, prodTo);

                if (items.Count == 0)
                    return NoContent();

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

        (DateTime? prodFrom, DateTime? prodTo) FormatDates(DateTimeOffset? productionFrom, DateTimeOffset? productionTo)
        {
            if (productionFrom is null && productionTo is null)
                return (null, null);
            else if (productionFrom is null && productionTo.HasValue)
                productionFrom = DateTime.MinValue;
            else if (productionFrom.HasValue && productionTo is null)
                productionTo = DateTime.UtcNow;

            return (productionFrom.Value.ToLocalTime().DateTime, productionTo.Value.ToLocalTime().DateTime);
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

                if (_itemSvc.GetByCode(item.Code) != null)
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

            StatusEnum reqStatus = (StatusEnum)itemToUpdate.History.StatusID;
            ItemModel item = GetItemInFormatByStatus(itemToUpdate.Item, reqStatus);
            ItemHistoryModel itemHistory = itemToUpdate.History;
            itemHistory.CreatedAt = DateTime.Now;

            try
            {
                if (ShouldCheckParentCode(reqStatus) == true)
                {
                    item = _itemSvc.GetByParentCode(item.ParentCode);
                    if (item == null)
                        return NotFound("Kód dílu nenalezen v systému!");
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

        private bool ShouldCheckParentCode(StatusEnum status)
        {
            switch (status)
            {
                case StatusEnum.Production:
                case StatusEnum.ComplaintToSupplier:
                    return false;

                case StatusEnum.Tests:
                case StatusEnum.Dispatched:
                case StatusEnum.Complaint:
                    return true;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Během stavů Tests, Dispatched, Complaint se skenuje ParentCode, ovšem klient tohle neřeší a posílá skenovanou hodnotu jako Code,
        /// je tedy třeba zde hodnoty uvést na pravou míru
        /// </summary>
        private ItemModel GetItemInFormatByStatus(ItemModel item, StatusEnum status)
        {
            switch (status)
            {
                case StatusEnum.Tests:
                case StatusEnum.Dispatched:
                case StatusEnum.Complaint:
                    if (!string.IsNullOrWhiteSpace(item.ParentCode))
                        break;

                    item.ParentCode = item.Code;
                    item.Code = string.Empty;
                    break;
            }

            return item;
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