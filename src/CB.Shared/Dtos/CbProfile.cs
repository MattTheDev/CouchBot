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
        CreateMap<Channel, ChannelConfigurationSummaryDto>().ReverseMap();
        CreateMap<ClipEmbed, ClipEmbedDto>().ReverseMap();
        CreateMap<Creator, CreatorDto>().ReverseMap();
        CreateMap<CreatorChannel, CreatorChannelDto>().ReverseMap();
        CreateMap<DiscordLiveConfiguration, DiscordLiveConfigurationDto>().ReverseMap();
        CreateMap<DropdownPayload, DropdownPayloadDto>().ReverseMap();
        CreateMap<Filter, FilterDto>().ReverseMap();
        CreateMap<FilterType, FilterTypeDto>().ReverseMap();
        CreateMap<Game, GameDto>().ReverseMap();
        CreateMap<GameChannel, GameChannelDto>().ReverseMap();
        CreateMap<GuildConfiguration, GuildConfigurationDto>().ReverseMap();
        CreateMap<Guild, GuildConfigurationSummaryDto>().ReverseMap();
        CreateMap<Guild, GuildDto>().ReverseMap();
        CreateMap<LiveEmbed, LiveEmbedDto>().ReverseMap();
        CreateMap<MessageConfiguration, MessageConfigurationDto>().ReverseMap();
        CreateMap<Platform, PlatformDto>().ReverseMap();
        CreateMap<RoleConfiguration, RoleConfigurationDto>().ReverseMap();
        CreateMap<Team, TeamDto>().ReverseMap();
        CreateMap<TeamChannel, TeamChannelDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<VodEmbed, VodEmbedDto>().ReverseMap();
    }
}