using CDS;
using CDS.FileSystem;
using CDS.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unit
{
    [TestClass]
    public class ChangeSetTests
    {
        static ChangeSetTests()
        {
            EmbeddedAssembly.InitializeResolver();
        }

        [TestMethod]
        public void ChangeSet_CompareEmpty_NoChanges()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(new string('0', 32), string.Empty);
            var right = new DirectoryEntry(new string('0', 32), string.Empty);

            var changeSet = new GetChangeSet(left, right);
            changeSet.Process();
            changeSet.Wait();

            Assert.AreEqual(0, changeSet.Changes.Count);
        }

        [TestMethod]
        public void ChangeSet_CompareSubdir_NoChanges()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(new string('0', 32), string.Empty);
            var right = new DirectoryEntry(new string('0', 32), string.Empty);

            left.AddDirectories(new DirectoryEntry[] { new DirectoryEntry(new string('1', 32), "subdir") });
            right.AddDirectories(new DirectoryEntry[] { new DirectoryEntry(new string('1', 32), "subdir") });
            
            var changeSet = new GetChangeSet(left, right);
            changeSet.Process();
            changeSet.Wait();

            Assert.AreEqual(0, changeSet.Changes.Count);
        }

        [TestMethod]
        public void ChangeSet_CompareSubdir_Create()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(new string('0', 32), string.Empty);
            var right = new DirectoryEntry(new string('0', 32), string.Empty);

            //left.AddDirectories(new DirectoryEntry[] { new DirectoryEntry(new string('1', 32), "subdir") });
            right.AddDirectories(new DirectoryEntry[] { new DirectoryEntry(new string('2', 32), "subdir2") });

            var changeSet = new GetChangeSet(left, right);
            changeSet.Process();
            changeSet.Wait();

            Assert.AreEqual(1, changeSet.Changes.Count);
            Assert.AreEqual(ChangeType.Create, changeSet.Changes.Take().Type);
        }

        [TestMethod]
        public void ChangeSet_CompareSubdir_Delete()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(new string('0', 32), string.Empty);
            var right = new DirectoryEntry(new string('0', 32), string.Empty);

            left.AddDirectories(new DirectoryEntry[] { new DirectoryEntry(new string('1', 32), "subdir") });
            //right.AddDirectories(new DirectoryEntry[] { new DirectoryEntry(new string('2', 32), "subdir2") });

            var changeSet = new GetChangeSet(left, right);
            changeSet.Process();
            changeSet.Wait();

            Assert.AreEqual(1, changeSet.Changes.Count);
            Assert.AreEqual(ChangeType.Delete, changeSet.Changes.Take().Type);
        }

        [TestMethod]
        public void ChangeSet_CompareSubdir_DeleteOnlyParent()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(new string('0', 32), string.Empty);
            var right = new DirectoryEntry(new string('0', 32), string.Empty);

            var subleft = new DirectoryEntry[] { new DirectoryEntry(new string('1', 32), "subdir") };
            subleft[0].AddDirectories(new DirectoryEntry[] { new DirectoryEntry(new string('2', 32), "subdir") });
            left.AddDirectories(subleft);

            var changeSet = new GetChangeSet(left, right);
            changeSet.Process();
            changeSet.Wait();

            Assert.AreEqual(1, changeSet.Changes.Count);
            Assert.AreEqual(ChangeType.Delete, changeSet.Changes.Take().Type);
        }

        [TestMethod]
        public void ChangeSet_CompareSubdir_CreateParentAndChild()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(new string('0', 32), string.Empty);
            var right = new DirectoryEntry(new string('0', 32), string.Empty);

            var subright = new DirectoryEntry[] { new DirectoryEntry(new string('1', 32), "subdir") };
            subright[0].AddDirectories(new DirectoryEntry[] { new DirectoryEntry(new string('2', 32), "subdir") });
            right.AddDirectories(subright);

            var changeSet = new GetChangeSet(left, right);
            changeSet.Process();
            changeSet.Wait();

            Assert.AreEqual(2, changeSet.Changes.Count);
            Assert.AreEqual(ChangeType.Create, changeSet.Changes.Take().Type);
            Assert.AreEqual(ChangeType.Create, changeSet.Changes.Take().Type);
        }


        [TestMethod]
        public void ChangeSet_CompareSubdirFiles_CreateParentAndChild()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(new string('0', 32), string.Empty);
            var right = new DirectoryEntry(new string('0', 32), string.Empty);

            right.AddFiles(new FileEntry[] { new FileEntry(new string('4', 32), new string('4', 32), "subfile") });

            var subright = new DirectoryEntry[] { new DirectoryEntry(new string('1', 32), "subdir") };
            subright[0].AddFiles(new FileEntry[] { new FileEntry(new string('3', 32), new string('3', 32), "subfile") });
            subright[0].AddDirectories(new DirectoryEntry[] { new DirectoryEntry(new string('2', 32), "subdir") });
            right.AddDirectories(subright);

            var changeSet = new GetChangeSet(left, right);
            changeSet.Process();
            changeSet.Wait();

            Assert.AreEqual(4, changeSet.Changes.Count);

            var result = changeSet.Changes.Take();
            Assert.AreEqual(ChangeType.Create, result.Type);
            Assert.AreEqual(new string('4', 32), result.Hash);
            Assert.AreEqual(false, result.IsDirectory);

            result = changeSet.Changes.Take();
            Assert.AreEqual(ChangeType.Create, result.Type);
            Assert.AreEqual(new string('1', 32), result.Hash);
            Assert.AreEqual(true, result.IsDirectory);

            result = changeSet.Changes.Take();
            Assert.AreEqual(ChangeType.Create, result.Type);
            Assert.AreEqual(new string('3', 32), result.Hash);
            Assert.AreEqual(false, result.IsDirectory);

            result = changeSet.Changes.Take();
            Assert.AreEqual(ChangeType.Create, result.Type);
            Assert.AreEqual(new string('2', 32), result.Hash);
            Assert.AreEqual(true, result.IsDirectory);
        }

        [TestMethod]
        public void ChangeSet_CompareSubdirFiles_DeleteParentAndRoot()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(new string('0', 32), string.Empty);
            var right = new DirectoryEntry(new string('0', 32), string.Empty);

            left.AddFiles(new FileEntry[] { new FileEntry(new string('4', 32), new string('4', 32), "subfile") });

            var subleft = new DirectoryEntry[] { new DirectoryEntry(new string('1', 32), "subdir") };
            subleft[0].AddFiles(new FileEntry[] { new FileEntry(new string('3', 32), new string('3', 32), "subfile") });
            subleft[0].AddDirectories(new DirectoryEntry[] { new DirectoryEntry(new string('2', 32), "subdir") });
            left.AddDirectories(subleft);

            var changeSet = new GetChangeSet(left, right);
            changeSet.Process();
            changeSet.Wait();

            Assert.AreEqual(2, changeSet.Changes.Count);

            var result = changeSet.Changes.Take();
            Assert.AreEqual(ChangeType.Delete, result.Type);
            Assert.AreEqual(new string('4', 32), result.Hash);
            Assert.AreEqual(false, result.IsDirectory);

            result = changeSet.Changes.Take();
            Assert.AreEqual(ChangeType.Delete, result.Type);
            Assert.AreEqual(new string('1', 32), result.Hash);
            Assert.AreEqual(true, result.IsDirectory);

        }

        [TestMethod]
        public void ChangeSet_CompareSubdirFiles_ReplaceFiles()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(new string('0', 32), string.Empty);
            var right = new DirectoryEntry(new string('0', 32), string.Empty);

            left.AddFiles(new FileEntry[] { new FileEntry(new string('4', 32), new string('4', 32), "subfile") });

            var subleft = new DirectoryEntry[] { new DirectoryEntry(new string('1', 32), "subdir") };
            subleft[0].AddFiles(new FileEntry[] { new FileEntry(new string('3', 32), new string('3', 32), "subfile") });
            subleft[0].AddDirectories(new DirectoryEntry[] { new DirectoryEntry(new string('2', 32), "subdir") });
            left.AddDirectories(subleft);

            right.AddFiles(new FileEntry[] { new FileEntry(new string('4', 32), new string('6', 32), "subfile") });

            var subright = new DirectoryEntry[] { new DirectoryEntry(new string('1', 32), "subdir") };
            subright[0].AddFiles(new FileEntry[] { new FileEntry(new string('3', 32), new string('5', 32), "subfile") });
            subright[0].AddDirectories(new DirectoryEntry[] { new DirectoryEntry(new string('2', 32), "subdir") });
            right.AddDirectories(subright);

            var changeSet = new GetChangeSet(left, right);
            changeSet.Process();
            changeSet.Wait();

            Assert.AreEqual(2, changeSet.Changes.Count);

            var result = changeSet.Changes.Take();
            Assert.AreEqual(ChangeType.Replace, result.Type);
            Assert.AreEqual(new string('4', 32), result.Hash);
            Assert.AreEqual(false, result.IsDirectory);

            result = changeSet.Changes.Take();
            Assert.AreEqual(ChangeType.Replace, result.Type);
            Assert.AreEqual(new string('3', 32), result.Hash);
            Assert.AreEqual(false, result.IsDirectory);

        }
    }
}
