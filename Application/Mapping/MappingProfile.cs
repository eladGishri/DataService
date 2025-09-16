using AutoMapper;
using DataService.Application.DTOs;
using DataService.Models.Entities;

namespace DataService.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DataEntity, DataDto>().ReverseMap();
    }
}
