using System;

namespace ImGui
{
    internal class StateMachineEx
    {
        class Transition
        {
            private readonly string State;
            private readonly string Command;

            public Transition(string state, string command)
            {
                State = state;
                Command = command;
            }

            public override int GetHashCode()
            {
                return 17 + 31 * State.GetHashCode() + 31 * Command.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                Transition other = obj as Transition;
                return other != null && this.State == other.State && this.Command == other.Command;
            }
        }

        class Result
        {
            public readonly string State;
            public readonly Action<InputTextContext> CallBack;

            public Result(string state, Action<InputTextContext> callback)
            {
                this.State = state;
                this.CallBack = callback;
            }
        }

        System.Collections.Generic.Dictionary<Transition, Result> transitions;

        /// <summary>
        /// Current state of the state machine
        /// </summary>
        /// <remarks>DO NOT Set this property if you are not making a instant state transition!</remarks>
        public string CurrentState { get; set; }

        public StateMachineEx(string initialState, string[] stateTransitions, Action<InputTextContext>[] callBacks)
        {
            CurrentState = initialState;
            transitions = new System.Collections.Generic.Dictionary<Transition, Result>();
            for (int i = 0; i < stateTransitions.Length; i+=3)
            {
                transitions.Add(
                    new Transition(stateTransitions[i], stateTransitions[i + 1]),
                    new Result(stateTransitions[i + 2], callBacks[i / 3]));
            }
        }

        /// <summary>
        /// Move state according to the command
        /// </summary>
        /// <param name="command">command</param>
        /// <param name="context">context</param>
        /// <returns>true:valid command/false:invalid command</returns>
        public bool MoveNext(string command, InputTextContext context)
        {
            Transition transition = new Transition(CurrentState, command);
            Result result;
            if (!transitions.TryGetValue(transition, out result))
            {
                return false;
            }
            CurrentState = result.State;
            result.CallBack?.Invoke(context);
            return true;
        }
    }
}