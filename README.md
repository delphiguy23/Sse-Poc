# SSE-POC (Server-Sent Events Proof of Concept)

## Overview
This project demonstrates a real-time server-to-client communication implementation using Server-Sent Events (SSE) in ASP.NET Core. It showcases how to establish persistent connections between the server and multiple clients, enabling real-time updates and notifications.

## Features
- Real-time server-to-client communication using SSE
- Multiple channel support for different event streams
- Debug client interface with connection status and logs
- Pulse monitoring system
- Clean architecture with separation of concerns

## Technology Stack
- Backend: ASP.NET Core
- Frontend: HTML5, JavaScript (Vanilla)
- Communication Protocol: Server-Sent Events (SSE)

## Project Structure
- `Controllers/`
  - `SseController.cs`: Handles SSE endpoints and event publishing
- `Service/`
  - `SseService.cs`: Core SSE functionality and channel management
  - `EventPublisherService.cs`: Manages event publishing and subscriptions
  - `PulseService.cs`: Handles pulse monitoring functionality
- `Models/`
  - `MyEvent.cs`: Event data model
  - `PulseRecord.cs`: Pulse record data model

## API Endpoints

### SSE Stream Endpoint
```
GET /stream/{channelName}
```
Establishes an SSE connection for the specified channel.

### Event Publishing
```
POST /publish/{channel}
```
Publishes an event to the specified channel.

### Pulse Recording
```
POST /pulse
```
Records pulse data for monitoring.

## Getting Started

1. Clone the repository
2. Navigate to the project directory
3. Run the application:
   ```bash
   dotnet run
   ```
4. Connect to the SSE stream using your preferred client implementation

## Usage
- The SSE stream provides real-time updates from server to client
- Multiple clients can connect simultaneously to receive updates
- Events can be published through the API endpoints
- Built-in support for connection status monitoring and error handling

## Notes
- SSE connections are unidirectional (server to client only)
- The implementation includes automatic reconnection handling
- CORS headers are configured for local development

## Last Updated
2025-01-02
