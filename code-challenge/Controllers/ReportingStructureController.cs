using System;
using challenge.Models;
using challenge.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace challenge.Controllers
{
    [Route("api/reportingStructure")]
    public class ReportingStructureController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public ReportingStructureController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }
        [HttpGet("{id}")]
        public IActionResult GetReportingStructure(String id)
        {
            _logger.LogDebug($"Received reporting structure get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();


            return Ok(GetReportingStructure(employee));
        }

        private ReportingStructure GetReportingStructure(Employee employee)
        {
            ReportingStructure structure = new ReportingStructure();
            structure.Employee = employee;
            structure.NumberOfReports = GetTotalReports(employee);
            return structure;
        }

        private int GetTotalReports(Employee employee)
        {
            if (employee.DirectReports == null) return 0;
            int count = employee.DirectReports.Count;
            foreach (var reportee in employee.DirectReports)
            {
                count += GetTotalReports(reportee);
            }
            return count;
        }
    }
}
