# NServiceBus.Persistence.Sqlite

NServiceBus persistence implementation for Sqlite

## Usage

Install the NuGet package `CHG.NServiceBus.Persistence.Sqlite`.

```CSharp
var persistence = endpointConfiguration.UsePersistence<SqlitePersistence>();
persistence.UseConnectionString("Data Source=file:SagaStore.db");
```
