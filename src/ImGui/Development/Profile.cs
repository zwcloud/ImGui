using System.Diagnostics;

namespace ImGui
{
    public class Profile
    {
        private static string name;
        private static Stopwatch watch = new Stopwatch();
        private static bool useMicrosecond = false;

        /// <summary>
        /// Start profile
        /// </summary>
        /// <param name="name"></param>
        public static void Start(string name)
        {
            Profile.name = name;
            watch.Start();
        }

        /// <summary>
        /// End the profile and return microseconds (μs) elapsed
        /// </summary>
        /// <returns></returns>
        public static long End()
        {
            watch.Stop();
            long result = watch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));//microseconds
            if (useMicrosecond)
            {
                Debug.WriteLine("[Profile]{0} {1} us", name, result);
            }
            else
            {
                Debug.WriteLine("[Profile]{0} {1} ms", name, result/1000.0);
            }
            watch.Reset();
            return result;
        }
    }
}
