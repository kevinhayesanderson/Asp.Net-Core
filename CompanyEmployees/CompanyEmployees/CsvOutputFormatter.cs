using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Shared.DataTransferObjects;
using System.Text;

namespace CompanyEmployees
{
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            //// In the constructor, we define which media type this formatter should parse as well as encodings.
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        //// The CanWriteType method is overridden,
        //// and it indicates whether or not the CompanyDto type can be written by this serializer.
        protected override bool CanWriteType(Type? type)
        {
            if (typeof(CompanyDto).IsAssignableFrom(type)
                || typeof(IEnumerable<CompanyDto>).IsAssignableFrom(type))
            {
                return base.CanWriteType(type);
            }
            return false;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var buffer = new StringBuilder();

            if (context.Object is IEnumerable<CompanyDto> companyDtos)
            {
                foreach (var company in companyDtos)
                {
                    FormatCsv(buffer, company);
                }
            }
            else if (context.Object is CompanyDto companyDto)
            {
                FormatCsv(buffer, companyDto);
            }
            await response.WriteAsync(buffer.ToString());
        }

        private static void FormatCsv(StringBuilder buffer, CompanyDto company)
        {
            buffer.AppendLine($"{company.Id},\"{company.Name},\"{company.FullAddress}\"");
        }
    }
}