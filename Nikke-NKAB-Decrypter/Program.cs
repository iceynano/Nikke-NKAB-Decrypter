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
                        br.BaseStream.Position = 0xC;
                        int keyLen = br.ReadInt16() + 0x64;
                        int encryptedLen = br.ReadInt16() + 0x64;
                        byte[] keyHash = br.ReadBytes(keyLen);
                        byte[] iv = br.ReadBytes(keyLen);
                        byte[] encrypted = br.ReadBytes(encryptedLen);
                        HashAlgorithm hash = SHA256.Create();
                        byte[] key = hash.ComputeHash(keyHash);
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
                            bw.Write(br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position)));
                        }
                    }
                }
                File.WriteAllBytes(path, result.ToArray());
            }
        }
    }
}
