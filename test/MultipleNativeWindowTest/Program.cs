namespace MultipleNativeWindowTest
{
    class Program
    {
        #if TEST
        static void _Main(string[] args)
        #else
        static void Main(string[] args)
        #endif
        {
            MultipleWindowFact fact = new MultipleWindowFact();
            fact.ShowWindowWithChildWindow();
        }
    }
}
