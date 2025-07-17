using System.ComponentModel.DataAnnotations;

namespace CB.Data.Entities;

public class DropdownPayload
{
    [Key]
    public int Id { get; set; }
    public string Payload { get; set; }
    public string DropdownType { get; set; }
    public string OriginalMessageId { get; set; }
}