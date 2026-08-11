# ParkLink

## Cloud-Native Smart Parking Platform

ParkLink is a distributed, event-driven smart parking platform built with modern .NET technologies and orchestrated using .NET Aspire.

The project demonstrates production-oriented patterns for building observable, reliable and scalable distributed systems.

### Technology

.NET 10
.NET Aspire
ASP.NET Core
C#
EF Core
SQL Server
RabbitMQ
MassTransit
Redis
OpenTelemetry
Serilog
Swagger / OpenAPI
Docker
GitHub Actions

                    ┌─────────────────────┐
                    │   ParkLink Gateway   │
                    └──────────┬──────────┘
                               │
             ┌─────────────────┼─────────────────┐
             │                 │                 │
             ▼                 ▼                 ▼
       Identity            Users             Vehicle
             │                 │                 │
             └─────────────────┼─────────────────┘
                               │
                               ▼
                         RabbitMQ
                               │
            ┌──────────────────┼──────────────────┐
            ▼                  ▼                  ▼
        Parking           Reservation       Notification
            │                  │
            └──────────┬───────┘
                       ▼
                    Payment

## 1. Transactional Outbox
HTTP Request
     │
     ▼
Domain Change
     │
     ├──────────────► SQL Transaction
     │                    │
     │                    ├── Entity
     │                    └── OutboxMessage
     │
     ▼
Transaction Commit
     │
     ▼
MassTransit Outbox
     │
     ▼
RabbitMQ

## 2. Idempotent consumers

RabbitMQ
   │
   ▼
Consumer
   │
   ▼
Inbox / Processed Message
   │
   ├── Already processed → Ignore
   │
   └── New message → Process

## 3. Optimistic concurrency

public byte[] RowVersion { get; set; } = [];

## 4. Correlation IDs

HTTP Request
     │
     │ X-Correlation-ID
     ▼
Vehicle Service
     │
     ▼
RabbitMQ
     │
     ▼
Notification Service
     │
     ▼
Logs

## Project Status

Identity       ✅
Users          ✅
Vehicle        ✅
Parking        ✅
Notification   🚧
Reservation    🚧
Payment        ⏳
IoT Gateway    ⏳

# # Roadmap

## Phase 1 — Core Platform

[x] Identity
[x] Users
[x] Vehicle
[x] Parking

## Phase 2 — Messaging

[x] RabbitMQ
[x] MassTransit
[x] Integration Events
[x] Transactional Outbox
[x] Retry Policies
[x] Dead Letter Handling

## Phase 3 — Reliability

[x] Optimistic Concurrency
[x] Correlation IDs
[x] Global Exception Handling
[x] Health Checks
[x] OpenTelemetry
[ ] Outbox Monitoring
[ ] Distributed Idempotency

## Phase 4 — Reservation

[x] Reservation domain
[x] Reservation holds
[x] Reservation lifecycle
[ ] Payment integration
[ ] Advanced availability

## Phase 5 — Smart Parking

[ ] RFID
[ ] OCR
[ ] IoT Gateway
[ ] Gate Access
[ ] Real-time occupancy
[ ] Mobile application

