using AutoMapper;
using Contracts;
using Service.Contracts;

namespace Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICompanyService> _companyService;
        private readonly Lazy<IEmployeeService> _employeeService;

        public ServiceManager(IRepositoryManager repositoryManager, IMapper mapper, IEmployeeLinks employeeLinks)
        {
            _companyService = new(
                () => new CompanyService(repositoryManager, mapper),
                LazyThreadSafetyMode.PublicationOnly);
            _employeeService = new(
                () => new EmployeeService(repositoryManager, mapper, employeeLinks),
                LazyThreadSafetyMode.PublicationOnly);
        }

        public ICompanyService CompanyService => _companyService.Value;

        public IEmployeeService EmployeeService => _employeeService.Value;
    }
}