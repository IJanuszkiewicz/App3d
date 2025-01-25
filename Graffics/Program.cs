namespace Graffics;

class Program
{
    static void Main(string[] args)
    {
        using (SimWindow window = new SimWindow(1600, 900, "okienko"))
        {
            window.Run();
        }
    }
}