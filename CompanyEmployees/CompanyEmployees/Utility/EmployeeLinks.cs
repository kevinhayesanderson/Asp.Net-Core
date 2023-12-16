using Contracts;
using Entities.LinkModels;
using Entities.Models;
using Shared.DataTransferObjects;
using System.Net.Http.Headers;

namespace CompanyEmployees.Utility;

/// <summary>
/// We are going to use LinkGenerator to generate links for our responses and IDataShaper to shape our data.
/// </summary>
/// <param name="linkGenerator"></param>
/// <param name="dataShaper"></param>
public class EmployeeLinks(LinkGenerator _linkGenerator, IDataShaper<EmployeeDto> _dataShaper) : IEmployeeLinks
{

    ////public Dictionary<string, MediaTypeHeaderValue> AcceptHeader { get; set; } = new Dictionary<string, MediaTypeHeaderValue>();

    public LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeesDto, string fields, Guid companyId,
        HttpContext httpContext)
    {
        var shapedEmployees = ShapeData(employeesDto, fields);

        if (ShouldGenerateLinks(httpContext))
            return ReturnLinkdedEmployees(employeesDto, fields, companyId, httpContext, shapedEmployees);

        return ReturnShapedEmployees(shapedEmployees);
    }

    private List<Entity> ShapeData(IEnumerable<EmployeeDto> employeesDto, string fields) =>
        _dataShaper.ShapeData(employeesDto, fields)
            .Select(e => e.Entity)
            .ToList();

    private static bool ShouldGenerateLinks(HttpContext httpContext)
    {
        if (httpContext.Items["AcceptHeaderMediaType"] is MediaTypeHeaderValue mediaType && mediaType.MediaType != null)
        {
            return mediaType.MediaType.Contains("hateoas", StringComparison.InvariantCultureIgnoreCase);
        }
        else
        {
            return false;
        }
    }

    private static LinkResponse ReturnShapedEmployees(List<Entity> shapedEmployees) =>
        new()
        { ShapedEntities = shapedEmployees };

    /// <summary>
    /// In this method, we iterate through each employee and create links for it by calling the CreateLinksForEmployee method.
    /// Then, we just add it to the shapedEmployees collection.
    /// After that, we wrap the collection and create links that are important for the entire collection by calling the CreateLinksForEmployees method.
    /// </summary>
    /// <param name="employeesDto"></param>
    /// <param name="fields"></param>
    /// <param name="companyId"></param>
    /// <param name="httpContext"></param>
    /// <param name="shapedEmployees"></param>
    /// <returns></returns>
    private LinkResponse ReturnLinkdedEmployees(IEnumerable<EmployeeDto> employeesDto,
        string fields, Guid companyId, HttpContext httpContext, List<Entity> shapedEmployees)
    {
        var employeeDtoList = employeesDto.ToList();

        for (var index = 0; index < employeeDtoList.Count; index++)
        {
            var employeeLinks = CreateLinksForEmployee(httpContext, companyId, employeeDtoList[index].Id, fields);
            shapedEmployees[index].Add("Links", employeeLinks);
        }

        var employeeCollection = new LinkCollectionWrapper<Entity>(shapedEmployees);
        var linkedEmployees = CreateLinksForEmployees(httpContext, employeeCollection);

        return new LinkResponse { HasLinks = true, LinkedEntities = linkedEmployees };
    }

    private List<Link> CreateLinksForEmployee(HttpContext httpContext, Guid companyId, Guid id, string fields = "")
    {
        var links = new List<Link>
            {
                new(_linkGenerator.GetUriByAction(httpContext, "GetEmployeeForCompany", values: new { companyId, id, fields })!,
                "self",
                "GET"),
                new(_linkGenerator.GetUriByAction(httpContext, "DeleteEmployeeForCompany", values: new { companyId, id })!,
                "delete_employee",
                "DELETE"),
                new(_linkGenerator.GetUriByAction(httpContext, "UpdateEmployeeForCompany", values: new { companyId, id })!,
                "update_employee",
                "PUT"),
                new(_linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateEmployeeForCompany", values: new { companyId, id })!,
                "partially_update_employee",
                "PATCH")
            };
        return links;
    }

    private LinkCollectionWrapper<Entity> CreateLinksForEmployees(HttpContext httpContext,
        LinkCollectionWrapper<Entity> employeesWrapper)
    {
        employeesWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, "GetEmployeesForCompany", values: new { })!,
                "self",
                "GET"));

        return employeesWrapper;
    }
}