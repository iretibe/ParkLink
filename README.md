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

### Architecture

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

