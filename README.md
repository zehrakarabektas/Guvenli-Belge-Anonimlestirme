# 🛡️ Güvenli Akademik Makale Anonimleştirme Sistemi

[![.NET](https://img.shields.io/badge/.NET%208.0-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![FastAPI](https://img.shields.io/badge/FastAPI-005571?style=flat&logo=fastapi)](https://fastapi.tiangolo.com/)
[![Python](https://img.shields.io/badge/Python%203.9+-3776AB?style=flat&logo=python&logoColor=white)](https://www.python.org/)
[![MSSQL](https://img.shields.io/badge/MSSQL-CC2927?style=flat&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/sql-server)

[cite_start]Akademik yayın süreçlerinde **çift taraflı anonimlik** sağlayarak bilimsel tarafsızlığı korumak amacıyla geliştirilmiş, hibrit mimarili bir belge yönetim ve gizlilik sistemidir[cite: 3, 16].

---

## 🎯 Projenin Amacı
[cite_start]Geleneksel hakemli değerlendirme süreçlerinde yazar bilgilerinin açık olması, değerlendirme aşamasında önyargılara sebep olabilmektedir[cite: 17]. [cite_start]Bu sistem, **Yazar, Editör ve Hakem** rolleri arasındaki iş akışını dijitalleştirerek yazar bilgilerini otomatik olarak maskeler ve tarafsız bir değerlendirme ortamı sunar[cite: 4, 18].

## 🚀 Öne Çıkan Özellikler

* [cite_start]**Akıllı NLP Motoru:** `spaCy` (en_core_web_lg) kullanarak metin içerisindeki PERSON, ORG ve EMAIL etiketlerini yüksek doğrulukla tespit eder[cite: 26, 47].
* [cite_start]**Hibrit Konu Sınıflandırma:** Makale özetinden çıkarılan anahtar kelimeler ile semantik benzerlik (%65) ve keyword eşleşmesi (%35) üzerinden otomatik alan ataması yapar[cite: 73, 74].
* [cite_start]**Geri Döndürülebilir Maskeleme:** Gizlenen bilgiler referans kodlarıyla (#AD01 vb.) değiştirilir; orijinal veriler **AES-GCM** algoritması ile şifrelenmiş olarak saklanır[cite: 13, 94, 95].
* [cite_start]**Görsel Anonimleştirme:** Yazar isimleriyle ilişkili görselleri otomatik tespit ederek bulanıklaştırır[cite: 12, 97].
* [cite_start]**Mikroservis Mimarisi:** Web arayüzü, iş mantığı (API) ve NLP motoru birbirinden bağımsız katmanlar olarak çalışır[cite: 25, 45].

## 🛠️ Teknoloji Yığını

### **Backend & Web**
* [cite_start]**Framework:** ASP.NET Core MVC & Web API [cite: 24, 33]
* [cite_start]**Veritabanı:** Microsoft SQL Server (3NF Tasarım) [cite: 43, 61]
* [cite_start]**ORM:** Entity Framework Core [cite: 43]
* [cite_start]**Mapping:** AutoMapper [cite: 40, 41]

### **Yapay Zeka & Belge İşleme**
* [cite_start]**Framework:** Python FastAPI [cite: 25, 46]
* [cite_start]**NLP:** spaCy [cite: 7, 26]
* [cite_start]**PDF Engine:** PyMuPDF [cite: 7, 27]
* [cite_start]**Şifreleme:** AES-GCM Algoritması [cite: 13, 28]

---

## 📊 Konu Alanı Belirleme Algoritması (Kabakod)
[cite_start]Sistem, makalenin akademik alanını şu formül ile hesaplar[cite: 83]:
`Toplam Skor = (Semantik Benzerlik * 0.65) + (Keyword Eşleşme Oranı * 0.35)`

## 📖 Kullanıcı Rolleri
1. [cite_start]**Yazar:** Makalesini sisteme yükler ve takip numarası ile süreci izler[cite: 4, 21].
2. [cite_start]**Editör:** NLP sonuçlarını denetler, anonimleştirme seviyesini seçer ve hakem ataması yapar[cite: 5, 22].
3. [cite_start]**Hakem:** Sadece maskelenmiş (anonim) belgeleri görerek değerlendirme yapar[cite: 6, 23].
