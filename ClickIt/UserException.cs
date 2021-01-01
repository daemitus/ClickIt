using System;

namespace ClickItPlugin
{
    public class UserException : Exception
    {
        public UserException(string message) : base(message) { }
    }
}
