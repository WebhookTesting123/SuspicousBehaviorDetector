using AutoMapper;
using Domain;

namespace Gateway.Github.Dtos;

public class DtosMappingProfile : Profile
{
    public DtosMappingProfile()
    {
        CreateMap<EventDto, Event>();
        CreateMap<RepositoryDto, Repository>();
        CreateMap<TeamDto, Team>();
    }
    
}