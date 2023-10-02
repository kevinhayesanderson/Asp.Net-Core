using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

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
            ////throw new Exception("Exception Test");
            var companies = _service.CompanyService.GetAllCompanies(trackChanges: false);
            return Ok(companies);

        }
    }
}
