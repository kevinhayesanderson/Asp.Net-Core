using Entities.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController(LinkGenerator linkGenerator) : ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType)
        {
            if (mediaType.Contains("application/vnd.kevin.apiroot"))
            {
                var list = new List<Link>
                {
                    new()
                    {
                        Href = linkGenerator.GetUriByName(HttpContext, nameof(GetRoot), new {}) ?? string.Empty,
                        Rel = "self",
                        Method = "GET"
                    },
                    new()
                    {
                        Href = linkGenerator.GetUriByName(HttpContext, "GetCompanies", new {}) ?? string.Empty,
                        Rel = "companies",
                        Method = "GET"
                    },
                    new()
                    {
                        Href = linkGenerator.GetUriByName(HttpContext, "CreateCompany", new {}) ?? string.Empty,
                        Rel = "create_company",
                        Method = "POST"
                    }
                };

                return Ok(list);
            }

            return NoContent();
        }
    }
}