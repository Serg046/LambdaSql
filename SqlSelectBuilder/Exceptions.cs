using System;
using GuardExtensions;

namespace SqlSelectBuilder
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
}
