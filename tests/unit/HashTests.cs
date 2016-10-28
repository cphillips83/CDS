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
    public class HashTests
    {
        static HashTests()
        {
            EmbeddedAssembly.InitializeResolver();
        }

        [TestMethod]
        public void Hash_Equals_Zero()
        {
            var hash = Hash.Create(new byte[32]);

            Assert.AreEqual(new string('0', 64), hash.ToString());                        
            CollectionAssert.AreEqual(new byte[32], hash.ToBytes());
        }

        [TestMethod]
        public void Hash_Equals_1000000000000000_0000000000000000_0000000000000000_0000000000000000()
        {
            var hash = Hash.Create(0x1000000000000000, 0, 0, 0);
            var data = new byte[32];
            data[0] = 16;

            Assert.AreEqual("1" + new string('0', 63), hash.ToString());
            CollectionAssert.AreEqual(data, hash.ToBytes());
        }

        [TestMethod]
        public void Hash_Equals_0000000000000000_1000000000000000_0000000000000000_0000000000000000()
        {
            var hash = Hash.Create(0, 0x1000000000000000, 0, 0);
            var data = new byte[32];
            data[8] = 16;

            Assert.AreEqual("0000000000000000100000000000000000000000000000000000000000000000", hash.ToString());
            CollectionAssert.AreEqual(data, hash.ToBytes());
        }

        [TestMethod]
        public void Hash_Equals_0000000000000000_0000000000000000_1000000000000000_0000000000000000()
        {
            var hash = Hash.Create(0, 0, 0x1000000000000000, 0);
            var data = new byte[32];
            data[16] = 16;

            Assert.AreEqual("0000000000000000000000000000000010000000000000000000000000000000", hash.ToString());
            CollectionAssert.AreEqual(data, hash.ToBytes());
        }

        [TestMethod]
        public void Hash_Equals_0000000000000000_0000000000000000_0000000000000000_1000000000000000()
        {
            var hash = Hash.Create(0, 0, 0, 0x1000000000000000);
            var data = new byte[32];
            data[24] = 16;

            Assert.AreEqual("0000000000000000000000000000000000000000000000001000000000000000", hash.ToString());
            CollectionAssert.AreEqual(data, hash.ToBytes());
        }

        [TestMethod]
        public void Hash_Equals_FF5A5A5A5A5A5A5A_A5A5A5A5A5A5A5A5_5A5A5A5A5A5A5A5A_A5A5A5A5A5A5A5FF()
        {
            var hash = Hash.Create(0xFF5A5A5A5A5A5A5A, 0xA5A5A5A5A5A5A5A5, 0x5A5A5A5A5A5A5A5A, 0xA5A5A5A5A5A5A5FF);
            var data = new byte[32];
            for(var i = 0; i< data.Length; i++)
            {
                if ((i >= 0 && i < 8) || (i >= 16 && i < 24))
                    data[i] = 0x5A;
                else
                    data[i] = 0xA5;
            }

            data[0] = 0xFF;
            data[31] = 0xFF;

            Assert.AreEqual("FF5A5A5A5A5A5A5AA5A5A5A5A5A5A5A55A5A5A5A5A5A5A5AA5A5A5A5A5A5A5FF", hash.ToString());
            CollectionAssert.AreEqual(data, hash.ToBytes());
        }
    }
}
