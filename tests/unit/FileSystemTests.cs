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
            var left = new FileEntry("04818562074444716185293154509581", "04818562074444716185293154509581", @"C:\path\file.txt");
            var right = new FileEntry("04818562074444716185293154509581", "04818562074444716185293154509581", @"C:\path\file.txt");

            var result = FileEntry.Compare(left, right);

            Assert.AreEqual(ChangeType.OK, result);
        }

        [TestMethod]
        public void FileSystem_CompareFile_Replace()
        {
            var left = new FileEntry("04818562074444716185293154509581", "04818562074444716185293154509581", @"C:\path\file.txt");
            var right = new FileEntry("12374828797858048973219621096805", "12374828797858048973219621096805", @"C:\path\file.txt");

            var result = FileEntry.Compare(left, right);

            Assert.AreEqual(ChangeType.Replace, result);
        }

        [TestMethod]
        public void FileSystem_CompareFile_Create()
        {
            var right = new FileEntry("04818562074444716185293154509581", "04818562074444716185293154509581", @"C:\path\file.txt");

            var result = FileEntry.Compare(null, right);

            Assert.AreEqual(ChangeType.Create, result);
        }

        [TestMethod]
        public void FileSystem_CompareFile_Delete()
        {
            var left = new FileEntry("04818562074444716185293154509581", "04818562074444716185293154509581", @"C:\path\file.txt");

            var result = FileEntry.Compare(left, null);

            Assert.AreEqual(ChangeType.Delete, result);
        }

        [TestMethod]
        public void FileSystem_CompareRootDir_OK()
        {
            var left = new DirectoryEntry(new string('0', 32), string.Empty);
            var right = new DirectoryEntry(new string('0', 32), string.Empty);

            var result = DirectoryEntry.Compare(left, right);

            Assert.AreEqual(ChangeType.OK, result);
        }

        [TestMethod]
        public void FileSystem_CompareDir_OK()
        {
            var left = new DirectoryEntry(new string('0', 32), "subdir");
            var right = new DirectoryEntry(new string('0', 32), "subdir");

            var result = DirectoryEntry.Compare(left, right);

            Assert.AreEqual(ChangeType.OK, result);
        }

        [TestMethod]
        public void FileSystem_CompareDir_Delete()
        {
            var left = new DirectoryEntry(new string('0', 32), "subdir");

            var result = DirectoryEntry.Compare(left, null);

            Assert.AreEqual(ChangeType.Delete, result);
        }

        [TestMethod]
        public void FileSystem_CompareDir_Create()
        {
            var right = new DirectoryEntry(new string('0', 32), "subdir");

            var result = DirectoryEntry.Compare(null, right);

            Assert.AreEqual(ChangeType.Create, result);
        }
    }
}
