using System.Security.Cryptography;
using System.Text;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebUI.Services
{
    public class Sha256
    {
        public static string Hash(string takipNo)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(takipNo));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in data)
                {
                    sb.Append(b.ToString("x2")); 
                }
                return sb.ToString();
            }
        }
    }
}
