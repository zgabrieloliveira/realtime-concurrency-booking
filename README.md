# 🎟️ Real-Time Concurrency Booking System

> A high-performance ticket booking API engineered to handle **Race Conditions** and prevent overbooking using **Redis Distributed Locks** and **.NET 8**.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Redis](https://img.shields.io/badge/Redis-Distributed%20Lock-red?style=flat&logo=redis)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=flat&logo=docker)
![Architecture](https://img.shields.io/badge/Clean-Architecture-green)

## 🧠 The Challenge
In high-demand systems (like concert ticket sales or Black Friday deals), thousands of users attempt to book the specific same seat simultaneously. Standard database transactions often fail to prevent **Race Conditions** under heavy load, leading to **Overbooking** (selling the same inventory to multiple users).

## 💡 The Solution
This project implements a robust **Distributed Locking** mechanism (Pessimistic Locking) to ensure data consistency:

1.  **Redis Atomic Lock:** Uses Redis `SETNX` (Set if Not Exists) to enforce a strict lock on a specific seat key. Only one request can acquire the lock within the atomic execution window.
2.  **Clean Architecture:** Decoupled architecture separating Domain, Application, and Infrastructure layers.
3.  **Real-Time Feedback:** A background listener connects via **SignalR** to broadcast `LOCKED` and `SOLD` statuses instantly to all connected clients.

## 🛠️ Tech Stack
- **Backend:** .NET 8 (C#)
- **Database:** PostgreSQL (Entity Framework Core)
- **Cache & Locking:** Redis
- **Real-Time:** SignalR (Websockets)
- **Containerization:** Docker & Docker Compose
- **Testing:** Custom Console App for Stress/Load Testing

## 🏗️ Project Structure
The solution follows **Clean Architecture** principles:

- **`Api`**: REST Endpoints, Swagger, and SignalR Hubs.
- **`Application`**: Use Cases (`HoldSeat`, `BuySeat`, `GetSeats`) and Orchestration.
- **`Domain`**: Pure Entities (`Seat`, `Ticket`) and Business Exceptions.
- **`Infra`**: Repositories (EF Core) and Redis Cache Implementation.
- **`Listener`**: A separate console service acting as a real-time client (receives SignalR events).
- **`StressTest`**: A specialized console tool to simulate concurrent attacks.

## 🔌 Key Endpoints

- **GET /Api/Seats:** Lists all seats (Merges DB status + Redis Locks).
- **POST /Api/Seats/Hold:** Attempts to acquire a Redis Lock (TTL 10min).
- **POST /Api/Seats/Buy:** Confirms purchase and persists on PostgreSQL.


## ⚡ How to Run
The entire environment (API, Database, Redis, Worker) is containerized. You don't need to install .NET or Postgres locally.

1. After clone this repo, just run:

```bash
docker compose up --build
```

2. Access the API Documentation in: **http://localhost:5000/swagger**

## 💣 Stress Test
To validate the architecture, I built a `StressTest` application that fires 50 concurrent HTTP requests trying to reserve the exact same seat ID simultaneously. Here's the results:
- **Total Requests:** 50 concurrent threads.
- **Success (Lock Acquired):** 1 (Strictly enforced).
- **Blocked (Protected by Redis):** 49.
- **Execution Time:** ~600ms.
