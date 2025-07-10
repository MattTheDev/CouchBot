using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CB.Data.Entities;

public class User
{
    [Key]
    public string Id { get; set; }

    public string DisplayName { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime ModifiedDate { get; set; }

    public virtual ICollection<Guild> Guilds { get; set; }
    public virtual ICollection<Creator> Creators { get; set; }

}