using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XState;

namespace Demo
{
    class Program
    {
        class OrderState
        {
            public const string Initial = "Initial";
            public const string Pending = "Pending";
            public const string Passed = "Passed";
            public const string Rejected = "Rejected";
            public const string Shipping = "Shipping";
            public const string Finished = "Finished";
        }

        class OrderInput
        {
            public const string Submit = "Submit";
            public const string Agree = "Agree";
            public const string Deny = "Deny";
            public const string I_Know = "I_Know";
            public const string I_Edit = "I_Edit";
            public const string OK = "OK";
        }

        static void Main(string[] args)
        {

            StateMachine<string, string, string> orderStateMachine = InitalStateMachine();

            #region 连续转换状态的示例
            orderStateMachine.SetCurrentStateTo(OrderState.Initial)
                .ChangeState(OrderInput.Submit)
                .ChangeState(OrderInput.Deny)
                .ChangeState(OrderInput.I_Edit)
                .ChangeState(OrderInput.Agree)
                .ChangeState(OrderInput.Agree)
                .ChangeState(OrderInput.OK);
            #endregion


            #region 任意状态转换
            orderStateMachine.ChangeState(OrderState.Pending, OrderInput.Agree);
            orderStateMachine.ChangeState(OrderState.Pending, OrderInput.Deny);
            #endregion


        }

        static StateMachine<string, string, string> InitalStateMachine()
        {

            StateMachine<string, string, string> orderStateMachine = new StateMachine<string, string, string>("订单状态机");

            orderStateMachine.CreateState(OrderState.Initial)
                .AsOriginalState()//设置成初始状态
                .Rule(OrderInput.Submit, OrderState.Pending, "提交成功，转入Pending状态")
                .OnQuit((toState, input, output) =>
                {
                    Console.Write(string.Format("离开状态Initial;"));
                    Console.Write(string.Format("前往状态{0};", toState));
                    Console.Write(string.Format("输入：{0};", input));
                    Console.WriteLine(string.Format("输出：{0};", output));
                });

            orderStateMachine.CreateState(OrderState.Pending)
                .Rule(OrderInput.Agree, OrderState.Passed, "已经批准，转入Passed状态")
                .Rule(OrderInput.Deny, OrderState.Rejected, "已经否决，转入Rejected状态")
                .OnQuit((toState, input, output) =>
                {
                    Console.Write(string.Format("离开状态Pending;"));
                    Console.Write(string.Format("前往状态{0};", toState));
                    Console.Write(string.Format("输入：{0};", input));
                    Console.WriteLine(string.Format("输出：{0};", output));
                });

            orderStateMachine.CreateState(OrderState.Passed)
                .Rule(OrderInput.Agree, OrderState.Shipping, "已经批准，转入Shipping状态")
                .OnEntry((fromState, input, output) =>
                {
                    Console.Write(string.Format("进入状态Passed;"));
                    Console.Write(string.Format("先前状态{0};", fromState));
                    Console.Write(string.Format("输入：{0};", input));
                    Console.WriteLine(string.Format("输出：{0};", output));
                })
                .OnQuit((toState, input, output) =>
                {
                    Console.Write(string.Format("离开状态Passed;"));
                    Console.Write(string.Format("前往状态{0};", toState));
                    Console.Write(string.Format("输入：{0};", input));
                    Console.WriteLine(string.Format("输出：{0};", output));
                });

            orderStateMachine.CreateState(OrderState.Rejected)
               .Rule(OrderInput.I_Know, OrderState.Finished, "已经知晓，转入Finished状态")
               .Rule(OrderInput.I_Edit, OrderState.Pending, "已经修改，转入Pending状态")
               .OnEntry((fromState, input, output) =>
               {
                   Console.Write(string.Format("进入状态Rejected;"));
                   Console.Write(string.Format("先前状态{0};", fromState));
                   Console.Write(string.Format("输入：{0};", input));
                   Console.WriteLine(string.Format("输出：{0};", output));
               })
               .OnQuit((toState, input, output) =>
               {
                   Console.Write(string.Format("离开状态Rejected;"));
                   Console.Write(string.Format("前往状态{0};", toState));
                   Console.Write(string.Format("输入：{0};", input));
                   Console.WriteLine(string.Format("输出：{0};", output));
               });


            orderStateMachine.CreateState(OrderState.Shipping)
                .Rule(OrderInput.OK, OrderState.Finished, "已经OK，转入Finished状态")
                .OnEntry((fromState, input, output) =>
                {
                    Console.Write(string.Format("进入状态Shipping;"));
                    Console.Write(string.Format("先前状态{0};", fromState));
                    Console.Write(string.Format("输入：{0};", input));
                    Console.WriteLine(string.Format("输出：{0};", output));
                })
                .OnQuit((toState, input, output) =>
                {
                    Console.Write(string.Format("离开状态Shipping;"));
                    Console.Write(string.Format("前往状态{0};", toState));
                    Console.Write(string.Format("输入：{0};", input));
                    Console.WriteLine(string.Format("输出：{0};", output));
                });

            orderStateMachine.CreateState(OrderState.Finished)
                .OnEntry((fromState, input, output) =>
                {
                    Console.Write(string.Format("进入状态Finished;"));
                    Console.Write(string.Format("先前状态{0};", fromState));
                    Console.Write(string.Format("输入：{0};", input));
                    Console.WriteLine(string.Format("输出：{0};", output));
                });

            return orderStateMachine;
        }
    }
}
