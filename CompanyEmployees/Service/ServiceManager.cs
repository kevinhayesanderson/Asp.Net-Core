using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Service.Contracts;

namespace Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICompanyService> _companyService;
        private readonly Lazy<IEmployeeService> _employeeService;
        private readonly Lazy<IAuthenticationService> _authenticationService;

        public ServiceManager(
            IRepositoryManager repositoryManager,
            IMapper mapper,
            IEmployeeLinks employeeLinks,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILoggerManager logger,
            IConfiguration configuration)
        {
            _companyService = new(
                () => new CompanyService(repositoryManager, mapper),
                LazyThreadSafetyMode.PublicationOnly);
            _employeeService = new(
                () => new EmployeeService(repositoryManager, mapper, employeeLinks),
                LazyThreadSafetyMode.PublicationOnly);
            _authenticationService = new(
                () => new AuthenticationService(logger, mapper, userManager, roleManager, configuration),
                LazyThreadSafetyMode.PublicationOnly);
        }

        public ICompanyService CompanyService => _companyService.Value;

        public IEmployeeService EmployeeService => _employeeService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;
    }
}