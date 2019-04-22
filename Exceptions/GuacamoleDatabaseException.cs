using System;
namespace OVD.API.Exceptions
{
    public class GuacamoleDatabaseException : Exception
    {
        public GuacamoleDatabaseException(string Message) : base(Message)
        {
        }
    }
}