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

        public static bool operator ==(FileEntry left, FileEntry right)
        {
            if ((object)left == null)
                return (object)right == null;

            return left.Equals(right);
        }

        public static bool operator !=(FileEntry left, FileEntry right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((FileEntry)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + NameHash.GetHashCode();
                hash = hash * 23 + DataHash.GetHashCode();
                return hash;
            }
        }

        public bool Equals(FileEntry other)
        {
            if ((object)other == null) return false;
            return NameHash == other.NameHash && DataHash == other.DataHash;
        }
    }
}
