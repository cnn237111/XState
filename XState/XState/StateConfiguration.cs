using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XState
{
    public partial class StateMachine<TState, TInput, TOutput>
    {
        public class StateConfiguration
        {
            public StateConfiguration(TState _state)
            {
                State = _state;
                Triggers = new List<Trigger>();
            }


            public TState State;
            public List<Trigger> Triggers;
            public delegate void DelegateEntryAction(TState fromState, TInput input, TOutput output);
            public delegate void DelegateQuitAction(TState toState, TInput input, TOutput output);
            internal event DelegateEntryAction EntryAction;
            internal event DelegateQuitAction QuitAction;
            public StateConfiguration Rule(TInput input, TState nextState, TOutput output = default(TOutput))
            {
                this.Triggers.Add(new Trigger
                {
                    Input = input,
                    NextState = nextState,
                    Output = output
                });
                return this;
            }

            public StateConfiguration AsOriginalState()
            {
                Owner.OriginalState = this.State;
                return this;
            }

            public StateConfiguration OnEntry(DelegateEntryAction entryAction)
            {
                EntryAction += entryAction;
                return this;
            }

            public StateConfiguration OnQuit(DelegateQuitAction quitAction)
            {
                QuitAction += quitAction;
                return this;
            }

            public void ExecuteEntryAction(TState fromState, TInput input, TOutput output)
            {
                if (EntryAction != null)
                    EntryAction(fromState, input, output);
            }

            public void ExecuteQuitAction(TState toState, TInput input, TOutput output)
            {
                if (QuitAction != null)
                    QuitAction(toState, input, output);
            }
            /// <summary>
            /// 该配置所属的Owner状态机
            /// </summary>
            public StateMachine<TState, TInput, TOutput> Owner { get; set; }
        }
    }
}
