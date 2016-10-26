using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CDS.FileSystem;
using CDS.Tasks;

namespace CDS
{
    public static class Program
    {
        public static readonly bool ActiveConsole = false;
 
        static Program()
        {
            EmbeddedAssembly.InitializeResolver();

        }

        static void Main()
        {
            var stage = new StageFilesForCommit(@"C:\inetpub\AMPProjects");
            stage.Process().Wait();
            //var pfs = new PhysicalFileSystem(@"C:\inetpub\AMPProjects");
            //pfs.Load();

            Console.ReadKey();
        }
    }
}
