namespace ImGui
{
    class LinuxInputContext : IInputContext
    {
        Cursor mouseCursor = Cursor.Default;

        public Cursor MouseCursor
        {
            get
            {
                return mouseCursor;
            }

            set
            {
                if (value != mouseCursor)
                {
                    mouseCursor = value;
                    //TODO
                }
            }
        }
    }
}
