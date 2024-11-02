namespace DotNetInterview.API.Utility
{
    public class MockedDateTimeScope : IDisposable
    {
        private static DateTime? _fixedDateTime;
        private readonly DateTime? _previousDateTime;

        public MockedDateTimeScope(DateTime fixedDateTime)
        {
            _previousDateTime = _fixedDateTime;
            _fixedDateTime = fixedDateTime;
        }

        public void Dispose()
        {
            _fixedDateTime = _previousDateTime;
        }

        public static DateTime Now => _fixedDateTime ?? DateTime.Now;
    }
}
