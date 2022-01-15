using System;
using challenge.Models;
using challenge.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace challenge.Controllers
{
    [Route("api/compensation")]
    public class CompensationController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService)
        {
            _logger = logger;
            _compensationService = compensationService;
        }
        [HttpPost]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            _logger.LogDebug($"Received compensation create request for '{compensation.Employee}'");

            _compensationService.Create(compensation);

            return CreatedAtRoute("getCompensationById", new { id = compensation.CompensationId }, compensation);
        }


        [HttpGet("{id}", Name = "getCompensationById")]
        public IActionResult GetCompensationById(String id)
        {
            _logger.LogDebug($"Received compensation get request for '{id}'");

            var compensation = _compensationService.GetById(id);

            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }


        [HttpGet("byEmployee/{employeeId}", Name = "getCompensationsByEmployee")]
        public IActionResult GetCompensationsByEmployee(String employeeId)
        {
            _logger.LogDebug($"Received compensation by employee get request for '{employeeId}'");
            var compensations = _compensationService.GetByEmployee(employeeId);


            if (compensations.Count == 0)
                return NotFound();

            return Ok(compensations);

        }

    }
}
