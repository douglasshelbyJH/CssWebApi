// -----------------------------------------------------------------------
// <copyright file="SampleServiceClient.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Net.Http.Json;
using System.Text.Json;

using Functional.Tests.Domain.Request;

using Microsoft.AspNetCore.WebUtilities;

namespace Functional.Tests.Support
{
    public class SampleServiceClient : IDisposable
    {
        private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new(JsonSerializerDefaults.Web);
        private readonly string samplesEndpoint = "samples";

        private readonly HttpClient httpClient;
        private bool disposedValue;

        public SampleServiceClient(string baseUrl)
        {
            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new Uri(baseUrl);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<HttpResponseMessage> GetSampleAsync(string sampleId, Dictionary<string, string> headers)
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{this.samplesEndpoint}/{sampleId}");

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            return await this.httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> CreateSampleAsync(SampleCreateRequestModel sample, Dictionary<string, string> headers)
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this.samplesEndpoint);

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            request.Content = JsonContent.Create(sample, options: DefaultJsonSerializerOptions);

            return await this.httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> UpdateSampleAsync(SampleUpdateRequestModel sample, Dictionary<string, string> headers)
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{this.samplesEndpoint}/{sample.Id}");

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            request.Content = JsonContent.Create(sample, options: DefaultJsonSerializerOptions);

            return await this.httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> DeleteSampleAsync(string sampleId, Dictionary<string, string> headers)
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{this.samplesEndpoint}/{sampleId}");

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            return await this.httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> SearchSamplesAsync(SampleSearchRequestModel searchBody, string offset, int count, Dictionary<string, string> headers)
        {
            var uri = QueryHelpers.AddQueryString(
                $"{this.samplesEndpoint}/search",
                new List<KeyValuePair<string, string?>>
                {
                    new("offset", offset),
                    new("count", count.ToString())
                });

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            request.Content = JsonContent.Create(searchBody, options: DefaultJsonSerializerOptions);

            return await this.httpClient.SendAsync(request);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.httpClient.Dispose();
                }

                this.disposedValue = true;
            }
        }
    }
}
