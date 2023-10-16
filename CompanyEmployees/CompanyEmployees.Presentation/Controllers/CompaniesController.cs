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
        public IActionResult GetCompanies()
        {
            //// throw new Exception("Exception Test");
            IEnumerable<CompanyDto> companies = _service.CompanyService.GetAllCompanies(trackChanges: false);
            return Ok(companies);
        }

        [HttpGet("{id:guid}", Name = "CompanyById")]
        public IActionResult GetCompany(Guid id)
        {
            CompanyDto company = _service.CompanyService.GetCompany(id, trackChanges: false);
            return Ok(company);
        }

        [HttpPost]
        public IActionResult CreateCompany([FromBody] CompanyForCreationDto company)
        {
            if (company is null)
            {
                return BadRequest("CompanyForCreationDto object is null");
            }

            if (!ModelState.IsValid)
			return UnprocessableEntity(ModelState);

            CompanyDto createdCompany = _service.CompanyService.CreateCompany(company);

            //// CreatedAtRoute will return a status code 201, which stands for Created.
            //// Also, it will populate the body of the response with the new company object
            //// as well as the Location attribute within the response header with the address to retrieve that company.
            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public IActionResult GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            var companies = _service.CompanyService.GetByIds(ids, trackChanges: false);
            return Ok(companies);
        }

        [HttpPost("collection")]
        //// ArrayModelBinder will be triggered before an action executes. It will convert the sent string parameter to the IEnumerable<Guid> type,
        //// and then the action will be executed:
        public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            var (companies, ids) = _service.CompanyService.CreateCompanyCollection(companyCollection);

            //// we sending a comma-separated string when we expect a collection of ids in the GetCompanyCollection action
            return CreatedAtRoute("CompanyCollection", new { ids }, companies);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteCompany(Guid id)
        {
            _service.CompanyService.DeleteCompany(id, trackChanges: false);
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public IActionResult UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            if (company == null)
                return BadRequest("CompanyForUpdateDto object is null");

            if (!ModelState.IsValid)
			return UnprocessableEntity(ModelState);

            _service.CompanyService.UpdateCompany(id, company, trackChanges: true);

            return NoContent();
        }
    }
}