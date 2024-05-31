// -----------------------------------------------------------------------
// <copyright file="RequestExtensionsTests.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

using CssWebApi.CssWebApi.Extensions;
using CssWebApi.CssWebApi.Infrastructure;
using JackHenry.CSS.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CssWebApi.Tests.Extensions
{
    public class RequestExtensionsTests
    {
        [Fact]
        public async Task GetValidatedRequestAsync_ShouldReturnValidRequest_WhenRequestIsValid()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.RequestServices = new ServiceCollection().BuildServiceProvider();
            var model = new TestRequest(true);

            context.Request.Body = new MemoryStream();

            await JsonSerializer.SerializeAsync(context.Request.Body, model, TestRequestSerializerContext.Default.TestRequest);
            context.Request.Body.Position = 0;
            context.Request.ContentType = "application/json";

            // Act
            var (isValid, request, error) = await context.Request.GetValidatedRequestAsync<TestRequest>(CancellationToken.None);

            // Assert
            Assert.True(isValid);
            Assert.NotNull(request);
            Assert.Null(error);
        }

        [Fact]
        public async Task GetValidatedRequestAsync_ShouldReturnError_WhenRequestIsInvalid()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.RequestServices = new ServiceCollection().BuildServiceProvider();
            var model = new TestRequest(false);

            context.Request.Body = new MemoryStream();

            await JsonSerializer.SerializeAsync(context.Request.Body, model, TestRequestSerializerContext.Default.TestRequest);
            context.Request.Body.Position = 0;
            context.Request.ContentType = "application/json";

            // Act
            var (isValid, request, error) = await context.Request.GetValidatedRequestAsync<TestRequest>(CancellationToken.None);

            // Assert
            Assert.False(isValid);
            Assert.Null(request);
            Assert.NotNull(error);
        }

        [Fact]
        public async Task GetValidatedRequestAsync_ShouldReturnError_WithDeserializationError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.RequestServices = new ServiceCollection().BuildServiceProvider();
            var model = new TestRequest(false);

            context.Request.Body = new MemoryStream();

            await JsonSerializer.SerializeAsync(context.Request.Body, model, TestRequestSerializerContext.Default.TestRequest);
            context.Request.Body.Position = 0;

            // Act
            var (isValid, request, error) = await context.Request.GetValidatedRequestAsync<TestRequest>(CancellationToken.None);

            // Assert
            Assert.False(isValid);
            Assert.Null(request);
            Assert.NotNull(error);
        }

        [Fact]
        public async Task GetValidatedRequestAsync_ShouldReturnError_WithDeserializationException()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.RequestServices = new ServiceCollection().BuildServiceProvider();
            var model = new TestRequest(false);

            context.Request.Body = new MemoryStream();
            context.Request.ContentType = "application/json";

            await JsonSerializer.SerializeAsync(context.Request.Body, model, TestRequestSerializerContext.Default.TestRequest);
            context.Request.Body.Position = 0;

            // Act
            var (isValid, request, error) = await context.Request.GetValidatedRequestAsync<IValidatableRequest>(CancellationToken.None);

            // Assert
            Assert.False(isValid);
            Assert.Null(request);
            Assert.NotNull(error);
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public record TestRequest(bool Valid) : IValidatableRequest
#pragma warning restore SA1402 // File may only contain a single type
    {
        public (bool IsValid, StandardErrorHttpResult? Error) Validate(HttpRequest httpRequest)
        {
            if (this.Valid)
            {
                return (true, null);
            }

            return (false, StandardErrorResults.BadRequest(["Model is invalid."]));
        }
    }

    [ExcludeFromCodeCoverage]
    [JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(TestRequest))]
#pragma warning disable SA1402 // File may only contain a single type
    public partial class TestRequestSerializerContext : JsonSerializerContext
#pragma warning restore SA1402 // File may only contain a single type
    {
    }
}
