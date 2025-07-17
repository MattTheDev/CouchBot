using System.ComponentModel.DataAnnotations;

namespace CB.Data.Entities;

public class Game
{
    [Key]
    public int Id { get; set; }

    public string DisplayName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public int PlatformId { get; set; }
    public Platform Platform { get; set; }
    public string GameId { get; set; }

    public virtual ICollection<GameChannel> GameChannels { get; set; }
}