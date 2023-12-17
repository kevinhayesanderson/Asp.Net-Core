using CompanyEmployees.Presentation.ActionFilters;
using Entities.LinkModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Text.Json;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController(IServiceManager _service) : ControllerBase
    {
        [HttpGet]
        [HttpHead]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId,
            [FromQuery] EmployeeParameters employeeParameters)
        {
            var linkParams = new LinkParameters(employeeParameters, HttpContext);

            var result = await _service.EmployeeService.GetEmployeesAsync(companyId, linkParams, trackChanges: false);

            Response.Headers["X-Pagination"] = JsonSerializer.Serialize(result.metaData);

            return result.linkResponse.HasLinks ?
                Ok(result.linkResponse.LinkedEntities) :
                Ok(result.linkResponse.ShapedEntities);
        }

        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
        {
            EmployeeDto employee = await _service.EmployeeService.GetEmployeeAsync(companyId, id, trackChanges: false);
            return Ok(employee);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            //// Below commented code moved to action filter
            ////if (employee is null)
            ////{
            ////    return BadRequest("EmployeeForCreationDto object is null");
            ////}

            ////if (!ModelState.IsValid)
            ////{
            ////    return UnprocessableEntity(ModelState);
            ////}

            EmployeeDto employeeToReturn = await _service.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employee, trackChanges: false);

            //// object? routeValues = new { companyId, id = employeeToReturn.Id }
            //// routeValues anonymous object is for specifying parameters for GetEmployeeForCompany method
            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            await _service.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, id, trackChanges: false);
            return NoContent(); //// returns the status code 204 No Content.
        }

        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
        {
            //// Below commented code moved to action filter
            ////if (employee is null)
            ////{
            ////    return BadRequest("EmployeeForUpdateDto object is null");
            ////}

            ////if (!ModelState.IsValid)
            ////{
            ////    return UnprocessableEntity(ModelState);
            ////}

            await _service.EmployeeService.UpdateEmployeeForCompanyAsync(
                companyId,
                id,
                employee,
                compTrackChanges: false,
                empTrackChanges: true);

            return NoContent();
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest("patchDoc object sent from client is null.");
            }

            (EmployeeForUpdateDto employeeToPatch, Entities.Models.Employee employeeEntity) = await _service.EmployeeService.GetEmployeeForPatchAsync(companyId, id, compTrackChanges: false, empTrackChanges: true);

            patchDoc.ApplyTo(employeeToPatch, ModelState);

            _ = TryValidateModel(employeeToPatch);

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            await _service.EmployeeService.SaveChangesForPatchAsync(employeeToPatch, employeeEntity);

            return NoContent();
        }
    }
}