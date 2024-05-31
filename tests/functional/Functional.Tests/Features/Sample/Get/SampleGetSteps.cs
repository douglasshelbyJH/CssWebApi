// -----------------------------------------------------------------------
// <copyright file="SampleGetSteps.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;

using Functional.Tests.Domain.Request;
using Functional.Tests.Domain.Response;
using Functional.Tests.Support;
using NUnit.Framework;

namespace Functional.Tests.Features.Sample.Search
{
    [Binding]
    internal class SampleGetSteps : IDisposable
    {
        private readonly string sampleName = "GetSample";
        private readonly SampleServiceClient client;
        private readonly HeaderProvider headerProvider;

        private readonly ScenarioContext scenarioContext;
        private bool disposedValue;

        public SampleGetSteps(ScenarioContext scenarioContext, HeaderProvider headerProvider)
        {
            string baseUrl = TestContext.Parameters.Get("BaseUrl", string.Empty);
            string institutionUniversalId = TestContext.Parameters.Get("InstitutionUniversalId", string.Empty);
            this.headerProvider = headerProvider;
            this.client = new SampleServiceClient(string.Format(baseUrl, institutionUniversalId));
            this.scenarioContext = scenarioContext;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [Given(@"a sample is available")]
        public async Task CreateSample()
        {
            SampleCreateRequestModel sampleCreateRequest = new SampleCreateRequestModel { Name = this.sampleName };
            var headers = await this.headerProvider.GetHeaders();
            using var sampleCreateResponse = await this.client.CreateSampleAsync(sampleCreateRequest, headers);

            var rs = await sampleCreateResponse.Content.ReadFromJsonAsync<SampleCreateResponseModel>();

            this.scenarioContext["sampleId"] = rs?.Id.ToString();
        }

        [When(@"api consumer gets valid sample")]
        public async Task WhenProxyApiConsumerGetsValidSampleServiceEndpoint()
        {
            var headers = await this.headerProvider.GetHeaders();
            var sampleId = this.scenarioContext.Get<string>("sampleId");

            this.scenarioContext["getResponse"] = await this.client.GetSampleAsync(sampleId, headers);
        }

        [Then(@"an expected response is returned")]
        public async Task ThenASuccessResponseIsReturned()
        {
            using var rs = this.scenarioContext.Get<HttpResponseMessage>("getResponse");

            this.scenarioContext.Remove("getResponse");

            Assert.AreEqual(HttpStatusCode.OK, rs.StatusCode);

            var model = await rs.Content.ReadFromJsonAsync<SampleModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(this.sampleName, model?.Name);
            Assert.AreEqual(Guid.Parse(this.scenarioContext.Get<string>("sampleId")), model?.Id);
        }

        [AfterScenario("@sample-get")]
        public async Task Cleanup()
        {
            // Cleanup the sample created in the scenario.
            var sampleId = this.scenarioContext.Get<string?>("sampleId");

            if (sampleId != null)
            {
                var headers = await this.headerProvider.GetHeaders();
                using var rs = await this.client.DeleteSampleAsync(sampleId, headers);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.client.Dispose();
                }

                this.disposedValue = true;
            }
        }
    }
}
