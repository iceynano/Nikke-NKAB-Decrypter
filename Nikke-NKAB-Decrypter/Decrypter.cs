/*
* MIT License
* 
* Copyright (c) 2022 Razmoth
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Nikke_NKAB_Decrypter
{
    public static class Decrypter
    {
        private static readonly int MAGIC_BYTES = 0x42414B4E;
        private static readonly int VERSION = 3;
        private static readonly int KEY_VERSION = 3;
        private static readonly int KEY_COUNT = 5;

        private static readonly Dictionary<int, string> CRYPTO_KEYS_V0 = new Dictionary<int, string>()
            {
                { 0, "FD349BC9466D2164C7C3D70F6A74B9789BBA88E8A082AA79D1FF2BE43CB6B8E4" },
                { 1, "1E5201A2CE36D4D2C6C90960A9D83DD5582145D0A07133C99DE64075B1760A08" },
                { 2, "68513206DAEAFC575810E08DA863C1E632F74F908BF0F8C26A2092D46A06D63A" },
                { 3, "7EFBABB49C7CA113422952AA8CE61078BD05DB12D89C6AEEB52F4055D99EDEE2" },
                { 4, "09700174614C7095FC06A44F4165127839D940374E130D300A7A182A6AD52F7A" }
            };

        private static readonly Dictionary<int, string> CRYPTO_KEYS_V2 = new Dictionary<int, string>()
            {
                { 0, "22FE0B17ABCA1037016BABC2E63CA9E9491513F5BB1CA10F0BB107DE8C626047" },
                { 1, "7A624586CF64AEE191ABE5A6AFBF0D305E9335E838A509673C83E3B3D8423F13" },
                { 2, "C610B5C24C3C8412073C17F29F76F853914C35A5137B8696824986446C6ECEAA" },
                { 3, "6DA00E8237E90B4EFD1DBA233FA10E23D26A6093390D8D30A05C82EF5C7B3DD3" },
                { 4, "9EB872D96BE1AD103C215E22522F95725E4E81098313445CE3F8BC7A98E2FA11" }
            };
        private static readonly Dictionary<int, string> CRYPTO_KEYS_V3 = new Dictionary<int, string>()
            {
                { 0, "C578729E293E291E5AC4D32E0353A9E2CD01A414C074B1D3E4B637BCA5BDA633" },
                { 1, "143F8B0551033DAEC3D4D5022BC3A4D3CFBA0FC204A54C84E2F4039C0AD31C02" },
                { 2, "F2D0A0CC929A2B665C84F3BF308B66AE4839AD4E556FD74FFE64BB644A8F2468" },
                { 3, "5F3CEFF0BC900BC2FA833A0EC1D2B4F94A23E3329CAEB93EB33E9DD3EEE803E4" },
                { 4, "9CA68AD188F1AC4C6126C4A0BE46DF7D6168165D0A1DF9199603A4D3B98AFD90" }
            };
        private class BundleHeaderParam
        {
            public int Version { get; set; }
            public int KeyVersion { get; set; }
            public int KeyIndex { get; set; }
            public int ObfuscateValue { get; set; }
            public int HeaderSize { get; set; }
            public int EncryptMode { get; set; }
            public int EncryptFlags { get; set; }
            public int BlockCount { get; set; }
            public byte[] IV { get; set; }
            public byte[] KeyB { get; set; }
            public BundleHeaderParam(int ver, int keyVer, int keyIndex, int obfuscateValue, int headerSize, int encryptMode, int encryptFlags, int blockCount, byte[] iv, byte[] keyB)
            {
                Version = ver;
                KeyVersion = keyVer;
                KeyIndex = keyIndex;
                ObfuscateValue = obfuscateValue;
                HeaderSize = headerSize;
                EncryptMode = encryptMode;
                EncryptFlags = encryptFlags;
                BlockCount = blockCount;
                IV = iv;
                KeyB = keyB;
            }
        }
        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        private static bool IsEncrypted(int value, int offset)
        {
            int mask = 0x8000 >> offset;
            return (value & mask) != 0;
        }
        static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                return ms.ToArray();
            }
        }
        private static Dictionary<int, string> GetKeySet(int keyVersion)
        {
            switch (keyVersion)
            {
                case 0:
                    return CRYPTO_KEYS_V0;
                case 2:
                    return CRYPTO_KEYS_V2;
                case 3:
                    return CRYPTO_KEYS_V3;
                default: return null;
            }
        }
        private static BundleHeaderParam ReadBundleHeader(ref BinaryReader br)
        {
            int magic = br.ReadInt32();
            int version = br.ReadInt32();
            byte[] iv = br.ReadBytes(0x10);
            long pos = br.BaseStream.Position;
            br.BaseStream.Seek(-2, SeekOrigin.End);
            short obfuscateValue = br.ReadInt16();
            br.BaseStream.Position = pos;
            int headerSize = (br.ReadInt16() + obfuscateValue) & 0xFFFF;
            int keyVersion = (br.ReadInt16() + obfuscateValue) & 0xFFFF;
            int keyIndex = (br.ReadInt16() + obfuscateValue) & 0xFFFF;
            int encryptMode = (br.ReadInt16() + obfuscateValue) & 0xFFFF;
            int encryptFlags = (br.ReadInt16() + obfuscateValue) & 0xFFFF;
            int blockCount = (br.ReadInt16() + obfuscateValue) & 0xFFFF;
            byte[] keyB = br.ReadBytes(0x20);
            var header = new BundleHeaderParam(version, keyVersion, keyIndex, obfuscateValue, headerSize, encryptMode, encryptFlags, blockCount, iv, keyB);
            return header;
        }
        private static byte[] GenerateByteArray(int size)
        {
            Random rnd = new Random();
            byte[] b = new byte[size];
            rnd.NextBytes(b);
            return b;
        }

        static void EncryptBlock(ref BinaryWriter bw, ref BinaryReader br, ICryptoTransform encrypter, int flags, int size, int index = 0)
        {
            for (int i = 0; i < size; i += 0x10)
            {
                byte[] blockData = br.ReadBytes(0x10);
                if (IsEncrypted(flags, index))
                {
                    byte[] blockEncrypted = PerformCryptography(blockData, encrypter);
                    bw.Write(blockEncrypted);
                }
                else
                {
                    bw.Write(blockData);
                }
                index = (index + 1) % 0x10;
            }
        }
        public static void EncryptV3(string input, string output)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            var br = new BinaryReader(File.OpenRead(input));
            HashAlgorithm hash = SHA256.Create();
            bw.Write(MAGIC_BYTES);
            bw.Write(VERSION);
            bw.BaseStream.Position += 0x10;
            Random rd = new Random();
            int obfuscateValue = rd.Next(0, 0xFF);
            int flags = rd.Next(0xFF, 0xFFFF);
            br.BaseStream.Seek(0, SeekOrigin.Begin);
            bw.Write((short)(0x3C - obfuscateValue));
            bw.Write((short)(KEY_VERSION - obfuscateValue));
            int keyIndex = rd.Next(0, 4);
            bw.Write((short)(keyIndex - obfuscateValue));
            bw.Write((short)(1 - obfuscateValue));
            bw.Write((short)(flags - obfuscateValue));
            bw.Write((short)(obfuscateValue * -1));
            byte[] keyB = GenerateByteArray(0x20);
            bw.Write(keyB);
            var keySet = GetKeySet(KEY_VERSION);
            byte[] keyA = StringToByteArray(keySet[keyIndex]);
            byte[] key = hash.ComputeHash(keyA.Concat(keyB).ToArray());
            var aes = new AesManaged
            {
                Key = key,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.Zeros
            };
            ICryptoTransform encryptor = aes.CreateEncryptor();
            EncryptBlock(ref bw, ref br, encryptor, flags, (int)br.BaseStream.Length);
            bw.Write((short)obfuscateValue);
            br.Close();
            bw.Close();
            File.WriteAllBytes(output, ms.ToArray());
        }
        private static void DecryptBlock(ref BinaryWriter bw, ref BinaryReader br, ICryptoTransform decrypter, int flags, int size, int index = 0)
        {
            for (int i = 0; i < size; i += 0x10)
            {

                byte[] blockData = br.ReadBytes(0x10);
                if (IsEncrypted(flags, index))
                {
                    byte[] blockDecrypted = decrypter.TransformFinalBlock(blockData, 0, 0x10);
                    bw.Write(blockDecrypted);
                }
                else
                {
                    bw.Write(blockData);
                }
                index = (index + 1) % 0x10;
            }
        }
        public static void DecryptV3(string input, string output)
        {
            var br = new BinaryReader(File.OpenRead(input));
            var header = ReadBundleHeader(ref br);
            if (header.Version != VERSION) return;
            int headerBlockCount = header.BlockCount >> 8;
            int footerBlockCount = header.BlockCount & 0xFF;
            int decryptedSize = (int)(br.BaseStream.Length - br.BaseStream.Position);
            int dataOffset = (int)br.BaseStream.Position;
            var keySet = GetKeySet(header.KeyVersion);

            if (keySet == null || header.KeyIndex >= KEY_COUNT) throw new Exception("Crypto Key version is not yet supported.");
            byte[] keyA = StringToByteArray(keySet[header.KeyIndex]);

            HashAlgorithm hash = SHA256.Create();
            byte[] key = hash.ComputeHash(keyA.Concat(header.KeyB).ToArray());
            var aes = new AesManaged
            {
                Key = key,
                IV = header.IV,
                Mode = header.EncryptMode == 1 ? CipherMode.ECB : CipherMode.CBC,
                Padding = PaddingMode.Zeros
            };
            ICryptoTransform decrypter = aes.CreateDecryptor();
            var decrypted = new MemoryStream();
            var bw = new BinaryWriter(decrypted);
            if (headerBlockCount == 0 && footerBlockCount == 0)
            {
                int encryptedSize = 0x10 * (int)Math.Floor((decimal)decryptedSize / 0x10);
                DecryptBlock(ref bw, ref br, decrypter, header.EncryptFlags, encryptedSize);
            }
            else
            {
                int encryptedHeaderSize = 0x10 * Math.Min(headerBlockCount * 0x10, (int)Math.Floor((decimal)decryptedSize / 0x10));
                int encryptedFooterSize = 0x10 * Math.Min(footerBlockCount * 0x10, (int)Math.Floor((decimal)(decryptedSize - encryptedHeaderSize) / 0x10));
                if (encryptedHeaderSize > 0)
                {
                    DecryptBlock(ref bw, ref br, decrypter, header.EncryptFlags, encryptedHeaderSize);
                }
                if (encryptedFooterSize > 0)
                {
                    int encryptedFooterOffset = decryptedSize - encryptedFooterSize;
                    bw.Write(br.ReadBytes(encryptedFooterOffset - (int)br.BaseStream.Position + dataOffset));
                    int blockIndex = (int)Math.Floor((double)(encryptedFooterOffset % (0x10 * 0x10) / 0x10));
                    DecryptBlock(ref bw, ref br, decrypter, header.EncryptFlags, encryptedFooterSize, blockIndex);
                }
            }
            br.Close();
            bw.Close();
            File.WriteAllBytes(output, decrypted.ToArray());
        }

        // Old files
        /*public static void Decrypt(string input, string output)
        {
            var result = new MemoryStream();
            var stream = File.OpenRead(input);
            var br = new BinaryReader(stream);
            string sig = Encoding.ASCII.GetString(br.ReadBytes(4));
            if (sig != "NKAB") return;
            int version = br.ReadInt32();
            byte[] key, iv, encrypted, body;
            if (version == 1)
            {
                br.BaseStream.Position = 0xC;
                int keyLen = br.ReadInt16() + 0x64;
                int encryptedLen = br.ReadInt16() + 0x64;
                byte[] keyHash = br.ReadBytes(keyLen);
                iv = br.ReadBytes(0x10);
                encrypted = br.ReadBytes(encryptedLen);
                HashAlgorithm hash = SHA256.Create();
                key = hash.ComputeHash(keyHash);
                body = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
            }
            else if (version == 2)
            {
                br.BaseStream.Position = br.BaseStream.Length - 0x20;
                int num = br.ReadInt16();
                br.BaseStream.Position = 0xC;
                int keyLen = br.ReadInt16() + num;
                if (keyLen < 0)
                {
                    br.BaseStream.Position -= 2;
                    keyLen = br.ReadUInt16() + num;
                }
                int encryptedLen = br.ReadInt16() + num;
                if (encryptedLen < 0)
                {
                    br.BaseStream.Position -= 2;
                    encryptedLen = br.ReadUInt16() + num;
                }
                iv = br.ReadBytes(0x10);
                encrypted = br.ReadBytes(encryptedLen);
                body = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position - 0x20));
                key = br.ReadBytes(keyLen);
            }
            else return;
            var aes = new AesManaged
            {
                Key = key,
                IV = iv,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.Zeros
            };
            ICryptoTransform decrypter = aes.CreateDecryptor();
            byte[] decrypted = decrypter.TransformFinalBlock(encrypted, 0, encrypted.Length);
            using (var bw = new BinaryWriter(result))
            {
                bw.Write(decrypted);
                bw.Write(body);
            }
            br.Close();
 
            File.WriteAllBytes(Path.Combine(output, Path.GetFileName(input)), result.ToArray());
        }*/


    }
}
