using Entities.Models;
using Repository.Extensions.Utility;
using System.Linq.Dynamic.Core;

namespace Repository.Extensions
{
    public static class RepositoryEmployeeExtensions
    {
        public static IQueryable<Employee> FilterEmployees(this IQueryable<Employee> employees, uint minAge, uint maxAge)
            => employees.Where(e => e.Age >= minAge && e.Age <= maxAge);

        public static IQueryable<Employee> Search(this IQueryable<Employee> employees, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return employees;

            var lowerCaseTerm = searchTerm.Trim().ToLower();

            return employees.Where(e => !string.IsNullOrEmpty(e.Name) && e.Name.Contains(lowerCaseTerm, StringComparison.CurrentCultureIgnoreCase));
        }

        public static IQueryable<Employee> Sort(this IQueryable<Employee> employees, string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return employees.OrderBy(e => e.Name);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Employee>(orderByQueryString);

            return string.IsNullOrWhiteSpace(orderQuery) ? employees.OrderBy(e => e.Name) : (IQueryable<Employee>)employees.OrderBy(orderQuery);
        }
    }
}