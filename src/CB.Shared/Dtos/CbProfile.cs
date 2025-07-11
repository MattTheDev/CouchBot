using AutoMapper;
using CB.Data.Entities;

namespace CB.Shared.Dtos;

public class CbProfile : Profile
{
    public CbProfile()
    {
        CreateMap<AllowConfiguration, AllowConfigurationDto>().ReverseMap();
        CreateMap<ChannelConfiguration, ChannelConfigurationDto>().ReverseMap();
        CreateMap<Channel, ChannelDto>().ReverseMap();
        CreateMap<Creator, CreatorDto>().ReverseMap();
        CreateMap<GuildConfiguration, GuildConfigurationDto>().ReverseMap();
        CreateMap<Guild, GuildDto>().ReverseMap();
        CreateMap<MessageConfiguration, MessageConfigurationDto>().ReverseMap();
        CreateMap<RoleConfiguration, RoleConfigurationDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();

    }
}