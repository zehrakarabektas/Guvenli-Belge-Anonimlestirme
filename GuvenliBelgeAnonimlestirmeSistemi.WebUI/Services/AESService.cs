using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebUI.Services
{
    public class AESService
    {
        //Takip numarası SHA-256 ile hashlendi.Elde edilen hash’in ilk 32 karakteri AES-256 anahtarı oldu.AES-256, 32 byte’lık bir anahtar gerektirir, bu yüzden SHA-256 hash’i tam olarak uygun
        public static byte[] CreateAESKeyFromTrackingId(string trackingId)
        {
            string hash = Sha256.Hash(trackingId);
            return Encoding.UTF8.GetBytes(hash.Substring(0, 32)); 
        }
        public static byte[] CreateAESIVFromTrackingId(string trackingId)
        {
            string hash = Sha256.Hash(trackingId);
            return Encoding.UTF8.GetBytes(hash.Substring(0, 16)); 
        }
        public static void EncryptPdf(IFormFile inputFile, string outputFile, string trackingId)
        {
            byte[] key = CreateAESKeyFromTrackingId(trackingId);
            byte[] iv = CreateAESIVFromTrackingId(trackingId);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor())
                using (var fileStream = new FileStream(outputFile, FileMode.Create))
                using (var cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write))
                {
                    inputFile.OpenReadStream().CopyTo(cryptoStream);
                }
            }
        }
        public static void DecryptPdf(string encryptedFilePath, string decryptedFilePath, string trackingId)
        {
            byte[] key = CreateAESKeyFromTrackingId(trackingId);
            byte[] iv = CreateAESIVFromTrackingId(trackingId);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                using (var inputFile = new FileStream(encryptedFilePath, FileMode.Open))
                using (var outputFile = new FileStream(decryptedFilePath, FileMode.Create))
                using (var cryptoStream = new CryptoStream(inputFile, decryptor, CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(outputFile);
                }
            }
        }
    }
}
