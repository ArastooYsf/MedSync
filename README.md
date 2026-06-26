<div align="center">

<img src="https://img.shields.io/badge/status-in%20development-orange?style=for-the-badge" alt="In Development"/>
<img src="https://img.shields.io/badge/license-MIT-blue?style=for-the-badge" alt="MIT License"/>
<img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET 9"/>
<img src="https://img.shields.io/badge/Avalonia-12.0-8B5CF6?style=for-the-badge" alt="Avalonia 12"/>
<img src="https://img.shields.io/badge/PRs-welcome-brightgreen?style=for-the-badge" alt="PRs Welcome"/>

</div>

---

> 🌐 **Language / زبان:**
> &nbsp;[🇬🇧 English](#-medsync--patient-management-system) &nbsp;|&nbsp; [🇮🇷 فارسی](#-medsync---سیستم-مدیریت-بیماران)

---

<br/>

# 🏥 MedSync — Patient Management System

**MedSync** is a cross-platform desktop application for managing patients, appointments, and medical records in clinics and medical offices. Built with Avalonia UI and .NET 9, it runs natively on Windows, Linux, and macOS.

---

## ✨ Features

### 👤 Patient Management
- Register and manage full patient profiles
- Complete medical records and condition history
- Detailed patient history log

### 📅 Appointments
- Schedule and track patient appointments
- Automated reminders via the background reminder service

### 🔔 Notifications
- Real-time in-app notifications for important events
- Multiple notification types supported

### 🔐 Authentication & Access Control
- Secure login system
- Role-based access control — Admin, Doctor, Staff

### 🎨 User Interface
- Clean, modern UI with Fluent Design
- Full **Persian (Farsi) language** support with Shabnam font
- Built-in Persian date picker
- Dark / Light mode support
- Phosphor icon set

### 🗄️ Data & Storage
- Local SQLite database via Entity Framework Core
- Offline-first — no internet connection required
- Database migrations for safe schema updates

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| UI Framework | [Avalonia UI](https://avaloniaui.net/) 12.0 |
| Language | C# / .NET 9 |
| Architecture | MVVM (CommunityToolkit.Mvvm) |
| Database | SQLite + Entity Framework Core 9 |
| Icons | Phosphor Icons |
| Persian Font | Shabnam-FD |

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Windows, Linux, or macOS

### Installation

**1. Clone the repository**
```bash
git clone https://github.com/ArastooYsf/MedSync.git
cd MedSync
```

**2. Restore dependencies**
```bash
dotnet restore
```

**3. Apply database migrations**
```bash
dotnet ef database update
```

**4. Run the app**
```bash
dotnet run
```

---

## 📦 Building for Release

**Windows**
```bash
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish/windows
```

**Linux**
```bash
dotnet publish -c Release -r linux-x64 --self-contained true -o ./publish/linux
```

**macOS**
```bash
dotnet publish -c Release -r osx-x64 --self-contained true -o ./publish/mac
```

---

## 🗂️ Project Structure

```
MedSync/
├── Assets/             # Fonts, icons, images
├── Controls/           # Reusable UI controls (Persian date picker, etc.)
├── Converters/         # XAML value converters
├── Data/               # DbContext and EF configuration
├── DialogViewModels/   # ViewModels for dialog windows
├── DialogViews/        # Dialog XAML views
├── Factories/          # Page and DbContext factories
├── Helpers/            # Persian calendar utilities
├── Migrations/         # EF Core database migrations
├── Models/             # Data models (Patient, Appointment, User...)
├── Services/           # Business logic (Auth, Patient, Appointment...)
├── Styles/             # Global XAML styles
├── ViewModels/         # Page ViewModels (MVVM)
└── Views/              # Page XAML views
```

---

## 🤝 Contributing

Contributions are welcome from developers around the world!

- 🐛 Found a bug? [Open an issue](https://github.com/ArastooYsf/MedSync/issues)
- 💡 Have an idea? Submit a pull request

Please make sure your code follows the existing MVVM structure and includes relevant migrations if you change the data model.

---

## 📄 License

Copyright © 2024 **Arastoo Yousefi (ارسطو یوسفی)**

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
Any fork or derivative work must include clear attribution to the original author and repository.

---

<br/>
<br/>

---

> 🌐 **Language / زبان:**
> &nbsp;[🇬🇧 English](#-medsync--patient-management-system) &nbsp;|&nbsp; [🇮🇷 فارسی](#-medsync---سیستم-مدیریت-بیماران)

---

<div dir="rtl" lang="fa">

<br/>

# 🏥 MedSync - سیستم مدیریت بیماران

**MedSync** یک اپلیکیشن دسکتاپ چندسکویی برای مدیریت بیماران، نوبت‌دهی و پرونده‌های پزشکی در مطب‌ها و کلینیک‌هاست. با Avalonia UI و .NET 9 ساخته شده و روی Windows، Linux و macOS اجرا می‌شود.

---

## ✨ قابلیت‌ها

### 👤 مدیریت بیماران
- ثبت و مدیریت پروفایل کامل بیماران
- پرونده پزشکی کامل و سوابق بیماری‌ها
- تاریخچه کامل بیمار

### 📅 نوبت‌دهی
- برنامه‌ریزی و پیگیری نوبت‌های بیماران
- یادآوری خودکار از طریق سرویس پس‌زمینه

### 🔔 اعلان‌ها
- اعلان‌های درون‌برنامه‌ای برای رویدادهای مهم
- پشتیبانی از انواع مختلف اعلان

### 🔐 احراز هویت و کنترل دسترسی
- سیستم ورود امن
- کنترل دسترسی مبتنی بر نقش — مدیر، پزشک، کارمند

### 🎨 رابط کاربری
- UI مدرن و تمیز با طراحی Fluent
- پشتیبانی کامل از زبان فارسی با فونت شبنم
- انتخابگر تاریخ شمسی داخلی
- پشتیبانی از حالت تاریک / روشن
- آیکون‌های Phosphor

### 🗄️ داده و ذخیره‌سازی
- پایگاه داده SQLite محلی از طریق Entity Framework Core
- آفلاین-فرست — بدون نیاز به اینترنت
- مایگریشن‌های پایگاه داده برای به‌روزرسانی امن

---

## 🛠️ تکنولوژی‌ها

| لایه | تکنولوژی |
|------|----------|
| فریم‌ورک UI | [Avalonia UI](https://avaloniaui.net/) 12.0 |
| زبان | C# / .NET 9 |
| معماری | MVVM (CommunityToolkit.Mvvm) |
| پایگاه داده | SQLite + Entity Framework Core 9 |
| آیکون‌ها | Phosphor Icons |
| فونت فارسی | Shabnam-FD |

---

## 🚀 شروع به کار

### پیش‌نیازها
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Windows، Linux یا macOS

### نصب

**۱. مخزن را کلون کنید**
```bash
git clone https://github.com/ArastooYsf/MedSync.git
cd MedSync
```

**۲. وابستگی‌ها را نصب کنید**
```bash
dotnet restore
```

**۳. مایگریشن‌های پایگاه داده را اعمال کنید**
```bash
dotnet ef database update
```

**۴. برنامه را اجرا کنید**
```bash
dotnet run
```

---

## 📦 خروجی برای Release

**ویندوز**
```bash
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish/windows
```

**لینوکس**
```bash
dotnet publish -c Release -r linux-x64 --self-contained true -o ./publish/linux
```

**مک**
```bash
dotnet publish -c Release -r osx-x64 --self-contained true -o ./publish/mac
```

---

## 🗂️ ساختار پروژه

```
MedSync/
├── Assets/             # فونت‌ها، آیکون‌ها، تصاویر
├── Controls/           # کنترل‌های UI قابل استفاده مجدد (انتخابگر تاریخ شمسی و...)
├── Converters/         # مبدل‌های XAML
├── Data/               # DbContext و تنظیمات EF
├── DialogViewModels/   # ViewModel های پنجره‌های دیالوگ
├── DialogViews/        # XAML دیالوگ‌ها
├── Factories/          # فکتوری‌های صفحه و DbContext
├── Helpers/            # ابزارهای تقویم شمسی
├── Migrations/         # مایگریشن‌های پایگاه داده EF Core
├── Models/             # مدل‌های داده (بیمار، نوبت، کاربر و...)
├── Services/           # منطق کسب‌وکار (احراز هویت، بیمار، نوبت و...)
├── Styles/             # استایل‌های XAML سراسری
├── ViewModels/         # ViewModel های صفحات (MVVM)
└── Views/              # XAML صفحات
```

---

## 🤝 مشارکت

از مشارکت توسعه‌دهندگان سراسر جهان استقبال می‌کنیم!

- 🐛 باگ پیدا کردید؟ [یک Issue باز کنید](https://github.com/ArastooYsf/MedSync/issues)
- 💡 ایده دارید؟ یک Pull Request ارسال کنید

لطفاً مطمئن شوید کدتان از ساختار MVVM موجود پیروی می‌کند و در صورت تغییر مدل داده، مایگریشن مربوطه را اضافه کنید.

---

## 📄 مجوز

حق نشر © ۱۴۰۳ **ارسطو یوسفی (Arastoo Yousefi)**

این پروژه تحت مجوز MIT ارائه شده است — برای جزئیات به فایل [LICENSE](LICENSE) مراجعه کنید.
هر fork یا اثر مشتق باید شامل attribution واضح به نویسنده و مخزن اصلی باشد.

</div>