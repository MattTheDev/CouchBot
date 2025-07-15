using CB.Accessors.Contracts;
using CB.Shared.Models.DLive;
using CB.Shared.Utilities;
using Newtonsoft.Json;

namespace CB.Accessors.Implementations;

public class DLiveAccessor : IDLiveAccessor
{
    private const string DLiveUrl = "https://graphigo.prd.dlive.tv/";
    private const string GetUserByDisplayNameQuery =
        "query {" +
        "userByDisplayName(displayname:\"%DISPLAYNAME%\") {" +
        "   id" +
        "   username" +
        "   displayname" +
        "}" +
        "}";

    private const string GetUserByUsernameQuery =
        "query {" +
        "user(username:\"%USERNAME%\") { " +
        "displayname " +
        "avatar " +
        "followers {" +
        "totalCount " +
        "} " +
        "livestream { " +
        "thumbnailUrl " +
        "title " +
        "watchingCount " +
        "category " +
        "{ " +
        "title " +
        "} " +
        "} " +
        "} " +
        "}";

    public async Task<DLiveUser> GetUserByDisplayNameAsync(string displayName)
    {
        var query = GetUserByDisplayNameQuery.Replace("%DISPLAYNAME%", displayName);

        var dliveQuery = new DLiveQuery
        {
            Query = query,
            OperationName = null,
            Variables = null
        };

        return await ApiUtilities
            .PostApiHelper<DLiveUser>(DLiveUrl, 
                payloadString: JsonConvert.SerializeObject(dliveQuery))
            .ConfigureAwait(false);
    }

    public async Task<DLiveUser> GetUserByUsernameAsync(string username)
    {
        var query = GetUserByUsernameQuery.Replace("%USERNAME%", username);

        var dliveQuery = new DLiveQuery
        {
            Query = query,
            OperationName = null,
            Variables = null
        };

        return await ApiUtilities
            .PostApiHelper<DLiveUser>(DLiveUrl, 
                payloadString: JsonConvert.SerializeObject(dliveQuery))
            .ConfigureAwait(false);
    }
}

public class DLiveQuery
{
    public string Query { get; set; }

    public object Variables { get; set; }

    public object OperationName { get; set; }
}