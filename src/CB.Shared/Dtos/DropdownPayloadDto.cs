namespace CB.Shared.Dtos;

public class DropdownPayloadDto
{
    public int Id { get; set; }
    public string Payload { get; set; }
    public string DropdownType { get; set; }
    public string OriginalMessageId { get; set; }
}