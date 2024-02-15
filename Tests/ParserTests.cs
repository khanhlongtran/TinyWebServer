using TinyWebServer.Abstractions.HttpParser.Http11;
using TinyWebServer.HttpParser.Http11;

namespace Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        [DataRow("GET /path/to/file/index.html HTTP/1.1", "GET", "/path/to/file/index.html", "1", "1")]
        public void IsValidRequestLine(string text, string method, string url, string majorVersion, string minorVersion)
        {
            IHttp11Parser http11Parser = new RegexHttp11Parsers();

            var result = http11Parser.ParseRequestLine(text);
            Assert.IsNotNull(result);
            Assert.AreEqual(method, result.Method);
            Assert.AreEqual(url, result.Url);
            Assert.AreEqual(majorVersion, result.ProtocolVersion.Major);
            Assert.AreEqual(minorVersion, result.ProtocolVersion.Minor);
        }

        [TestMethod]
        [DataRow("Host: www.example.com", "Host", "www.example.com")]
        [DataRow("Accept-Language: en", "Accept-Language", "en")]
        [DataRow("Accept-Encoding: gzip, deflate, br", "Accept-Encoding", "gzip, deflate, br")]
        [DataRow("Connection: keep-alive", "Connection", "keep-alive")]
        public void IsValidHeaderLine(string text, string name, string value)
        {
            IHttp11Parser http11Parser = new RegexHttp11Parsers();

            var result = http11Parser.ParseHeaderLine(text);
            Assert.IsNotNull(result);
            Assert.AreEqual(name, result.Name);
            Assert.AreEqual(value, result.Value);
        }
    }
}