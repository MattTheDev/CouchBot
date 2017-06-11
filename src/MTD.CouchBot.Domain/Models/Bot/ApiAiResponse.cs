using System;
using System.Collections.Generic;
using System.Text;

namespace MTD.CouchBot.Domain.Models.Bot
{
    public class ApiAiResponse
    {
        public class Parameters
        {
        }

        public class Metadata
        {
            public List<object> inputContexts { get; set; }
            public List<object> outputContexts { get; set; }
            public List<object> contexts { get; set; }
        }

        public class Result
        {
            public string source { get; set; }
            public string resolvedQuery { get; set; }
            public string speech { get; set; }
            public string action { get; set; }
            public Parameters parameters { get; set; }
            public Metadata metadata { get; set; }
            public double score { get; set; }
        }

        public class Status
        {
            public int code { get; set; }
            public string errorType { get; set; }
        }

        public string id { get; set; }
        public string timestamp { get; set; }
        public string lang { get; set; }
        public Result result { get; set; }
        public Status status { get; set; }
        public string sessionId { get; set; }
    }
}
