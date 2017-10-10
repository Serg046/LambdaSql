using System;
using GuardExtensions;

namespace LambdaSql
{
    public class JoinException : Exception
    {
        public JoinException(string message) : base(message)
        {
            Guard.IsNotNull(message);
        }
    }

    public class DuplicateAliasException : Exception
    {
        public DuplicateAliasException(string message) : base(message)
        {
            Guard.IsNotNull(message);
        }
    }

    public class IncorrectAliasException : Exception
    {
        public IncorrectAliasException(string message) : base(message)
        {
            Guard.IsNotNull(message);
        }
    }
}
