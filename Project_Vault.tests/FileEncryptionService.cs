using System.Security.Cryptography;
using System.Text;
using Project_Vault;
using Xunit;

namespace Project_Vault_Test
{
    public class FileEncryptionServiceTests
    {
        private readonly IFileEncryptionService _encryptor = new FileEncryptionService();

        [Fact]
        public void Encrypt_Then_Decrypt_ReturnsOriginalContent()
        {

            // Arrange
            var input = Path.GetTempFileName();
            var encrypted = Path.GetTempFileName();
            var output = Path.GetTempFileName();

            File.WriteAllText(input, "secret data");

            // Act
            _encryptor.EncryptFile(input, encrypted, "P@ssw0rd");
            _encryptor.DecryptFile(encrypted, output, "P@ssw0rd");

            var result = File.ReadAllText(output);

            // Assert
            Assert.Equal("secret data", result);
        }

        [Fact]
        public void EncryptedFile_DoesNotContainPlaintext()
        {
            // Arrange
            var encryptor = new FileEncryptionService();

            var input = Path.GetTempFileName();
            var encrypted = Path.GetTempFileName();

            const string plaintext = "Very secret data";
            File.WriteAllText(input, plaintext);

            // Act
            encryptor.EncryptFile(input, encrypted, "P@ssw0rd");

            var encryptedBytes = File.ReadAllBytes(encrypted);
            var encryptedText = Encoding.UTF8.GetString(encryptedBytes);

            // Assert
            Assert.DoesNotContain(plaintext, encryptedText);
        }


        [Fact]
        public void Decrypt_WithWrongPassword_Throws()
        {
            // Arrange
            IFileEncryptionService encryptor = new FileEncryptionService();

            var input = Path.GetTempFileName();
            var encrypted = Path.GetTempFileName();
            var output = Path.GetTempFileName();

            File.WriteAllText(input, "data");

            encryptor.EncryptFile(input, encrypted, "P@ssw0rd");

            // Act / Assert
            Assert.ThrowsAny<CryptographicException>(() =>
                encryptor.DecryptFile(encrypted, output, "password"));
        }

        [Fact]
        public void Encrypt_EmptyFile_Then_Decrypt_ReturnsEmpty()
        {
            // Arrange
            IFileEncryptionService encryptor = new FileEncryptionService();

            var input = Path.GetTempFileName();
            var encrypted = Path.GetTempFileName();
            var output = Path.GetTempFileName();

            File.WriteAllText(input, string.Empty);

            // Act
            encryptor.EncryptFile(input, encrypted, "P@ssw0rd");
            encryptor.DecryptFile(encrypted, output, "P@ssw0rd");

            // Assert
            Assert.Equal(string.Empty, File.ReadAllText(output));
        }

        [Fact]
        public void EncryptedFile_IsNotPlaintext()
        {
            // Arrange
            IFileEncryptionService encryptor = new FileEncryptionService();

            var input = Path.GetTempFileName();
            var encrypted = Path.GetTempFileName();

            File.WriteAllText(input, "plaintext");

            // Act
            encryptor.EncryptFile(input, encrypted, "P@ssw0rd");

            // Assert
            Assert.NotEqual(
                File.ReadAllText(input),
                File.ReadAllText(encrypted));
        }


    }
}
