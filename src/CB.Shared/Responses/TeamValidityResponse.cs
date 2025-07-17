using CB.Shared.Models.Twitch;

namespace CB.Shared.Responses;

public class TeamValidityResponse : ValidityResponse
{
    public TwitchTeam.Datum TwitchTeam { get; set; }
}