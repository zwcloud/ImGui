namespace ImGui
{
    public class StateMachine
    {
        class StateTransition
        {
            readonly string CurrentState;
            readonly string Command;

            public StateTransition(string currentState, string command)
            {
                CurrentState = currentState;
                Command = command;
            }

            public override int GetHashCode()
            {
                return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                StateTransition other = obj as StateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
            }
        }

        readonly System.Collections.Generic.Dictionary<StateTransition, string> transitions;
        /// <summary>
        /// Current state of the state machine
        /// </summary>
        /// <remarks>DO NOT Set this property if you are not making a instant state transition!</remarks>
        public string CurrentState { get; set; }

        public StateMachine(string initialState, string[] stateTransitions)
        {
            CurrentState = initialState;
            transitions = new System.Collections.Generic.Dictionary<StateTransition, string>();
            for (int i = 0; i < stateTransitions.Length; i+=3)
            {
                transitions.Add(new StateTransition(stateTransitions[i], stateTransitions[i+1]), stateTransitions[i+2]);
            }
        }

        public bool GetNext(string command, out string nextState)
        {
            StateTransition transition = new StateTransition(CurrentState, command);
            if(!transitions.TryGetValue(transition, out nextState))
            {
                //throw new System.Exception("Invalid transition: " + CurrentState + " -> " + command);
                nextState = CurrentState;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据命令移动状态
        /// </summary>
        /// <param name="command"></param>
        /// <returns>true:命令有效/false:命令无效</returns>
        /// <remarks>仅在命令有效时（命令导致的变化为状态图中的一条有向边时）使用Fetch命令！</remarks>
        public bool MoveNext(string command)
        {
            string newState;
            var valid = GetNext(command, out newState);
            CurrentState = newState;
            return valid;
        }
    }
}