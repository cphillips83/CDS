using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDS.FileSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace unit
{
    [TestClass]
    public class ManifestTest
    {

        [TestMethod]
        public void Manifest_ReadWrite_NoChanges()
        {
            DirectoryEntry right = null;
            var left = new DirectoryEntry(Hash.Empty);

            left.AddFiles(new FileEntry[] { new FileEntry(Hash.Create(4, 0, 0, 0), Hash.Create(4, 0, 0, 0)) });

            var subleft = new DirectoryEntry[] { new DirectoryEntry(Hash.Create(1, 0, 0, 0)) };
            subleft[0].AddFiles(new FileEntry[] { new FileEntry(Hash.Create(3, 0, 0, 0), Hash.Create(3, 0, 0, 0)) });
            subleft[0].AddDirectories(new DirectoryEntry[] { new DirectoryEntry(Hash.Create(2, 0, 0, 0)) });
            left.AddDirectories(subleft);

            byte[] data;
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                left.WriteManifest(bw);
                ms.Flush();
                data = ms.ToArray();
            }

            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                right = DirectoryEntry.ReadManifest(br);
            }

            Assert.IsFalse(DirectoryEntry.FindChanges(left, right));
        }

        [TestMethod]
        public void Manifest_ReadWrite_Changes()
        {
            DirectoryEntry right = null;
            var left = new DirectoryEntry(Hash.Empty);

            left.AddFiles(new FileEntry[] { new FileEntry(Hash.Create(4, 0, 0, 0), Hash.Create(4, 0, 0, 0)) });

            var subleft = new DirectoryEntry[] { new DirectoryEntry(Hash.Create(1, 0, 0, 0)) };
            subleft[0].AddFiles(new FileEntry[] { new FileEntry(Hash.Create(3, 0, 0, 0), Hash.Create(3, 0, 0, 0)) });
            subleft[0].AddDirectories(new DirectoryEntry[] { new DirectoryEntry(Hash.Create(2, 0, 0, 0)) });
            left.AddDirectories(subleft);

            byte[] data;
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                left.WriteManifest(bw);
                ms.Flush();
                data = ms.ToArray();
            }

            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                right = DirectoryEntry.ReadManifest(br);
                right.AddFiles(new FileEntry[] { new FileEntry(Hash.Create(7, 0, 0, 0), Hash.Create(7, 0, 0, 0)) });
            }

            Assert.IsTrue(DirectoryEntry.FindChanges(left, right));
        }
    }
}
