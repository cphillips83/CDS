﻿using System;
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

        //public static List<ChangeEntry> CompareAll(DirectoryEntry left, DirectoryEntry right)
        //{
        //    var changes = new List<ChangeEntry>();

        //}

        //private static void CompareFiles(DirectoryEntry left, DirectoryEntry right, List<ChangeEntry> changes)
        //{
        //    var hash = new HashSet<string>();
        //    foreach (var fe in left.Files.Keys)
        //        hash.Add(fe);

        //    foreach (var fe in right.Files.Keys)
        //        hash.Add(fe);

        //    foreach (var file in hash)
        //    {
        //        var leftfe = left.Files[file];
        //        var rightfe = right.Files[file];

        //        var result = FileEntry.Compare(leftfe, rightfe);

        //        var compare = CompareFile(other, file);
        //        if (compare != null)
        //            changes.Add(compare);
        //    }
        //}

        public override string ToString()
        {
            return $"{{ Name: {Name}, Files: {Files.Count}, Directories: {Directories.Count}, Path: {Path} }}";
        }
    }
}
