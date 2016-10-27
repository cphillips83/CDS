//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace CDS.FileSystem
//{
//    public class PhysicalFileSystem
//    {
//        private DirectoryEntry _root = null;
//        public readonly string BasePath;

//        public PhysicalFileSystem(string basePath)
//        {
//            BasePath = basePath;

//            if (!System.IO.Directory.Exists(BasePath))
//                throw new Exception($"Directory [{BasePath}] not found.");
//        }

//        public void Load()
//        {
//            var sw = Stopwatch.StartNew();
//            var fileCount = 0;
//            var dirCount = 0;
//            var dirsProcessed = 0;

//            var directories = new BlockingCollection<DirectoryEntry>();

//            _root = new DirectoryEntry(BasePath);

//            var dataTask = Task.Run(() =>
//            {
//                var dirsToProcess = new Stack<DirectoryEntry>();
//                dirsToProcess.Push(_root);

//                while (dirsToProcess.Count > 0)
//                {
//                    dirCount++;

//                    var de = dirsToProcess.Pop();
//                    directories.Add(de);

//                    var dirs = Directory.GetDirectories(de.Path);
//                    var dirEntries = new DirectoryEntry[dirs.Length];
//                    for (int i = 0; i < dirs.Length; i++)
//                    {
//                        dirEntries[i] = new DirectoryEntry(dirs[i]);
//                        if (!dirEntries[i].Path.Contains(".git") && !dirEntries[i].Path.Contains("\\cache\\"))
//                            dirsToProcess.Push(dirEntries[i]);
//                    }
//                    de.AddDirectories(dirEntries);
//                }

//                directories.CompleteAdding();
//            });

//            var processTask = Task.Run(() =>
//            {
//                Parallel.ForEach(directories.GetConsumingPartitioner(), d =>
//                {
//                    var files = Directory.GetFiles(d.Path);
//                    if (files.Length > 0)
//                    {
//                        var hasher = new FileHasher();

//                        var fileEntries = new FileEntry[files.Length];
//                        for (int i = 0; i < files.Length; i++)
//                        {
//                            using (var s = File.OpenRead(files[i]))
//                            {
//                                var hash = hasher.ComputeFileHash(files[i], s);
//                                var fe = new FileEntry(hash, files[i]);
//                                fileEntries[i] = fe;
//                            }
//                        }

//                        d.AddFiles(fileEntries);

//                        Interlocked.Add(ref fileCount, files.Length);
//                        Interlocked.Increment(ref dirsProcessed);
//                    }
//                });
//            });

//            if (Globals.ActiveConsole)
//            {
//                var index = 0;
//                var symbols = new string[] { "-", "\\", "|", "/" };
//                while (!dataTask.Wait(200))
//                {
//                    index = (index + 1) % symbols.Length;
//                    Console.CursorLeft = 0;
//                    Console.Write($"{symbols[index]} Loading [{dirCount} directories and {fileCount} files]");
//                }

//                while (!processTask.Wait(200))
//                {
//                    var percent = (int)((dirsProcessed / (float)dirCount) * 100);
//                    index = (index + 1) % symbols.Length;
//                    Console.CursorLeft = 0;
//                    Console.Write($"{symbols[index]} Processing [{dirCount} directories and {fileCount} files] {percent}%");
//                }

//                Console.CursorLeft = 0;
//                Console.WriteLine($"{symbols[index]} Processing [{dirCount} directories and {fileCount} files] 100%");
//            }
//            else
//            {
//                Task.WaitAll(dataTask, processTask);
//            }
//            sw.Stop();
//            Console.WriteLine($"Physical File System loaded in {sw.Elapsed}, found {dirCount} directories and {fileCount} files.");

//        }

//        public DirectoryEntry Root { get { return _root; } }
//    }
//}
