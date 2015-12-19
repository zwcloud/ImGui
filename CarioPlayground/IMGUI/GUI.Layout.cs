namespace ImGui
{
    public partial class GUI
    {
        enum LayoutMode
        {
            Horizontal,
            Vertical,
        }
        #region State machine define
        static class GUIState
        {
            public const string Initial = "Initial";
            public const string GroupBegun = "GroupBegun";
            public const string Intermidiate = "Intermidiate";
        }

        static class GUICommand
        {
            public const string BeginGroup = "BeginGroup";
            public const string EndGroup = "EndGroup";
            public const string AddRect = "AddRect";
        }

        static readonly string[] states =
        {
            GUIState.Initial, GUICommand.BeginGroup, GUIState.GroupBegun,
            GUIState.GroupBegun, GUICommand.AddRect, GUIState.Intermidiate,
            GUIState.GroupBegun, GUICommand.BeginGroup, GUIState.GroupBegun,
            GUIState.Intermidiate, GUICommand.AddRect, GUIState.Intermidiate,
            GUIState.Intermidiate, GUICommand.EndGroup, GUIState.Intermidiate,
            GUIState.Intermidiate, GUICommand.BeginGroup, GUIState.GroupBegun,
        };
        #endregion

        private readonly StateMachine stateMachine = new StateMachine(GUIState.Initial, states);
        private LayoutMode currentMode;
        private Point currentPoint;
        private Size currentSize;

        private readonly System.Collections.Generic.Stack<Point> pointStack = new System.Collections.Generic.Stack<Point>(8);
        private readonly System.Collections.Generic.Stack<LayoutMode> modeStack = new System.Collections.Generic.Stack<LayoutMode>(8);
        private readonly System.Collections.Generic.Stack<Size> sizeStack = new System.Collections.Generic.Stack<Size>(8);

        private bool Layouting { get; set; }

        void BeginGroup(LayoutMode mode)
        {
            //System.Diagnostics.Debug.WriteLine("BeginGroup {0}", mode, null);
            if(stateMachine.MoveNext(GUICommand.BeginGroup))
            {
                pointStack.Push(currentPoint);
                modeStack.Push(currentMode);
                sizeStack.Push(currentSize);

                currentMode = mode;
                currentSize = new Size();
                Layouting = true;
            }
            else
            {
                throw new System.Exception();
            }
        }

        void EndGroup(LayoutMode mode)
        {
            //System.Diagnostics.Debug.WriteLine("EndGroup {0}", mode, null);
            if(currentMode != mode)
            {
                throw new System.InvalidOperationException();
            }

            if(stateMachine.MoveNext(GUICommand.EndGroup))
            {
                var oldPoint = pointStack.Pop();
                var oldMode = modeStack.Pop();
                var oldSize = sizeStack.Pop();

                if(oldMode == LayoutMode.Horizontal && currentMode == LayoutMode.Vertical)
                {
                    currentPoint = new Point(oldPoint.X + currentSize.Width, oldPoint.Y);
                }
                else if(oldMode == LayoutMode.Vertical && currentMode == LayoutMode.Horizontal)
                {
                    currentPoint = new Point(oldPoint.X, oldPoint.Y + currentSize.Height);
                }

                currentMode = oldMode;
                if(pointStack.Count == 0)
                {
                    Layouting = false;
                }
            }
        }

        Rect AddRect(Rect rect)
        {
            if(stateMachine.MoveNext(GUICommand.AddRect))
            {
                rect.Location = currentPoint;
                switch (currentMode)
                {
                    case LayoutMode.Horizontal:
                        currentPoint.X += rect.Width;
                        currentSize.Width += rect.Width;
                        if(rect.Height>currentSize.Height)
                        {
                            currentSize.Height = rect.Height;
                        }
                        break;
                    case LayoutMode.Vertical:
                        currentPoint.Y += rect.Height;
                        if(rect.Width>currentSize.Width)
                        {
                            currentSize.Width = rect.Width;
                        }
                        currentSize.Height += rect.Height;
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }
            }
            return rect;
        }

        Rect DoLayout(Rect rect)
        {
            if (Layouting)
            {
                rect = AddRect(rect);
            }
            return rect;
        }
    }
}