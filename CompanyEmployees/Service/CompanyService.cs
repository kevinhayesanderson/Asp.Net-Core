﻿using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    internal sealed class CompanyService : ICompanyService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompanyService(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _mapper = mapper;
        }

        public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges)
        {
            var companies = _repositoryManager.Company.GetAllCompanies(trackChanges);
            ////var companiesDto = companies.Select(c => new CompanyDto(c.Id, c.Name ?? "", string.Join(' ', c.Address, c.Country)))
            ////                            .ToList();
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return companiesDto;
        }

        public CompanyDto GetCompany(Guid companyId, bool trackChanges)
        {
            var company = _repositoryManager.Company.GetCompany(
                companyId,
                trackChanges) ?? throw new CompanyNotFoundException(companyId);
            var companyDto = _mapper.Map<CompanyDto>(company);
            return companyDto;
        }
    }
}