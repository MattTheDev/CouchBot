namespace MTD.CouchBot.Domain.Models
{
    public class BeamPayload
    {
        public class Payload
        {
            public bool online { get; set; }
        }

        public class Data
        {
            public string channel { get; set; }
            public Payload payload { get; set; }
        }

        public string type { get; set; }
        public string @event { get; set; }
        public Data data { get; set; }
    }
}
