using GuvenliBelgeAnonimlestirmeSistemi.WebUI.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebUI.Services
{
    public class CreateTrackingNumber
    {
        private static readonly char[] chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private readonly HttpClient _client;

        public CreateTrackingNumber(HttpClient client)
        {
            _client=client;
        }
        public async Task<string> GenerateUniqueTrackingNo()
        {
            string trackingNumber;
            string hashedTrackingNumber;
            bool response;

            do
            {
                trackingNumber = CreateArticleTrackingNo(); 
                hashedTrackingNumber = Sha256.Hash(trackingNumber); 
                response = await CheckTrackingNo(hashedTrackingNumber); 
            }
            while (response);

            return trackingNumber; 
        }

        private static string CreateArticleTrackingNo()
        {
            byte[] randomBytes = new byte[12];
            RandomNumberGenerator.Fill(randomBytes);

            StringBuilder trackingNumber = new StringBuilder();
            for (int i = 0; i < randomBytes.Length; i++)
            {
                trackingNumber.Append(chars[randomBytes[i] % chars.Length]);

                if (i == 3 || i == 7)
                {
                    trackingNumber.Append("-");
                }
            }

            return trackingNumber.ToString();
        }

        private async Task<bool> CheckTrackingNo(string hashedTrackingNo)
        {
            string apiUrl = $"https://localhost:7057/api/Articles/CheckTrackingNumber?trackingNo={hashedTrackingNo}";

            try
            {
                HttpResponseMessage response = await _client.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return false; 
                }

                string result = await response.Content.ReadAsStringAsync();
                return bool.Parse(result); 
            }
            catch (Exception ex)
            {
                Console.WriteLine("API isteği sırasında hata oluştu: " + ex.Message);
                return false;
            }
        }
    }
}
