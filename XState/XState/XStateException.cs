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
        public InvalidInputException(string currentState, string input)
            : base(String.Format("Invalid input Value. {Current State:{0},Input:{1}}", currentState, input)) { }
    }
}
