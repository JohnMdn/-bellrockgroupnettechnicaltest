using System.Security.Cryptography.Xml;

namespace DotNetInterview.API.Utility
{
    public static class DefaultGuid
    {
        public static Guid Default { get; set; } = new Guid("ACDA4EC8-8D11-410B-8C16-D5B4EE506FD7");
    }
}
