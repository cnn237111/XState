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
        static void Main(string[] args)
        {
            StateMachine<string, int, int> sm = new StateMachine<string, int, int>("A");

            sm.CreateState("A").Rule(1, "B",1000).Rule(2, "C",3000)
                .OnEntry((fromState, input, output) =>
                {
                    Console.Write(string.Format("OnEntry;"));
                    Console.Write(string.Format("From state {0};", fromState));
                    Console.Write(string.Format("Input is {0};", input));
                    Console.WriteLine(string.Format("Output is {0};", output));

                }).OnQuit((toState, input, output) =>
                {
                    Console.Write(string.Format("OnQuit;"));
                    Console.Write(string.Format("To state {0};", toState));
                    Console.Write(string.Format("Input is {0};", input));
                    Console.WriteLine(string.Format("Output is {0};", output));
                });

            sm.CreateState("B").Rule(1, "C").Rule(0, "A");
            sm.CreateState("C");
            Console.WriteLine(sm.ChangeState(1).CurrentState);
            Console.WriteLine(sm.ChangeState(0).CurrentState);
            Console.WriteLine(sm.ChangeState(2).CurrentState);

            Console.WriteLine(sm.CurrentState);
            Console.WriteLine(sm.CanChangeTo("A","C"));
            Console.WriteLine(sm.CanChangeTo("B"));

            foreach(var s in sm.AllStates)
            {
                Console.WriteLine(s);

            }
        }
    }
}
