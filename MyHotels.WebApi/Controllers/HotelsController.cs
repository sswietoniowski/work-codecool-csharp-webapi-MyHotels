using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyHotels.WebApi.Infrastructure;
using MyHotels.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHotels.WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<HotelsController> _logger;

        public HotelsController(IUnitOfWork uwo, IMapper mapper, ILogger<HotelsController> logger)
        {
            this._uow = uwo;
            this._mapper = mapper;
            this._logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<HotelDto>>> GetHotels()
        {
            _logger.LogInformation($"{nameof(GetHotels)} called...");

            var hotels = await _uow.Hotels.GetAll();
            var results = _mapper.Map<IList<HotelDto>>(hotels);

            return Ok(results);
        }

        [HttpGet("{id:int}", Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            _logger.LogInformation($"{nameof(GetHotel)} called...");

            var hotel = await _uow.Hotels.Get(h => h.Id == id, includes: new List<string> { "Country" });

            if (hotel == null)
            {
                return NotFound($"Not found hotel with id = {id}");
            }

            var result = _mapper.Map<HotelDto>(hotel);

            return Ok(hotel);
        }
    }
}
