using System;
using System.Runtime.Serialization;

namespace MTD.CouchBot.Domain.Exceptions
{
    public class TwitchClientIdMissingException : Exception
    {
        public TwitchClientIdMissingException()
        {
        }

        public TwitchClientIdMissingException(string message) : base(message)
        {
        }

        public TwitchClientIdMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TwitchClientIdMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
