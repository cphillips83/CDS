using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDS.FileSystem;

namespace CDS.Tasks
{
    public abstract class Task2
    {
        //public virtual bool IsCompleted { get; protected set; }
        protected Task _task;

        public Task Process()
        {
            _task = Task.Run(new Action(Execute));
            return _task;
        }

        public virtual bool Wait(int ms = -1)
        {
            return _task.Wait(ms);
        }

        protected abstract void Execute();
    }

    public class GetLocalDirectories : Task2
    {
        public readonly string BasePath;
        public readonly DirectoryEntry Root;
        public readonly BlockingCollection<DirectoryEntry> Directories = new BlockingCollection<DirectoryEntry>();

        private int _dirCount = 0;
        public int TotalDirectories { get { return _dirCount; } }

        public GetLocalDirectories(string basePath)
        {
            BasePath = basePath;
            Root = new DirectoryEntry(basePath);
        }

        protected override void Execute()
        {
            var dirsToProcess = new Stack<DirectoryEntry>();
            dirsToProcess.Push(Root);

            while (dirsToProcess.Count > 0)
            {
                _dirCount++;

                var de = dirsToProcess.Pop();
                Directories.Add(de);

                var dirs = Directory.GetDirectories(de.Path);
                var dirEntries = new DirectoryEntry[dirs.Length];
                for (int i = 0; i < dirs.Length; i++)
                {
                    dirEntries[i] = new DirectoryEntry(dirs[i]);
                    if (!dirEntries[i].Path.Contains(".git") && !dirEntries[i].Path.Contains("\\cache\\"))
                        dirsToProcess.Push(dirEntries[i]);
                }
                de.AddDirectories(dirEntries);
                //System.Threading.Thread.Sleep(1);
            }

            Directories.CompleteAdding();
        }

        public override bool Wait(int ms = -1)
        {
            if (Globals.ActiveConsole)
            {
                var index = 0;
                var symbols = new string[] { "-", "\\", "|", "/" };
                while (!base.Wait(Globals.ConsoleUpdateDelay))
                {
                    index = (index + 1) % symbols.Length;
                    Console.CursorLeft = 0;
                    Console.Write($"Discovering {TotalDirectories} directories...");
                }

                Console.CursorLeft = 0;
            }

            var result = base.Wait(ms);
            if(result)
                Console.WriteLine($"Discovered {TotalDirectories} directories...  ");
            return result;
        }
    }
}
