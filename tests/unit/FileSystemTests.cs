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
        public void FileEntry_Equals_Operator()
        {
            var fe1 = new FileEntry(Hash.Create(1, 2, 3, 4), Hash.Create(4, 3, 2, 1));
            var fe2 = new FileEntry(Hash.Create(1, 2, 3, 4), Hash.Create(4, 3, 2, 1));
            
            Assert.IsTrue(fe1 == fe2);
        }

        [TestMethod]
        public void FileEntry_NotEquals_Operator()
        {
            var fe1 = new FileEntry(Hash.Create(1, 2, 3, 4), Hash.Create(4, 3, 2, 1));
            var fe2 = new FileEntry(Hash.Create(1, 2, 3, 4), Hash.Create(4, 3, 2, 1));

            Assert.IsFalse(fe1 != fe2);
        }

        [TestMethod]
        public void DirectoryEntry_Equals_Operator()
        {
            var de1 = new DirectoryEntry(Hash.Create(1, 2, 3, 4));
            var de2 = new DirectoryEntry(Hash.Create(1, 2, 3, 4));

            Assert.IsTrue(de1 == de2);
        }

        [TestMethod]
        public void DirectoryEntry_NotEquals_Operator()
        {
            var de1 = new DirectoryEntry(Hash.Create(1, 2, 3, 4));
            var de2 = new DirectoryEntry(Hash.Create(1, 2, 3, 4));

            Assert.IsFalse(de1 != de2);
        }

        [TestMethod]
        public void FileSystem_CompareFile_OK()
        {
            var left = new FileEntry(Hash.Create(1, 2, 3, 4), Hash.Create(4, 3, 2, 1));
            var right = new FileEntry(Hash.Create(1, 2, 3, 4), Hash.Create(4, 3, 2, 1));

            var result = FileEntry.Compare(left, right);

            Assert.AreEqual(ChangeEntryAction.OK, result);
        }

        [TestMethod]
        public void FileSystem_CompareFile_Replace()
        {
            var left = new FileEntry(Hash.Create(1, 0, 0, 0), Hash.Create(1, 1, 1, 1));
            var right = new FileEntry(Hash.Create(1, 0, 0, 0), Hash.Create(2, 2, 2, 2));

            var result = FileEntry.Compare(left, right);

            Assert.AreEqual(ChangeEntryAction.Replace, result);
        }

        [TestMethod]
        public void FileSystem_CompareFile_Create()
        {
            var right = new FileEntry(Hash.Create(1, 0, 0, 0), Hash.Create(1, 1, 1, 1));

            var result = FileEntry.Compare(null, right);

            Assert.AreEqual(ChangeEntryAction.Create, result);
        }

        [TestMethod]
        public void FileSystem_CompareFile_Delete()
        {
            var left = new FileEntry(Hash.Create(1, 0, 0, 0), Hash.Create(1, 1, 1, 1));

            var result = FileEntry.Compare(left, null);

            Assert.AreEqual(ChangeEntryAction.Delete, result);
        }

        [TestMethod]
        public void FileSystem_CompareRootDir_OK()
        {
            var left = new DirectoryEntry(Hash.Empty);
            var right = new DirectoryEntry(Hash.Empty);

            var result = DirectoryEntry.Compare(left, right);

            Assert.AreEqual(ChangeEntryAction.OK, result);
        }

        [TestMethod]
        public void FileSystem_CompareDir_OK()
        {
            var left = new DirectoryEntry(Hash.Empty);
            var right = new DirectoryEntry(Hash.Empty);

            var sleft = new DirectoryEntry(Hash.Empty);
            var sright = new DirectoryEntry(Hash.Empty);

            left.AddDirectories(new DirectoryEntry[] { sleft });
            right.AddDirectories(new DirectoryEntry[] { sright });

            var result = DirectoryEntry.Compare(left, right);

            Assert.AreEqual(ChangeEntryAction.OK, result);
        }

        [TestMethod]
        public void FileSystem_CompareDir_Delete()
        {
            var left = new DirectoryEntry(Hash.Empty);

            var result = DirectoryEntry.Compare(left, null);

            Assert.AreEqual(ChangeEntryAction.Delete, result);
        }

        [TestMethod]
        public void FileSystem_CompareDir_Create()
        {
            var right = new DirectoryEntry(Hash.Empty);

            var result = DirectoryEntry.Compare(null, right);

            Assert.AreEqual(ChangeEntryAction.Create, result);
        }
    }
}
