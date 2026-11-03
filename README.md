# Seyitname – Personal Portfolio & Management System

🌐 **Live Demo:** [seyitname.vercel.app](https://seyitname.vercel.app)

<details open>
<summary><b>🇬🇧 English</b></summary>

A full-stack personal portfolio and contact platform built with **ASP.NET Core 8.0 MVC** and **Entity Framework Core**. It includes user authentication with role-based authorization and database-driven portfolio and contact content.

## Tech Stack

- **Backend:** C#, ASP.NET Core 8.0 MVC, ASP.NET Core Identity
- **Data:** Entity Framework Core 8.0 — SQLite (local development), PostgreSQL (production)
- **Frontend:** HTML5, CSS3, Bootstrap, JavaScript
- **Deployment:** Docker (multi-stage build), Vercel

## Features

- Registration, login, and role-based authorization (Admin / Member) with ASP.NET Core Identity
- Database-driven portfolio and contact content
- Public pages: portfolio, contact, information pages, games section, privacy policy, and terms of service
- Environment-based setup: a local SQLite database is created automatically in development; EF Core migrations are applied automatically on startup in production
- Responsive layout

## Getting Started

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
git clone https://github.com/Seyitname/SeyitnameWebSite.git
cd SeyitnameWebSite
dotnet run
```

The app listens on port 80 by default. To change it, set the `PORT` environment variable:

```bash
# Windows PowerShell:  $env:PORT=5000; dotnet run
# macOS / Linux:       PORT=5000 dotnet run
```

In production, the PostgreSQL connection string is provided through environment variables and is not stored in the repository.

</details>

<details>
<summary><b>🇹🇷 Türkçe</b></summary>

**ASP.NET Core 8.0 MVC** ve **Entity Framework Core** ile geliştirilmiş, full-stack bir kişisel portföy ve iletişim platformudur. Rol tabanlı yetkilendirmeli kimlik doğrulama ve veritabanı tabanlı portföy/iletişim içeriği sunar.

## Teknolojiler

- **Backend:** C#, ASP.NET Core 8.0 MVC, ASP.NET Core Identity
- **Veri:** Entity Framework Core 8.0 — SQLite (yerel geliştirme), PostgreSQL (canlı ortam)
- **Frontend:** HTML5, CSS3, Bootstrap, JavaScript
- **Yayınlama:** Docker (çok aşamalı build), Vercel

## Özellikler

- ASP.NET Core Identity ile kayıt, giriş ve rol tabanlı yetkilendirme (Admin / Member)
- Veritabanı tabanlı portföy ve iletişim içeriği
- Herkese açık sayfalar: portföy, iletişim, bilgi sayfaları, oyunlar bölümü, gizlilik politikası ve kullanım koşulları
- Ortama göre kurulum: geliştirmede yerel SQLite veritabanı otomatik oluşturulur, canlıda EF Core migration'ları açılışta otomatik uygulanır
- Responsive tasarım

## Başlangıç

[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) gerekir.

```bash
git clone https://github.com/Seyitname/SeyitnameWebSite.git
cd SeyitnameWebSite
dotnet run
```

Uygulama varsayılan olarak 80. portu dinler. Değiştirmek için `PORT` ortam değişkenini ayarla:

```bash
# Windows PowerShell:  $env:PORT=5000; dotnet run
# macOS / Linux:       PORT=5000 dotnet run
```

Canlı ortamda PostgreSQL bağlantı bilgisi ortam değişkenleriyle verilir, repoda tutulmaz.

</details>