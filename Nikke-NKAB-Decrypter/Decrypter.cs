using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Nikke_NKAB_Decrypter
{
    public static class Decrypter
    {
        public static void Decrypt(string input, string output)
        {
            var result = new MemoryStream();
            using (var stream = File.OpenRead(input))
            {
                
                using (var br = new BinaryReader(stream))
                {
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
                }
            }
            File.WriteAllBytes(Path.Combine(output, Path.GetFileName(input)), result.ToArray());
        }
    }
}
