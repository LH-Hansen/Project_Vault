using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Vault
{
    public interface IFileEncryptionService
    {
        void EncryptFile(string inputPath, string outputPath, string password);
        void DecryptFile(string inputPath, string outputPath, string password);
    }
}
