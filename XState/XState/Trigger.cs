using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XState
{
    public partial class StateMachine<TState, TInput, TOutput>
    {
        public class Trigger
        {
            public TState NextState { set; get; }
            public TInput Input { set; get; }
            public TOutput Output { set; get; }
        }
    }
}
