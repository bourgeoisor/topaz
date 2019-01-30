using System;

namespace Topaz
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            #if DEBUG
                using (var topaz = new Topaz())
                    topaz.Run();
            #else
                try
                {
                    using (var topaz = new Topaz())
                        topaz.Run();
                }
                catch (Exception e)
                {
                    Engine.Window.Instance.Terminate();
                    System.Windows.Forms.MessageBox.Show(e.ToString(), "Runtime Error");
                }
            #endif
        }
    }
}
