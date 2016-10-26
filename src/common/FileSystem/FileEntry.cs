using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.FileSystem
{
    public class Dummy
    {

    }

    public class FileEntry
    {
        public readonly string Hash;
        public readonly string Name;
        public readonly string Path;

        public FileEntry(string hash, string path)
        {
            this.Path = path;
            this.Name = System.IO.Path.GetFileName(path);
            this.Hash = hash;
        }

        public static ChangeType Compare(FileEntry left, FileEntry right)
        {
            if (left == null && right != null)
                return ChangeType.Create;
            else if (left != null && right == null)
                return ChangeType.Delete;
            else if (left.Hash != right.Hash)
                return ChangeType.Replace;

            return ChangeType.OK;
        }

        public override string ToString()
        {
            return $"{{ Hash: {Hash}, Name: {Name}, Path: {Path} }}";
        }

        //public string ObjectFolder { get { return System.IO.Path.Combine(Config.ObjectPath, Hash.Substring(0, 2)); } }

        //public string ObjectFileName { get { return Hash.Substring(2); } }

        //public string ObjectPath { get { return System.IO.Path.Combine(ObjectFolder, ObjectFileName); } }
        //public abstract Stream GetStream();
    }
}
