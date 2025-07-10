using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CB.Data.Entities;

public class Guild
{
    [Key]
    public string Id { get; set; }

    public string DisplayName { get; set; }

    public string OwnerId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual User User { get; set; }
}