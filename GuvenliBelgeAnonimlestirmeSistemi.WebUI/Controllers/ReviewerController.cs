using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.LogDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebUI.Models;
using GuvenliBelgeAnonimlestirmeSistemi.WebUI.Models.Reviewer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebUI.Controllers
{
    public class ReviewerController : Controller
    {
        private readonly HttpClient _httpClient;
        public ReviewerController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MakaleApi");
        }
        public IActionResult Index()
        {
            var apiUrl = "https://localhost:7057/api/Reviewers";
            List<ReviewerDto> hakemler = new List<ReviewerDto>();
            HttpResponseMessage response = _httpClient.GetAsync(apiUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                hakemler = JsonConvert.DeserializeObject<List<ReviewerDto>>(json);
            }
            else
            {
                ViewBag.Error = "Hakem verileri yüklenirken bir hata oluştu.";
            }
            return View(hakemler);
        }
        [HttpGet]
        public IActionResult AssignedArticle(int id)
        {
            if(id<=0)
            {
                return BadRequest("Geçersiz hakem ID");
            }
            string apiUrl = $"https://localhost:7057/api/Articles/GetArticleByReviewerId?id={id}";
            List<ArticleViewModel> makaleler = new List<ArticleViewModel>();
            HttpResponseMessage response = _httpClient.GetAsync(apiUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                makaleler = JsonConvert.DeserializeObject<List<ArticleViewModel>>(json);
            }
            else
            {
                ViewBag.Error = "Makale verileri yüklenirken bir hata oluştu.";
            }

            return View(makaleler);
        }
        [HttpGet]
        public IActionResult EvaluateArticle(int id)
        {
            string apiUrl = $"https://localhost:7057/api/Articles/GetArticleById?id={id}";
            ArticleViewModel makale = new ArticleViewModel();
            HttpResponseMessage response = _httpClient.GetAsync(apiUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                makale = JsonConvert.DeserializeObject<ArticleViewModel>(json);
            }

            if (makale == null)
            {
                return NotFound("Makale bulunamadı!");
            }

            return View(makale);
        }
        [HttpPost]
        public async Task<IActionResult> SubmitReview(int MakaleId, string ReviewText)
        {
            if (string.IsNullOrWhiteSpace(ReviewText))
            {
                TempData["Error"] = "Değerlendirme boş olamaz!";
                return RedirectToAction("EvaluateArticle", new { id = MakaleId });
            }

            string apiUrl = $"https://localhost:7057/api/Articles/GetArticleById?id={MakaleId}";
            ArticleViewModel makale = new ArticleViewModel();
            HttpResponseMessage response = _httpClient.GetAsync(apiUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                makale = JsonConvert.DeserializeObject<ArticleViewModel>(json);
            }
            else
            {
                TempData["Error"] = "Makale bilgileri alınamadı.";
                return RedirectToAction("EvaluateArticle", new { id = MakaleId });
            }

            makale.HakemDegerlendirmesi = ReviewText;
            makale.MakaleDurumu = ArticleStatus.HakemDegerlendirdi;
            makale.EnSonYapilanIsleminTarihi = DateTime.UtcNow;

            var updateUrl = $"https://localhost:7057/api/Articles";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(makale), Encoding.UTF8, "application/json");

            var updateResponse = await _httpClient.PutAsync(updateUrl, jsonContent);

            if (updateResponse.IsSuccessStatusCode)
            {
                TempData["Success"] = "Değerlendirme başarıyla kaydedildi.";
            }
            else
            {
                TempData["Error"] = "Değerlendirme güncellenemedi!";
            }
            var logDto = new CreateLogDto
            {
                MakaleId = makale.MakaleId,
                islemZamani = DateTime.UtcNow,
                LogDetayi = $"Hakem {DateTime.UtcNow:dd.MM.yyyy HH:mm:ss} tarihinde cevap verdi "

            };

            var logContent = new StringContent(JsonConvert.SerializeObject(logDto), Encoding.UTF8, "application/json");
            var createLogUrl = "https://localhost:7057/api/Logs";
            HttpResponseMessage logResponse = await _httpClient.PostAsync(createLogUrl, logContent);

            if (!logResponse.IsSuccessStatusCode)
            {
                ViewBag.LogHata = "Log kaydedilirken bir sorun oluştu.";
            }
            return RedirectToAction("EvaluateArticle", new { id = MakaleId });
        }

    }
}
