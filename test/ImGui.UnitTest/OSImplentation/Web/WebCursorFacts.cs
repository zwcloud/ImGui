using ImGui.Input;
using ImGui.OSImplementation.Web;
using Xunit;

namespace ImGui.UnitTest.OSImplementation.Web
{
    public class WebCursorFacts
    {
        public class ChangeCursor
        {
            [Fact]
            public void ChangeCursors()
            {
                //TODO start a WebTemplate website as a test runner.
                WebCursor.ChangeCursor(Cursor.Default);
                WebCursor.ChangeCursor(Cursor.Text);
                WebCursor.ChangeCursor(Cursor.EwResize);
                WebCursor.ChangeCursor(Cursor.NsResize);
                WebCursor.ChangeCursor(Cursor.NeswResize);
            }
        }
    }
}