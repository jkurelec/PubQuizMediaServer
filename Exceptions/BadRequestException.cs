namespace PubQuizMediaServer.Exceptions
{
    public class BadRequestException(string message) : Exception(message)
    {
        public BadRequestException() : this("") { }
    }
}
