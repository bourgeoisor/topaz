using System;

namespace Topaz.Engine
{
    class Logger
    {
        private readonly string _classifier;
        public bool IsLogging { get; set; }

        public Logger(string classifier)
        {
            _classifier = classifier;
            IsLogging = true;
        }

        void WriteLine(string msg)
        {
            if (IsLogging)
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
