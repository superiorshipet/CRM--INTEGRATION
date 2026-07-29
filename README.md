# LavvaMessaging

LavvaMessaging is a clean architecture-based .NET solution that integrates Infobip with a CRM system. It acts as a webhook receiver for incoming messages across multiple social channels (WhatsApp, Instagram, and Facebook Messenger), stores them locally, and forwards them to a designated CRM endpoint.

## Architecture

The project is structured into four main layers following Clean Architecture principles:

1. **LavvaMessaging.Domain**: Contains the core entities (Conversation, Message) and enums (MessageChannel, MessageDirection) and interface definitions for repositories.
2. **LavvaMessaging.Application**: Contains the business logic, DTOs, interfaces for external services, and CQRS commands (using MediatR).
3. **LavvaMessaging.Infrastructure**: Implements data access using Entity Framework Core (PostgreSQL), CRM HTTP client, and Infobip specific implementations.
4. **LavvaMessaging.Api**: The presentation layer containing the ASP.NET Core Web API controllers to receive webhooks from Infobip.

## Prerequisites

- .NET SDK (Compatible with the project's target framework)
- PostgreSQL Database
- Infobip Account (for receiving messages)
- A running CRM system with an endpoint to accept forwarded messages

## Setup and Configuration

1. **Database Configuration**
   Open `LavvaMessaging.Api/appsettings.json` and update the `DefaultConnection` string with your PostgreSQL credentials:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=lavva_messaging;Username=postgres;Password=YOUR_PASSWORD"
   }
   ```

2. **Infobip Configuration**
   In the same `appsettings.json`, update the `Infobip` section:
   ```json
   "Infobip": {
     "ApiKey": "YOUR_INFOBIP_API_KEY",
     "BaseUrl": "https://your-base-url.api.infobip.com",
     "WebhookVerifyToken": "YOUR_SECURE_TOKEN"
   }
   ```
   Note: The `WebhookVerifyToken` must match the `X-Webhook-Token` header you configure in your Infobip webhook settings.

3. **CRM Configuration**
   Update the `Crm` section with your CRM's base URL:
   ```json
   "Crm": {
     "BaseUrl": "https://your-crm-api.example.com"
   }
   ```

## Running Migrations

Before running the application, you need to create the database schema. Navigate to the Infrastructure project and run the following Entity Framework Core CLI command:

```bash
cd LavvaMessaging.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../LavvaMessaging.Api
dotnet ef database update --startup-project ../LavvaMessaging.Api
```
Alternatively, the API is configured to run migrations automatically on startup in development environments.

## Running the Application

Navigate to the API directory and run the project:

```bash
cd LavvaMessaging.Api
dotnet run
```
The API will start and Swagger UI will be available at `/swagger/index.html` (in Development mode).

## Webhook Endpoints

The application exposes three primary endpoints to receive inbound messages from Infobip. Configure these URLs in your Infobip portal:

- **WhatsApp**: `POST /api/webhooks/whatsapp`
- **Instagram**: `POST /api/webhooks/instagram`
- **Messenger**: `POST /api/webhooks/messenger`

All endpoints expect the Infobip SaaS/Omnichannel JSON payload format.

## Security

The endpoints are secured using a custom webhook verifier. Every request from Infobip must include a header:
`X-Webhook-Token: YOUR_SECURE_TOKEN`
If this header is missing or does not match the `WebhookVerifyToken` in `appsettings.json`, the API will return a 401 Unauthorized response.
