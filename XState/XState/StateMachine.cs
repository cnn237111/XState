using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XState
{
    /// <summary>
    /// 状态机类
    /// </summary>
    /// <typeparam name="TState">状态的类型</typeparam>
    /// <typeparam name="TInput">输入的类型</typeparam>
    /// <typeparam name="TOutput">输出的类型</typeparam>
    public partial class StateMachine<TState, TInput, TOutput> where TState : IEquatable<TState>
    {
        /// <summary>
        /// 状态机初始状态
        /// </summary>
        public TState OriginalState { internal set; get; }


        /// <summary>
        /// 状态机终结状态
        /// </summary>
        public TState FinalState { internal set; get; }

        /// <summary>
        /// 状态机名字
        /// </summary>
        public string StateMachineName { private set; get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="initialState">初始状态</param>
        public StateMachine(TState initialState)
        {
            OriginalState = initialState;
            CurrentState = initialState;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stateMachineName">状态机名称</param>
        public StateMachine(string stateMachineName = null)
        {
            StateMachineName = stateMachineName ?? "";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="initialState">初始状态</param>
        /// <param name="stateMachineName">状态机名称</param>
        public StateMachine(TState initialState, string stateMachineName = null)
            : this(initialState)
        {
            StateMachineName = string.IsNullOrEmpty(stateMachineName) ? Guid.NewGuid().ToString() : stateMachineName;
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        public TState CurrentState { private set; get; }

        /// <summary>
        /// 设置当前状态为
        /// </summary>
        /// <param name="toState">需要转换为当前状态</param>
        /// <returns>状态机自身</returns>
        public StateMachine<TState, TInput, TOutput> SetCurrentStateTo(TState toState)
        {
            this.CurrentState = toState;
            return this;
        }

        /// <summary>
        /// 重置状态机至初始化状态
        /// </summary>
        /// <returns>状态机自身</returns>
        public StateMachine<TState, TInput, TOutput> Reset()
        {
            this.CurrentState = OriginalState;
            return this;
        }

        public bool IsFinal()
        {
            return this.CurrentState.Equals(this.FinalState);
        }

        /// <summary>
        /// 创建状态
        /// </summary>
        /// <param name="state">创建状态</param>
        /// <returns>状态机本身</returns>
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

        private Dictionary<TState, StateConfiguration> _configurations = new Dictionary<TState, StateConfiguration>();
        /// <summary>
        /// 状态机的配置信息表
        /// </summary>
        internal Dictionary<TState, StateConfiguration> Configurations { get { return _configurations; } }

        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="fromState">源状态</param>
        /// <param name="input">输入</param>
        /// <param name="output">输出</param>
        /// <returns>状态机本身</returns>
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

                if (conf.DlgAbortWhen != null)
                {
                    foreach (Delegate dele in conf.DlgAbortWhen.GetInvocationList())
                    {
                        bool IsAbort = ((StateConfiguration.DelegateAbortWhen)dele)(input, output);
                        if (IsAbort)    //如果放弃执行
                        {
                            if (conf.AbortAction != null)
                                conf.AbortAction();
                            return this;
                        }
                    }

                }

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
                throw new InvalidInputException(this.CurrentState.ToString(), input.ToString());
                //output = default(TOutput);
                //return this;
            }

        }
        /// <summary>
        /// 改变状态，以当前状态为源状态
        /// </summary>
        /// <param name="input">输入</param>
        /// <param name="output">输出</param>
        /// <returns>状态机本身</returns>
        public StateMachine<TState, TInput, TOutput> ChangeState(TInput input, out TOutput output)
        {
            return ChangeState(this.CurrentState, input, out output);
        }

        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="fromState">源状态</param>
        /// <param name="input">输入</param>
        /// <returns>状态机本身</returns>
        public StateMachine<TState, TInput, TOutput> ChangeState(TState fromState, TInput input)
        {
            TOutput output;
            return ChangeState(fromState, input, out  output);
        }
        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns>状态机本身</returns>
        public StateMachine<TState, TInput, TOutput> ChangeState(TInput input)
        {
            return ChangeState(this.CurrentState, input);
        }

        /// <summary>
        /// 获取状态配置信息
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>状态配置，无效状态会导致异常</returns>
        public StateConfiguration GetStateConfiguration(TState state)
        {
            if (ContainsState(state))
                return Configurations[state];
            else
                throw new InvalidStateException(state.ToString());
        }

        /// <summary>
        /// 检测是否包含某状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>包含则返回True</returns>
        public bool ContainsState(TState state)
        {
            return Configurations.ContainsKey(state);
        }

        /// <summary>
        /// 返回所有的状态集合
        /// </summary>
        public IEnumerable<TState> AllStates
        {
            get
            {
                return Configurations.Keys;
            }
        }

        /// <summary>
        /// 判断两个状态之间转换是否合法
        /// </summary>
        /// <param name="fromState">源状态</param>
        /// <param name="toState">目标状态</param>
        /// <returns>合法则返回Ture，反之则反</returns>
        public bool CanChangeTo(TState fromState, TState toState)
        {
            if (!ContainsState(fromState) || !ContainsState(toState))
                return false;
            var conf = GetStateConfiguration(fromState);
            return conf.Triggers.Where(x => x.NextState.Equals(toState)).Count() > 0;
        }

        /// <summary>
        /// 判断当前状态转换到目标状态是否合法
        /// </summary>
        /// <param name="toState"></param>
        /// <returns>合法则返回Ture，反之则反</returns>
        public bool CanChangeTo(TState toState)
        {
            return CanChangeTo(this.CurrentState, toState);
        }

        /// <summary>
        /// 查找某个状态后续状态
        /// </summary>
        /// <param name="state">某状态</param>
        /// <returns>返回后续状态的列表</returns>
        public List<TState> FindNextState(TState state)
        {
            var conf=GetStateConfiguration(state);
            return conf.Triggers.Select(x => x.NextState).ToList();
        }

        /// <summary>
        /// 查找某个状态前续状态
        /// </summary>
        /// <param name="state">某状态</param>
        /// <returns>返回前续状态的列表</returns>
        public List<TState> FindPreviousState(TState state)
        {
            List<TState> lstPreviousState = new List<TState>(); 
            foreach(var conf in Configurations)
            {
                if( conf.Value.Triggers.Where(x=>x.NextState.Equals(state)).Count()>0)
                {
                    lstPreviousState.Add(conf.Value.State);
                }
            }
            return lstPreviousState;
        }

        /// <summary>
        /// 查找某个状态的可输入值
        /// </summary>
        /// <param name="state">某状态</param>
        /// <returns>返回某个状态的可接受的输入值</returns>
        public List<TInput> GetValidInputs(TState state)
        {
            var conf = GetStateConfiguration(state);
            return conf.Triggers.Select(x => x.Input).ToList();
        }
    }
}
