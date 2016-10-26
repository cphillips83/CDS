using System.Collections.Generic;

namespace CDS.FileSystem
{
    public interface IFileSystem 
    {
        DirectoryEntry Root { get; }
        //IEnumerable<FileEntry> GetFiles(string path);
        //FileEntry GetFile(string path);
    }
}
