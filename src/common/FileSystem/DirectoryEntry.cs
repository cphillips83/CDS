using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.FileSystem
{
    public class DirectoryEntry
    {
        public readonly string Hash;
        public readonly string Path;
        public readonly string Name;
        public readonly Dictionary<string, FileEntry> Files = new Dictionary<string, FileEntry>();
        public readonly Dictionary<string, DirectoryEntry> Directories = new Dictionary<string, DirectoryEntry>();

        public DirectoryEntry(string hash, string path)
        {
            Hash = hash;
            Path = path;
            Name = System.IO.Path.GetFileName(path);
        }

        public void AddFiles(FileEntry[] entries)
        {
            foreach (var fe in entries)
                if (fe != null)
                    Files.Add(fe.NameHash, fe);
        }

        public void AddDirectories(DirectoryEntry[] entries)
        {
            foreach (var de in entries)
                if (de != null)
                    Directories.Add(de.Hash, de);
        }

        public static ChangeType Compare(DirectoryEntry left, DirectoryEntry right)
        {
            if (left == null && right != null)
                return ChangeType.Create;
            else if (left != null && right == null)
                return ChangeType.Delete;

            return ChangeType.OK;
        }

        public static bool FindChanges(DirectoryEntry left, DirectoryEntry right, Action<ChangeEntry> callback)
        {
            var result = false;
            CompareDirectories(left, right, callback, ref result);
            return result;
        }

        public static bool FindChanges(DirectoryEntry left, DirectoryEntry right, BlockingCollection<ChangeEntry> changes)
        {
            var result = FindChanges(left, right, x => changes.Add(x));
            changes.CompleteAdding();
            return result;
        }

        private static void CompareDirectories(DirectoryEntry left, DirectoryEntry right, Action<ChangeEntry> callback, ref bool result)
        {
            CompareFiles(left, right, callback, ref result);

            var lefthash = left != null ? new HashSet<string>(left.Directories.Keys) : new HashSet<string>();
            var righthash = right != null ? new HashSet<string>(right.Directories.Keys) : new HashSet<string>();
            var bothhash = new HashSet<string>(lefthash);
            foreach (var key in righthash)
                bothhash.Add(key);

            foreach (var dir in bothhash)
            {
                var sdleft = left != null ? left.Directories.SafeGet(dir) : null;
                var sdright = right != null ? right.Directories.SafeGet(dir) : null;

                var r = DirectoryEntry.Compare(sdleft, sdright);
                if (r != ChangeType.OK)
                {
                    callback(new ChangeEntry(dir, r, true));
                    result = true;
                }

                if (r != ChangeType.Delete)
                    CompareDirectories(sdleft, sdright, callback, ref result);
            }
        }

        private static void CompareFiles(DirectoryEntry left, DirectoryEntry right, Action<ChangeEntry> callback, ref bool result)
        {
            var hash = new HashSet<string>();

            if (left != null)
            {
                foreach (var fe in left.Files.Keys)
                    hash.Add(fe);
            }

            if (right != null)
            {
                foreach (var fe in right.Files.Keys)
                    hash.Add(fe);
            }

            foreach (var file in hash)
            {
                var fileleft = left != null ? left.Files.SafeGet(file) : null;
                var fileright = right != null ? right.Files.SafeGet(file) : null;
                var r = FileEntry.Compare(fileleft, fileright);

                if (r != ChangeType.OK)
                {
                    callback(new ChangeEntry(file, r, false));
                    result = true;
                }
            }
        }

        public override string ToString()
        {
            return $"{{ Name: {Name}, Files: {Files.Count}, Directories: {Directories.Count}, Path: {Path} }}";
        }
    }
}
