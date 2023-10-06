# NServiceBus.Persistence.Sqlite

![Build](https://github.com/CHG-MERIDIAN/NServiceBus.Persistence.Sqlite/workflows/Build%20(and%20release)/badge.svg?branch=main)
[![NuGet Version](http://img.shields.io/nuget/v/CHG.NServiceBus.Persistence.Sqlite.svg?style=flat)](https://www.nuget.org/packages/CHG.NServiceBus.Persistence.Sqlite/) [![License](https://img.shields.io/badge/license-APACHE-blue.svg)](LICENSE)

[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=CHG-MERIDIAN_NServiceBus.Persistence.Sqlite&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=CHG-MERIDIAN_NServiceBus.Persistence.Sqlite)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=CHG-MERIDIAN_NServiceBus.Persistence.Sqlite&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=CHG-MERIDIAN_NServiceBus.Persistence.Sqlite)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=CHG-MERIDIAN_NServiceBus.Persistence.Sqlite&metric=security_rating)](https://sonarcloud.io/dashboard?id=CHG-MERIDIAN_NServiceBus.Persistence.Sqlite)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CHG-MERIDIAN_NServiceBus.Persistence.Sqlite&metric=bugs)](https://sonarcloud.io/dashboard?id=CHG-MERIDIAN_NServiceBus.Persistence.Sqlite)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CHG-MERIDIAN_NServiceBus.Persistence.Sqlite&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=CHG-MERIDIAN_NServiceBus.Persistence.Sqlite)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CHG-MERIDIAN_NServiceBus.Persistence.Sqlite&metric=coverage)](https://sonarcloud.io/dashboard?id=CHG-MERIDIAN_NServiceBus.Persistence.Sqlite)

NServiceBus persistence implementation for Sqlite

## Usage

Install the NuGet package `CHG.NServiceBus.Persistence.Sqlite`.

```CSharp
var persistence = endpointConfiguration.UsePersistence<SqlitePersistence>();
persistence.UseConnectionString("Data Source=file:SagaStore.db");
```
