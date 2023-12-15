using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;

namespace Repository
{
    public class EmployeeRepository(RepositoryContext repositoryContext) : RepositoryBase<Employee>(repositoryContext), IEmployeeRepository
    {
        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }

        //// table has few rows
        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            var employees = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
                .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
                .Search(employeeParameters.SearchTerm)
                .Sort(employeeParameters.OrderBy)
                .ToListAsync();

            return PagedList<Employee>
                .ToPagedList(employees, employeeParameters.PageNumber, employeeParameters.PageSize);
        }

        ////// table has millions of rows
        //public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        //{
        //    var employees = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
        //        .OrderBy(e => e.Name)
        //        .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
        //        .Take(employeeParameters.PageSize)
        //        .ToListAsync();

        //    var count = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
        //        .CountAsync();

        //    return new PagedList<Employee>(employees,
        //                                   count,
        //                                   employeeParameters.PageNumber,
        //                                   employeeParameters.PageSize);
        //}

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>
            await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
            .SingleOrDefaultAsync();
    }
}