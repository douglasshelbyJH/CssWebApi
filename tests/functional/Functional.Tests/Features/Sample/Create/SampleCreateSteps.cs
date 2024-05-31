// -----------------------------------------------------------------------
// <copyright file="SampleCreateSteps.cs" company="Jack Henry &amp; Associates, Inc.">
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

namespace Functional.Tests.Features.Sample.Create
{
    [Binding]
    internal class SampleCreateSteps : IDisposable
    {
        private readonly string sampleName = "createSample";
        private readonly ScenarioContext scenarioContext;
        private readonly SampleServiceClient client;
        private readonly HeaderProvider headerProvider;
        private bool disposedValue;

        public SampleCreateSteps(HeaderProvider headerProvider, ScenarioContext scenarioContext)
        {
            string baseUrl = TestContext.Parameters.Get("BaseUrl", string.Empty);
            string institutionUniversalId = TestContext.Parameters.Get("InstitutionUniversalId", string.Empty);
            this.headerProvider = headerProvider;
            this.client = new SampleServiceClient(string.Format(baseUrl, institutionUniversalId));
            this.scenarioContext = scenarioContext;
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [When(@"service requests to create a valid sample")]
        public async Task CreateSample()
        {
            // Create a sample and save it to the scenarioContext for later assertions.
            SampleCreateRequestModel sampleCreateRequest = new SampleCreateRequestModel { Name = this.sampleName };
            var headers = await this.headerProvider.GetHeaders();
            this.scenarioContext["sampleCreateResponse"] = await this.client.CreateSampleAsync(sampleCreateRequest, headers);
        }

        [Then(@"a valid sample should be created")]
        public async Task AValidSampleShouldBeCreated()
        {
            // Get the sample by id.
            using var sampleCreateResponse = this.scenarioContext.Get<HttpResponseMessage>("sampleCreateResponse");

            this.scenarioContext.Remove("sampleCreateResponse");

            Assert.AreEqual(HttpStatusCode.Created, sampleCreateResponse.StatusCode);

            var rs = await sampleCreateResponse.Content.ReadFromJsonAsync<SampleCreateResponseModel>();

            Assert.IsNotNull(rs?.Id);
            this.scenarioContext["sampleId"] = rs?.Id.ToString();

            Assert.AreNotEqual(Guid.Empty, rs?.Id);
        }

        [AfterScenario("sample-create")]
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
