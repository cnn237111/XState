using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XState
{
    public partial class StateMachine<TState, TInput, TOutput>
    {
        [Serializable]
        public class Trigger : IComparable
        {
            public TState NextState { set; get; }
            public TInput Input { set; get; }
            public TOutput Output { set; get; }
            public int CompareTo(object obj)
            {
                return obj.GetHashCode()-this.GetHashCode();
            }

        }
    }
}
