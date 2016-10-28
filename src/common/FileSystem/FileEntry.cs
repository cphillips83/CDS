using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.FileSystem
{
    public class FileEntry
    {
        public readonly Hash NameHash;
        public readonly Hash DataHash;
        public readonly string Name;
        public readonly string Path;

        public FileEntry(Hash nameHash, Hash dataHash, string path)
        {
            this.NameHash = nameHash;
            this.DataHash = dataHash;
            this.Path = path;
            this.Name = System.IO.Path.GetFileName(path);
        }

        public static ChangeEntryAction Compare(FileEntry left, FileEntry right)
        {
            if (left == null && right != null)
                return ChangeEntryAction.Create;
            else if (left != null && right == null)
                return ChangeEntryAction.Delete;
            else if (left.DataHash != right.DataHash)
                return ChangeEntryAction.Replace;

            return ChangeEntryAction.OK;
        }

        public override string ToString()
        {
            return $"{{ NameHash: {NameHash}, DataHash: {DataHash}, Name: {Name}, Path: {Path} }}";
        }

        //public string ObjectFolder { get { return System.IO.Path.Combine(Config.ObjectPath, Hash.Substring(0, 2)); } }

        //public string ObjectFileName { get { return Hash.Substring(2); } }

        //public string ObjectPath { get { return System.IO.Path.Combine(ObjectFolder, ObjectFileName); } }
        //public abstract Stream GetStream();
    }
}
