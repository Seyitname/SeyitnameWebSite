# Seyitname - Kişisel Portföy ve Yönetim Sistemi

Bu proje, **ASP.NET Core 8.0 MVC** ve **Entity Framework Core** kullanılarak geliştirilmiş, katmanlı mimariye (N-Layer Architecture) sahip kişisel bir web portföy projesidir.

## 🛠️ Kullanılan Teknolojiler

* **Backend:** C#, .NET 8.0 MVC, ASP.NET Core Identity
* **Veritabanı:** SQLite / EF Core 8.0
* **Frontend:** HTML5, CSS3 (Custom Responsive & Animations), JavaScript
* **Araçlar & Üretkenlik:** Git, GitHub Copilot (UI ve Şablon geliştirmelerinde verimlilik aracı olarak kullanıldı)

## ✨ Öne Çıkan Özellikler

* **Identity Authentication:** Kullanıcı kayıt olma, giriş yapma ve rol tabanlı yetkilendirme sistemi.
* **Dinamik İçerik:** EF Core Migration yapısı ile veritabanı üzerinden dinamik portföy ve iletişim formu yönetimi.
* **Modern Arayüz:** Custom CSS animasyonları ve responsive layout tasarımı.

## 🚀 Projeyi Yerelde Çalıştırma

1. Repoyu klonlayın: `git clone https://github.com/Seyitname/SeyitnameWebSite.git`
2. Proje dizinine gidin: `cd SeyitnameWebSite`
3. Veritabanını güncelleyin: `dotnet ef database update`
4. Uygulamayı çalıştırın: `dotnet run`
