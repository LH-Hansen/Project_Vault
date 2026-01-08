using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Project_Vault
{
    public class FileEncryptionService : IFileEncryptionService
    {
        public void EncryptFile(string inputPath, string outputPath, string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            using var key = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            using var aes = Aes.Create();
            aes.Key = key.GetBytes(32);
            aes.GenerateIV();

            using var outFile = new FileStream(outputPath, FileMode.Create);
            outFile.Write(salt);
            outFile.Write(aes.IV);

            using var cryptoStream = new CryptoStream(outFile, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using var inFile = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
            inFile.CopyTo(cryptoStream);
        }

        public void DecryptFile(string inputPath, string outputPath, string password)
        {
            using var inFile = new FileStream(inputPath, FileMode.Open);

            byte[] salt = new byte[16];
            byte[] iv = new byte[16];
            inFile.Read(salt);
            inFile.Read(iv);

            using var key = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            using var aes = Aes.Create();
            aes.Key = key.GetBytes(32);
            aes.IV = iv;

            using var cryptoStream = new CryptoStream(inFile, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var outFile = new FileStream(outputPath, FileMode.Create);
            cryptoStream.CopyTo(outFile);
        }

    }
}
