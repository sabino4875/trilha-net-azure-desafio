using AutoMapper;
using System;
using TrilhaNetAzureDesafio.Models;
using TrilhaNetAzureDesafio.ViewModels;

namespace TrilhaNetAzureDesafio.Profiles
{
    public class FuncionarioProfile : Profile
    {
        public FuncionarioProfile() 
        {
            CreateMap<Funcionario, FuncionarioViewModel>();
            CreateMap<FuncionarioViewModel, Funcionario>();
            CreateMap<AddFuncionarioViewModel, Funcionario>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
        }
    }
}
