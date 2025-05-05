
namespace BLL.CustomException
{
    public class ConflictException : Exception
    {
        public ConflictException() : base() { }

        public ConflictException(string message) : base(message) { }
    }
}
