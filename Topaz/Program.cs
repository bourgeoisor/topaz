using System;

namespace Topaz
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var topaz = new Topaz())
                topaz.Run();
        }
    }
}
