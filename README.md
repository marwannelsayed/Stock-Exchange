# Stock Exchange API

This project is an ASP.NET Core Web API for managing stock exchange operations, including user authentication, stock trading, and real-time updates.

## Features

- **RESTful API Endpoints**
  - JWT Authentication
  - Stock portfolio management
  - Historical data queries

- **Real-time Capabilities**
  - WebSocket support via SignalR
  - Live stock price updates

- **Database Integration**
  - Entity Framework Core 8
  - SQLite for development
  - Automatic migrations

- **Background Services**
  - Periodic stock price updates
  - Market simulation engine

- **Developer Tools**
  - Swagger/OpenAPI documentation
  - Integrated testing endpoints
  - Detailed logging configuration

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Entity Framework Core tools:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### Environment Setup
1. Clone the repository
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Configure database connection in `appsettings.json`

### Database Setup
```bash
cd StockExchangeAPI
dotnet ef database update
```

### Running the Application
```bash
cd /Users/marwannelsayed/Desktop/M/Derayah/Stock-Exchange
dotnet run --project StockExchangeAPI/StockExchangeAPI.csproj
```

The API will be available at [http://localhost:5000](http://localhost:5000).

### API Documentation
Visit [http://localhost:5000/swagger](http://localhost:5000/swagger) for interactive API documentation using Swagger UI.
