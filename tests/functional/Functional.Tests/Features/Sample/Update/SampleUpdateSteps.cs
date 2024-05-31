// -----------------------------------------------------------------------
// <copyright file="SampleUpdateSteps.cs" company="Jack Henry &amp; Associates, Inc.">
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

namespace Functional.Tests.Features.Sample.Update
{
    [Binding]
    internal class SampleUpdateSteps
    {
        private readonly string sampleName = "Name";
        private readonly string sampleNameUpdated = "UpdateName";

        private readonly SampleServiceClient client;
        private readonly ScenarioContext scenarioContext;
        private readonly HeaderProvider headerProvider;
        private bool disposedValue;

        public SampleUpdateSteps(HeaderProvider headerProvider, ScenarioContext scenarioContext)
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

        [Given(@"sample service has valid update test data")]
        public async Task SampleServiceHasValidDeleteTestData()
        {
            SampleCreateRequestModel sampleCreateRequest = new SampleCreateRequestModel { Name = this.sampleName };
            var headers = await this.headerProvider.GetHeaders();
            using var sampleCreateResponse = await this.client.CreateSampleAsync(sampleCreateRequest, headers);

            var rs = await sampleCreateResponse.Content.ReadFromJsonAsync<SampleCreateResponseModel>();

            this.scenarioContext["sampleId"] = rs?.Id.ToString();
        }

        [When(@"external service updates an existing sample")]
        public async Task ExternalServiceUpdatesAnExistingSample()
        {
            var headers = await this.headerProvider.GetHeaders();

            SampleUpdateRequestModel sampleUpdateRequest = new SampleUpdateRequestModel
            {
                Name = this.sampleNameUpdated,
                Id = Guid.Parse(this.scenarioContext.Get<string>("sampleId"))
            };

            this.scenarioContext["sampleUpdateResponse"] = await this.client.UpdateSampleAsync(sampleUpdateRequest, headers);
        }

        [Then(@"the sample should be updated")]
        public async Task TheSampleShouldBeUpdated()
        {
            using var sampleUpdateResponse = this.scenarioContext.Get<HttpResponseMessage>("sampleUpdateResponse");

            this.scenarioContext.Remove("sampleUpdateResponse");

            Assert.AreEqual(HttpStatusCode.NoContent, sampleUpdateResponse.StatusCode);

            var headers = await this.headerProvider.GetHeaders();
            using var rs = await this.client.GetSampleAsync(this.scenarioContext.Get<string>("sampleId"), headers);

            var model = await rs.Content.ReadFromJsonAsync<SampleModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(this.sampleNameUpdated, model?.Name);
        }

        [AfterScenario("sample-update")]
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

                    if (this.scenarioContext.ContainsKey("GetResponse"))
                    {
                        this.scenarioContext.Get<HttpResponseMessage>("GetResponse")?.Dispose();
                    }
                }

                this.disposedValue = true;
            }
        }
    }
}
