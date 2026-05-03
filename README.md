# GroceryPromoApi

A REST API for tracking grocery promotions across Bulgarian supermarkets (Kaufland, Lidl, Fantastico, Billa). Users can browse products on promo, save favourites, and receive push notifications when a favourite goes on sale.

## Tech Stack

- **ASP.NET Core** — Web API
- **PostgreSQL** — primary database (via EF Core)
- **RabbitMQ** — notification job queue
- **.NET Aspire** — local orchestration
- **Firebase FCM** — push notifications

## Features

- Nightly sync from the [Price Barometer API](https://prices.alexandergekov.com) — one catalogue entry per unique product, one offer row per supermarket
- JWT authentication with refresh tokens and rate limiting
- Favourites — link to catalogue products, not fragile string matching
- Preferred stores — filter notifications by supermarket
- Quantity normalizer — maps Cyrillic units to canonical Latin form for deduplication

## API Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/v1/auth/register` | — | Register |
| POST | `/api/v1/auth/login` | — | Login |
| POST | `/api/v1/auth/refresh` | — | Refresh token |
| POST | `/api/v1/auth/logout` | ✓ | Logout |
| GET | `/api/v1/catalogue` | — | Search products with offers |
| GET | `/api/v1/favourites` | ✓ | Get user favourites |
| POST | `/api/v1/favourites` | ✓ | Add favourite |
| DELETE | `/api/v1/favourites/{id}` | ✓ | Remove favourite |
| GET | `/api/v1/preferred-stores` | ✓ | Get preferred stores |
| POST | `/api/v1/preferred-stores` | ✓ | Add preferred store |
| DELETE | `/api/v1/preferred-stores/{id}` | ✓ | Remove preferred store |
| GET | `/api/v1/supermarkets` | — | List supermarkets |

## Getting Started

**Prerequisites:** .NET 9, Docker (PostgreSQL + RabbitMQ), valid Price Barometer API key

```bash
# Start infrastructure
docker-compose up -d

# Run via Aspire
dotnet run --project GroceryPromoApi.AppHost
```

Add the following to `appsettings.Development.json`:

```json
{
  "Jwt": { "SecretKey": "your-32-char-secret-key-here" },
  "PriceBarometer": { "ApiKey": "your-api-key" }
}
```

Swagger UI is available at `https://localhost:7277/swagger` in development.
