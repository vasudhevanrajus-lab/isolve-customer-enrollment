using System.Security.Cryptography;
using System.Text;

namespace CustomerEnrollment.Api.Security;

public interface IClientPayloadCryptor
{
    string Decrypt(string base64CipherText);
}

public class AesClientPayloadCryptor : IClientPayloadCryptor
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public AesClientPayloadCryptor(IConfiguration config)
    {
        var keyBase64 = config["Crypto:ClientAesKeyBase64"]
            ?? throw new InvalidOperationException("Crypto:ClientAesKeyBase64 is not configured.");
        var ivBase64 = config["Crypto:ClientAesIvBase64"]
            ?? throw new InvalidOperationException("Crypto:ClientAesIvBase64 is not configured.");

        _key = Convert.FromBase64String(keyBase64);
        _iv = Convert.FromBase64String(ivBase64);

        if (_key.Length != 32)
            throw new InvalidOperationException("AES key must be 32 bytes for AES-256.");

        if (_iv.Length != 16)
            throw new InvalidOperationException("AES IV must be 16 bytes.");
    }

    public string Decrypt(string base64CipherText)
    {
        if (string.IsNullOrWhiteSpace(base64CipherText))
            throw new ArgumentException("Cipher text is empty.", nameof(base64CipherText));

        var cipherBytes = Convert.FromBase64String(base64CipherText);

        using var aes = Aes.Create();
        aes.Key     = _key;
        aes.IV      = _iv;
        aes.Mode    = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        var plain = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(plain);
    }
}
