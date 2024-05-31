// -----------------------------------------------------------------------
// <copyright file="ServiceDefaultExtensions.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

using JackHenry.CSS.AspNetCore.Extensions;
using JackHenry.CSS.AspNetCore.Handlers;
using JackHenry.CSS.Authentication.Extensions;
using JackHenry.Enterprise.Serilog;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Sinks.SystemConsole.Themes;

using static JackHenry.Enterprise.Serilog.Constants;

namespace CssWebApi.CssWebApi.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceDefaultExtensions
    {
        private static bool usePrometheus = true;

        public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
        {
            builder.ConfigureSerilog();

            builder.Services.AddHealthChecks();
            builder.Services.AddCSSObservability();
            builder.Services.AddJxrContext();

            builder.AddOtel();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.UseProductGatewayToken();
                    options.UseProductGatewayKey(builder.Configuration.GetSection("ProductGatewayKeyUrl").Value ?? string.Empty);
                    options.AddClaimsToSpan();
                });

            builder.Services.AddAuthorization();

            builder.Services.AddCSSExceptionHandler<StandardErrorUnhandledExceptionHandler>();

            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            app.MapHealthChecks("/health").AllowAnonymous();

            if (usePrometheus)
            {
                app.UsePrometheusEndpoint(app.Configuration.GetValue<string>("PrometheusScrapingPort"));
            }

            return app;
        }

        public static WebApplication UsePrometheusEndpoint(this WebApplication app, string? port = null)
        {
            if (int.TryParse(port, out int portNumber))
            {
                app.UseOpenTelemetryPrometheusScrapingEndpoint(
                    context => context.Request.Path == "/metrics"
                        && context.Connection.LocalPort == portNumber);

                app.MapWhen(
                 ctx => ctx.Connection.LocalPort == portNumber && ctx.Request.Path != "/metrics",
                 b => b.Run(ctx =>
                 {
                     ctx.Response.StatusCode = 404;
                     return Task.CompletedTask;
                 }));
            }
            else
            {
                app.UseOpenTelemetryPrometheusScrapingEndpoint();
            }

            return app;
        }

        public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog(configureLogger: (context, services, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration)
                .Enrich.WithProperty(Logging.Environment, context.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty(
                    Logging.IpAddress,
                    Array.Find(Dns.GetHostEntry(Dns.GetHostName()).AddressList, x => x.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? string.Empty).Enrich.WithSpan();

                if (context.HostingEnvironment.IsDevelopment())
                {
                    configuration.WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss.ffff} {Level:u3} {SourceContext}] {Message:lj} {Properties} {NewLine}{Exception}",
                        theme: AnsiConsoleTheme.Code).Enrich.FromLogContext();
                }
                else
                {
                    JxrSerilogProvider.InitializeLogger(context, services, configuration);
                }
            });

            return builder;
        }

        public static WebApplicationBuilder AddOtel(this WebApplicationBuilder builder)
        {
            var serviceName = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME");

            if (string.IsNullOrWhiteSpace(serviceName))
            {
                serviceName = Constants.Service.ServiceName;
            }

            var otelDisabled = builder.Configuration.GetValue<bool>("OTEL_SDK_DISABLED");

            var metricsDisabled = builder.Configuration.GetValue<bool>("MetricsDisabled");
            var prometheusExporter = builder.Configuration.GetValue<bool>("UsePrometheusExporter");

            var configurePrometheus = !otelDisabled && !metricsDisabled && prometheusExporter;
            usePrometheus = configurePrometheus;

            builder.Services.AddSingleton(_ => new ActivitySource(serviceName));

            if (!otelDisabled)
            {
                Sdk.SetDefaultTextMapPropagator(new CompositeTextMapPropagator(new TextMapPropagator[]
                {
                    new OpenTelemetry.Extensions.Propagators.B3Propagator(),
                    new TraceContextPropagator()
                }));

                var serviceVersion = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();

                var otelBuilder = builder.Services.AddOpenTelemetry()
                .WithTracing(traceBuilder =>
                {
                    traceBuilder.ConfigureResource(resourceBuilder => resourceBuilder.AddService(serviceName, Constants.Service.ProductId, serviceVersion))
                    .AddSource(serviceName)
                    .SetSampler(new AlwaysOnSampler())
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.Filter = httpContext =>
                        {
                            return httpContext.Request.Path.Value?.StartsWith("/v", StringComparison.OrdinalIgnoreCase) ?? false;
                        };
                    })
                    .AddHttpClientInstrumentation(options => options.RecordException = true)
                    .AddGrpcClientInstrumentation()
                    .AddOtlpExporter();
                });

                if (!metricsDisabled)
                {
                    otelBuilder.WithMetrics(metricsBuilder =>
                    {
                        metricsBuilder
                            .ConfigureResource(resourceBuilder => resourceBuilder.AddService(serviceName, Constants.Service.ProductId, serviceVersion))
                            .AddProcessInstrumentation()
                            .AddRuntimeInstrumentation()
                            .AddHttpClientInstrumentation()
                            .AddAspNetCoreInstrumentation();

                        if (configurePrometheus)
                        {
                            metricsBuilder.AddPrometheusExporter();
                        }
                        else
                        {
                            metricsBuilder.AddOtlpExporter();
                        }
                    });
                }
            }

            return builder;
        }
    }
}
