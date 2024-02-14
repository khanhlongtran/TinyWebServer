using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions.HttpParser.Http11
{
    // Ex: 1.1 -> first 1 is Major, second 1 is Minor
    public class Http11ProtocolVersion
    {
        public Http11ProtocolVersion(string major, string minor)
        {
            Major = major ?? throw new ArgumentNullException(nameof(major));
            Minor = minor ?? throw new ArgumentNullException(nameof(minor));
        }

        public string Major { get; }
        public string Minor { get; }
    }
    public class Http11RequestLine
    {
        /// <summary>
        ///  Request-Line   = Method SP Request-URI SP HTTP-Version CRLF
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="protocolVersion"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Http11RequestLine(string method, string url, Http11ProtocolVersion protocolVersion)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            ProtocolVersion = protocolVersion ?? throw new ArgumentNullException(nameof(protocolVersion));
        }

        public string Method { get; }
        public string Url { get; }
        public Http11ProtocolVersion ProtocolVersion { get; }
    }

    /// <summary>
    /// Name-Value Ex: Accept: ...,
    ///               User-agent:...'
    /// </summary>
    public class Http11HeaderLine
    {
        public Http11HeaderLine(string name, string value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Name { get; }
        public string Value { get; }
    }
}
