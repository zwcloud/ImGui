using System.Collections.Generic;
using ImGui.OSAbstraction.Text;
using ImGui.Rendering;

//BUG Hover state persists when move from mainRect to outside.
//BUG Abnormal representation when drag from mainRect to outside.
//TODO GUILayout version

namespace ImGui
{
    public partial class GUI
    {
        #region State machine constants
        static class ComboBoxState
        {
            public const string Normal = "Normal";
            public const string Hovered = "Hovered";
            public const string Active = "Active";
            public const string ShowingItems = "ShowingItems";
        }

        static class ComboBoxCommand
        {
            public const string MoveIn = "MoveIn";
            public const string MoveOut = "MoveOut";
            public const string MousePress = "MouseDown";
            public const string ShowItems = "ShowItems";
            public const string SelectItem = "SelectItem";
        }

        static readonly string[] states =
        {
            ComboBoxState.Normal, ComboBoxCommand.MoveIn, ComboBoxState.Hovered,
            ComboBoxState.Hovered, ComboBoxCommand.MoveOut, ComboBoxState.Normal,
            ComboBoxState.Hovered, ComboBoxCommand.MousePress, ComboBoxState.Active,
            ComboBoxState.Active, ComboBoxCommand.ShowItems, ComboBoxState.ShowingItems,
            ComboBoxState.ShowingItems, ComboBoxCommand.SelectItem, ComboBoxState.Normal,
        };
        
        private readonly System.Action<int>[] callBacks = new System.Action<int>[5]
        {
            null,
            null,
            null,
            null,
            null
        };

        #endregion

        class ComboBoxContext
        {
            public StateMachine<int> stateMachine;

            public string[] Texts { get; set; }
            public Form ItemsContainer { get; set; }

            public int SelectedIndex { get; set; }
            public Rect Rect { get; set; }

            public string Text { get; set; }
            public ITextContext TextContext { get; set; }
        }

        private ComboBoxContext comboBoxContext;

        internal int ComboBox(Rect rect, string[] texts, LayoutOptions? options)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return comboBoxContext.SelectedIndex;
            
            //get or create the root node
            var id = window.GetID(texts);
            var container = window.AbsoluteVisualList;
            var node = (Node)container.Find(visual => visual.Id == id);
            if (node == null)
            {
                //create button node
                node = new Node(id, $"Combobox<{texts[0]}..>");
                container.Add(node);
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.ComboBox]);
                
                //set initial states TODO move to shared storage
                comboBoxContext.Rect = rect;
                comboBoxContext.Texts = texts;
                comboBoxContext.Text = texts[0];
                comboBoxContext.SelectedIndex = 0;
                
                comboBoxContext.stateMachine = new StateMachine<int>(ComboBoxState.Normal, states, null);
                /*//TODO
                 i =>
                    {
                        comboBoxContext.SelectedIndex = i;
                        comboBoxContext.stateMachine.MoveNext(ComboBoxCommand.SelectItem, i);
                        return true;
                    }
                 */
            }
            node.RuleSet.ApplyStack();
            node.RuleSet.ApplyOptions(options);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(rect);

            // interact
            var pressed = GUIBehavior.ButtonBehavior(node.Rect, node.Id, out var hovered, out var held);
            node.State = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            
            //Execute state commands
            //TODO move these into statemachine callbacks
#if false
            var containMousePosition = Rect.Contains(Form.current.ScreenToClient(Mouse.Instance.MousePos));
            if (!Rect.Contains(Form.current.ScreenToClient(Mouse.Instance.LastPosition)) && containMousePosition)
            {
                stateMachine.MoveNext(ComboBoxCommand.MoveIn);
            }
            if (Rect.Contains(Form.current.ScreenToClient(Mouse.Instance.LastMousePos)) && !containMousePosition)
            {
                stateMachine.MoveNext(ComboBoxCommand.MoveOut);
            }
            if (Mouse.Instance.LeftButtonPressed && containMousePosition && Form.current.Focused)
            {
                if (stateMachine.MoveNext(ComboBoxCommand.MousePress))
                {
                    Mouse.Instance.stateMachine.MoveNext(Mouse.Instance.MouseCommand.Fetch);
                }
            }
            if(stateMachine.CurrentState == ComboBoxState.Active)//instant transition of state
            {
                ItemsContainer.Position = Form.current.ClientToScreen(Rect.BottomLeft);
                Application.Forms.Add(ItemsContainer);
                ItemsContainer.Show();
                stateMachine.MoveNext(ComboBoxCommand.ShowItems);
            }
#endif

            var screenPos = Form.current.ClientToScreen(comboBoxContext.Rect.TopLeft);
            var screenRect = new Rect(screenPos, comboBoxContext.Rect.Size);
            comboBoxContext.ItemsContainer = new ComboxBoxItemsForm(
                screenRect,
                comboBoxContext.Texts, null);

            comboBoxContext.Text = comboBoxContext.Texts[comboBoxContext.SelectedIndex];
            comboBoxContext.TextContext.Text = comboBoxContext.Text;
            
            // last item state
            window.TempData.LastItemState = node.State;

            //render
            var g = node.RenderOpen();
            g.DrawBoxModel(comboBoxContext.Text, node.RuleSet, node.Rect);
            g.RenderArrow(node.Rect.Min + new Vector(node.PaddingLeft, 0), node.Height,
                node.RuleSet.FontColor, Internal.Direcion.Down, 1.0);

            return comboBoxContext.SelectedIndex;
        }
    }

    internal sealed class ComboxBoxItemsForm : Form
    {
        private Rect Rect { get; set; }
        private readonly List<string> TextList;
        private System.Action<int> CallBack { get; set; }

        public ComboxBoxItemsForm(Rect rect, string[] texts, System.Action<int> callBack)
            : base(rect)
        {
            Position = rect.TopLeft;
            rect.X = rect.Y = 0;
            Rect = rect;
            TextList = new List<string>(texts);
            CallBack = callBack;
        }

        protected override void OnGUI()
        {
            GUILayout.BeginVertical("CombolBox");
            for (int i = 0; i < TextList.Count; i++)
            {
                var itemRect = Rect;
                itemRect.Y += (i + 1) * Rect.Height;
                if(GUI.Button(new Rect(Rect.Width, itemRect.Height), TextList[i]))
                {
                    if(CallBack!=null)
                    {
                        CallBack(i);
                    }
                    this.Hide();
                    Application.Forms.Remove(this);
                }
            }
            GUILayout.EndVertical();
        }
    }
    
    internal class StateMachine<T>
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

        class Result<T>
        {
            public readonly string State;
            public readonly System.Action<T> CallBack;

            public Result(string state, System.Action<T> callback)
            {
                this.State = state;
                this.CallBack = callback;
            }
        }

        System.Collections.Generic.Dictionary<Transition, Result<T>> transitions;

        /// <summary>
        /// Current state of the state machine
        /// </summary>
        /// <remarks>DO NOT Set this property if you are not making a instant state transition!</remarks>
        public string CurrentState { get; set; }

        public StateMachine(string initialState, string[] stateTransitions,
            System.Action<T>[] callBacks)
        {
            CurrentState = initialState;
            transitions = new System.Collections.Generic.Dictionary<Transition, Result<T>>();
            for (int i = 0; i < stateTransitions.Length; i+=3)
            {
                transitions.Add(
                    new Transition(stateTransitions[i], stateTransitions[i + 1]),
                    new Result<T>(stateTransitions[i + 2], callBacks[i / 3]));
            }
        }

        /// <summary>
        /// Move state according to the command
        /// </summary>
        /// <param name="command">command</param>
        /// <param name="context">context</param>
        /// <returns>true:valid command/false:invalid command</returns>
        public bool MoveNext(string command, T context)
        {
            Transition transition = new Transition(CurrentState, command);
            Result<T> result;
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
