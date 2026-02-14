# CAP Alert Hub

CAP Alert Hub is a ASP.NET Core Web API that ingests **CAP (Common Alerting Protocol)** alerts, stores them, routes them to **subscriptions**, and processes **asynchronous deliveries** via RabbitMQ.  
It also exposes query endpoints for active alerts and delivery audit/retry.

---

## Why this project exists

This repo is a weekend-sized backend project built to explore .NET/C# concepts that differ from a typical Java stack:

- **ASP.NET Core** middleware + controllers + DI
- **LINQ** (IQueryable vs IEnumerable) and how it maps to SQL via EF Core
- **EF Core** mapping, migrations, owned types, tracking/no-tracking queries
- **ProblemDetails** (RFC7807-style errors) and consistent API error contracts
- **JWT Bearer auth** + authorization policies
- **RabbitMQ** integration and background workers (`BackgroundService`)
- **Outbox pattern** for reliable publish of delivery messages

---

## High-level overview

**Core idea:** ingest a CAP alert → persist it → match against subscriptions → create delivery jobs → publish to RabbitMQ → delivery worker executes delivery attempts and persists audit results.