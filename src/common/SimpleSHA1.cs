﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CDS.FileSystem;

namespace CDS
{
    public class SimpleSHA1
    {
        public readonly static SimpleSHA1 NonThreadSafe = new SimpleSHA1();
        //private class SimpleSHA1Internal : SHA1Managed
        //{
        //    public async Task<byte[]> ComputeHashAsync(Stream inputStream)
        //    {
        //        byte[] array = new byte[4096];
        //        int num;
        //        do
        //        {
        //            num = await inputStream.ReadAsync(array, 0, 4096).ConfigureAwait(false);
        //            if (num > 0)
        //            {
        //                this.HashCore(array, 0, num);
        //            }
        //        }
        //        while (num > 0);
        //        this.HashValue = this.HashFinal();
        //        byte[] result = (byte[])this.HashValue.Clone();
        //        this.Initialize();
        //        return result;
        //    }
        //}

        private SHA256 _hashAlgorithm = new SHA256Managed();

        public Hash ComputeHash(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Hash.Empty;

            var fileBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(value.ToLower());
            var fileHash = _hashAlgorithm.ComputeHash(fileBytes);

            return Hash.Create(fileHash);
        }

        public Hash ComputeHash(Stream inputStream)
        {
            var fileData = _hashAlgorithm.ComputeHash(inputStream);

            return Hash.Create(fileData);
        }

        //public async Task<string> ComputeHashAsync(string fileName, Stream inputStream)
        //{
        //    var fileBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(fileName);
        //    var fileHash = _hashAlgorithm.ComputeHash(fileBytes);
        //    var fileData = await _hashAlgorithm.ComputeHashAsync(inputStream);

        //    return BytesToHex(fileHash, fileData);
        //}

        //private static string BytesToHex(byte[] file)
        //{
        //    var sb = new StringBuilder(file.Length * 2);
        //    for (var i = 0; i < file.Length; i++)
        //        sb.AppendFormat("{0:X2}", file[i]);

        //    return sb.ToString();
        //}

        //private static string BytesToHex(byte[] file, byte[] data)
        //{
        //    var sb = new StringBuilder(file.Length * 2);
        //    for (var i = 0; i < file.Length; i++)
        //        sb.AppendFormat("{0:X2}", file[i] ^ data[i]);

        //    return sb.ToString();
        //}
    }
}
