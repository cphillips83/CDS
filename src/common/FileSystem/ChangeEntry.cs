using System;
using System.IO;

namespace CDS.FileSystem
{
    public class ChangeEntry
    {
        public readonly ChangeEntryAction Action;
        public readonly Hash Hash;
        public readonly bool IsDirectory;

        public ChangeEntry(Hash hash, ChangeEntryAction action, bool isDirectory)
        {
            Hash = hash;
            Action = action;
            IsDirectory = isDirectory;
        }

        public void Save(BinaryWriter bw)
        {
            var flags = (byte)Action;
            if (IsDirectory)
                flags |= 0x80;

            Hash.Write(bw);
            bw.Write(flags);
        }

        public static ChangeEntry Create(BinaryReader br)
        {
            var hash = Hash.Create(br);
            var flags = br.ReadByte();
            var isDirectory = (flags & 0x80) == 0x80;
            flags &= 0x7f;

            return new FileSystem.ChangeEntry(hash, (ChangeEntryAction)flags, isDirectory);
        }

        public string GetChangeLog()
        {
            if (IsDirectory)
            {
                if (Action == ChangeEntryAction.Create) return $"dc {Hash}";
                else if (Action == ChangeEntryAction.Delete) return $"dd {Hash}";
            }
            else
            {
                if (Action == ChangeEntryAction.Create) return $"fc {Hash}";
                if (Action == ChangeEntryAction.Delete) return $"fd {Hash}";
                if (Action == ChangeEntryAction.Replace) return $"fr {Hash}";
            }

            var fstype = IsDirectory ? "directory" : "file";
            throw new NotImplementedException($"Type[{Action.ToString()}] not implemented for {fstype}.");
        }
    }
}
