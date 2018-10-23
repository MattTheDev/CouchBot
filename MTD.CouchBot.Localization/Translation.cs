namespace MTD.CouchBot.Localization
{
    public class Translation
    {
        public string Language { get; set; }
        public string LanguageCode { get; set; }
        public Defaults Defaults { get; set; }
        public SetCommands SetCommands { get; set; }
        public LanguageCommands LanguageCommands { get; set; }
        public GroupCommands GroupCommands { get; set; }
        public Labels Labels { get; set; }
        public ChannelCommands ChannelCommands { get; set; }
        public MessageCommands MessageCommands { get; set; }
    }
}
