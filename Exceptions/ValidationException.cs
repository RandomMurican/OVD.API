using System;
namespace OVD.API.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string Message) : base(Message)
        {
        }
    }
}