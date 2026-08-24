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

## Architecture

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

Each service owns its business logic and persistence boundary.

The system uses synchronous HTTP communication where immediate responses are required and asynchronous messaging for decoupled integration workflows.


## Technology Stack

| Area | Technology |
|---|---|
| **Runtime** | .NET 10 |
| **Language** | C# |
| **Application Framework** | ASP.NET Core |
| **Orchestration** | .NET Aspire |
| **ORM** | Entity Framework Core |
| **Database** | SQL Server |
| **Messaging** | RabbitMQ |
| **Message Bus** | MassTransit |
| **Caching** | Redis |
| **API** | REST / OpenAPI |
| **Logging** | Serilog |
| **Observability** | OpenTelemetry |
| **Containers** | Docker |
| **CI/CD** | GitHub Actions |
| **Authentication** | ASP.NET Identity / Duende |
| **API Documentation** | Swagger / OpenAPI |


# Core Services

## Identity Service

Responsible for:

- User authentication and identity management
- Authentication and authorization
- OAuth 2.0 / OpenID Connect
- Access token issuance and validation
- Role- and claims-based authorization
- External identity providers

**Status:** ✅ Implemented
> Who are you, and are you allowed to access the system?
---

## Users Service

Responsible for:

- User profile management
- User account information
- User lifecycle management
- User role and permission management
- User preferences and profile settings
- User-related business operations
- Integration with Identity and other platform services

**Status:** ✅ Implemented

---

## Vehicle Service

Responsible for:

- Vehicle registration and management
- Vehicle ownership and user association
- Vehicle profile and identification data
- Vehicle type and classification
- Vehicle-related business rules
- Vehicle lifecycle management

**Status:** ✅ Implemented

---

## Parking Service

Responsible for:

- Parking facility management
- Parking area and zone management
- Parking space management
- Parking space availability
- Parking space status and lifecycle
- Parking inventory management
- Parking-related business rules

**Status:** ✅ Implemented

---

## Reservation Service

Responsible for:

- Parking reservation creation
- Reservation holds
- Reservation lifecycle management
- Reservation status management
- Parking availability validation
- Reservation-related business rules
- Integration with payment workflows
- Integration with notification workflows

**Status:** 🚧 In Progress

---

## Payment Service

Responsible for:

- Payment initiation
- Payment transaction management
- Payment status management
- Payment lifecycle management
- Payment confirmation and failure handling
- Payment-related integration events
- External payment provider integration
- Payment reconciliation

**Status:** 🚧 In Progress

---

## Notification Service

Responsible for:

- Event-driven notification processing
- User notification management
- Email notifications
- SMS notifications
- Push notifications
- Notification templates
- Notification delivery status
- Integration with external notification providers

**Status:** 🚧 In Progress

---

## Gate Service

Responsible for:

- Parking gate management
- Gate access control
- Entry and exit processing
- Vehicle access validation
- Reservation-based access validation
- Gate access events
- RFID integration
- License plate recognition / OCR
- Real-time gate status
- Integration with parking and reservation services

**Status:** 🚧 In Progress

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

