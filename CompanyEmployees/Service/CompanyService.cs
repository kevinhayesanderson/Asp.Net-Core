using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    internal sealed class CompanyService : ICompanyService
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;

        public CompanyService(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper)
        {
            _repository = repositoryManager;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company)
        {
            Company companyEntity = _mapper.Map<Company>(company);

            _repository.Company.CreateCompany(companyEntity);
            await _repository.SaveAsync();

            CompanyDto companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return companyToReturn;
        }

        public async Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                throw new CompanyCollectionBadRequestException();
            }

            IEnumerable<Company> companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (Company company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
            await _repository.SaveAsync();

            IEnumerable<CompanyDto> companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            string ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));

            return (companies: companyCollectionToReturn, ids);
        }

        public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
        {
            Company company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);
            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();
        }

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges)
        {
            IEnumerable<Company> companies = await _repository.Company.GetAllCompaniesAsync(trackChanges);
            ////var companiesDto = companies.Select(c => new CompanyDto(c.Id, c.Name ?? "", string.Join(' ', c.Address, c.Country)))
            ////                            .ToList();
            IEnumerable<CompanyDto> companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return companiesDto;
        }

        public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        {
            if (ids is null)
            {
                throw new IdParametersBadRequestException();
            }

            IEnumerable<Company> companyEntities = await _repository.Company.GetByIdsAsync(ids, trackChanges);
            if (ids.Count() != companyEntities.Count())
            {
                throw new CollectionByIdsBadRequestException();
            }

            IEnumerable<CompanyDto> companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return companiesToReturn;
        }

        public async Task<CompanyDto> GetCompanyAsync(Guid companyId, bool trackChanges)
        {
            Company company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);
            CompanyDto companyDto = _mapper.Map<CompanyDto>(company);
            return companyDto;
        }

        public async Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges)
        {
            Company company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);
            _ = _mapper.Map(companyForUpdate, company);
            await _repository.SaveAsync();
        }

        private async Task<Company> GetCompanyAndCheckIfItExists(Guid id, bool trackChanges)
        {
            Company? company = await _repository.Company.GetCompanyAsync(id, trackChanges);
            return company is null ? throw new CompanyNotFoundException(id) : company;
        }
    }
}