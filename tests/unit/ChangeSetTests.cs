using CDS;
using CDS.FileSystem;
using CDS.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
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
            var left = new DirectoryEntry(Hash.Empty, string.Empty);
            var right = new DirectoryEntry(Hash.Empty, string.Empty);

            var changeSet = new BlockingCollection<ChangeEntry>();
            var changed = DirectoryEntry.FindChanges(left, right, changeSet);

            Assert.AreEqual(false, changed);
            Assert.AreEqual(0, changeSet.Count);
        }

        [TestMethod]
        public void ChangeSet_CompareSubdir_NoChanges()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(Hash.Empty, string.Empty);
            var right = new DirectoryEntry(Hash.Empty, string.Empty);

            left.AddDirectories(new DirectoryEntry[] { new DirectoryEntry(Hash.Create(1, 0, 0, 0), "subdir") });
            right.AddDirectories(new DirectoryEntry[] { new DirectoryEntry(Hash.Create(1, 0, 0, 0), "subdir") });

            var changeSet = new BlockingCollection<ChangeEntry>();
            var changed = DirectoryEntry.FindChanges(left, right, changeSet);

            Assert.AreEqual(false, changed);
            Assert.AreEqual(0, changeSet.Count);
        }

        [TestMethod]
        public void ChangeSet_CompareSubdir_Create()
        {
            var left = new DirectoryEntry(Hash.Empty, string.Empty);
            var right = new DirectoryEntry(Hash.Empty, string.Empty);

            right.AddDirectories(new DirectoryEntry[] { new DirectoryEntry(Hash.Create(2, 0, 0, 0), "subdir2") });

            var changeSet = new BlockingCollection<ChangeEntry>();
            var changed = DirectoryEntry.FindChanges(left, right, changeSet);

            Assert.AreEqual(true, changed);
            Assert.AreEqual(1, changeSet.Count);
            Assert.AreEqual(ChangeType.Create, changeSet.Take().Type);
        }

        [TestMethod]
        public void ChangeSet_CompareSubdir_Delete()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(Hash.Empty, string.Empty);
            var right = new DirectoryEntry(Hash.Empty, string.Empty);

            left.AddDirectories(new DirectoryEntry[] { new DirectoryEntry(Hash.Create(1, 0, 0, 0), "subdir") });

            var changeSet = new BlockingCollection<ChangeEntry>();
            var changed = DirectoryEntry.FindChanges(left, right, changeSet);

            Assert.AreEqual(true, changed);
            Assert.AreEqual(1, changeSet.Count);
            Assert.AreEqual(ChangeType.Delete, changeSet.Take().Type);
        }

        [TestMethod]
        public void ChangeSet_CompareSubdir_DeleteOnlyParent()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(Hash.Empty, string.Empty);
            var right = new DirectoryEntry(Hash.Empty, string.Empty);

            var subleft = new DirectoryEntry[] { new DirectoryEntry(Hash.Create(1, 0, 0, 0), "subdir") };
            subleft[0].AddDirectories(new DirectoryEntry[] { new DirectoryEntry(Hash.Create(2, 0, 0, 0), "subdir") });
            left.AddDirectories(subleft);

            var changeSet = new BlockingCollection<ChangeEntry>();
            var changed = DirectoryEntry.FindChanges(left, right, changeSet);

            Assert.AreEqual(true, changed);
            Assert.AreEqual(1, changeSet.Count);
            Assert.AreEqual(ChangeType.Delete, changeSet.Take().Type);
        }

        [TestMethod]
        public void ChangeSet_CompareSubdir_CreateParentAndChild()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(Hash.Empty, string.Empty);
            var right = new DirectoryEntry(Hash.Empty, string.Empty);

            var subright = new DirectoryEntry[] { new DirectoryEntry(Hash.Create(1, 0, 0, 0), "subdir") };
            subright[0].AddDirectories(new DirectoryEntry[] { new DirectoryEntry(Hash.Create(2, 0, 0, 0), "subdir") });
            right.AddDirectories(subright);

            var changeSet = new BlockingCollection<ChangeEntry>();
            var changed = DirectoryEntry.FindChanges(left, right, changeSet);

            Assert.AreEqual(true, changed);
            Assert.AreEqual(2, changeSet.Count);
            Assert.AreEqual(ChangeType.Create, changeSet.Take().Type);
            Assert.AreEqual(ChangeType.Create, changeSet.Take().Type);
        }


        [TestMethod]
        public void ChangeSet_CompareSubdirFiles_CreateParentAndChild()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(Hash.Empty, string.Empty);
            var right = new DirectoryEntry(Hash.Empty, string.Empty);

            right.AddFiles(new FileEntry[] { new FileEntry(Hash.Create(4, 0, 0, 0), Hash.Create(4, 0, 0, 0), "subfile") });

            var subright = new DirectoryEntry[] { new DirectoryEntry(Hash.Create(1, 0, 0, 0), "subdir") };
            subright[0].AddFiles(new FileEntry[] { new FileEntry(Hash.Create(3, 0, 0, 0), Hash.Create(3, 0, 0, 0), "subfile") });
            subright[0].AddDirectories(new DirectoryEntry[] { new DirectoryEntry(Hash.Create(2, 0, 0, 0), "subdir") });
            right.AddDirectories(subright);

            var changeSet = new BlockingCollection<ChangeEntry>();
            var changed = DirectoryEntry.FindChanges(left, right, changeSet);

            Assert.AreEqual(true, changed);
            Assert.AreEqual(4, changeSet.Count);

            var result = changeSet.Take();
            Assert.AreEqual(ChangeType.Create, result.Type);
            Assert.AreEqual(Hash.Create(4, 0, 0, 0), result.Hash);
            Assert.AreEqual(false, result.IsDirectory);

            result = changeSet.Take();
            Assert.AreEqual(ChangeType.Create, result.Type);
            Assert.AreEqual(Hash.Create(1, 0, 0, 0), result.Hash);
            Assert.AreEqual(true, result.IsDirectory);

            result = changeSet.Take();
            Assert.AreEqual(ChangeType.Create, result.Type);
            Assert.AreEqual(Hash.Create(3, 0, 0, 0), result.Hash);
            Assert.AreEqual(false, result.IsDirectory);

            result = changeSet.Take();
            Assert.AreEqual(ChangeType.Create, result.Type);
            Assert.AreEqual(Hash.Create(2, 0, 0, 0), result.Hash);
            Assert.AreEqual(true, result.IsDirectory);
        }

        [TestMethod]
        public void ChangeSet_CompareSubdirFiles_DeleteParentAndRoot()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(Hash.Empty, string.Empty);
            var right = new DirectoryEntry(Hash.Empty, string.Empty);

            left.AddFiles(new FileEntry[] { new FileEntry(Hash.Create(4, 0, 0, 0), Hash.Create(4, 0, 0, 0), "subfile") });

            var subleft = new DirectoryEntry[] { new DirectoryEntry(Hash.Create(1, 0, 0, 0), "subdir") };
            subleft[0].AddFiles(new FileEntry[] { new FileEntry(Hash.Create(3, 0, 0, 0), Hash.Create(3, 0, 0, 0), "subfile") });
            subleft[0].AddDirectories(new DirectoryEntry[] { new DirectoryEntry(Hash.Create(2, 0, 0, 0), "subdir") });
            left.AddDirectories(subleft);

            var changeSet = new BlockingCollection<ChangeEntry>();
            var changed = DirectoryEntry.FindChanges(left, right, changeSet);

            Assert.AreEqual(true, changed);
            Assert.AreEqual(2, changeSet.Count);

            var result = changeSet.Take();
            Assert.AreEqual(ChangeType.Delete, result.Type);
            Assert.AreEqual(Hash.Create(4, 0, 0, 0), result.Hash);
            Assert.AreEqual(false, result.IsDirectory);

            result = changeSet.Take();
            Assert.AreEqual(ChangeType.Delete, result.Type);
            Assert.AreEqual(Hash.Create(1, 0, 0, 0), result.Hash);
            Assert.AreEqual(true, result.IsDirectory);

        }

        [TestMethod]
        public void ChangeSet_CompareSubdirFiles_ReplaceFiles()
        {
            //"01234567890123456789012345678901"
            var left = new DirectoryEntry(Hash.Empty, string.Empty);
            var right = new DirectoryEntry(Hash.Empty, string.Empty);

            left.AddFiles(new FileEntry[] { new FileEntry(Hash.Create(4, 0, 0, 0), Hash.Create(4, 0, 0, 0), "subfile") });

            var subleft = new DirectoryEntry[] { new DirectoryEntry(Hash.Create(1, 0, 0, 0), "subdir") };
            subleft[0].AddFiles(new FileEntry[] { new FileEntry(Hash.Create(3, 0, 0, 0), Hash.Create(3, 0, 0, 0), "subfile") });
            subleft[0].AddDirectories(new DirectoryEntry[] { new DirectoryEntry(Hash.Create(2, 0, 0, 0), "subdir") });
            left.AddDirectories(subleft);

            right.AddFiles(new FileEntry[] { new FileEntry(Hash.Create(4, 0, 0, 0), Hash.Create(6, 0, 0, 0), "subfile") });

            var subright = new DirectoryEntry[] { new DirectoryEntry(Hash.Create(1, 0, 0, 0), "subdir") };
            subright[0].AddFiles(new FileEntry[] { new FileEntry(Hash.Create(3, 0, 0, 0), Hash.Create(5, 0, 0, 0), "subfile") });
            subright[0].AddDirectories(new DirectoryEntry[] { new DirectoryEntry(Hash.Create(2, 0, 0, 0), "subdir") });
            right.AddDirectories(subright);

            var changeSet = new BlockingCollection<ChangeEntry>();
            var changed = DirectoryEntry.FindChanges(left, right, changeSet);

            Assert.AreEqual(true, changed);
            Assert.AreEqual(2, changeSet.Count);

            var result = changeSet.Take();
            Assert.AreEqual(ChangeType.Replace, result.Type);
            Assert.AreEqual(Hash.Create(4, 0, 0, 0), result.Hash);
            Assert.AreEqual(false, result.IsDirectory);

            result = changeSet.Take();
            Assert.AreEqual(ChangeType.Replace, result.Type);
            Assert.AreEqual(Hash.Create(3, 0, 0, 0), result.Hash);
            Assert.AreEqual(false, result.IsDirectory);

        }
    }
}
