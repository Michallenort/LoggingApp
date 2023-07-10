using AutoMapper;
using LoggingApp.Backend.Entities;
using LoggingApp.Shared.Models;

namespace LoggingApp.Backend.Mappers;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<User, UserDto>()
            .ForMember(m => m.RoleName, 
                c => c.MapFrom(s => s.Role.Name));
    }
}