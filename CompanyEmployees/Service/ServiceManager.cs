using AutoMapper;
using Contracts;
using Service.Contracts;

namespace Service
{
    public sealed class ServiceManager(IRepositoryManager repositoryManager, IMapper mapper, IEmployeeLinks employeeLinks) : IServiceManager
    {
        private readonly Lazy<ICompanyService> _companyService =
            new(
                () => new CompanyService(repositoryManager, mapper),
                LazyThreadSafetyMode.PublicationOnly);
        private readonly Lazy<IEmployeeService> _employeeService =
            new(
                () => new EmployeeService(repositoryManager, mapper, employeeLinks),
                LazyThreadSafetyMode.PublicationOnly);

        public ICompanyService CompanyService
        {
            get
            {
                return _companyService.Value;
            }
        }

        public IEmployeeService EmployeeService => _employeeService.Value;
    }
}