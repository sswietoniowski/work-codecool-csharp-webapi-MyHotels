using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyHotels.WebApi.Controllers;
using MyHotels.WebApi.Infrastructure;
using MyHotels.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHotels.WebApi.Configurations.Mapper
{
    [ApiController]
    [Route("api/hotels")]
    //[Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class NewHotelsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<HotelsController> _logger;

        public NewHotelsController(IUnitOfWork uwo, IMapper mapper, ILogger<HotelsController> logger)
        {
            this._uow = uwo;
            this._mapper = mapper;
            this._logger = logger;
        }

        [HttpGet("{id:int}", Name = "GetHotelNew")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HotelDto>> GetHotelNew(int id)
        {
            _logger.LogInformation($"{nameof(GetHotelNew)} called...");

            var hotel = await _uow.Hotels.Get(h => h.Id == id, includes: new List<string> { "Country" });

            if (hotel == null)
            {
                return NotFound($"Not found hotel with id = {id}");
            }

            var result = _mapper.Map<HotelDto>(hotel);

            return Ok(hotel);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<HotelDto>>> GetHotels([FromQuery] RequestParams requestParams)
        {
            _logger.LogInformation($"{nameof(GetHotels)} called...");

            var hotels = await _uow.Hotels.GetAllPaged(requestParams);
            var results = _mapper.Map<IList<HotelDto>>(hotels);

            return Ok(results);
        }
    }
}
