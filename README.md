-ReadMe TR

  *Oyun Tanımı
Wordle bir kelime tahmin etme oyunudur.  Oyuncu her ayrı güne özel bir 5 harfli kelimeyi, yine 5 harfli kelimeler ile tahmin yaparak bulmaya çalışır. Eğer günlük kelime ile tahmin kelimesinin harflerinde eşleşme olursa oyuncuya ipucu olarak gösterilir.
Yeşil: Tahmin kelimesindeki ilgili harf günlük kelimede mevcut ve yeri doğru.
Sarı: Tahmin kelimesindeki ilgili harf günlük kelimede mevcut ama yeri yanlış.
Gri: Tahmin kelimesindeki ilgili harf günlük kelimede mevcut değil.
Oyuncunun amacı mümkün olduğu kadar az tahminde bulunarak günlük kelimeyi tahmin etmektir. Maksimum tahmin hakkı 5'tir. Puanlandırma kaç tahmin hakkı kalmışken doğru cevap verildiğine göre değişmektedir.
Oyuncu oyunu oynayabilmek için kayıt olup giriş yapmalı ve e-posta adresini doğrulamalıdır.

  *Mimari Yapı
Bu proje, Clean Architecture prensiplerine uygun şekilde kurgulanmış, DDD (Domain-Driven Design) yaklaşımıyla modüler olarak ayrılmıştır.
CQRS modeli MediatR kütüphanesi ile uygulanmış, her komut ve sorgu Vertical Slice Architecture anlayışına göre organize edilmiştir,
Yani her feature kendi klasöründe bağımsız olarak yer almakta; Command, Query, Validator, DTO, Handler gibi yapıların bir arada bulunması sağlanmıştır.
Katmanlar:
Domain: İş kuralları ve varlıklar
Application: Use-case’ler (MediatR Handler’ları), DTO’lar, Validasyonlar
Infrastructure: EF Core, veri erişim, servis implementasyonları (mail, token, vs.)
WebAPI: Giriş noktası, Controller’lar ve servis konfigürasyonları
Proje, SOLID prensipleri, dependency injection, custom exception handling, ve unit of work pattern gibi modern yaklaşımlar ile desteklenmiştir.

  *Kullanılan Teknolojiler
ASP.NET Core 9 Web API,
Entity Framework Core 9 (SQL Server),
MediatR (CQRS + Vertical Slice Pattern),
FluentValidation,
AutoMapper,
JWT Authentication + Refresh Token,
BCrypt.Net,
Serilog (MSSQL Sink ile),
Swashbuckle / Swagger,
AspNetCoreRateLimit

-ReadMe Eng

  *Game Description
Wordle is a word-guessing game. Each day has a unique 5-letter word, and the player tries to guess it using other 5-letter words. If there are matching letters between the guessed word and the daily word, visual hints are given to the player.
Green: The letter exists in the daily word and is in the correct position.
Yellow: The letter exists in the daily word but is in the wrong position.
Gray: The letter does not exist in the daily word.
The goal is to find the daily word in as few attempts as possible. The player has a maximum of 5 guesses. The score depends on how many guesses were left when the correct word was found.
To play the game, users must register, log in, and verify their email address.

  *Architectural Overview
This project follows the principles of Clean Architecture, structured with a modular and maintainable design aligned with Domain-Driven Design (DDD) practices.
It implements the CQRS pattern using MediatR, and each feature follows the Vertical Slice Architecture approach — meaning commands, queries, DTOs, validators, and handlers are grouped together by feature rather than by type.
Layers:
Domain: Core business logic and entities
Application: Use-cases (MediatR handlers), DTOs, validation rules
Infrastructure: Data access (EF Core), external services (e.g., email, token)
WebAPI: Entry point, controllers, and configuration
The project also embraces modern practices such as SOLID principles, dependency injection, custom exception handling, and the unit of work pattern, ensuring a robust and scalable backend architecture.

  *Technologies Used
ASP.NET Core 9 Web API,
Entity Framework Core 9 (SQL Server),
MediatR (CQRS + Vertical Slice Pattern),
FluentValidation,
AutoMapper
JWT Authentication + Refresh Token,
BCrypt.Net,
Serilog (With MSSQL Sink),
Swashbuckle / Swagger,
AspNetCoreRateLimit,
