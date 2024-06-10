// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CssWebApi.CssWebApi.Apis;
using CssWebApi.CssWebApi.Constants;
using CssWebApi.CssWebApi.Extensions;
using JackHenry.CSS.AspNetCore.Extensions;
using JackHenry.CSS.AspNetCore.Middlewares;
using Serilog;
using Serilog.Events;

Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.Console()
            .CreateBootstrapLogger();

try
{
    WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);

    builder.AddServiceDefaults();

    builder.AddApplicationServices();

    WebApplication app = builder.Build();
    app.UseCSSExceptionHandler();
    app.MapDefaultEndpoints();

    app.UseMiddleware<ObservabilityMiddleware>();
    app.UseMiddleware<ResponseHeadersMiddleware>(Service.ProductId);
    app.UseMiddleware<JxrContextMiddleware>();

    app.MapGroup("/v1/{InstitutionUniversalId}/samples")
        .MapSamplesApi()
        .RequireAuthorization();

    app.MapGroup("/v1/{InstitutionUniversalId}/samples-ef")
        .MapSamplesEfApi();

    app.Run();
}
catch (Exception ex) when (ex.GetType().FullName is not "Microsoft.Extensions.Hosting.HostFactoryResolver+HostingListener+StopTheHostException") // handle ef cli exceptions https://github.com/dotnet/efcore/issues/28478#issuecomment-1196245359
{
    Serilog.Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Serilog.Log.CloseAndFlush();
}
