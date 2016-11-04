using System;
using System.Collections.Concurrent;

namespace CDS
{
    public class Errors
    {
        private static readonly ConcurrentBag<string> _errors = new ConcurrentBag<string>();

        public static bool HasErrors { get; private set; }
        public static bool ReachedMaxErrors { get; private set; }

        public static void Add(Exception ex)
        {
            _errors.Add(ex.Message);
            if (_errors.Count > 100)
                ReachedMaxErrors = true;

            HasErrors = true;
        }

        public static void WriteErrors()
        {
            foreach (var err in _errors)
                Console.WriteLine(err);
        }
    }
}
