using System;

namespace IntegrationApi.Entries
{
    public class Log
    {
        public Guid Id { get; set; }
        public string HeaderValue { get; set; }
        public DateTime Time { get; set; }

        internal Log() {}

        public Log(string headerValue)
        {
            HeaderValue = headerValue;
        }
    }
}