// -----------------------------------------------------------------------
// <copyright file="HeaderProvider.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using JackHenry.CSS.Authentication.Extensions;
using JackHenry.CSS.Authentication.TokenRetriever;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace Functional.Tests.Support
{
    [Binding]
    public class HeaderProvider(ITokenRetriever tokenRetriever)
    {
        private static readonly string ClientId = TestContext.Parameters.Get("ClientId", string.Empty);
        private static readonly string PrivateKey = TestContext.Parameters.Get("PrivateKey", string.Empty);
        private static readonly string OidcProvider = TestContext.Parameters.Get("OidcProvider", string.Empty);
        private static readonly string ProductGatewayBaseUrl = TestContext.Parameters.Get("ProductGatewayBaseUrl", string.Empty);

        private readonly ITokenRetriever tokenRetriever = tokenRetriever;
        private string? bannoToken;

        public async Task<string> GetToken(string authToken)
        {
            var token = await this.tokenRetriever.RequestProductGatewayTokenAsync(authToken, ProductGatewayBaseUrl);

            this.bannoToken = token.ToString();
            return this.bannoToken;
        }

        public async Task<string> GetAuthorization()
        {
            var authorization = await this.tokenRetriever.RequestClientCredentialsTokenAsync(ClientId, OidcProvider, PrivateKey, false);

            var token = authorization.ToString();
            return token;
        }

        public async Task<Dictionary<string, string>> GetHeaders(string? requestId = null)
        {
            var dictionaryHeaders = new Dictionary<string, string>();

            requestId ??= Guid.NewGuid().ToString();

            var authToken = await this.GetAuthorization();
            dictionaryHeaders.Add("X-BannoEnterprise0", await this.GetToken(authToken));
            dictionaryHeaders.Add("Authorization", "Bearer " + authToken);
            dictionaryHeaders.Add("X-Request-ID", requestId);
            return dictionaryHeaders;
        }
    }
}
