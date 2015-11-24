namespace IMGUI
{
    internal class GUILayout
    {
        public enum LayoutMode
        {
            Horizontal,
            Vertical,
        }
        #region State machine define
        static class GUILayoutState
        {
            public const string Initial = "Initial";
            public const string GroupBegun = "GroupBegun";
            public const string Intermidiate = "Intermidiate";
        }

        static class GUILayoutCommand
        {
            public const string BeginGroup = "BeginGroup";
            public const string EndGroup = "EndGroup";
            public const string AddRect = "AddRect";
        }

        static readonly string[] states =
        {
            GUILayoutState.Initial, GUILayoutCommand.BeginGroup, GUILayoutState.GroupBegun,
            GUILayoutState.GroupBegun, GUILayoutCommand.AddRect, GUILayoutState.Intermidiate,
            GUILayoutState.Intermidiate, GUILayoutCommand.AddRect, GUILayoutState.Intermidiate,
            GUILayoutState.Intermidiate, GUILayoutCommand.EndGroup, GUILayoutState.Intermidiate,
            GUILayoutState.Intermidiate, GUILayoutCommand.BeginGroup, GUILayoutState.GroupBegun,
        };
        #endregion

        private readonly StateMachine stateMachine = new StateMachine(GUILayoutState.Initial, states);
        private LayoutMode currentMode;
        private Point currentPoint;
        private Size currentSize;

        private readonly System.Collections.Generic.Stack<Point> pointStack = new System.Collections.Generic.Stack<Point>(8);
        private readonly System.Collections.Generic.Stack<LayoutMode> modeStack = new System.Collections.Generic.Stack<LayoutMode>(8);
        private readonly System.Collections.Generic.Stack<Size> sizeStack = new System.Collections.Generic.Stack<Size>(8);
        
        public void BeginGroup(LayoutMode mode)
        {
            if(stateMachine.MoveNext(GUILayoutCommand.BeginGroup))
            {
                pointStack.Push(currentPoint);
                modeStack.Push(currentMode);
                sizeStack.Push(currentSize);

                currentMode = mode;
                currentSize = new Size();
            }
            else
            {
                throw new System.Exception();
            }
        }

        public void EndGroup(LayoutMode mode)
        {
            if (currentMode != mode)
            {
                throw new System.InvalidOperationException();
            }

            if(stateMachine.MoveNext(GUILayoutCommand.EndGroup))
            {
                var oldPoint = pointStack.Pop();
                var oldMode = modeStack.Pop();
                var oldSize = sizeStack.Pop();
                
                if (oldMode == LayoutMode.Horizontal && currentMode == LayoutMode.Vertical)
                {
                    currentPoint = new Point(oldPoint.X + currentSize.Width, oldPoint.Y);
                }
                else if (oldMode == LayoutMode.Vertical && currentMode == LayoutMode.Horizontal)
                {
                    currentPoint = new Point(oldPoint.X, oldPoint.Y + currentSize.Height);
                }

                currentMode = oldMode;
            }
        }

        public Rect AddRect(Rect rect)
        {
            if(stateMachine.MoveNext(GUILayoutCommand.AddRect))
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
    }
}