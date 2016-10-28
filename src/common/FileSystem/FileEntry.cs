using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.FileSystem
{
    public class FileEntry
    {
        public readonly Hash NameHash;
        public readonly Hash DataHash;

        public FileEntry(Hash nameHash, Hash dataHash)
        {
            this.NameHash = nameHash;
            this.DataHash = dataHash;
        }

        public void WriteManifest(BinaryWriter bw)
        {
            NameHash.Write(bw);
            DataHash.Write(bw);
        }

        public static FileEntry ReadManifest(BinaryReader br)
        {
            var nameHash = Hash.Create(br);
            var dataHash = Hash.Create(br);
            return new FileSystem.FileEntry(nameHash, dataHash);
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
            return $"{{ NameHash: {NameHash}, DataHash: {DataHash} }}";
        }

        //public string ObjectFolder { get { return System.IO.Path.Combine(Config.ObjectPath, Hash.Substring(0, 2)); } }

        //public string ObjectFileName { get { return Hash.Substring(2); } }

        //public string ObjectPath { get { return System.IO.Path.Combine(ObjectFolder, ObjectFileName); } }
        //public abstract Stream GetStream();
    }
}
