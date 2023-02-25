using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Nikke_NKAB_Decrypter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            foreach (string arg in args)
            {
                Decrypt(arg);
            }
        }
        static void Decrypt(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                string[] files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    Decrypt(file);
                }
            }
            else
            {
                var result = new MemoryStream();
                using (var stream = File.OpenRead(path))
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
                File.WriteAllBytes(path, result.ToArray());
            }
        }
    }
}
