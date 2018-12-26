using System;

namespace Topaz.Engine
{
    class Logger
    {
        string classifier;

        public Logger(string classifier)
        {
            this.classifier = classifier;
        }

        void WriteLine(string msg)
        {
            Console.WriteLine("[" + classifier + "]\t" + msg);
        }

        public void Info(string msg)
        {
            WriteLine(msg);
        }

        public void Debug(string msg)
        {
            WriteLine("DEBUG: " + msg);
        }

        public void Warn(string msg)
        {
            WriteLine("WARN: " + msg);
        }

        public void Error(string msg)
        {
            WriteLine("ERROR: " + msg);
        }
    }
}
