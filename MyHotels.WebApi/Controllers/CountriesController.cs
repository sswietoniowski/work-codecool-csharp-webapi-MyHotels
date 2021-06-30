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
    public class CountriesController : ControllerBase
    {
        private readonly IUnitOfWork _uwo;
        private readonly IMapper _mapper;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(IUnitOfWork uwo, IMapper mapper, ILogger<CountriesController> logger)
        {
            this._uwo = uwo;
            this._mapper = mapper;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IList<CountryDto>>> GetAll()
        {
            _logger.LogInformation($"{nameof(GetAll)} called...");
            try
            {
                var countries = await _uwo.Countries.GetAll();
                var results = _mapper.Map<IList<CountryDto>>(countries);
                return Ok(results);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Something went wrong in {nameof(GetAll)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error, please try again later...");
            }
        }
    }
}
