using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS
{
    public static class Globals
    {
        public static readonly bool ActiveConsole = false;
        public static readonly int ConsoleUpdateDelay = 100;
        public static readonly int ProgressLength = 20;

        static Globals()
        {
            try
            {
                Console.CursorLeft = 0;
                ActiveConsole = true;
            }
            catch { }
        }
    }
}
