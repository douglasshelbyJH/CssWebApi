
# CssWebApi
This solution was created from the [Jack Henry CSS Web API Project Template](https://github.com/JHAECP/templates#readme) it contains a minimal api service, repository, and unit tests utilizing xUnit and Moq.

This solution utilizes the Artifactory NuGet Feed, see [documentation](https://jackhenry.visualstudio.com/DigitalCore/_wiki/wikis/DigitalCore.wiki/44980/Artifactory) for more information on connecting to this feed.

> Fix code marked with //TODO: comments before using the service. Some constant values need to be specified for your particular service, as well as logging properties in appsettings.json

> If using spanner, for local development there are a couple of options for specifying credentials 
Use [Application Default Credentials](https://cloud.google.com/docs/authentication/provide-credentials-adc) or you can specify a credentials file in the [connection string](https://cloud.google.com/dotnet/docs/reference/Google.Cloud.Spanner.Data/latest/connection_string#credentialfile).

> Spanner Samples DDL statement

```
    CREATE TABLE Samples (
    Id STRING(36),
    Name STRING(100),
    InstitutionUniversalId STRING(36),
    ETag STRING(36),
    ) PRIMARY KEY(Id);
```

## Solution Structure
- src
    - CssWebApi - .net core minimal web api project
- tests
    - CssWebApi.Tests - service tests

## Features
- .editorConfig 
    - Rules that align with analyzers
- Analyzers
    - [CSS Analyzers](https://github.com/JHAECP/analyzers#readme)
        - Stylecop Analyzers 
        - SonarQube Analyzers
- [JH-Api Patterns](https://github.com/JHAECP/asp-net-core#readme)
    - Headers
    - Route parameters
    - Standard Errors for response
    - Paging for searches
- [JxrContext](https://github.com/JHAECP/asp-net-core#readme)
- Unhandled Exception handler
- MongoDB
    - V3 GUID representation
    - CamelCaseElementNameConvention
    - Commands are logged to console in 'Development' environment
    - Client Tracing is enabled
- Logging
    - Serilog 
        - Text formatter in 'Development' environment
        - JackHenry Enterprise serilog formatter for datadog in all other environments
- Open Telemetry
    - Can be disabled for local development using `OTEL_SDK_DISABLED` configuration, or use [Jaeger and Prometheus docker compose](https://github.com/JHAECP/samples#otel-collector)
    - Middleware to include selected headers and/or route parameters in all log entries and tracing via `ObservabilityMiddleware` from [JackHenry.CSS.AspNetCore](https://github.com/JHAECP/asp-net-core#readme)
    - Tracing
        - Resolve `ActivitySource` to create internal spans [Tracing documentation](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-instrumentation-walkthroughs)
        - Uses defined [Configuration](https://opentelemetry.io/docs/concepts/sdk-configuration/)
        - Uses B3Propagator and TraceContext propagator due to NAG currently using B3 Multi
        - Uses OTLP Exporter
        - TraceId and SpanId added to Logging Context via [Span Enricher](https://github.com/RehanSaeed/Serilog.Enrichers.Span)
    - Metrics
        - Can be disabled indepently of tracing using `MetricsDisabled` configuration value
        - Can be configured to use Prometheus Exporter or OTLP Exporter with `UsePrometheusExporter` configuration or Env Variable
            - Prometheus Export can be exposed on a separate port by setting `PrometheusScrapingPort`, when null or empty it will use the same port as the service
- Supported IDE
    - Visual Studio
        - [Unit Test Boilerplate Generator](https://marketplace.visualstudio.com/items?itemName=RandomEngy.UnitTestBoilerplateGenerator) recommended extension 
    - Visual Studio Code
        - .code-workspace file
        - debugging, build, and test tasks
        - launch is configured to open swagger in browser
        - [.Net Test Explorer](https://marketplace.visualstudio.com/items?itemName=formulahendry.dotnet-test-explorer) recommended extension
- Unit Tests - Tests follow pattern used by [Unit Test Boilerplate Generator Extension](https://marketplace.visualstudio.com/items?itemName=RandomEngy.UnitTestBoilerplateGenerator)
    - xUnit
    - Moq
    - CodeCoverage.ps1 script can be used to view a code coverage html report that allows you to view covered and uncovered lines of code in your .cs files
- Functional Tests
    - Run via Test Explorer. Service must be running and connected to data store for tests to succeed. 
    - Functional tests require Local.runsettings file to be configured, and set as your Solution Wide runsettings file

## [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

Additional classes have been added using EF Core which correspond to the existing feature set.  ```.\Features\EfSample```

### Dependencies
#### Note: As of 2024-05-28 Google.Cloud.EntityFrameworkCore.Spanner v2.1.3 supports EF Core 6 only
- [EF Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
  - dotnet-tools.json manifest should perform a local install on restore</br>
  local install: ```dotnet tool install dotnet-ef```</br>
  global install: ```dotnet tool install --global dotnet-ef```
  - Run ```dotnet ef``` to confirm installation
- [EF Core Tools for Powershell](https://learn.microsoft.com/en-us/ef/core/cli/powershell)
    - Install from Package Manager Console</br>
    ```Install-Package Microsoft.EntityFrameworkCore.Tools```
- [Google Cloud CLI](https://cloud.google.com/sdk/gcloud)
    - Download and install the latest GCloud SDK</br>
    ```winget install Google.CloudSDK```
    - Create local credentials</br>
    ```gcloud auth application-default login```
    - Set the default gcloud project to the sandbox
    ```gcloud config set project sdb-dig-core-jheisessb977```

### EF Core CLI Commands for schema management
#### [Migrations Overview](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)
#### *Common commands*
#### NOTE: Execute from the project directory
#### List DbContexts in the project
```dotnet ef dbcontext list```
#### Create a migration
```dotnet ef migrations add InitialCreate --output-dir .\Features\EfSample\EfCore\Migrations```
#### Remove changes for the latest migration
```dotnet ef migrations remove```
#### Update database with migrations
```dotnet ef database update```
#### Revert the database to a specific migration
```dotnet ef database update InitialCreate```
#### Revert all database migrations
```dotnet ef database update 0```
