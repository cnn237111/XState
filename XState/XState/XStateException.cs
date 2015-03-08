using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XState
{
    class InvalidStateException : Exception
    {
        public InvalidStateException(string value)
            : base(String.Format("State is invalid or undefined. Value: {0}", value)) { }
    }

    class InvalidInputException : Exception
    {
        public InvalidInputException(string value)
            : base(String.Format("Invalid input Value. Value: {0}", value)) { }
    }
}
