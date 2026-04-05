using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ArticleFieldsDto;
using GuvenliBelgeAnonimlestirmeSistemi.WebUI.Models;
using GuvenliBelgeAnonimlestirmeSistemi.WebUI.Models.Article;
using GuvenliBelgeAnonimlestirmeSistemi.WebUI.Models.Reviewer;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using Newtonsoft.Json;
using System.Text;
using iText.Kernel.Font;
using iText.Layout;
using iText.Kernel.Geom;
using iText.Kernel.Utils;
using iText.IO.Font;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.LogDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebUI.Models.FieldTopic;
using System.Net.Http;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.FieldTopicDtos;
using GetArticleTopicDto = GuvenliBelgeAnonimlestirmeSistemi.WebUI.Models.FieldTopic.GetArticleTopicDto;
using System.Text.Json;
using Azure;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebUI.Controllers
{
    public class EditorController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _env;

        public EditorController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
        {
            _httpClient = httpClientFactory.CreateClient("MakaleApi");
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region eskiarticlelist
        public IActionResult ArticleList()
        {
            string articleApiUrl = "https://localhost:7057/api/Articles";
            string articleFieldUrl = "https://localhost:7057/api/ArticleFields/GetFieldsByArticleId?articleId={0}";
            string reviewerApiUrl = "https://localhost:7057/api/ReviewerFieldTopics/GetReviewersByFieldId?fieldId={0}";

            List<ArticleViewModel> makaleler = new List<ArticleViewModel>();

            HttpResponseMessage articleResponse = _httpClient.GetAsync(articleApiUrl).Result;
            if (articleResponse.IsSuccessStatusCode)
            {
                string json = articleResponse.Content.ReadAsStringAsync().Result;
                makaleler = JsonConvert.DeserializeObject<List<ArticleViewModel>>(json);
            }
            else
            {
                ViewBag.Error = "Makale verileri yüklenirken hata oluştu.";
                return View(makaleler);
            }

            foreach (var makale in makaleler)
            {
                string articleFieldApiUrl = string.Format(articleFieldUrl, makale.MakaleId);
                HttpResponseMessage fieldResponse = _httpClient.GetAsync(articleFieldApiUrl).Result;

                List<ArticleFieldsDto> makaleAlanlari = new List<ArticleFieldsDto>();

                if (fieldResponse.IsSuccessStatusCode)
                {
                    string fieldJson = fieldResponse.Content.ReadAsStringAsync().Result;
                    makaleAlanlari = JsonConvert.DeserializeObject<List<ArticleFieldsDto>>(fieldJson);
                }
                else
                {
                    ViewBag.Warning = $"Makale {makale.MakaleId} için alanlar yüklenirken hata oluştu.";
                    continue;
                }

                List<int> fieldIds = makaleAlanlari.Select(f => f.FieldTopicId).Distinct().ToList();
                //makale.IlgiAlanlari = makaleAlanlari.Select(f => f.FieldName).ToList();
                makale.IlgiAlanlari = fieldIds.Select(id => id.ToString()).ToList();
                makale.OnerilenHakemler = new List<ReviewerDto>();

                foreach (var fieldId in fieldIds)
                {
                    string requestUrl = string.Format(reviewerApiUrl, fieldId);


                    HttpResponseMessage reviewerResponse = _httpClient.GetAsync(requestUrl).Result;

                    if (reviewerResponse.IsSuccessStatusCode)
                    {
                        string reviewerJson = reviewerResponse.Content.ReadAsStringAsync().Result;
                   
                        var hakemler = JsonConvert.DeserializeObject<List<ReviewerDto>>(reviewerJson);

                        makale.OnerilenHakemler.AddRange(
                            hakemler.Where(h => !makale.OnerilenHakemler.Any(x => x.ReviewerId == h.ReviewerId))
                        );
                    }
                    else
                    {
                        ViewBag.Warning = $"Makale {makale.MakaleId} için fieldId {fieldId} ile hakemler yüklenirken hata oluştu.";
                       
                    }
                }
            }

            return View(makaleler);
        }
        #endregion

        #region  Makale Listeleme
        public IActionResult ArticleListt()
        {
            string articleApiUrl = "https://localhost:7057/api/Articles";
            string articleFieldUrl = "https://localhost:7057/api/ArticleFields/GetFieldsByArticleId?articleId={0}";
            string reviewerApiUrl = "https://localhost:7057/api/ReviewerFieldTopics/GetReviewersByFieldId?fieldId={0}";

            List<ArticleViewModel> makaleler = new List<ArticleViewModel>();

            HttpResponseMessage articleResponse = _httpClient.GetAsync(articleApiUrl).Result;
            if (articleResponse.IsSuccessStatusCode)
            {
                string json = articleResponse.Content.ReadAsStringAsync().Result;
                makaleler = JsonConvert.DeserializeObject<List<ArticleViewModel>>(json);
            }
            else
            {
                ViewBag.Error = "Makale verileri yüklenirken hata oluştu.";
                return View(makaleler);
            }

            foreach (var makale in makaleler)
            {
                string articleFieldApiUrl = string.Format(articleFieldUrl, makale.MakaleId);
                HttpResponseMessage fieldResponse = _httpClient.GetAsync(articleFieldApiUrl).Result;

                List<ArticleFieldsDto> makaleAlanlari = new List<ArticleFieldsDto>();

                if (fieldResponse.IsSuccessStatusCode)
                {
                    string fieldJson = fieldResponse.Content.ReadAsStringAsync().Result;
                    makaleAlanlari = JsonConvert.DeserializeObject<List<ArticleFieldsDto>>(fieldJson);
                }
                else
                {
                    ViewBag.Warning = $"Makale {makale.MakaleId} için alanlar yüklenirken hata oluştu.";
                    continue;
                }
            }

            return View(makaleler);
        }
        #endregion
       
        #region eskisayfa anonimlestirme için
        [HttpGet]
        public IActionResult AnonymizedArticle(int id)
        {
            string apiUrl = $"https://localhost:7057/api/Articles/GetArticleById?id={id}"; 
            ArticleViewModel makale = null;

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
        #endregion

        #region Anonimleştirme İşlemi
        [HttpPost]
        public async Task<IActionResult> Anonimlestir(int MakaleId, bool? AnonimYazarAdi, bool? AnonimYazarIletisim, bool? AnonimYazarKurum)
        {
            Console.WriteLine($"Ad: {AnonimYazarAdi}, İletişim: {AnonimYazarIletisim}, Kurum: {AnonimYazarKurum}");
            string apiUrl = $"https://localhost:7057/api/Articles/GetArticleById?id={MakaleId}";
            ArticleViewModel makale = null;

            HttpResponseMessage articleresponse = _httpClient.GetAsync(apiUrl).Result;

            if (articleresponse.IsSuccessStatusCode)
            {
                string articlejson = articleresponse.Content.ReadAsStringAsync().Result;
                makale = JsonConvert.DeserializeObject<ArticleViewModel>(articlejson);
            }

            if (makale == null)
            {
                return NotFound("Makale bulunamadı!");
            }
            var pdfFullPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", makale.PdfFilePath.TrimStart('/'));
            if (!System.IO.File.Exists(pdfFullPath))
                return NotFound("Makale bulunamadı!");

            using var httpClient = new HttpClient();
            using var form = new MultipartFormDataContent();

            form.Add(new StringContent((AnonimYazarAdi ?? false).ToString().ToLower()), "anon_ad");
            form.Add(new StringContent((AnonimYazarIletisim ?? false).ToString().ToLower()), "anon_email");
            form.Add(new StringContent((AnonimYazarKurum ?? false).ToString().ToLower()), "anon_kurum");

            Console.WriteLine($"[Form Seçimleri] Ad: {AnonimYazarAdi}, Email: {AnonimYazarIletisim}, Kurum: {AnonimYazarKurum}");

            var fileStream = System.IO.File.OpenRead(pdfFullPath);
            form.Add(new StreamContent(fileStream), "file", System.IO.Path.GetFileName(pdfFullPath));

            var response = await httpClient.PostAsync("http://127.0.0.1:8001/anonymize_pdf/", form);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Anonimleştirme servisi hata verdi");

            var responseContent = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(responseContent).RootElement;

            var base64Pdf = json.GetProperty("encrypted_data").GetProperty("pdf_bytes").GetString();
            var pdfBytes = Convert.FromBase64String(base64Pdf);

            var anonFileName = $"{makale.TakipNo}.pdf";
            var anonPdfPath = System.IO.Path.Combine("wwwroot", "anonimPdfleri", anonFileName);
            await System.IO.File.WriteAllBytesAsync(anonPdfPath, pdfBytes);

            var encryptedInfo = json.GetProperty("encrypted_data").GetProperty("encrypted_info").ToString();

            var encryptedImagesJson = json.GetProperty("encrypted_data").GetProperty("encrypted_images").ToString();

            var aesKeyBase64 = json.GetProperty("encrypted_data").GetProperty("aes_key").GetString();
            var aesKey = Convert.FromBase64String(aesKeyBase64);

            var encryptedImages = JsonConvert.DeserializeObject<List<dynamic>>(encryptedImagesJson);

            var combinedEncryptedData = new
            {
                encrypted_info = JsonConvert.DeserializeObject(encryptedInfo),
                encrypted_images = encryptedImages
            };

            var combinedEncryptedJson = JsonConvert.SerializeObject(combinedEncryptedData);

            makale.AnonimPdfFilePath = "/anonimPdfleri/" + anonFileName;
            makale.EncryptedInfoJson = combinedEncryptedJson;
            makale.EnSonYapilanIsleminTarihi = DateTime.UtcNow;

            var updateResponse = await _httpClient.PutAsJsonAsync("https://localhost:7057/api/Articles", makale);
            if (!updateResponse.IsSuccessStatusCode)
            {
                return StatusCode(500, "Makale güncellenemedi.");
            }

            return RedirectToAction("ArticleIslem", new { id = MakaleId });

        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> AnonimKaldir(int makaleId)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7057/api/Articles/GetArticleById?id={makaleId}");
            if (!response.IsSuccessStatusCode)
                return NotFound("Makale bulunamadı.");

            var jsonString = await response.Content.ReadAsStringAsync();
            var makale = JsonConvert.DeserializeObject<ArticleViewModel>(jsonString);

            if (makale == null || string.IsNullOrEmpty(makale.AnonimPdfFilePath) || string.IsNullOrEmpty(makale.EncryptedInfoJson))
                return BadRequest("Gerekli bilgiler eksik.");

            var pdfFullPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", makale.AnonimPdfFilePath.TrimStart('/'));
            if (!System.IO.File.Exists(pdfFullPath))
                return NotFound("PDF dosyası bulunamadı.");

            using var client = new HttpClient();
            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(makale.EncryptedInfoJson, Encoding.UTF8, "application/json"), "encrypted_info");

            var fileStream = System.IO.File.OpenRead(pdfFullPath);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            form.Add(fileContent, "file", System.IO.Path.GetFileName(pdfFullPath));

            var apiResponse = await client.PostAsync("http://127.0.0.1:8001/deanonymize_pdf/", form);
            if (!apiResponse.IsSuccessStatusCode)
                return StatusCode((int)apiResponse.StatusCode, "De-anonimleştirme başarısız.");

            var resultContent = await apiResponse.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(resultContent).RootElement;
            var base64Pdf = json.GetProperty("pdf_bytes").GetString();
            var originalBytes = Convert.FromBase64String(base64Pdf);

            var deanonymizedFileName = $"{makale.TakipNo}_sonuc.pdf";
            var deanonymizedFilePath = System.IO.Path.Combine("wwwroot", "sonucPdfleri", deanonymizedFileName);
            await System.IO.File.WriteAllBytesAsync(deanonymizedFilePath, originalBytes);

            makale.SonucPdfFilePath = "/sonucPdfleri/" + deanonymizedFileName;
            makale.MakaleDurumu=ArticleStatus.EditorSonuclandirmada;
            makale.EnSonYapilanIsleminTarihi = DateTime.UtcNow;

            var updateResponse = await _httpClient.PutAsJsonAsync("https://localhost:7057/api/Articles", makale);
            if (!updateResponse.IsSuccessStatusCode)
            {
                return StatusCode(500, "Makale güncellenemedi.");
            }

            TempData["Success"] = "PDF başarıyla de-anonimleştirildi!";
            return RedirectToAction("ArticleIslem", new { id = makale.MakaleId });
        }



        #region Hakem Atama
        [HttpPost]
        public async Task<IActionResult> AssignReviewer(int ArticleId, int ReviewerId)
        {

            var getMakaleUrl = $"https://localhost:7057/api/Articles/GetArticleById?id={ArticleId}";
            var response = await _httpClient.GetAsync(getMakaleUrl);

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

            makale.ReviewerId = ReviewerId;
            makale.EnSonYapilanIsleminTarihi = DateTime.UtcNow;
            makale.MakaleDurumu = Models.ArticleStatus.HakemeAtandi;

            var updateUrl = $"https://localhost:7057/api/Articles";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(makale), Encoding.UTF8, "application/json");

            var updateResponse = await _httpClient.PutAsync(updateUrl, jsonContent);

            if (!updateResponse.IsSuccessStatusCode)
            {
                return BadRequest("Makale güncellenemedi.");
            }
            var logDto = new CreateLogDto
            {
                MakaleId = makale.MakaleId,
                islemZamani = DateTime.UtcNow,
                LogDetayi = $"Makale {DateTime.UtcNow:dd.MM.yyyy HH:mm:ss} tarihinde hakeme atandı. " 
                           
            };

            var logContent = new StringContent(JsonConvert.SerializeObject(logDto), Encoding.UTF8, "application/json");
            var createLogUrl = "https://localhost:7057/api/Logs";
            HttpResponseMessage logResponse = await _httpClient.PostAsync(createLogUrl, logContent);

            if (!logResponse.IsSuccessStatusCode)
            {
                ViewBag.LogHata = "Log kaydedilirken bir sorun oluştu.";
            }
            TempData["Success"] = "Hakem başarıyla atandı!";
            return RedirectToAction("ArticleIslem", new { id = makale.MakaleId });

        }
        #endregion
        public IActionResult EditorMessages()
        {
            return View();
        }
        public async Task<IActionResult> ArticleIslem(int id)
        {
            string apiUrl = $"https://localhost:7057/api/Articles/GetArticleById?id={id}";
            ArticleViewModel makale = null;

            var response = _httpClient.GetAsync(apiUrl).Result;
            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                makale = JsonConvert.DeserializeObject<ArticleViewModel>(json);
            }

            if (makale == null)
                return NotFound("Makale bulunamadı!");

            string articleFieldUrl = $"https://localhost:7057/api/ArticleFields/GetFieldsByArticleId?articleId={id}";
            var fieldResponse = _httpClient.GetAsync(articleFieldUrl).Result;

            List<ArticleFieldsDto> makaleAlanlari = new();
            if (fieldResponse.IsSuccessStatusCode)
            {
                var fieldJson = fieldResponse.Content.ReadAsStringAsync().Result;
                makaleAlanlari = JsonConvert.DeserializeObject<List<ArticleFieldsDto>>(fieldJson);
            }
            else
            {
                ViewBag.Warning = $"Makale {makale.MakaleId} için alanlar yüklenirken hata oluştu.";
            }

            List<int> fieldIds = makaleAlanlari.Select(f => f.FieldTopicId).Distinct().ToList();
            makale.IlgiAlanlari = fieldIds.Select(id => id.ToString()).ToList();

            makale.OnerilenHakemler = new List<ReviewerDto>();
            foreach (var fieldId in fieldIds)
            {
                string reviewerApiUrl = $"https://localhost:7057/api/ReviewerFieldTopics/GetReviewersByFieldId?fieldId={fieldId}";
                var reviewerResponse = _httpClient.GetAsync(reviewerApiUrl).Result;

                if (reviewerResponse.IsSuccessStatusCode)
                {
                    var reviewerJson = reviewerResponse.Content.ReadAsStringAsync().Result;
                    var hakemler = JsonConvert.DeserializeObject<List<ReviewerDto>>(reviewerJson);

                    makale.OnerilenHakemler.AddRange(
                        hakemler.Where(h => !makale.OnerilenHakemler.Any(x => x.ReviewerId == h.ReviewerId))
                    );
                }
                else
                {
                    ViewBag.Warning = $"Makale {makale.MakaleId} için fieldId {fieldId} ile hakemler yüklenirken hata oluştu.";
                }
            }

            using (var client = new HttpClient())
            {
                var logsApiUrl = $"https://localhost:7057/api/Logs/GetLogsByMakaleId?makaleId={id}";
                HttpResponseMessage logsResponse = await client.GetAsync(logsApiUrl);

                if (logsResponse.IsSuccessStatusCode)
                {
                    var logsContent = await logsResponse.Content.ReadAsStringAsync();
                    var logs = JsonConvert.DeserializeObject<List<LogDto>>(logsContent);
                    ViewBag.Loglar = logs;
                }
                else
                {
                    ViewBag.Loglar = new List<LogDto>(); 
                }
            }
            var alanlarResponse = await _httpClient.GetAsync($"https://localhost:7057/api/ArticleFields/GetFieldsBilgiByArticleId?articleId={id}");
            if (alanlarResponse.IsSuccessStatusCode)
            {
                var json = await alanlarResponse.Content.ReadAsStringAsync();
                ViewBag.AtanmisAlanlar = JsonConvert.DeserializeObject<List<GetArticleTopicDto>>(json);
            }
            var yazarBilgi = await GetYazarBilgileriAsync(makale.PdfFilePath);
            if (yazarBilgi != null)
            {
                makale.YazarAdlari = yazarBilgi.Authors ?? new List<string>();
                makale.EmailBilgileri = yazarBilgi.Emails ?? new List<string>();
                makale.KurumBilgileri = yazarBilgi.Institutions ?? new List<string>();
            }


            return View(makale);
        }

        #region Pdf ve hakem yorum birlestirme
        [HttpPost]
        public async Task<IActionResult> PdfBirlestir(int MakaleId)
        {
            string apiUrl = $"https://localhost:7057/api/Articles/GetArticleById?id={MakaleId}";
            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Makale bilgileri alınamadı.";
                return RedirectToAction("MakaleDetay", new { id = MakaleId });
            }

            var json = await response.Content.ReadAsStringAsync();
            var makale = JsonConvert.DeserializeObject<ArticleViewModel>(json);

            if (makale == null || string.IsNullOrWhiteSpace(makale.SonucPdfFilePath) ||
                string.IsNullOrWhiteSpace(makale.HakemDegerlendirmesi))
            {
                TempData["Error"] = "Gerekli PDF veya değerlendirme metni eksik.";
                return RedirectToAction("MakaleDetay", new { id = MakaleId });
            }

            var orjinalPath = System.IO.Path.Combine(_env.WebRootPath, makale.SonucPdfFilePath.TrimStart('/'));
            var geciciPath = System.IO.Path.Combine(_env.WebRootPath, "geciciPdfler", $"eval_{MakaleId}.pdf");
            var birlestirmePath = System.IO.Path.Combine(_env.WebRootPath, "geciciPdfler", $"merged_{MakaleId}.pdf");

            string fontPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
            PdfFont customFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);

            using (var geciciWriter = new PdfWriter(geciciPath))
            using (var geciciPdf = new PdfDocument(geciciWriter))
            {
                var doc = new Document(geciciPdf, PageSize.A4);
                doc.SetMargins(50, 50, 50, 50);

                doc.Add(new Paragraph("Hakem Değerlendirmesi")
                    .SetFont(customFont)
                    .SetFontSize(14)
                    .SetBold()
                    .SetMarginBottom(20)
                    .SetTextAlignment(TextAlignment.LEFT));

                doc.Add(new Paragraph(makale.HakemDegerlendirmesi)
                    .SetFont(customFont)
                    .SetFontSize(12)
                    .SetTextAlignment(TextAlignment.LEFT));

                doc.Close(); 
            }

            using (var mergeWriter = new PdfWriter(birlestirmePath))
            using (var mergedPdf = new PdfDocument(mergeWriter))
            {
                var merger = new PdfMerger(mergedPdf);

                using (var origReader = new PdfReader(orjinalPath))
                using (var origPdf = new PdfDocument(origReader))
                {
                    merger.Merge(origPdf, 1, origPdf.GetNumberOfPages());
                }

                using (var evalReader = new PdfReader(geciciPath))
                using (var evalPdf = new PdfDocument(evalReader))
                {
                    merger.Merge(evalPdf, 1, evalPdf.GetNumberOfPages());
                }
            }

            System.IO.File.Copy(birlestirmePath, orjinalPath, overwrite: true);

            System.IO.File.Delete(birlestirmePath);
            System.IO.File.Delete(geciciPath);

            makale.EnSonYapilanIsleminTarihi = DateTime.UtcNow;
            makale.MakaleDurumu = Models.ArticleStatus.MakaleSonuclandirildi;

            var updateUrl = $"https://localhost:7057/api/Articles";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(makale), Encoding.UTF8, "application/json");
            var updateResponse = await _httpClient.PutAsync(updateUrl, jsonContent);

            if (!updateResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Makale güncellenemedi.";
                return RedirectToAction("ArticleIslem", new { id = MakaleId });
            }

            TempData["Success"] = "Değerlendirme başarıyla yeni sayfaya eklendi.";
            return RedirectToAction("ArticleIslem", new { id = MakaleId });
        }
        #endregion

        #region Yazar Sonuc Gonderme
        [HttpPost]
        public async Task<IActionResult> SonucGonder(int MakaleId)
        {
            string apiUrl = $"https://localhost:7057/api/Articles/GetArticleById?id={MakaleId}";
            ArticleViewModel makale = null;
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                makale = JsonConvert.DeserializeObject<ArticleViewModel>(json);
            }

            if (makale == null)
            {
                TempData["Error"] = "Makale bulunamadı.";
                return RedirectToAction("MakaleDetay", new { id = MakaleId });
            }

            if (string.IsNullOrEmpty(makale.SonucPdfFilePath))
            {
                TempData["Error"] = "Sonuç PDF dosyası bulunamadı.";
                return RedirectToAction("MakaleDetay", new { id = MakaleId });
            }

            makale.MakaleDurumu = Models.ArticleStatus.MakaleSonucuYazaraIletildi;
            makale.EnSonYapilanIsleminTarihi = DateTime.UtcNow;

            string updateUrl = $"https://localhost:7057/api/Articles";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(makale), Encoding.UTF8, "application/json");
            var updateResponse = await _httpClient.PutAsync(updateUrl, jsonContent);

            if (!updateResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Makale güncellenemedi.";
                return RedirectToAction("MakaleDetay", new { id = MakaleId });
            }
            var logDto = new CreateLogDto
            {
                MakaleId = makale.MakaleId,
                islemZamani = DateTime.UtcNow,
                LogDetayi = $"{DateTime.UtcNow:dd.MM.yyyy HH:mm:ss} tarihinde sonuç yazara iletildi. "

            };

            var logContent = new StringContent(JsonConvert.SerializeObject(logDto), Encoding.UTF8, "application/json");
            var createLogUrl = "https://localhost:7057/api/Logs";
            HttpResponseMessage logResponse = await _httpClient.PostAsync(createLogUrl, logContent);

            if (!logResponse.IsSuccessStatusCode)
            {
                ViewBag.LogHata = "Log kaydedilirken bir sorun oluştu.";
            }
            TempData["Success"] = "Sonuç Yazara Gönderildi.";
            return RedirectToAction("ArticleIslem", new { id = MakaleId });
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> AlanAtama(int MakaleId)
        {
            string apiUrl = $"https://localhost:7057/api/Articles/GetArticleById?id={MakaleId}";
            ArticleViewModel makale = null;

            var articleResponse = _httpClient.GetAsync(apiUrl).Result;
            if (articleResponse.IsSuccessStatusCode)
            {
                var articleJson = articleResponse.Content.ReadAsStringAsync().Result;
                makale = JsonConvert.DeserializeObject<ArticleViewModel>(articleJson);
            }

            if (makale == null || string.IsNullOrEmpty(makale.PdfFilePath))
                return NotFound();

            var filePath = System.IO.Path.Combine("wwwroot", makale.PdfFilePath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound("PDF dosyası bulunamadı.");

            using var client = new HttpClient();
            using var content = new MultipartFormDataContent();
            using var fileStream = System.IO.File.OpenRead(filePath);

            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            content.Add(fileContent, "file", System.IO.Path.GetFileName(filePath));

            var response = await client.PostAsync("http://127.0.0.1:8001/articleTopic/", content);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "FastAPI servisi hata verdi");

            var json = await response.Content.ReadAsStringAsync();
            var topicResponse = JsonConvert.DeserializeObject<FieldTopicResponse>(json);
           
            var getFieldsUrl = $"https://localhost:7057/api/ArticleFields/GetFieldsByArticleId?articleId={MakaleId}";
            var getFieldsResponse = await _httpClient.GetAsync(getFieldsUrl);

            if (getFieldsResponse.IsSuccessStatusCode)
            {
                var jsonFields = await getFieldsResponse.Content.ReadAsStringAsync();
                var existingFields = JsonConvert.DeserializeObject<List<ArticleFieldsDto>>(jsonFields);

                if (existingFields != null && existingFields.Any())
                {
                    var deleteUrl = $"https://localhost:7057/api/ArticleFields/DeleteFieldsByArticleId?articleId={MakaleId}";
                    var deleteResponse = await _httpClient.DeleteAsync(deleteUrl);

                    if ((int)deleteResponse.StatusCode >= 500)
                    {
                        return StatusCode((int)deleteResponse.StatusCode, "Eski alanlar silinemedi.");
                    }
                }
            }
            foreach (var topic in topicResponse.Konular)
            {
                var getTopicUrl = $"https://localhost:7057/api/FieldTopics/GetByKonuAdiEn?name={Uri.EscapeDataString(topic.Topic)}";
                var topicResponseMsg = await _httpClient.GetAsync(getTopicUrl);
                if (!topicResponseMsg.IsSuccessStatusCode) continue;

                var topicJson = await topicResponseMsg.Content.ReadAsStringAsync();
                var fieldTopic = JsonConvert.DeserializeObject<FieldTopicDto>(topicJson);
                if (fieldTopic == null) continue;

                var dto = new CreateArticleFieldsDto
                {
                    MakaleId = MakaleId,
                    FieldTopicId = fieldTopic.FieldTopicId,
                    Skor = topic.Score
                };

                await _httpClient.PostAsJsonAsync("https://localhost:7057/api/ArticleFields", dto);
            }
            return RedirectToAction("ArticleIslem", new { id = MakaleId });
        }
        private async Task<YazarBilgiDto?> GetYazarBilgileriAsync(string pdfFilePath)
        {
            var filePath = System.IO.Path.Combine("wwwroot", pdfFilePath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return null;

            using var client = new HttpClient();
            using var content = new MultipartFormDataContent();
            using var fileStream = System.IO.File.OpenRead(filePath);

            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            content.Add(fileContent, "file", System.IO.Path.GetFileName(filePath));

            var response = await client.PostAsync("http://127.0.0.1:8001/ArticleYazarBolumu/", content);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<YazarBilgiDto>(json);
        }

    }
}
