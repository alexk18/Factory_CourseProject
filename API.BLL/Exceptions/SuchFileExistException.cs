namespace API.BLL.Exceptions
{
    [Serializable]
    public class SuchFileExistException : Exception
    {
        public string FileName { get; } = string.Empty;
        public SuchFileExistException() { }

        public SuchFileExistException(string message)
            : base(message) { }

        public SuchFileExistException(string message, Exception inner)
            : base(message, inner) { }

        public SuchFileExistException(string message, string fileName)
        : this(message)
        {
            FileName = fileName;
        }
    }
}
