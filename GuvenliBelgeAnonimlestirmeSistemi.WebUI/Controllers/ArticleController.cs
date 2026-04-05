using GuvenliBelgeAnonimlestirmeSistemi.WebUI.Models.Article;
using GuvenliBelgeAnonimlestirmeSistemi.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.LogDtos;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebUI.Controllers
{
    public class ArticleController : Controller
    {
        private readonly HttpClient _client;

        public ArticleController(HttpClient client)
        {
            _client=client;
        }
        #region Article Yukleme
        [HttpGet]
        public IActionResult ArticleUpload()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ArticleUpload(IFormFile makale, string email)
        {
            if (makale == null || makale.Length == 0)
            {
                ViewBag.Email = email;
                ViewBag.PdfError = "Lütfen geçerli bir PDF yükleyiniz.";
                return View();
            }

            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                ViewBag.Email = email;
                ViewBag.EmailError = "Lütfen geçerli bir e-posta adresi giriniz.";
                return View();
            }

            var trackingService = new CreateTrackingNumber(new HttpClient());
            string trackingId = await trackingService.GenerateUniqueTrackingNo();

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/makalePdfleri");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string fileName = $"{trackingId}.pdf";
            string filePath = Path.Combine(uploadsFolder, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await makale.CopyToAsync(stream);
            }
            string orjinalPdfPath = "/makalePdfleri/" + fileName;

            var newMakale = new CreateArticleDto
            {
                YazarEPosta = email,
                PdfFilePath = orjinalPdfPath,
                TakipNo = trackingId,
                MakaleYuklemeTarihi = DateTime.UtcNow,
                MakaleDurumu = Models.ArticleStatus.MakaleBeklemede
            };

            var createArticleUrl = "https://localhost:7057/api/Articles";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(newMakale), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync(createArticleUrl, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Hata = "Makale kaydedilirken bir sorun oluştu.";
                return View();
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdArticle = JsonConvert.DeserializeObject<ArticleDto>(responseContent);

            if (createdArticle == null || createdArticle.MakaleId == 0)
            {
                ViewBag.Hata = "Makale oluşturuldu ancak ID alınamadı.";
                return View();
            }

            var logDto = new CreateLogDto
            {
                MakaleId = createdArticle.MakaleId,
                islemZamani = DateTime.UtcNow,
                LogDetayi = $"Makale {DateTime.UtcNow: dd.MM.yyyy HH:mm:ss} tarihinde yazar tarafından sisteme yüklendi. " +
                             $"Editöre {DateTime.UtcNow.AddMinutes(2): dd.MM.yyyy HH:mm:ss} tarihinde geldi. "
            };

            var logContent = new StringContent(JsonConvert.SerializeObject(logDto), Encoding.UTF8, "application/json");
            var createLogUrl = "https://localhost:7057/api/Logs";
            HttpResponseMessage logResponse = await _client.PostAsync(createLogUrl, logContent);

            if (!logResponse.IsSuccessStatusCode)
            {
                ViewBag.LogHata = "Log kaydedilirken bir sorun oluştu.";
            }
            ViewBag.TrackingId = trackingId;
           
            // await SendTrackingNumberByEmail(email, trackingId);

            return View("ArticleUploadSuccess");
        }
        public IActionResult ArticleUploadSuccess()
        {
            return View();
        }
        #endregion

        #region Article Sorgulama
        [HttpGet]
        public IActionResult ArticleInquiry()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ArticleInquiry(string makaleTakipNo, string email)
        {
            if (string.IsNullOrEmpty(makaleTakipNo) || string.IsNullOrEmpty(email))
            {
                ViewBag.Hata = "Lütfen geçerli bir Makale Takip Numarası ve E-posta adresi girin.";
                return View();
            }
            //makaleTakipNo= Sha256.Hash(makaleTakipNo);

            string encodedMakaleNo = HttpUtility.UrlEncode(makaleTakipNo);
            string encodedEmail = HttpUtility.UrlEncode(email);

            var validateRequest = $"https://localhost:7057/api/Articles/GetArticleByTrackingNumberEmail?makaleTakipNo={encodedMakaleNo}&email={encodedEmail}";


            HttpResponseMessage validateResponse = _client.GetAsync(validateRequest).Result;
            if (!validateResponse.IsSuccessStatusCode)
            {
                ViewBag.Hata = "Girilen bilgiler hatalı veya kayıtlı makale bulunamadı.";
                return View();
            }
            var makaleUrl =$"https://localhost:7057/api/Articles/GetArticleByTrackingNumber?takipno={makaleTakipNo}";
            ArticleDto makale = null;

            HttpResponseMessage response = _client.GetAsync(makaleUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                makale = JsonConvert.DeserializeObject<ArticleDto>(json);
            }

            if (makale != null)
            {
                Console.WriteLine("Makale başarıyla çekildi: " + JsonConvert.SerializeObject(makale));
            }
            else
            {
                Console.WriteLine("Makale modeli NULL döndü.");
            }


            return View("ArticleStatusPage", makale);
        }
        #endregion
        
        #region Article Detay Sayfası
        public IActionResult ArticleStatusPage(ArticleDto makale)
        {
            if (makale == null)
            {
                TempData["Error"] = "Lütfen önce makale sorgulaması yapın.";
                return RedirectToAction("ArticleInquiry");
            }

            return View(makale);
        }
        #endregion

        #region Mail Dogrulama
        private bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                string pattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }
        #endregion
        
        #region Article Guncelleme
        [HttpPost]
        public async Task<IActionResult> UpdateArticle(int makaleNo, IFormFile revizeDosya)
        {
            var getMakaleUrl = $"https://localhost:7057/api/Articles/GetArticleById?id={makaleNo}";
            var response = await _client.GetAsync(getMakaleUrl);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound("Makale bulunamadı.");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var makale = JsonConvert.DeserializeObject<ArticleDto>(jsonString);

            if (makale == null)
            {
                return NotFound("Makale verisi alınamadı.");
            }
            if (revizeDosya == null || revizeDosya.Length == 0)
            {
                TempData["Error"] = "Revize dosyası yüklenemedi.";
                return RedirectToAction("ArticleStatusPage", new { id = makaleNo });
            }
            var fileName = $"{makale.TakipNo}.pdf"; 
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/makalePdfleri");
            Directory.CreateDirectory(uploadsPath); 
            var filePath = Path.Combine(uploadsPath, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await revizeDosya.CopyToAsync(stream);
            }

            makale.MakaleDurumu= Models.ArticleStatus.RevizeEdildi;
            makale.EnSonYapilanIsleminTarihi = DateTime.UtcNow;

            var updateUrl = $"https://localhost:7057/api/Articles";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(makale), Encoding.UTF8, "application/json");

            var updateResponse = await _client.PutAsync(updateUrl, jsonContent);

            response = await _client.GetAsync(getMakaleUrl);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound("Makale bulunamadı.");
            }

            jsonString = await response.Content.ReadAsStringAsync();
            makale = JsonConvert.DeserializeObject<ArticleDto>(jsonString);

            if (makale == null)
            {
                return NotFound("Makale verisi alınamadı.");
            }
            if (!updateResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Revize dosyası yüklenemedi.";
                return RedirectToAction("ArticleStatusPage", makale);
            }

            TempData["Success"] = "Revize dosyası başarıyla yüklendi.";
            return RedirectToAction("ArticleStatusPage", makale);
        }
        #endregion

    }
}
