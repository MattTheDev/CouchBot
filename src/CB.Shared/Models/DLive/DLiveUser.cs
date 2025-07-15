namespace CB.Shared.Models.DLive;

public class DLiveUser
{
    public Data Data { get; set; }
}

public class UserByDisplayName
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string DisplayName { get; set; }
}
public class Followers
{
    public int TotalCount { get; set; }
}

public class Category
{
    public string Title { get; set; }
}

public class Livestream
{
    public string ThumbnailUrl { get; set; }
    public string Title { get; set; }
    public int WatchingCount { get; set; }
    public Category Category { get; set; }
}

public class User
{
    public string DisplayName { get; set; }
    public string Avatar { get; set; }
    public Followers Followers { get; set; }
    public Livestream Livestream { get; set; }
}

public class Data
{
    public UserByDisplayName UserByDisplayName { get; set; }
    public User User { get; set; }
}