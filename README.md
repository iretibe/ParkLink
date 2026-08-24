# ParkLink

## Cloud-Native Smart Parking Platform

ParkLink is a cloud-native, distributed and event-driven smart parking platform built with modern .NET technologies and orchestrated using .NET Aspire.

The platform is designed to demonstrate production-oriented approaches to building reliable, observable and scalable distributed systems, including service isolation, asynchronous messaging, transactional outbox processing, idempotent message consumption, optimistic concurrency, distributed tracing and health monitoring.

> **Project status:** Active development

## Why ParkLink?

Finding and managing parking spaces involves several independent business capabilities:

- Identity and authentication
- User management
- Vehicle management
- Parking inventory
- Reservations
- Payments
- Notifications
- Gate/access control
- Real-time occupancy

ParkLink models these capabilities as independently deployable services communicating through synchronous APIs and asynchronous integration events.

The project focuses not only on implementing business functionality, but also on the engineering challenges that arise when building distributed systems.

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

