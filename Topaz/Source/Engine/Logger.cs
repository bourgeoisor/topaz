using System;

namespace Topaz.Engine
{
    class Logger
    {
        string _classifier;

        public Logger(string classifier)
        {
            _classifier = classifier;
        }

        void WriteLine(string msg)
        {
            Console.WriteLine("[" + _classifier + "]\t" + msg);
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
