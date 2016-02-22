using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
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
