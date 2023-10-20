using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        public EmployeeService(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper)
        {
            _repository = repositoryManager;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            Employee employeeEntity = _mapper.Map<Employee>(employeeForCreation);

            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);

            await _repository.SaveAsync();

            EmployeeDto employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

            return employeeToReturn;
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            Employee employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);

            _repository.Employee.DeleteEmployee(employeeDb);

            await _repository.SaveAsync();
        }

        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            Employee employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);

            EmployeeDto employee = _mapper.Map<EmployeeDto>(employeeDb);

            return employee;
        }

        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync
        (Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            await CheckIfCompanyExists(companyId, compTrackChanges);

            Employee employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);

            EmployeeForUpdateDto employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeDb);

            return (employeeToPatch, employeeEntity: employeeDb);
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            IEnumerable<Employee> employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges);

            IEnumerable<EmployeeDto> employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

            return employeesDto;
        }

        public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _ = _mapper.Map(employeeToPatch, employeeEntity);

            await _repository.SaveAsync();
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges)
        {
            await CheckIfCompanyExists(companyId, compTrackChanges);

            Employee employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);

            //// the trackChanges parameter will be set to true for the employeeEntity.
            //// That’s because we want EF Core to track changes on this entity.
            //// This means that as soon as we change any property in this entity,
            //// EF Core will set the state of that entity to Modified.
            _ = _mapper.Map(employeeForUpdate, employeeDb);

            //// Because our entity has a modified state,
            //// it is enough to call the Save method without any additional update actions.
            //// As soon as we call the Save method, our entity is going to be updated in the database.
            await _repository.SaveAsync();
        }

        private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
        {
            _ = await _repository.Company.GetCompanyAsync(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        }

        private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges)
        {
            Employee? employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);

            return employeeDb is null ? throw new EmployeeNotFoundException(id) : employeeDb;
        }
    }
}