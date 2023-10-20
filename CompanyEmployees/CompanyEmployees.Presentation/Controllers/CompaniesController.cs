using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase // add <FrameworkReference Include="Microsoft.AspNetCore.App" />
    {
        private readonly IServiceManager _service;

        public CompaniesController(IServiceManager service)
        {
            _service = service;
        }

        //// Because there is no route attribute right above the action,
        //// the route for the GetCompanies action will be api/companies which is the route placed on top of our controller.
        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            //// throw new Exception("Exception Test");
            IEnumerable<CompanyDto> companies = await _service.CompanyService.GetAllCompaniesAsync(trackChanges: false);
            return Ok(companies);
        }

        [HttpGet("{id:guid}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            CompanyDto company = await _service.CompanyService.GetCompanyAsync(id, trackChanges: false);
            return Ok(company);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            //// Below commented code moved to action filter
            //if (company is null)
            //{
            //    return BadRequest("CompanyForCreationDto object is null");
            //}

            //if (!ModelState.IsValid)
            //    return UnprocessableEntity(ModelState);

            CompanyDto createdCompany = await _service.CompanyService.CreateCompanyAsync(company);

            //// CreatedAtRoute will return a status code 201, which stands for Created.
            //// Also, it will populate the body of the response with the new company object
            //// as well as the Location attribute within the response header with the address to retrieve that company.
            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            var companies = await _service.CompanyService.GetByIdsAsync(ids, trackChanges: false);
            return Ok(companies);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            var (companies, ids) = await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);

            //// we sending a comma-separated string when we expect a collection of ids in the GetCompanyCollection action
            return CreatedAtRoute("CompanyCollection", new { ids }, companies);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            //// Below commented code moved to action filter
            //if (company == null)
            //    return BadRequest("CompanyForUpdateDto object is null");

            //if (!ModelState.IsValid)
            //    return UnprocessableEntity(ModelState);

            await _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);

            return NoContent();
        }
    }
}