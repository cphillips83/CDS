using System;

namespace CDS.FileSystem
{
    public class ChangeEntry
    {
        public readonly ChangeType Type;
        public readonly string Hash;
        public readonly bool IsDirectory;

        public ChangeEntry(string hash, ChangeType type, bool isDirectory)
        {
            Hash = hash;
            Type = type;
            IsDirectory = isDirectory;
        }

        //public string GetChangeLog()
        //{
        //    if (IsDirectory)
        //    {
        //        if (Type == ChangeType.Create) return $"dc {Path}";
        //        else if (Type == ChangeType.Delete) return $"dd {Path}";
        //    }
        //    else
        //    {
        //        if (Type == ChangeType.Create) return $"fc {Path}";
        //        if (Type == ChangeType.Delete) return $"fd {Path}";
        //        if (Type == ChangeType.Replace) return $"fr {Path}";
        //    }

        //    var fstype = IsDirectory ? "directory" : "file";
        //    throw new NotImplementedException($"Type[{Type.ToString()}] not implemented for {fstype}.");
        //}
    }
}
