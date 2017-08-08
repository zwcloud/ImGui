using System.Diagnostics;

namespace ImGui
{
    public class Profile
    {
        private static string name;
        private static Stopwatch watch = new Stopwatch();

        [Conditional("DEBUG")]
        public static void Start(string name)
        {
            Profile.name = name;
            watch.Start();
        }

        [Conditional("DEBUG")]
        public static void End()
        {
            watch.Stop();
            Debug.WriteLine("[Profile]{0} {1} ms", name, watch.ElapsedMilliseconds.ToString());
            watch.Reset();
        }
    }
}
