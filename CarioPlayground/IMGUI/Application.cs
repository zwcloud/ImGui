namespace IMGUI
{
    public delegate void OnGUIDelegate(GUI gui);

    /// <summary>
    /// A single window IMGUI application
    /// </summary>
    public sealed class Application
    {
        public static void Run(Form form)
        {
            System.Windows.Forms.Application.Run(form);
        }
    }
}