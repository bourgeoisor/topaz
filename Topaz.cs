using System;

namespace Topaz
{
    public static class Topaz
    {
        [STAThread]
        static void Main()
        {
            #if DEBUG
                using (var topaz = Engine.Core.Instance)
                    topaz.Run();
            #else
                try
                {
                    using (var topaz = Engine.Core.Instance)
                        topaz.Run();
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.ToString(), "Runtime Error");
                }
            #endif
        }
    }
}
