using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ToyTrucks.Catalog.Api.Filters;
using ToyTrucks.Catalog.Api.Models;
using ToyTrucks.Catalog.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace ToyTrucks.Catalog.Api.Controllers
{
    [Route("api/trucks")]
    [ApiVersion("1.0")]
    [ApiController]
    //    [Authorize(Policy = "CanRead")]
    public class TruckController : ControllerBase
    {
        private readonly ITruckRepository _service;
        private IMapper _mapper;
        private readonly ILogger<TruckController> _logger;
        public TruckController(ITruckRepository service, IMapper mapper, ILogger<TruckController> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [TrucksFilter]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<TruckDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<TruckDto>>> AllTrucks()
        {
            var trucks = await _service.GetTrucks();
            if (trucks == null)
                return NotFound();
            return Ok(trucks);
        }


        [HttpGet]
        [Route("{categoryId:int}")]
        [TrucksFilter]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<TruckDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<TruckDto>>> TrucksByCategory(int categoryId)
        {
            var trucks = await _service.GetTrucksByCategoryId(categoryId);
            if (trucks == null)
                return NotFound();
            return Ok(trucks);
        }

        [HttpGet]
        [Route("{truckId:Guid}")]
        [TruckFilter]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(TruckDto), (int)HttpStatusCode.OK)]

        public async Task<ActionResult<TruckDto>> TruckById(Guid truckId)
        {
            if (truckId == Guid.Empty)
                return BadRequest();

            var truck = await _service.GetTruckById(truckId);
            if (truck == null)
                return NotFound();
            return Ok(truck);
        }

        [HttpGet]
        [Route("[action]")]
        [TruckFilter]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(TruckDto), (int)HttpStatusCode.OK)]

        public async Task<ActionResult<TruckDto>> TruckByName([FromBody] TruckByNameObject tbno)
        {
            // var truckName = "sds";
            if (string.IsNullOrEmpty(tbno.TruckName))
                return BadRequest();

            var truck = await _service.GetTruckByName(tbno.TruckName);
            if (truck == null)
                return NotFound();
            return Ok(truck);
        }

        [HttpGet]
        [Route("Inventory/{truckId:Guid}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(TruckInventoryDto), (int)HttpStatusCode.OK)]

        public async Task<ActionResult<TruckInventoryDto>> TruckInventoryById(Guid truckId)
        {
            if (truckId == Guid.Empty)
                return BadRequest();

            var truckInventory = await _service.GetTruckInventory(truckId);
            if (truckInventory == null)
                return NotFound();
            return Ok(truckInventory);
        }

        [HttpPut]
        [TruckFilter]
        public async Task<ActionResult<TruckDto>> UpdateTruck([FromBody] TruckDto truckDto)
        {
            if (truckDto == null)
                return BadRequest();
            var truck = _mapper.Map<Entities.Truck>(truckDto);
            if (await _service.UpdateTruck(truck))
                return Ok();
            else
                return BadRequest();
        }
    }

    public class TruckByNameObject
    {
        public string TruckName { get; set; }
    }
}
