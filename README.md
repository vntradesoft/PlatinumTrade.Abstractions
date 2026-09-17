# Pt.Okx.Abstractions

[![NuGet Version](https://img.shields.io/nuget/v/Pt.Okx.Abstractions.svg?style=flat-square)](https://www.nuget.org/packages/Pt.Okx.Abstractions)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Pt.Okx.Abstractions.svg?style=flat-square)](https://www.nuget.org/packages/Pt.Okx.Abstractions)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-purple.svg?style=flat-square)](https://dotnet.microsoft.com/)

**`Pt.Okx.Abstractions`** defines interface contracts, service abstractions, and extensible plugin hooks for the **Platinum Trade** algorithmic trading platform and OKX exchange ecosystem.

By depending on pure abstractions rather than concrete engine implementations, developers can build decoupled strategy plugins, custom order routers, automated indicators, mock services for unit tests, and headless trading daemons.

---

## Features

- **Decoupled Service Contracts**: Completely separated from heavy exchange client implementations or platform UI code.
- **Multi-Targeting**: Supports **.NET 8.0**, **.NET 9.0**, and **.NET 10.0**.
- **Interface Coverage**:
  - **Exchange Clients**: `IOkxClient`, `ITradeClient`, `IAccountClient`, `IInstrumentClient`, `ITimeSeriesClient`.
  - **Trading Strategies**: `IStrategy`, `IStrategyLogger`, `IStrategyStateStore`, `IStrategyPlugin`, `IInputParamManager`, `InputSchema`.
  - **Indicators**: `IIndicator`, `IIndicatorBuffer`, `CalcIndBase`, `IIndicatorFactory`, `IIndicatorManager`, `IIndicatorPlugin`.
  - **Chart Drawing**: `IDrawingManager`, `NullDrawingManager`.
  - **Storage & Paths**: `IStoragePathProvider`.
  - **Notifications**: `ITelegramCommandExtension`.
- **First-Class Unit Testing & Mocking**: Easily mock any trading operation, market feed, or indicator buffer in automated tests.

---

## Installation

Install via the .NET CLI:

```bash
dotnet add package Pt.Okx.Abstractions
```

Or via the Visual Studio Package Manager Console:

```powershell
Install-Package Pt.Okx.Abstractions
```

Or reference directly in your `.csproj`:

```xml
<PackageReference Include="Pt.Okx.Abstractions" Version="0.12.0-beta.2" />
```

> **Note**: `Pt.Okx.Abstractions` automatically brings in its required companion package [`Pt.Okx.Shared`](https://www.nuget.org/packages/Pt.Okx.Shared) (data contracts, enums, and models).

---

## Quick Start & Usage Examples

### 1. Consuming `ITradeClient` via Dependency Injection

```csharp
using Microsoft.Extensions.Logging;
using Pt.Okx.Abstractions.Clients;
using Pt.Okx.Shared.Clients.Trading.Models;
using Pt.Okx.Shared.Common;
using Pt.Okx.Shared.Enums;

public sealed class AutomatedOrderManager
{
    private readonly ITradeClient _tradeClient;
    private readonly ILogger<AutomatedOrderManager> _logger;

    public AutomatedOrderManager(ITradeClient tradeClient, ILogger<AutomatedOrderManager> logger)
    {
        _tradeClient = tradeClient;
        _logger = logger;
    }

    public async Task<string?> PlaceLimitBuyAsync(string symbol, decimal price, decimal quantity, CancellationToken ct)
    {
        var request = new OrderPlaceRequest
        {
            InstrumentId = symbol,
            TradeMode = TradeMode.Cross,
            Side = OrderSide.Buy,
            OrderType = OrderType.Limit,
            Price = price,
            Quantity = quantity
        };

        ApiResult<OrderPlaceResponse> response = await _tradeClient.PlaceOrderAsync(request, ct);
        if (!response.Success)
        {
            _logger.LogError("Failed to place order: {Message}", response.Error?.Message);
            return null;
        }

        return response.Data?.OrderId;
    }
}
```

### 2. Mocking Contracts in Unit Tests (e.g. with Moq)

```csharp
using Moq;
using NUnit.Framework;
using Pt.Okx.Abstractions.Clients;
using Pt.Okx.Shared.Clients.Trading.Models;
using Pt.Okx.Shared.Common;

[TestFixture]
public class AutomatedOrderManagerTests
{
    [Test]
    public async Task PlaceLimitBuy_Success_ReturnsOrderId()
    {
        // Arrange
        var mockTradeClient = new Mock<ITradeClient>();
        mockTradeClient
            .Setup(c => c.PlaceOrderAsync(It.IsAny<OrderPlaceRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult<OrderPlaceResponse>.Ok(new OrderPlaceResponse { OrderId = "test-ord-123" }));

        var manager = new AutomatedOrderManager(mockTradeClient.Object, Mock.Of<ILogger<AutomatedOrderManager>>());

        // Act
        var orderId = await manager.PlaceLimitBuyAsync("BTC-USDT-SWAP", 65000m, 1.0m, CancellationToken.None);

        // Assert
        Assert.That(orderId, Is.EqualTo("test-ord-123"));
    }
}
```

---

## Architecture Context

```
┌───────────────────────────────────────────────────────────┐
│                    Pt.Okx.Sdk                             │
│       (Plugin Developer SDK, StrategyBase, CalcIndBase)    │
└──────────────┬─────────────────────────────┬──────────────┘
               │                             │
               ▼                             │
┌─────────────────────────────┐              │
│    Pt.Okx.Abstractions      │              │
│ (IOkxClient, IStrategy, ...)│              │
└──────────────┬──────────────┘              │
               │                             │
               ▼                             ▼
┌───────────────────────────────────────────────────────────┐
│                    Pt.Okx.Shared                          │
│        (Pure Enums, Data Contracts, DTOs & Models)        │
└───────────────────────────────────────────────────────────┘
```

- **`Pt.Okx.Abstractions`** depends on **`Pt.Okx.Shared`** and **`Microsoft.Extensions.DependencyInjection.Abstractions`**.
- Platform engines (`Pt.Okx.Core`, `Pt.Okx.Bot`, `Pt.Okx.Gui`) and public plugins (`Pt.Okx.Sdk`) program strictly against these abstractions.

---

## Repository & Contributing

- **Source Code**: [https://github.com/vntradesoft/PlatinumTrade.Abstractions](https://github.com/vntradesoft/PlatinumTrade.Abstractions)
- **Data Contracts**: [https://github.com/vntradesoft/PlatinumTrade.Shared](https://github.com/vntradesoft/PlatinumTrade.Shared)
- **Main SDK**: [https://github.com/vntradesoft/PlatinumTrade.Sdk](https://github.com/vntradesoft/PlatinumTrade.Sdk)
- **Documentation**: [https://docs.platinumtrade.io](https://docs.platinumtrade.io)

## License

This project is licensed under the [MIT License](LICENSE).
