using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XState
{
    public partial class StateMachine<TState, TInput, TOutput>
    {
        public TState OriginalState { internal set; get; }

        public string StateMachineName { set; get; }
        public StateMachine()
        {

        }
        public StateMachine(TState initialState)
        {
            OriginalState = initialState;
            CurrentState = initialState;
        }

        public StateMachine(string stateMachineName)
        {
            StateMachineName = stateMachineName ?? "";
        }

        public StateMachine(TState initialState, string stateMachineName)
            : this(initialState)
        {
            StateMachineName = stateMachineName ?? "";
        }

        public TState CurrentState { private set; get; }

        public StateMachine<TState, TInput, TOutput> SetCurrentStateTo(TState currentState)
        {
            this.CurrentState = currentState;
            return this;
        }
        public StateConfiguration CreateState(TState state)
        {
            StateConfiguration stateConfiguration = null;
            if (!Configurations.TryGetValue(state, out stateConfiguration))
            {
                stateConfiguration = new StateConfiguration(state);
                stateConfiguration.Owner = this;
                Configurations.Add(state, stateConfiguration);
            }

            return stateConfiguration;

        }

        internal Dictionary<TState, StateConfiguration> Configurations = new Dictionary<TState, StateConfiguration>();

        public StateMachine<TState, TInput, TOutput> ChangeState(TState fromState, TInput input, out TOutput output)
        {
            StateConfiguration conf;
            if (Configurations.ContainsKey(fromState))
                conf = Configurations[fromState];
            else
                throw new InvalidStateException(fromState.ToString());

            Trigger trigger = conf.Triggers.FirstOrDefault(x => x.Input.Equals(input));

            if (trigger != null && trigger.Input.Equals(input))
            {
                output = trigger.Output;
                this.CurrentState = trigger.NextState;

                //trigger EntryAction
                if (Configurations.ContainsKey(trigger.NextState))
                {
                    StateConfiguration conf_next = Configurations[trigger.NextState];
                    conf_next.ExecuteEntryAction(fromState, input, output);
                }

                //trigger QuitAction
                conf.ExecuteQuitAction(trigger.NextState, input, output);
                return this;

            }
            else
            {
                throw new InvalidInputException(input.ToString());
                //output = default(TOutput);
                //return this;
            }

        }
        public StateMachine<TState, TInput, TOutput> ChangeState(TInput input, out TOutput output)
        {
            return ChangeState(this.CurrentState, input, out output);
        }
        public StateMachine<TState, TInput, TOutput> ChangeState(TState fromState, TInput input)
        {
            TOutput output;
            return ChangeState(fromState, input, out  output);
        }
        public StateMachine<TState, TInput, TOutput> ChangeState(TInput input)
        {
            return ChangeState(this.CurrentState, input);
        }
        public StateConfiguration GetStateConfiguration(TState state)
        {
            if (ContainsState(state))
                return Configurations[state];
            else
                throw new InvalidStateException(state.ToString());
        }

        public bool ContainsState(TState state)
        {
            return Configurations.ContainsKey(state);
        }

        public IEnumerable<TState> AllStates
        {
            get
            {
                return Configurations.Keys;
            }
        }

        public bool CanChangeTo(TState fromState, TState toState)
        {
            if (!ContainsState(fromState) || !ContainsState(toState))
                return false;
            var conf = GetStateConfiguration(fromState);
            return conf.Triggers.Where(x => x.NextState.Equals(toState)).Count() > 0;
        }

        public bool CanChangeTo(TState toState)
        {
            return CanChangeTo(this.CurrentState, toState);
        }
    }
}
