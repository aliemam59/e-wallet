# 🏦 Core E-Wallet Engine (Enterprise API)

[![.NET API CI](https://github.com/aliemam59/e-wallet/actions/workflows/dotnet.yml/badge.svg)](https://github.com/aliemam59/e-wallet/actions)
![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)
![Architecture](https://img.shields.io/badge/Architecture-Modular%20Monolith-orange)

A robust, enterprise-grade financial backend built with **.NET 8**. This project demonstrates a production-ready E-Wallet system, incorporating advanced architectural patterns, high-performance caching, and asynchronous messaging.

## 🚀 Architectural Highlights

*   **Modular Monolith Architecture:** Clean separation of concerns between Identity, Wallet, and Transaction modules to ensure maintainability and scalability.
*   **Asynchronous Messaging (RabbitMQ):** Leveraged **MassTransit** to decouple time-consuming background tasks (like email confirmation and audit logging) from the main API thread, significantly reducing response times.
*   **High-Performance Caching (Redis):** Implemented the **Cache-Aside pattern** for balance inquiries, reducing database load and ensuring sub-millisecond data retrieval.
*   **Automated CI/CD Pipeline:** Integrated **GitHub Actions** to perform automated builds and execute **Unit Tests (xUnit & Moq)** on every push, ensuring zero-regression in financial business logic.
*   **Bank-Grade Security:** 
    *   **JWT Authentication:** Secure identity management and role-based access.
    *   **Rate Limiting:** Protects endpoints from DDoS and brute-force attacks using fixed-window policies.
*   **Container Orchestration:** Fully dockerized ecosystem for seamless "one-click" deployment across any environment.

## 🛠️ Technology Stack

*   **Backend:** ASP.NET Core Web API (.NET 8).
*   **Database:** SQL Server with Entity Framework Core.
*   **Caching:** Redis (Distributed Caching).
*   **Message Broker:** RabbitMQ via MassTransit.
*   **Testing:** xUnit, Moq, EF Core In-Memory.
*   **DevOps:** Docker, Docker Compose, GitHub Actions.

## ⚙️ Setup & Installation

Ensure you have **Docker Desktop** installed, then run:

```bash
# Clone the repository
git clone [https://github.com/aliemam59/e-wallet.git](https://github.com/aliemam59/e-wallet.git)
cd e-wallet

# Start the entire ecosystem
docker-compose up -d
