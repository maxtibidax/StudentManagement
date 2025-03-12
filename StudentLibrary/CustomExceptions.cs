using System;

namespace StudentLibrary
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException() : base("Ошибка аутентификации") { }
        public AuthenticationException(string message) : base(message) { }
        public AuthenticationException(string message, Exception inner) : base(message, inner) { }
    }

    public class DataOperationException : Exception
    {
        public DataOperationException() : base("Ошибка операции с данными") { }
        public DataOperationException(string message) : base(message) { }
        public DataOperationException(string message, Exception inner) : base(message, inner) { }
    }
}
