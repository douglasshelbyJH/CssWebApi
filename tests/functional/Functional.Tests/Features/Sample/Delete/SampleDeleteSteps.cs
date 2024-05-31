// -----------------------------------------------------------------------
// <copyright file="SampleDeleteSteps.cs" company="Jack Henry &amp; Associates, Inc.">
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

namespace Functional.Tests.Features.Sample.Delete
{
    [Binding]
    internal class SampleDeleteSteps
    {
        private readonly string sampleName = "deleteSample";
        private readonly ScenarioContext scenarioContext;
        private readonly SampleServiceClient client;
        private readonly HeaderProvider headerProvider;
        private bool disposedValue;

        public SampleDeleteSteps(HeaderProvider headerProvider, ScenarioContext scenarioContext)
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

        [Given(@"sample service has valid delete test data")]
        public async Task SampleServiceHasValidDeleteTestData()
        {
            SampleCreateRequestModel sampleCreateRequest = new SampleCreateRequestModel { Name = this.sampleName };
            var headers = await this.headerProvider.GetHeaders();
            using var sampleCreateResponse = await this.client.CreateSampleAsync(sampleCreateRequest, headers);

            var rs = await sampleCreateResponse.Content.ReadFromJsonAsync<SampleCreateResponseModel>();

            this.scenarioContext["sampleId"] = rs?.Id.ToString();
        }

        [When(@"external service deletes an existing sample")]
        public async Task ExternalServiceDeletesAnExistingSample()
        {
            var headers = await this.headerProvider.GetHeaders();
            this.scenarioContext["sampleDeleteResponse"] = await this.client.DeleteSampleAsync(this.scenarioContext.Get<string>("sampleId"), headers);
        }

        [Then(@"the sample should be deleted")]
        public void TheSampleShouldBeDeleted()
        {
            using var rs = this.scenarioContext.Get<HttpResponseMessage>("sampleDeleteResponse");

            this.scenarioContext.Remove("sampleDeleteResponse");

            Assert.AreEqual(HttpStatusCode.NoContent, rs.StatusCode);
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
