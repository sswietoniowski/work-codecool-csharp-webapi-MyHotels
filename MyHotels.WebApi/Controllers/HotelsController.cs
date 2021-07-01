using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyHotels.WebApi.Domain;
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

        [HttpPost]
        [Authorize(Roles = "User,Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDto hotelDto)
        {
            _logger.LogInformation($"{nameof(CreateHotel)} called...");

            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateHotel)}");

                return BadRequest(ModelState);
            }

            var hotel = _mapper.Map<Hotel>(hotelDto);
            await _uow.Hotels.Add(hotel);
            await _uow.Save();

            return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "User,Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDto hotelDto)
        {
            _logger.LogInformation($"{nameof(UpdateHotel)} called...");

            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid PUT attempt in {nameof(UpdateHotel)}");

                return BadRequest(ModelState);
            }

            var hotel = await _uow.Hotels.Get(h => h.Id == id);

            if (hotel == null)
            {
                return BadRequest("Submitted data is invalid!");
            }

            _mapper.Map(hotelDto, hotel);
            _uow.Hotels.Modify(hotel);
            await _uow.Save();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            _logger.LogInformation($"{nameof(DeleteHotel)} called...");

            var hotel = await _uow.Hotels.Get(h => h.Id == id);

            if (hotel == null)
            {
                return NotFound($"Not found hotel with id = {id}");
            }

            await _uow.Hotels.Remove(id);
            await _uow.Save();

            return NoContent();
        }
    }
}
