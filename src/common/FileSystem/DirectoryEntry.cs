using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.FileSystem
{
    public class DirectoryEntry : IEquatable<DirectoryEntry>
    {
        public readonly Hash Hash;
        public readonly Dictionary<Hash, FileEntry> Files = new Dictionary<Hash, FileEntry>();
        public readonly Dictionary<Hash, DirectoryEntry> Directories = new Dictionary<Hash, DirectoryEntry>();

        public DirectoryEntry(Hash hash)
        {
            Hash = hash;
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

        public void WriteManifest(BinaryWriter bw)
        {
            Hash.Write(bw);
            bw.Write(Directories.Count);
            foreach (var de in Directories.Values)
                de.WriteManifest(bw);

            bw.Write(Files.Count);
            foreach (var fe in Files.Values)
                fe.WriteManifest(bw);
        }

        public static DirectoryEntry ReadManifest(BinaryReader br)
        {
            var hash = Hash.Create(br);
            var de = new DirectoryEntry(hash);

            var dirs = br.ReadInt32();
            var subdirs = new DirectoryEntry[dirs];
            for (int i = 0; i < dirs; i++)
                subdirs[i] = ReadManifest(br);

            de.AddDirectories(subdirs);

            var files = br.ReadInt32();
            var subfiles = new FileEntry[files];
            for (int i = 0; i < files; i++)
                subfiles[i] = FileEntry.ReadManifest(br);

            de.AddFiles(subfiles);

            return de;        
        }

        public static ChangeEntryAction Compare(DirectoryEntry left, DirectoryEntry right)
        {
            if (left == null && right != null)
                return ChangeEntryAction.Create;
            else if (left != null && right == null)
                return ChangeEntryAction.Delete;

            return ChangeEntryAction.OK;
        }

        public static bool FindChanges(DirectoryEntry left, DirectoryEntry right)
        {
            var result = false;
            CompareDirectories(left, right, null, ref result);
            return result;
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

            var lefthash = left != null ? new HashSet<Hash>(left.Directories.Keys) : new HashSet<Hash>();
            var righthash = right != null ? new HashSet<Hash>(right.Directories.Keys) : new HashSet<Hash>();
            var bothhash = new HashSet<Hash>(lefthash);
            foreach (var key in righthash)
                bothhash.Add(key);

            foreach (var dir in bothhash)
            {
                var sdleft = left != null ? left.Directories.SafeGet(dir) : null;
                var sdright = right != null ? right.Directories.SafeGet(dir) : null;

                var r = DirectoryEntry.Compare(sdleft, sdright);
                if (r != ChangeEntryAction.OK)
                {
                    if(callback != null) callback(new ChangeEntry(dir, r, true));
                    result = true;
                }

                if (r != ChangeEntryAction.Delete)
                    CompareDirectories(sdleft, sdright, callback, ref result);
            }
        }

        private static void CompareFiles(DirectoryEntry left, DirectoryEntry right, Action<ChangeEntry> callback, ref bool result)
        {
            var hash = new HashSet<Hash>();

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

                if (r != ChangeEntryAction.OK)
                {
                    if (callback != null) callback(new ChangeEntry(file, r, false));
                    result = true;
                }
            }
        }

        public override string ToString()
        {
            return $"{{ Hash: {Hash}, Directories: {Directories.Count}, Files: {Files.Count} }}";
        }

        public static bool operator ==(DirectoryEntry left, DirectoryEntry right)
        {
            if ((object)left == null)
                return (object)right == null;

            return left.Equals(right);
        }

        public static bool operator != (DirectoryEntry left, DirectoryEntry right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((DirectoryEntry)obj);
        }

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }

        public bool Equals(DirectoryEntry other)
        {
            if ((object)other == null) return false;
            return Hash == other.Hash;
        }
    }
}
