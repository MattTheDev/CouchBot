namespace CB.Shared.Models.Trovo;

public class TrovoChannel
{
    public bool is_live { get; set; }
    public string category_id { get; set; }
    public string category_name { get; set; }
    public string live_title { get; set; }
    public string audi_type { get; set; }
    public string language_code { get; set; }
    public string thumbnail { get; set; }
    public int current_viewers { get; set; }
    public int followers { get; set; }
    public string streamer_info { get; set; }
    public string profile_pic { get; set; }
    public string channel_url { get; set; }
    public string created_at { get; set; }
}