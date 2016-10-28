using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.FileSystem
{
    public struct Hash : IEquatable<Hash>
    {
        private ulong _val0;
        private ulong _val1;
        private ulong _val2;
        private ulong _val3;

        public override string ToString()
        {
            var sb = new char[64];
            GetString(sb, 0, _val0);
            GetString(sb, 16, _val1);
            GetString(sb, 32, _val2);
            GetString(sb, 48, _val3);

            return new string(sb);
        }

        public byte[] ToBytes()
        {
            byte[] data = new byte[32];
            GetBytes(data, 0, _val0);
            GetBytes(data, 8, _val1);
            GetBytes(data, 16, _val2);
            GetBytes(data, 24, _val3);
            return data;
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_val0);
            bw.Write(_val1);
            bw.Write(_val2);
            bw.Write(_val3);
        }

        public static Hash Create(BinaryReader br)
        {
            var hash = new Hash();
            hash._val0 = br.ReadUInt64();
            hash._val1 = br.ReadUInt64();
            hash._val2 = br.ReadUInt64();
            hash._val3 = br.ReadUInt64();
            return hash;
        }

        public static Hash Create(ulong val0, ulong val1, ulong val2, ulong val3)
        {
            //byte[] data = new byte[32];
            //GetBytes(data, 0, val0);
            //GetBytes(data, 8, val1);
            //GetBytes(data, 16, val2);
            //GetBytes(data, 24, val3);

            //var hash = Create(data);
            //if (hash._val0 != val0 || hash._val1 != val1 || 
            //    hash._val2 != val2 || hash._val3 != val3)
            //    throw new Exception("Conversion failed");
            var hash = new Hash();
            hash._val0 = val0;
            hash._val1 = val1;
            hash._val2 = val2;
            hash._val3 = val3;
            return hash;
        }

        public static Hash Create(byte[] data)
        {
            if (data.Length != 32)
                throw new Exception("Hash struct requires 32 bytes");

            var hash = new Hash();
            hash._val0 = GetLong(data, 0);
            hash._val1 = GetLong(data, 8);
            hash._val2 = GetLong(data, 16);
            hash._val3 = GetLong(data, 24);

            return hash;
        }

        public static Hash Create(string data)
        {
            if (data.Length != 64)
                throw new Exception("Hash struct requires 64 chars");

            data = data.ToUpper();

            var bytes = new byte[32];
            for (int i = 0; i < bytes.Length; i += 2)
            {
                var ch0 = data[i * 2];
                var ch1 = data[i * 2 + 1];

                var b = 0;
                if (ch0 >= 48 && ch0 <= 57)
                    b = (ch0 - 48) * 16;
                else if (ch0 >= 65 && ch0 <= 70)
                    b = (ch0 - 65 + 10) * 16;
                else
                    throw new ArgumentOutOfRangeException("data");

                if (ch1 >= 48 && ch1 <= 57)
                    b += (ch1 - 48);
                else if (ch1 >= 65 && ch1 <= 70)
                    b += (ch1 - 65 + 10);
                else
                    throw new ArgumentOutOfRangeException("data");

                bytes[i] = (byte)b;
            }

            return Create(bytes);
        }

        private static void GetBytes(byte[] data, int index, ulong val)
        {
            for (var k = 0; k < 8; k++)
            {
                data[(7 - k) + index] = (byte)(val & 0xff);
                val >>= 8;
            }
        }

        private static void GetString(char[] sb, int index, ulong val)
        {
            for (var k = 0; k < 16; k++)
            {
                var b = val & 0xf;
                if (b < 10)
                    sb[(15 - k) + index] = (char)(48 + b);
                else
                    sb[(15 - k) + index] = (char)(65 + b - 10);

                val >>= 4;
            }
        }

        private static ulong GetLong(byte[] data, int index)
        {
            ulong val = 0;
            for (var k = 0; k < 8; k++)
            {
                val <<= 8;
                val += data[index +  k];
            }
            return val;
        }

        public static bool operator ==(Hash left, Hash right)
        {
            return left._val0 == right._val0 && left._val1 == right._val1 &&
                left._val2 == right._val2 && left._val3 == right._val3;
        }

        public static bool operator !=(Hash left, Hash right)
        {
            return left._val0 != right._val0 || left._val1 != right._val1 ||
                left._val2 != right._val2 || left._val3 != right._val3;
        }

        public static Hash Empty { get { return Create(0, 0, 0, 0); } }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals( (Hash)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + _val0.GetHashCode();
                hash = hash * 23 + _val1.GetHashCode();
                hash = hash * 23 + _val2.GetHashCode();
                hash = hash * 23 + _val3.GetHashCode();
                return hash;
            }
        }

        public bool Equals(Hash other)
        {
            return _val0 == other._val0 && _val1 == other._val1 &&
                    _val2 == other._val2 && _val3 == other._val3;
        }
    }
}
