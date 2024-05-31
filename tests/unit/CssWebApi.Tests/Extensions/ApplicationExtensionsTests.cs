// -----------------------------------------------------------------------
// <copyright file="ApplicationExtensionsTests.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CssWebApi.CssWebApi.Extensions;
using CssWebApi.CssWebApi.Features.Sample;
using Google.Cloud.Spanner.Data;
using JackHenry.CSS.AspNetCore.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

using Moq;

namespace CssWebApi.Tests.Extensions
{
    public class ApplicationExtensionsTests
    {
        [Fact]
        public void AddApplicationServices_RegistersExpectedServices()
        {
            // Arrange
            var services = new ServiceCollection();

            var config = new ConfigurationManager()
                .AddInMemoryCollection(
                new List<KeyValuePair<string, string?>>
                {
                    new KeyValuePair<string, string?>("ConnectionStrings:Spanner", string.Empty),
                }).Build();
            services.AddLogging();
            services.AddSingleton<IConfiguration>(config);
            services.AddJxrContext();

            IHostEnvironment env = new HostingEnvironment { EnvironmentName = Environments.Development };

            var builder = Mock.Of<IHostApplicationBuilder>(b => b.Services == services && b.Configuration == config && b.Environment == env);

            builder.AddApplicationServices();
            var servicesProvider = services.BuildServiceProvider();

            // Assert
            // Verify that services are added
            Assert.Contains(services, s => s.ServiceType == typeof(SampleServices) && s.Lifetime == ServiceLifetime.Scoped);
            Assert.Contains(services, s => s.ServiceType == typeof(ISampleRepository) && s.Lifetime == ServiceLifetime.Scoped);

            var repository = servicesProvider.GetRequiredService<ISampleRepository>();

            Assert.IsAssignableFrom<SampleSpannerRepository>(repository);
            Assert.Contains(services, s => s.ServiceType == typeof(SpannerConnection) && s.Lifetime == ServiceLifetime.Transient);
        }
    }
}
