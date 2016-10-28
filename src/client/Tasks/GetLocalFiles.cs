using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CDS.FileSystem;
using System.Security;

namespace CDS.Tasks
{
    public class GetLocalFiles : Task2
    {
        private GetLocalDirectories _directoryProducer;

        private int _totalFiles;
        public int TotalFiles { get { return _totalFiles; } }

        private int _directoriesProcessed;
        public int DirectoriesProcessed { get { return _directoriesProcessed; } }

        public GetLocalFiles(GetLocalDirectories producer)
        {
            _directoryProducer = producer;
        }

        protected override void Execute()
        {
            Parallel.ForEach(_directoryProducer.Directories.GetConsumingPartitioner(), d =>
            {
                if (Errors.ReachedMaxErrors)
                    return;

                try
                {
                    var fullPath = Path.Combine(_directoryProducer.BasePath, d.Path);
                    var files = Directory.GetFiles(fullPath);
                    if (files.Length > 0)
                    {
                        var hasher = new SimpleSHA1();

                        var fileEntries = new FileEntry[files.Length];
                        for (int i = 0; i < files.Length; i++)
                        {
                            if (Errors.ReachedMaxErrors)
                                return;

                            try
                            {
                                var relativePath = files[i].Substring(_directoryProducer.BasePath.Length);
                                using (var s = File.OpenRead(files[i]))
                                {
                                    var nameHash = hasher.ComputeHash(relativePath);
                                    var dataHash = hasher.ComputeHash(s);
                                    var fe = new FileEntry(nameHash, dataHash, relativePath);
                                    fileEntries[i] = fe;
                                }
                            }
                            catch (Exception ex)
                            {
                                Errors.Add(ex);
                            }
                            //catch (FileNotFoundException) { }
                            //catch (IOException) { }
                            //catch (UnauthorizedAccessException) { }
                            //catch (SecurityException) { }
                            //System.Threading.Thread.Sleep(25);
                        }

                        d.AddFiles(fileEntries);

                        Interlocked.Add(ref _totalFiles, files.Length);
                    }
                    Interlocked.Increment(ref _directoriesProcessed);
                }
                catch (Exception ex)
                {
                    Errors.Add(ex);
                }
            });
        }

        private string ProgressBar(float percent)
        {
            var size = (int)(Globals.ProgressLength * percent);
            var a = new string('#', size);

            if (size == Globals.ProgressLength)
                return a;

            var b = new string(' ', Globals.ProgressLength - size);
            return $"{a}{b}";
        }

        public override bool Wait(int ms = -1)
        {
            if (Globals.ActiveConsole)
            {
                while (!base.Wait(Globals.ConsoleUpdateDelay))
                {
                    var percent = (_directoriesProcessed / (float)_directoryProducer.TotalDirectories);

                    Console.CursorLeft = 0;
                    Console.Write($"Discovering {_totalFiles} files [{ProgressBar(percent)}] {(int)(percent * 100)}%");
                }

                {
                    var percent = (_directoriesProcessed / (float)_directoryProducer.TotalDirectories);

                    Console.CursorLeft = 0;
                    Console.WriteLine($"Discovered {_totalFiles} files [{ProgressBar(percent)}] {(int)(percent * 100)}%");
                }
                return true;
            }

            var result = base.Wait(ms);
            if (result)
                Console.WriteLine($"Discovered {_totalFiles} files...  ");
            return result;

        }
    }
}
