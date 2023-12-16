using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Presentation.Controllers
{
    [ApiVersion("2.0", Deprecated = true)]
    //[Route("api/companies")]
    [Route("api/{v:apiVersion}/companies")]
    [ApiController]
    public class CompaniesV2Controller(IServiceManager service) : ControllerBase
    {
        [HttpGet]
        
        public async Task<IActionResult> GetCompanies()
        {
            IEnumerable<CompanyDto> companies = await service.CompanyService.GetAllCompaniesAsync(trackChanges: false);
            var companiesV2 = companies.Select(x => $"{x.Name} V2"); 
            return Ok(companiesV2);
        }
    }
}
