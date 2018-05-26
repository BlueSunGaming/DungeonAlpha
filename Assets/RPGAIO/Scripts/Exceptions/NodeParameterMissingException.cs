using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Exceptions
{
    public class NodeParameterMissingException : Exception
    {
        public NodeParameterMissingException()
        {
        }

        public NodeParameterMissingException(string message)
            : base(message)
        {
        }

        public NodeParameterMissingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
