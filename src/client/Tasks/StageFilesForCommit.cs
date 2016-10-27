using CDS.FileSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Tasks
{
    public class StageFilesForCommit : Task2
    {
        public readonly string BasePath;

        public StageFilesForCommit(string basePath)
        {
            BasePath = basePath;
        }

        protected override void Execute()
        {
            var sw = Stopwatch.StartNew();
            var a = new GetLocalDirectories(BasePath);
            var b = new GetLocalFiles(a);
            
            //var c = new GetCurrentManifest();
            a.ProcessAsync();
            b.ProcessAsync();
            //c.Process();

            a.Wait();
            b.Wait();
            //c.Wait();

            if (Errors.HasErrors)
            {
                Errors.WriteErrors();
                return;
            }

            //now we need to get the current manifest
            //and compare to what we have
            var d = new GetChangeSet(null, a.Root); //change 2nd a to c
            d.Process();
            d.Wait();

            sw.Stop();
            Console.WriteLine($"Completed in {sw.Elapsed}...");
        }
    }
}
 