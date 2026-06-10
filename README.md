# GroceryPromoApi

A REST API for tracking grocery promotions across Bulgarian supermarkets (Kaufland, Lidl, Fantastico, Billa). Users can browse current promos, save favourites, and receive push notifications when a saved product goes on sale.

## Tech Stack

- **ASP.NET Core 9** — Web API, Clean Architecture
- **PostgreSQL** — primary database via EF Core
- **.NET Aspire** — local orchestration and service discovery
- **Firebase FCM** — push notifications
- **RabbitMQ** — async notification dispatch (planned)

## Features

- **Catalogue** — unified product catalogue across all stores; one entry per unique product, with per-store offer rows (price, discount, validity)
- **Smart deduplication** — products are matched across supermarkets using name normalization and token-similarity scoring; ambiguous matches go to an admin review queue
- **Favourites** — users favourite catalogue products, not store-specific listings; matches across all stores automatically
- **Preferred stores** — filter weekly digest by frequently visited supermarkets
- **Push notifications** — weekly digest via FCM when favourited products are on promo
- **JWT auth** — access + refresh token rotation, account lockout, rate limiting
- **Nightly sync** — pulls current brochures from Price Barometer API; resumes interrupted syncs from the last saved page

## Project Structure

```
GroceryPromoApi.Domain          — entities, no dependencies
GroceryPromoApi.Application     — services, interfaces, DTOs
GroceryPromoApi.Infrastructure  — EF Core, repositories, HTTP clients
GroceryPromoApi                 — controllers, DI wiring
GroceryPromoApi.AppHost         — .NET Aspire host
```

## API Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/v1/auth/register` | — | Register |
| POST | `/api/v1/auth/login` | — | Login |
| POST | `/api/v1/auth/refresh` | — | Refresh token |
| POST | `/api/v1/auth/logout` | ✓ | Logout |
| GET | `/api/v1/catalogue` | — | Search catalogue with per-store offers |
| GET | `/api/v1/favourites` | ✓ | Get favourites |
| POST | `/api/v1/favourites` | ✓ | Add favourite |
| DELETE | `/api/v1/favourites/{id}` | ✓ | Remove favourite |
| GET | `/api/v1/preferred-stores` | ✓ | Get preferred stores |
| POST | `/api/v1/preferred-stores` | ✓ | Add preferred store |
| DELETE | `/api/v1/preferred-stores/{id}` | ✓ | Remove preferred store |
| GET | `/api/v1/supermarkets` | — | List supermarkets |

## Getting Started

**Prerequisites:** .NET 9 SDK, Docker Desktop, Price Barometer API key

```bash
# Set your API key in User Secrets
dotnet user-secrets set "PriceBarometer:ApiKey" "your-api-key" --project GroceryPromoApi

# Run via Aspire (starts PostgreSQL automatically)
dotnet run --project GroceryPromoApi.AppHost
```

Swagger UI: `https://localhost:7277/swagger`
