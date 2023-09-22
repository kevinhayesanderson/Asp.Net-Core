using AutoMapper;
using CompanyEmployees.IDP.Entities;
using CompanyEmployees.IDP.Entities.ViewModels;

namespace CompanyEmployees.IDP;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<UserRegistrationModel, User>()
			.ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
	}
}
