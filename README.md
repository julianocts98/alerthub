# AlertHub

AlertHub is a demo ASP.NET Core Web API for ingesting CAP (Common Alerting Protocol) alerts, storing them, matching them to user subscriptions, and executing asynchronous deliveries.

The project is intentionally designed to exercise modern .NET backend concerns in a compact codebase:
- layered architecture (`Domain`, `Application`, `Infrastructure`, `Api`)
- JWT auth + role/scope authorization
- EF Core + PostgreSQL persistence
- RabbitMQ + background workers
- outbox-driven asynchronous pipeline

## What the system does

At a high level, the system processes alerts in this sequence:
1. A CAP alert is received (`JSON` or `XML`).
2. Payload is parsed and validated (including CAP XML schema validation for XML payloads).
3. Domain rules are applied and the alert is persisted.
4. Domain events are written to an outbox table in the same transaction.
5. `OutboxPublisher` pushes outbox messages to RabbitMQ.
6. `SubscriptionMatcherWorker` consumes messages and schedules delivery records.
7. `AlertDeliveryWorker` claims pending deliveries and sends through configured channels (Telegram in this demo).

## Architecture overview

### Domain
Contains business concepts and invariants:
- CAP alert aggregate and value objects
- subscription aggregate
- domain errors/events

### Application
Contains use cases and contracts:
- ingestion/query/subscription services
- repository interfaces
- delivery use case abstractions (`DeliveryService`, `IDeliveryRepository`)

### Infrastructure
Contains technical implementations:
- EF Core `AppDbContext`, entities, mappings, repositories
- background workers
- RabbitMQ integration
- Telegram delivery channel
- security adapter (`CurrentUser`)

### API
HTTP controllers and API-level concerns:
- endpoints for alerts, subscriptions, deliveries, and identity token issuance
- `ProblemDetails` mapping for ingestion errors

## Main endpoints

- `POST /api/identity/token`: issue a demo JWT (requires demo issuer header key)
- `POST /api/alerts/ingest`: ingest CAP payload (`application/json` or `application/xml`)
- `GET /api/alerts`: search alerts with filters and keyset cursor pagination
- `POST /api/subscriptions`: create subscription for authenticated user
- `GET /api/subscriptions/{id}`: fetch subscription by id
- `GET /api/deliveries`: list deliveries (admin only)
- `POST /api/deliveries/{id}/retry`: retry failed delivery (admin only)

## Running locally

### Prerequisites
- .NET SDK 10
- Docker + Docker Compose

### 1. Start infrastructure

```bash
docker compose up -d
```

This starts:
- PostgreSQL on `localhost:5432`
- RabbitMQ on `localhost:5672`
- RabbitMQ management UI on `http://localhost:15672` (`alerthub` / `alerthub`)

### 2. Apply database migrations

From repository root:

```bash
dotnet ef database update --project AlertHub --startup-project AlertHub
```

If `dotnet ef` is not installed:

```bash
dotnet tool install --global dotnet-ef
```

### 3. Run the API

```bash
dotnet run --project AlertHub
```

Default local URL (from launch settings):
- `http://localhost:5082`

In `Development`, OpenAPI is exposed at:
- `http://localhost:5082/openapi/v1.json`

## Authentication and authorization

All operational endpoints are protected by JWT bearer auth.

Policies/roles used by API:
- `alerts:ingest` scope required for `POST /api/alerts/ingest`
- `admin` role required for deliveries endpoints
- authenticated user required for subscriptions endpoints

### Get a demo JWT

`POST /api/identity/token` is a demo token issuer and requires:
- header `X-Demo-Issuer-Key` matching `Identity:IssuerApiKey`
- valid role (`admin` or `subscriber`)
- valid scopes (currently `alerts:ingest`)

Example:

```bash
curl -X POST "http://localhost:5082/api/identity/token" \
  -H "Content-Type: application/json" \
  -H "X-Demo-Issuer-Key: change-this-demo-issuer-key" \
  -d '{
    "userId": "demo-user",
    "role": "admin",
    "scopes": ["alerts:ingest"]
  }'
```

Use returned token:

```bash
-H "Authorization: Bearer <TOKEN>"
```

## Example: ingest an alert

```bash
curl -X POST "http://localhost:5082/api/alerts/ingest" \
  -H "Authorization: Bearer <TOKEN>" \
  -H "Content-Type: application/xml" \
  --data-binary @sample-cap.xml
```

## Configuration

Default config is in `AlertHub/appsettings.json`.

Important keys:
- `ConnectionStrings:DefaultConnection`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Key`
- `Identity:IssuerApiKey`
- `RabbitMQ:HostName` / `Port` / `UserName` / `Password` (workers have localhost defaults)
- `Telegram:BotToken` (optional, but required for real Telegram delivery success)
- `BackgroundJobs:OutboxIntervalMs` / `DeliveryIntervalMs` (optional tuning)

## Running tests

```bash
dotnet test
```

Test suite includes:
- domain unit tests
- application tests
- integration tests with PostgreSQL/RabbitMQ containers
- architecture guard test preventing `Application` from referencing `Infrastructure` namespaces

## Demo limitations

- identity endpoint is intentionally demo-focused (header-protected token minting)
- delivery channel set is minimal (Telegram)
- retry/backoff and observability are basic but functional
