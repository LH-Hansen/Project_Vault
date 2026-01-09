# Project_Vault

## Introduction

Project_Vault is a .NET-based file encryption library that enables secure, password-based encryption and decryption of files. It uses well-established cryptographic standards provided by the .NET runtime and is designed with a clear separation between application logic and orchestration. The project emphasizes correctness, testability, and maintainability, making it suitable both as a learning exercise in applied cryptography and as a foundation for a practical encryption utility.

**Disclaimer**: *This project is intended for educational and practical use and has not undergone a formal security audit.*

---

<details>
<summary><strong>Index</strong></summary>

- [Introduction](#introduction)
- [Project Structure](#project-structure)
- [Core Abstractions](#core-abstractions)
  - [IFileEncryptionService](#ifileencryptionservice)
- [Implementation: FileEncryptionService](#implementation-fileencryptionservice)
  - [Cryptographic Primitives Used](#cryptographic-primitives-used)
- [Encryption Process](#encryption-process)
- [Decryption Process](#decryption-process)
- [Security Properties](#security-properties)
  - [What the system guarantees](#what-the-system-guarantees)
  - [What the system does NOT guarantee](#what-the-system-does-not-guarantee)
- [Testing Strategy](#testing-strategy)
  - [Test Type](#test-type)
  - [Key Tests Implemented](#key-tests-implemented)
- [AAA Pattern example](#aaa-pattern-example)
- [Build & Test](#build--test)
- [Compatibility](#compatibility)

</details>


---

## Project Structure

```text
Project_Vault
├── IFileEncryptionService.cs
├── FileEncryptionService.cs
├── Program.cs (optional / minimal)
└── Project_Vault.csproj

Project_Vault_Test
├── FileEncryptionServiceTests.cs
└── Project_Vault_Test.csproj

```

---

## Core Abstractions

### `IFileEncryptionService`

Defines the public contract for file encryption:

```csharp
public interface IFileEncryptionService
{
    void EncryptFile(string inputPath, string outputPath, string password);
    void DecryptFile(string inputPath, string outputPath, string password);
}

```

---

Rationale:
* Enables testing against a contract.
* Allows future alternative implementations.
* Allows future alternative encryption algorithms to be added without modifying consumers.

---

## Implementation `FileEncryptionService`

### Cryptographic Primitives Used

| Purpose              | Algorithm                            |
|----------------------|--------------------------------------|
| Key derivation       | PBKDF2 (`Rfc2898DeriveBytes`)         |
| Hash function        | SHA-256                              |
| Symmetric encryption | AES-256                              |
| Mode                 | CBC (default .NET AES mode)           |
| Randomness           | `RandomNumberGenerator`              |

---

## Encryption Process

1. Generate a random **16-byte salt**
2. Derive a **256-bit encryption key** using PBKDF2 with:
   - **100,000 iterations**
   - **SHA-256**
3. Generate a random **AES initialization vector (IV)**
4. Write data to the output file in the following order:
```text
[ salt (16 bytes) ][ IV (16 bytes) ][ ciphertext (N bytes) ]
```
5. Encrypt the input file stream using `CryptoStream`

---

## Decryption Process

1. Read the first **16 bytes** → salt  
2. Read the next **16 bytes** → IV  
3. Re-derive the encryption key using:
   - Provided password
   - Stored salt
4. Decrypt the remaining bytes using AES
5. Write the plaintext to the output file

If the password is incorrect or the file is corrupted, decryption will fail with a `CryptographicException`.

---

## Security Properties

### What the system guarantees

* Confidentiality of file contents
* Unique ciphertext per encryption (random salt + IV)
* Resistance to brute-force attacks via PBKDF2


### What the system does NOT guarantee

* Integrity authentication (no MAC / AEAD)
* Protection against weak passwords
* Forward secrecy

---

## Testing Strategy

### Test Type

Tests are **integration-style unit tests**, which is appropriate for cryptographic code.

Rationale:
* Crypto primitives should not be mocked
* Behavior is verified via observable properties
* Tests validate correctness without asserting internals

---

## Key Tests Implemented
* Encrypt & Decrypt
* Wrong password causes failure
* Empty file handling
* Ciphertext differs from plaintext
* Non-deterministic encryption output

--- 

## AAA Pattern example

```csharp
// Arrange
var encryptor = new FileEncryptionService();

// Act
encryptor.EncryptFile(...);
encryptor.DecryptFile(...);

// Assert
Assert.Equal(expected, actual);
```

---

## Build & Test

### Build

```bash
dotnet build
```

### Test

```bash
dotnet test
```

---

## Compatibility
* .NET 8.0
* Windows/Linux/macOS
* No platform-specific dependencies
