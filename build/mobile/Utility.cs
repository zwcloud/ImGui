namespace ImGui
{
    internal static partial class Utility
    {
        internal static void LogEnvirenmentalVariables()
        {
            Application.logger.Msg("Environment Variables");
            var envars = System.Environment.GetEnvironmentVariables();
            var varEnumerator = envars.GetEnumerator();
            while (varEnumerator.MoveNext())
            {
                Application.logger.Msg("\t{0}={1}", varEnumerator.Key, varEnumerator.Value);
            }
        }
    }
}