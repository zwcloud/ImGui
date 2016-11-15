using System;

namespace ImGui
{
    internal class StateMachineEx
    {
#if false
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

        class Result<T> where T : Control
        {
            public readonly string State;
            public readonly Action<T> CallBack;

            public Result(string state, Action<T> callback)
            {
                this.State = state;
                this.CallBack = callback;
            }
        }

        System.Collections.Generic.Dictionary<Transition, Result<Control>> transitions;

        /// <summary>
        /// Current state of the state machine
        /// </summary>
        /// <remarks>DO NOT Set this property if you are not making a instant state transition!</remarks>
        public string CurrentState { get; set; }

        public StateMachineEx(string initialState, string[] stateTransitions, Action<Control>[] callBacks)
        {
            CurrentState = initialState;
            transitions = new System.Collections.Generic.Dictionary<Transition, Result<Control>>();
            for (int i = 0; i < stateTransitions.Length; i+=3)
            {
                transitions.Add(
                    new Transition(stateTransitions[i], stateTransitions[i + 1]),
                    new Result<Control>(stateTransitions[i + 2], callBacks[i / 3]));
            }
        }

        /// <summary>
        /// 根据命令移动状态
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="targetControl">目标控件</param>
        /// <returns>true:命令有效/false:命令无效</returns>
        public bool MoveNext(string command, Control targetControl)
        {
            Transition transition = new Transition(CurrentState, command);
            Result<Control> result;
            if (!transitions.TryGetValue(transition, out result))
            {
                return false;
            }
            CurrentState = result.State;
            if(result.CallBack != null)
            {
                result.CallBack(targetControl);
            }
            return true;
        }
#endif
    }
}