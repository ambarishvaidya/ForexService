# ForexService

The ForexService is a powerful Spot Simulator service that utilizes the [FinancialMarketSimulator](https://github.com/ambarishvaidya/FinancialMarketSimulator) library. This project is built with ASP.NET and employs the SignalR technology to provide real-time market data updates to clients in JSON format.

The primary goal of this service is to enable users to simulate trading activities and interact with financial data. The service relies on custom subscriptions that consumers can configure to monitor specific currency pairs. These subscriptions include details such as the currency pair, initial values for bid, ask and spread, and the publish frequency in milliseconds. When a subscription is activated, the service initiates data publishing.

## Dependencies

The ForexService project relies on the following key dependencies:

- [FinancialMarketSimulator](https://github.com/ambarishvaidya/FinancialMarketSimulator)
- FluentValidation
- Microsoft.Extensions.Logging

## Key Features

### Custom Subscriptions

The ForexService empowers users to define their own custom currency pairs, complete with initial values and publishing frequencies. This flexibility allows users to tailor the service to their specific trading needs and preferences.

### Smart Validations

Invalid subscription requests are meticulously validated and rejected. This ensures that only accurate and meaningful data is added to the subscription pool, enhancing the overall quality of the service's outputs.

### Dynamic Control

Users can control the behavior of their subscriptions through intuitive operations such as Start, Stop, Pause, and Resume. These dynamic actions offer greater control over the market data being visualized and analyzed.

### Realistic Market Data

The Financial Market Data Simulator generates realistic market data using a random walk model. This model mimics the dynamics of real financial markets, providing users with accurate and immersive trading simulations.

### Efficient Data Generation

The library employs a PriceProducer that generates ticks within preconfigured PriceLimits for each ticker symbol. This efficient data generation process ensures a seamless and responsive experience for users interacting with the simulated market data.

## Working

-  Clone or download the solution, pull in the nuget packages..
-  Modify launchSettings.json for any port changes.
-  CORS is configured to accept all requests.
   ```csharp
   app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(origin => true));
   ```
-  SignalR Hub is configured at ForexService
   ```csharp
   app.MapHub<ForexHub>("/ForexService");
   ```
-  All other operations are mapped to
  ```csharp
  app.MapPost("/api/v1/<operation name>....
  ```
-  Build the project and run it.
-  Add subscription
  ```csharp
  <your configured URL>/api/v1/addsubscription
  ```
  JSON data for subscription
  ```json
  {
    "CurrencyPair": "USDINR",
    "Bid":87.123,
    "Ask":87.130,
    "Spread": 0.006,
    "PublishFrequencyInMs": 100
  }
  ```
