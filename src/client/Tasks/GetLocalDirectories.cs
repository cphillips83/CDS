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

        public Task ProcessAsync()
        {
            _task = Task.Run(new Action(Execute));
            return _task;
        }

        public void Process()
        {
            Execute();
        }

        public virtual bool Wait(int ms = -1)
        {
            if (_task == null)
                return true;
            return _task.Wait(ms);
        }

        protected abstract void Execute();
    }

    public class GetLocalDirectories : Task2
    {
        public readonly string BasePath;
        public readonly DirectoryEntry Root;
        public readonly BlockingCollection<DirectoryEntry> Directories = new BlockingCollection<DirectoryEntry>();
        public readonly ConcurrentDictionary<Hash, string> DirectoryMaps = new ConcurrentDictionary<Hash, string>();

        private int _dirCount = 0;
        public int TotalDirectories { get { return _dirCount; } }

        public GetLocalDirectories(string basePath)
        {
            BasePath = basePath;

            if (!BasePath.EndsWith("\\"))
                BasePath += "\\";

            Root = new DirectoryEntry(Hash.Empty);
            DirectoryMaps.TryAdd(Root.Hash, string.Empty);
        }

        protected override void Execute()
        {
            var dirsToProcess = new Stack<DirectoryEntry>();
            dirsToProcess.Push(Root);

            while (dirsToProcess.Count > 0)
            {
                if (Errors.ReachedMaxErrors)
                    break;

                _dirCount++;

                var de = dirsToProcess.Pop();
                Directories.Add(de);

                try
                {
                    var path = DirectoryMaps[de.Hash];
                    var fullPath = Path.Combine(BasePath, path);
                    var dirs = Directory.GetDirectories(fullPath);
                    var dirEntries = new DirectoryEntry[dirs.Length];
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        if (Errors.ReachedMaxErrors)
                            break;

                        var relativePath = dirs[i].Substring(BasePath.Length);
                        var filenameHash = SimpleSHA1.NonThreadSafe.ComputeHash(relativePath);
                        dirEntries[i] = new DirectoryEntry(filenameHash);
                        //if (!dirEntries[i].Path.Contains(".git") && !dirEntries[i].Path.Contains("\\cache\\"))
                        DirectoryMaps.TryAdd(dirEntries[i].Hash, relativePath);
                        dirsToProcess.Push(dirEntries[i]);
                    }
                    de.AddDirectories(dirEntries);
                }
                catch (Exception ex)
                {
                    Errors.Add(ex);
                }
                //System.Threading.Thread.Sleep(1);
            }

            Directories.CompleteAdding();
        }

        public override bool Wait(int ms = -1)
        {
            if (Globals.ActiveConsole)
            {
                while (!base.Wait(Globals.ConsoleUpdateDelay))
                {
                    Console.CursorLeft = 0;
                    Console.Write($"Discovering {TotalDirectories} directories...");
                }

                Console.CursorLeft = 0;
            }

            var result = base.Wait(ms);
            if (result)
                Console.WriteLine($"Discovered {TotalDirectories} directories...  ");
            return result;
        }
    }
}
