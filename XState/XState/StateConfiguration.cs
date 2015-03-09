using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XState
{
    public partial class StateMachine<TState, TInput, TOutput>
    {
        /// <summary>
        /// 状态配置
        /// </summary>
        public class StateConfiguration
        {
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="state">待配置的状态</param>
            public StateConfiguration(TState state)
            {
                State = state;
                Triggers = new List<Trigger>();
            }

            /// <summary>
            /// 待配置的状态
            /// </summary>
            public TState State { private set; get; }

            /// <summary>
            /// 状态触发规则
            /// </summary>
            public List<Trigger> Triggers { private set; get; }
            
            /// <summary>
            /// 进入状态的委托方法
            /// </summary>
            /// <param name="fromState">源状态</param>
            /// <param name="input">输入</param>
            /// <param name="output">输出</param>
            public delegate void DelegateEntryAction(TState fromState, TInput input, TOutput output);
            
            /// <summary>
            /// 离开状态的委托方法
            /// </summary>
            /// <param name="toState">目标状态</param>
            /// <param name="input">输入</param>
            /// <param name="output">输出</param>
            public delegate void DelegateQuitAction(TState toState, TInput input, TOutput output);

            /// <summary>
            ///进入状态是触发的事件
            /// </summary>
            internal event DelegateEntryAction EntryAction;
            /// <summary>
            /// 离开状态触发的事件
            /// </summary>
            internal event DelegateQuitAction QuitAction;

            /// <summary>
            /// 设定状态转移规则
            /// </summary>
            /// <param name="input">输入</param>
            /// <param name="nextState">下一个状态</param>
            /// <param name="output">输出</param>
            /// <returns>返回配置项本身</returns>
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

            /// <summary>
            /// 将该状态设为状态机中的初始状态
            /// </summary>
            /// <returns>返回配置项本身</returns>
            public StateConfiguration AsOriginalState()
            {
                Owner.OriginalState = this.State;
                return this;
            }

            /// <summary>
            /// 进入状态触发的方法调用
            /// </summary>
            /// <param name="entryAction">委托方法</param>
            /// <returns>返回配置项本身</returns>
            public StateConfiguration OnEntry(DelegateEntryAction entryAction)
            {
                EntryAction += entryAction;
                return this;
            }
            /// <summary>
            /// 离开状态触发的方法调用
            /// </summary>
            /// <param name="quitAction">委托方法</param>
            /// <returns></returns>
            public StateConfiguration OnQuit(DelegateQuitAction quitAction)
            {
                QuitAction += quitAction;
                return this;
            }

            /// <summary>
            /// 执行进入方法的调用
            /// </summary>
            /// <param name="fromState">源状态</param>
            /// <param name="input">输入</param>
            /// <param name="output">输出</param>
            public void ExecuteEntryAction(TState fromState, TInput input, TOutput output)
            {
                if (EntryAction != null)
                    EntryAction(fromState, input, output);
            }

            /// <summary>
            /// 执行退出方法的调用
            /// </summary>
            /// <param name="toState">目标状态</param>
            /// <param name="input">输入</param>
            /// <param name="output">输出</param>
            internal void ExecuteQuitAction(TState toState, TInput input, TOutput output)
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
