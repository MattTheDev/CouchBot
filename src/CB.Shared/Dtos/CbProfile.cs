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
        CreateMap<ClipEmbed, ClipEmbedDto>().ReverseMap();
        CreateMap<Creator, CreatorDto>().ReverseMap();
        CreateMap<CreatorChannel, CreatorChannelDto>().ReverseMap();
        CreateMap<DiscordLiveConfiguration, DiscordLiveConfigurationDto>().ReverseMap();
        CreateMap<DropdownPayload, DropdownPayloadDto>().ReverseMap();
        CreateMap<GuildConfiguration, GuildConfigurationDto>().ReverseMap();
        CreateMap<Guild, GuildDto>().ReverseMap();
        CreateMap<LiveEmbed, LiveEmbedDto>().ReverseMap();
        CreateMap<MessageConfiguration, MessageConfigurationDto>().ReverseMap();
        CreateMap<RoleConfiguration, RoleConfigurationDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<VodEmbed, VodEmbedDto>().ReverseMap();
    }
}