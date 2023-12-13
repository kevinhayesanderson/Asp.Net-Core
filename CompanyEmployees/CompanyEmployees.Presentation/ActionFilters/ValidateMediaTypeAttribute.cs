using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;

namespace CompanyEmployees.Presentation.ActionFilters
{
    public class ValidateMediaTypeAttribute : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        //// Before action
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var acceptHeaderPresent = context.HttpContext.Request.Headers.ContainsKey("Accept");

            if (!acceptHeaderPresent)
            {
                context.Result = new BadRequestObjectResult("Accept header is missing");
                return;
            }

            var mediaType = context.HttpContext.Request.Headers.Accept.FirstOrDefault();
            if (!MediaTypeHeaderValue.TryParse(mediaType, out var mediaTypeValue))
            {
                context.Result = new BadRequestObjectResult($"Media type not present. Please add valid Accept hader with the required media type.");
                return;
            }

            context.HttpContext.Items.Add("AcceptHeaderMediaType", mediaTypeValue);
        }
    }
}
