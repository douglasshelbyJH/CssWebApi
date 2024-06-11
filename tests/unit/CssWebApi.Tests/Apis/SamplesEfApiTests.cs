// -----------------------------------------------------------------------
// <copyright file="SamplesEfApiTests.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CssWebApi.CssWebApi.Apis;
using CssWebApi.CssWebApi.Features.EfSample;
using CssWebApi.CssWebApi.Features.EfSample.EfCore.Entities;
using CssWebApi.CssWebApi.Features.EfSample.Models;

using JackHenry.CSS.AspNetCore;
using JackHenry.CSS.Jxr.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

using Moq;

namespace CssWebApi.Tests.Apis
{
    public class SamplesEfApiTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ISampleEfRepository> mockSampleRepository;
        private readonly Mock<IJxrContext> mockJxrContext;
        private readonly Mock<ILogger<SampleEfServices>> mockLogger;

        private readonly SampleEfServices sampleServices;
        private readonly System.Text.Json.JsonSerializerOptions serializerOptions = new(System.Text.Json.JsonSerializerDefaults.Web);

        public SamplesEfApiTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.mockSampleRepository = this.mockRepository.Create<ISampleEfRepository>();
            this.mockJxrContext = this.mockRepository.Create<IJxrContext>();
            this.mockLogger = this.mockRepository.Create<ILogger<SampleEfServices>>();

            this.sampleServices = new SampleEfServices(this.mockSampleRepository.Object, this.mockJxrContext.Object, this.mockLogger.Object);
        }

        [Fact]
        public async Task GetSampleAsync_ReturnsOk_WhenSampleExists()
        {
            // Arrange
            var sampleId = Guid.NewGuid().ToString();
            var institutionUniversalId = Guid.NewGuid().ToString();

            using var cts = new CancellationTokenSource();

            this.mockSampleRepository.Setup(s => s.FindAsync(Guid.Parse(sampleId), cts.Token))
                .ReturnsAsync(
                    new SampleEfEntity { Id = Guid.Parse(sampleId), Name = "Test", InstitutionUniversalId = institutionUniversalId });

            // Act
            Results<Ok<SampleEfModel>, NotFound> result = await SamplesEfApi.GetSampleAsync(sampleId, this.sampleServices, cts.Token);

            // Assert
            Ok<SampleEfModel> response = Assert.IsType<Ok<SampleEfModel>>(result.Result);
            Assert.NotNull(response.Value);
            Assert.Equal(Guid.Parse(sampleId), response.Value.Id);
            Assert.Equal("Test", response.Value.Name);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetSampleAsync_ReturnsNotFound_WhenSampleDoesNotExist()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            var sampleId = Guid.NewGuid().ToString();
            this.mockSampleRepository.Setup(s => s.FindAsync(Guid.Parse(sampleId), cts.Token))
                .ReturnsAsync((SampleEfEntity?)null);

            // Act
            Results<Ok<SampleEfModel>, NotFound> result = await SamplesEfApi.GetSampleAsync(sampleId, this.sampleServices, cts.Token);

            // Assert
            Assert.IsType<NotFound>(result.Result);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetSampleAsync_ReturnsNotFound_WhenIdIsInvalid()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            var sampleId = "test";

            // Act
            Results<Ok<SampleEfModel>, NotFound> result = await SamplesEfApi.GetSampleAsync(sampleId, this.sampleServices, cts.Token);

            // Assert
            Assert.IsType<NotFound>(result.Result);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateSampleAsync_ReturnsCreatedResult_WhenRequestIsValid()
        {
            // Arrange
            var institutionUniversalId = Guid.NewGuid().ToString();
            var id = Guid.NewGuid();
            var createRequestEfModel = new CreateRequestEfModel("Test") { InstitutionUniversalId = institutionUniversalId };

            DefaultHttpContext httpContext = new();
            var stream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(stream, createRequestEfModel, this.serializerOptions);
            stream.Position = 0;
            httpContext.Request.ContentLength = stream.Length;
            httpContext.Request.ContentType = "application/json";
            httpContext.Request.Body = stream;

            using var cts = new CancellationTokenSource();

            this.mockSampleRepository.Setup(r => r.AddAsync(It.Is<SampleEfEntity>(e => IsValidEntity(e, createRequestEfModel.Name, institutionUniversalId)), cts.Token))
                .ReturnsAsync(new SampleEfEntity { Id = id, Name = createRequestEfModel.Name });

            this.mockJxrContext.Setup(j => j.InstitutionUniversalId).Returns(institutionUniversalId);

            // Act
            Results<Created<CreateResponseEfModel>, StandardErrorHttpResult> result = await SamplesEfApi.CreateSampleAsync(httpContext, this.sampleServices, cts.Token);

            // Assert
            Created<CreateResponseEfModel> response = Assert.IsType<Created<CreateResponseEfModel>>(result.Result);
            Assert.Equal(id, response.Value?.Id);
            this.mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        public async Task CreateSampleAsync_ReturnsBadRequest_WhenRequestIsInValid(string? name)
        {
            // Arrange
            var institutionUniversalId = Guid.NewGuid().ToString();
            var createRequestEfModel = new CreateRequestEfModel(name) { InstitutionUniversalId = institutionUniversalId };

            DefaultHttpContext httpContext = new();
            var stream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(stream, createRequestEfModel, this.serializerOptions);
            stream.Position = 0;
            httpContext.Request.ContentLength = stream.Length;
            httpContext.Request.ContentType = "application/json";
            httpContext.Request.Body = stream;

            using var cts = new CancellationTokenSource();

            // Act
            Results<Created<CreateResponseEfModel>, StandardErrorHttpResult> result = await SamplesEfApi.CreateSampleAsync(httpContext, this.sampleServices, cts.Token);

            // Assert
            StandardErrorHttpResult error = Assert.IsType<StandardErrorHttpResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, error.StatusCode);
            Assert.NotNull(error.StandardError);
            Assert.NotNull(error.StandardError.Details);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateSampleAsync_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var sampleId = Guid.NewGuid().ToString();
            var institutionUniversalId = Guid.NewGuid().ToString();
            var updateRequestEfModel = new UpdateRequestEfModel("Test") { InstitutionUniversalId = institutionUniversalId };
            DefaultHttpContext httpContext = new();
            var stream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(stream, updateRequestEfModel, this.serializerOptions);
            stream.Position = 0;
            httpContext.Request.ContentLength = stream.Length;
            httpContext.Request.ContentType = "application/json";
            httpContext.Request.Body = stream;

            using var cts = new CancellationTokenSource();

            this.mockSampleRepository.Setup(s => s.UpdateAsync(It.Is<SampleEfEntity>(e => IsValidEntity(e, updateRequestEfModel.Name, institutionUniversalId)), cts.Token))
                .ReturnsAsync(true);

            this.mockJxrContext.Setup(j => j.InstitutionUniversalId).Returns(institutionUniversalId);

            // Act
            Results<NoContent, NotFound, StandardErrorHttpResult> result = await SamplesEfApi.UpdateSampleAsync(sampleId, httpContext, this.sampleServices, cts.Token);

            // Assert
            Assert.IsType<NoContent>(result.Result);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateSampleAsync_ReturnsNotFound_WhenIdIsInvalid()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            var sampleId = "test";

            DefaultHttpContext httpContext = new();

            // Act
            Results<NoContent, NotFound, StandardErrorHttpResult> result = await SamplesEfApi.UpdateSampleAsync(sampleId, httpContext, this.sampleServices, cts.Token);

            // Assert
            Assert.IsType<NotFound>(result.Result);
            this.mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        public async Task UpdateSampleAsync_ReturnsBadRequest_WhenInvalidRequest(string? name)
        {
            // Arrange
            var sampleId = Guid.NewGuid().ToString();
            var institutionUniversalId = Guid.NewGuid().ToString();
            var updateRequestEfModel = new UpdateRequestEfModel(name) { InstitutionUniversalId = institutionUniversalId };
            DefaultHttpContext httpContext = new();
            var stream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(stream, updateRequestEfModel, this.serializerOptions);
            stream.Position = 0;
            httpContext.Request.ContentLength = stream.Length;
            httpContext.Request.ContentType = "application/json";
            httpContext.Request.Body = stream;

            using var cts = new CancellationTokenSource();

            // Act
            Results<NoContent, NotFound, StandardErrorHttpResult> result = await SamplesEfApi.UpdateSampleAsync(sampleId, httpContext, this.sampleServices, cts.Token);

            // Assert
            StandardErrorHttpResult error = Assert.IsType<StandardErrorHttpResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, error.StatusCode);
            Assert.NotNull(error.StandardError);
            Assert.NotNull(error.StandardError.Details);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteSampleAsync_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            var sampleId = Guid.NewGuid().ToString();
            using var cts = new CancellationTokenSource();
            this.mockSampleRepository.Setup(s => s.DeleteAsync(Guid.Parse(sampleId), cts.Token))
                .ReturnsAsync(true);

            // Act
            Results<NoContent, NotFound> result = await SamplesEfApi.DeleteSampleAsync(sampleId, this.sampleServices, cts.Token);

            // Assert
            Assert.IsType<NoContent>(result.Result);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteSampleAsync_ReturnsNotFound_WhenIdIsInvalid()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            var sampleId = "test";

            // Act
            Results<NoContent, NotFound> result = await SamplesEfApi.DeleteSampleAsync(sampleId, this.sampleServices, cts.Token);

            // Assert
            Assert.IsType<NotFound>(result.Result);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task SearchSamplesAsync_ReturnsOk_WhenSearchIsSuccessful()
        {
            // Arrange
            var sampleId = Guid.NewGuid().ToString();
            var institutionUniversalId = Guid.NewGuid().ToString();

            var searchRequestModel = new SearchRequestEfModel("Test");
            DefaultHttpContext httpContext = new();
            var stream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(stream, searchRequestModel, this.serializerOptions);
            stream.Position = 0;
            httpContext.Request.ContentLength = stream.Length;
            httpContext.Request.ContentType = "application/json";
            httpContext.Request.Body = stream;

            var offset = 0;
            var count = 10;

            httpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "offset", offset.ToString() },
                { "count", count.ToString() }
            });

            using var cts = new CancellationTokenSource();

            var paging = new JackHenry.CSS.Jxr.Models.PagingModel
            {
                NextOffset = null,
                Results = 1,
                Total = 1
            };

            this.mockSampleRepository.Setup(s => s.SearchSampleAsync(
                It.Is<SearchRequestEfEntity>(e => e.InstitutionUniversalId == institutionUniversalId && e.Name == "Test"),
                offset,
                count,
                cts.Token))
                .ReturnsAsync(
                    new SearchResponseEfEntity
                    {
                        Paging = paging,
                        Samples =
                        [
                            new SampleEfEntity { Id = Guid.Parse(sampleId), Name = searchRequestModel.Name, InstitutionUniversalId = institutionUniversalId }
                        ]
                    });

            this.mockJxrContext.Setup(j => j.InstitutionUniversalId).Returns(institutionUniversalId);

            // Act
            Results<Ok<SearchResponseEfModel>, NotFound, StandardErrorHttpResult> result = await SamplesEfApi.SearchSamplesAsync(httpContext, this.sampleServices, cts.Token);

            // Assert
            Ok<SearchResponseEfModel> response = Assert.IsType<Ok<SearchResponseEfModel>>(result.Result);
            Assert.NotNull(response.Value?.Samples);
            Assert.Single(response.Value.Samples);
            Assert.Equal(Guid.Parse(sampleId), response.Value.Samples[0].Id);
            Assert.Equal("Test", response.Value.Samples[0].Name);
            Assert.Equivalent(paging, response.Value.Paging);
            this.mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        public async Task SearchSamplesAsync_ReturnsBadRequest_WhenSearchIsInvalid(string? name)
        {
            // Arrange
            var searchRequestModel = new SearchRequestEfModel(name);
            DefaultHttpContext httpContext = new();
            var stream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(stream, searchRequestModel, this.serializerOptions);
            stream.Position = 0;
            httpContext.Request.ContentLength = stream.Length;
            httpContext.Request.ContentType = "application/json";
            httpContext.Request.Body = stream;

            using var cts = new CancellationTokenSource();

            // Act
            Results<Ok<SearchResponseEfModel>, NotFound, StandardErrorHttpResult> result = await SamplesEfApi.SearchSamplesAsync(httpContext, this.sampleServices, cts.Token);

            // Assert
            StandardErrorHttpResult error = Assert.IsType<StandardErrorHttpResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, error.StatusCode);
            Assert.NotNull(error.StandardError);
            Assert.NotNull(error.StandardError.Details);
            this.mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(0, null)]
        [InlineData(null, 1)]
        public async Task SearchSamplesAsync_ReturnsOk_WhenPagingIsInvalid(int? offset, int? count)
        {
            // Arrange
            var searchRequestModel = new SearchRequestEfModel("Test");
            DefaultHttpContext httpContext = new();
            var stream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(stream, searchRequestModel, this.serializerOptions);
            stream.Position = 0;
            httpContext.Request.ContentLength = stream.Length;
            httpContext.Request.ContentType = "application/json";
            httpContext.Request.Body = stream;

            httpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "offset", offset.ToString() },
                { "count", count.ToString() }
            });

            using var cts = new CancellationTokenSource();

            // Act
            Results<Ok<SearchResponseEfModel>, NotFound, StandardErrorHttpResult> result = await SamplesEfApi.SearchSamplesAsync(httpContext, this.sampleServices, cts.Token);

            // Assert
            StandardErrorHttpResult error = Assert.IsType<StandardErrorHttpResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, error.StatusCode);
            Assert.NotNull(error.StandardError);
            Assert.NotNull(error.StandardError.Details);
            this.mockRepository.VerifyAll();
        }

        private static bool IsValidEntity(SampleEfEntity entity, string name, string institutionUniversalId)
        {
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(name, entity.Name);
            Assert.Equal(institutionUniversalId, entity.InstitutionUniversalId);

            return true;
        }
    }
}
