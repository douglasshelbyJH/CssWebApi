// -----------------------------------------------------------------------
// <copyright file="SampleSearchSteps.cs" company="Jack Henry &amp; Associates, Inc.">
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
using TechTalk.SpecFlow;

namespace Functional.Tests.Features.Sample.Search
{
    [Binding]
    internal class SampleSearchSteps
    {
        private readonly List<string> sampleNames = ["searchSample1", "searchSample2", "searchSample3"];

        private readonly SampleServiceClient client;
        private readonly ScenarioContext scenarioContext;
        private readonly HeaderProvider headerProvider;
        private bool disposedValue;

        public SampleSearchSteps(HeaderProvider headerProvider, ScenarioContext scenarioContext)
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

        [Given(@"sample service has valid search test data")]
        public async Task SampleServiceHasValidSearchTestData()
        {
            List<string?> sampleIds = new List<string?>();

            foreach (var sampleName in this.sampleNames)
            {
                SampleCreateRequestModel sampleCreateRequest = new SampleCreateRequestModel { Name = sampleName };
                var headers = await this.headerProvider.GetHeaders();
                using var sampleCreateResponse = await this.client.CreateSampleAsync(sampleCreateRequest, headers);

                var rs = await sampleCreateResponse.Content.ReadFromJsonAsync<SampleCreateResponseModel>();

                sampleIds.Add(rs?.Id.ToString());
            }

            this.scenarioContext["sampleIds"] = sampleIds;
        }

        [When(@"external service searches for sample with name ""([^""]*)""")]
        public async Task ExternalServiceSearchesForSampleWithName(string sampleName)
        {
            // Searches on name received from feature
            SampleSearchRequestModel searchBody = new SampleSearchRequestModel();
            searchBody.Name = sampleName;
            var headers = await this.headerProvider.GetHeaders();
            this.scenarioContext["sampleSearchResponse"] = await this.client.SearchSamplesAsync(searchBody, "0", 10, headers);
        }

        [Then(@"sample with name ""([^""]*)"" should be returned")]
        public async Task SampleWithNameShouldBeReturned(string sampleName)
        {
            using var sampleSearchResponse = this.scenarioContext.Get<HttpResponseMessage>("sampleSearchResponse");

            this.scenarioContext.Remove("sampleSearchResponse");

            Assert.AreEqual(HttpStatusCode.OK, sampleSearchResponse.StatusCode);

            var model = await sampleSearchResponse.Content.ReadFromJsonAsync<SampleSearchResponseModel>();

            // Assert sample returned from search is valid
            Assert.AreEqual(sampleName, model?.Samples?[0].Name);
        }

        [AfterScenario("sample-search")]
        public async Task Cleanup()
        {
            // Cleanup the samples created in the scenario.
            var sampleIds = this.scenarioContext.Get<List<string?>>("sampleIds");

            foreach (var sampleId in sampleIds)
            {
                if (sampleId != null)
                {
                    var headers = await this.headerProvider.GetHeaders();
                    using var rs = await this.client.DeleteSampleAsync(sampleId, headers);
                }
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
