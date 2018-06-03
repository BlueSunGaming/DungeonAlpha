using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Exceptions
{
    public class NodeParameterNotFoundException : Exception
    {
        public NodeParameterNotFoundException()
        {
        }

        public NodeParameterNotFoundException(string message)
            : base(message)
        {
        }

        public NodeParameterNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
