//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CDS.FileSystem
//{
//    public class MergedEntry
//    {
//        public readonly string Hash;
//        public readonly HashSet<string> Files = new HashSet<string>();
//        public readonly Dictionary<string, MergedEntry> Directories = new Dictionary<string, MergedEntry>();

//        public MergedEntry(string hash)
//        {
//            Hash = hash;
//        }

//        public void Add(DirectoryEntry de)
//        {
//            var entries = new Stack<DirectoryEntry>();
//            entries.Push(de);

//            while(entries.Count > 0)
//            {
//                var e = entries.Pop();
//                Directories.(e.Hash);
//            }
//        }

//        public static MergedEntry Create(MergedEntry me, DirectoryEntry left, DirectoryEntry right)
//        {
//            foreach (var f in left.Files.Keys)
//                me.Files.Add(f);

//            foreach (var f in right.Files.Keys)
//                me.Files.Add(f);

//            foreach (var d in left.Directories.Keys)
//            {

//                me.Directories.Add(d);
//            }

//            foreach (var d in right.Directories.Keys)
//            {

//                me.Directories.Add(d);
//            }
//        }

//        public static MergedEntry Create(DirectoryEntry left, DirectoryEntry right)
//        {
//            var me = new MergedEntry(new string('0', 32));
//            Create(me, left, right);

//            return me;
//        }
//    }
//}
