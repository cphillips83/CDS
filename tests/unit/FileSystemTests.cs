using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDS;
using CDS.FileSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace unit
{
    [TestClass]
    public class FileSystemTests
    {
        static FileSystemTests()
        {
            EmbeddedAssembly.InitializeResolver();
        }

        [TestMethod]
        public void FileSystem_CompareFile_OK()
        {
            var left = new FileEntry(Hash.Create(1, 2, 3, 4), Hash.Create(4, 3, 2, 1), @"C:\path\file.txt");
            var right = new FileEntry(Hash.Create(1, 2, 3, 4), Hash.Create(4, 3, 2, 1), @"C:\path\file.txt");

            var result = FileEntry.Compare(left, right);

            Assert.AreEqual(ChangeType.OK, result);
        }

        [TestMethod]
        public void FileSystem_CompareFile_Replace()
        {
            var left = new FileEntry(Hash.Create(1, 0, 0, 0), Hash.Create(1, 1, 1, 1), @"C:\path\file.txt");
            var right = new FileEntry(Hash.Create(1, 0, 0, 0), Hash.Create(2, 2, 2, 2), @"C:\path\file.txt");

            var result = FileEntry.Compare(left, right);

            Assert.AreEqual(ChangeType.Replace, result);
        }

        [TestMethod]
        public void FileSystem_CompareFile_Create()
        {
            var right = new FileEntry(Hash.Create(1, 0, 0, 0), Hash.Create(1, 1, 1, 1), @"C:\path\file.txt");

            var result = FileEntry.Compare(null, right);

            Assert.AreEqual(ChangeType.Create, result);
        }

        [TestMethod]
        public void FileSystem_CompareFile_Delete()
        {
            var left = new FileEntry(Hash.Create(1, 0, 0, 0), Hash.Create(1, 1, 1, 1), @"C:\path\file.txt");

            var result = FileEntry.Compare(left, null);

            Assert.AreEqual(ChangeType.Delete, result);
        }

        [TestMethod]
        public void FileSystem_CompareRootDir_OK()
        {
            var left = new DirectoryEntry(Hash.Empty, string.Empty);
            var right = new DirectoryEntry(Hash.Empty, string.Empty);

            var result = DirectoryEntry.Compare(left, right);

            Assert.AreEqual(ChangeType.OK, result);
        }

        [TestMethod]
        public void FileSystem_CompareDir_OK()
        {
            var left = new DirectoryEntry(Hash.Empty, string.Empty);
            var right = new DirectoryEntry(Hash.Empty, string.Empty);

            var sleft = new DirectoryEntry(Hash.Empty, "subdir");
            var sright = new DirectoryEntry(Hash.Empty, "subdir");

            left.AddDirectories(new DirectoryEntry[] { sleft });
            right.AddDirectories(new DirectoryEntry[] { sright });

            var result = DirectoryEntry.Compare(left, right);

            Assert.AreEqual(ChangeType.OK, result);
        }

        [TestMethod]
        public void FileSystem_CompareDir_Delete()
        {
            var left = new DirectoryEntry(Hash.Empty, "subdir");

            var result = DirectoryEntry.Compare(left, null);

            Assert.AreEqual(ChangeType.Delete, result);
        }

        [TestMethod]
        public void FileSystem_CompareDir_Create()
        {
            var right = new DirectoryEntry(Hash.Empty, "subdir");

            var result = DirectoryEntry.Compare(null, right);

            Assert.AreEqual(ChangeType.Create, result);
        }
    }
}
