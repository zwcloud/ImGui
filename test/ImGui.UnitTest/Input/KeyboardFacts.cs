using ImGui.Input;
using Xunit;

namespace ImGui.UnitTest
{
    public class KeyboardFacts
    {
        public class KeyDown
        {
            [Fact]
            public void Works()
            {
                //frame 0
                {
                    Keyboard.Instance.OnFrameBegin();
                    Keyboard.Instance.lastKeyStates[(int)Key.A] = Keyboard.Instance.keyStates[(int)Key.A];
                    Keyboard.Instance.keyStates[(int)Key.A] = KeyState.Down;

                    Assert.True(Keyboard.Instance.KeyDown(Key.A));

                    Keyboard.Instance.OnFrameEnd();
                }

                //frame 1
                {
                    Keyboard.Instance.OnFrameBegin();
                    Keyboard.Instance.lastKeyStates[(int)Key.A] = Keyboard.Instance.keyStates[(int)Key.A];
                    Keyboard.Instance.keyStates[(int)Key.A] = KeyState.Up;

                    Assert.False(Keyboard.Instance.KeyDown(Key.A));

                    Keyboard.Instance.OnFrameEnd();
                }
            }
        }

        public class KeyOn
        {
            [Fact]
            public void Works()
            {
                //frame 0
                {
                    Keyboard.Instance.OnFrameBegin();
                    Keyboard.Instance.lastKeyStates[(int)Key.CapsLock] = Keyboard.Instance.keyStates[(int)Key.CapsLock];
                    Keyboard.Instance.keyStates[(int)Key.CapsLock] = KeyState.On;

                    Assert.True(Keyboard.Instance.KeyOn(Key.CapsLock));

                    Keyboard.Instance.OnFrameEnd();
                }

                //frame 1
                {
                    Keyboard.Instance.OnFrameBegin();
                    Keyboard.Instance.lastKeyStates[(int)Key.CapsLock] = Keyboard.Instance.keyStates[(int)Key.CapsLock];
                    Keyboard.Instance.keyStates[(int)Key.CapsLock] = KeyState.Off;

                    Assert.False(Keyboard.Instance.KeyOn(Key.CapsLock));

                    Keyboard.Instance.OnFrameEnd();
                }
            }
        }

        public class KeyPressed
        {
            [Fact]
            public void Works()
            {
                //frame 0
                {
                    Keyboard.Instance.OnFrameBegin();
                    Keyboard.Instance.lastKeyStates[(int)Key.A] = Keyboard.Instance.keyStates[(int)Key.A];
                    Keyboard.Instance.keyStates[(int)Key.A] = KeyState.Down;

                    Assert.False(Keyboard.Instance.KeyPressed(Key.A));

                    Keyboard.Instance.OnFrameEnd();
                }

                //frame 1
                {
                    Keyboard.Instance.OnFrameBegin();
                    Keyboard.Instance.lastKeyStates[(int)Key.A] = Keyboard.Instance.keyStates[(int)Key.A];
                    Keyboard.Instance.keyStates[(int)Key.A] = KeyState.Up;

                    Assert.True(Keyboard.Instance.KeyPressed(Key.A));

                    Keyboard.Instance.OnFrameEnd();
                }
            }
        }
    }
}