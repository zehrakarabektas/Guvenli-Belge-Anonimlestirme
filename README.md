# 🛡️ Güvenli Akademik Makale Anonimleştirme Sistemi

[![.NET](https://img.shields.io/badge/.NET%208.0-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![FastAPI](https://img.shields.io/badge/FastAPI-005571?style=flat&logo=fastapi)](https://fastapi.tiangolo.com/)
[![Python](https://img.shields.io/badge/Python%203.9+-3776AB?style=flat&logo=python&logoColor=white)](https://www.python.org/)
[![MSSQL](https://img.shields.io/badge/MSSQL-CC2927?style=flat&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/sql-server)

Akademik yayın süreçlerinde **çift taraflı anonimlik** sağlayarak bilimsel tarafsızlığı korumak amacıyla geliştirilmiş, hibrit mimarili bir belge yönetim ve gizlilik sistemidir.

## 🎯 Projenin Amacı
Geleneksel hakemli değerlendirme süreçlerinde yazar bilgilerinin açık olması, değerlendirme aşamasında önyargılara sebep olabilmektedir. Bu sistem, **Yazar, Editör ve Hakem** rolleri arasındaki iş akışını dijitalleştirerek yazar bilgilerini otomatik olarak maskeler ve tarafsız bir değerlendirme ortamı sunar.

## 🚀 Öne Çıkan Özellikler

* **Akıllı NLP Motoru:** `spaCy` (en_core_web_lg) kullanarak metin içerisindeki PERSON (Kişi), ORG (Kurum) ve EMAIL etiketlerini yüksek doğrulukla tespit eder.
* **Hibrit Konu Sınıflandırma:** Makale özetinden çıkarılan anahtar kelimeler ile semantik benzerlik (%65) ve keyword eşleşmesi (%35) üzerinden otomatik alan ataması yapar.
* **Geri Döndürülebilir Maskeleme:** Gizlenen bilgiler referans kodlarıyla (#AD01 vb.) değiştirilir; orijinal veriler **AES-GCM** algoritması ile şifrelenmiş olarak saklanır.
* **Görsel Anonimleştirme:** Yazar isimleriyle konumsal olarak ilişkili görselleri otomatik tespit ederek bulanıklaştırır.
* **Mikroservis Mimarisi:** Web arayüzü, iş mantığı (API) ve NLP motoru birbirinden bağımsız katmanlar olarak çalışır.

## 🛠️ Teknoloji Yığını

### **Backend & Web**
* **Framework:** ASP.NET Core MVC & Web API
* **Veritabanı:** Microsoft SQL Server (3NF Tasarım)
* **ORM:** Entity Framework Core
* **Mapping:** AutoMapper

### **Yapay Zeka & Belge İşleme**
* **Framework:** Python FastAPI
* **NLP:** spaCy
* **PDF Engine:** PyMuPDF
* **Şifreleme:** AES-GCM Algoritması

## 📊 Konu Alanı Belirleme Algoritması
Sistem, makalenin akademik alanını şu formül ile hesaplar:
`Toplam Skor = (Semantik Benzerlik * 0.65) + (Keyword Eşleşme Oranı * 0.35)`

## 📖 Kullanıcı Rolleri
1. **Yazar:** Makalesini sisteme yükler ve benzersiz bir takip numarası ile süreci izler.
2. **Editör:** NLP sonuçlarını denetler, anonimleştirme seviyesini seçer ve uygun hakemi atar.
3. **Hakem:** Sadece maskelenmiş (anonim) belgeleri görerek değerlendirme puanlarını ve yorumlarını sisteme kaydeder.
