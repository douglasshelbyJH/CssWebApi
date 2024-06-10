// -----------------------------------------------------------------------
// <copyright file="CssWebApiSerializerContext.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using CssWebApi.CssWebApi.Features.EfSample.Models;
using CssWebApi.CssWebApi.Features.Sample.Models;

using JackHenry.CSS.Jxr.Models;

namespace CssWebApi.CssWebApi.Infrastructure
{
    [ExcludeFromCodeCoverage]
    [JsonSourceGenerationOptions(
        WriteIndented = false,
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(StandardErrorModel))]
    [JsonSerializable(typeof(SampleModel))]
    [JsonSerializable(typeof(SampleEfModel))]
    [JsonSerializable(typeof(CreateRequestModel))]
    [JsonSerializable(typeof(CreateResponseModel))]
    [JsonSerializable(typeof(SearchRequestModel))]
    [JsonSerializable(typeof(SearchResponseModel))]
    [JsonSerializable(typeof(UpdateRequestModel))]
    public partial class CssWebApiSerializerContext : JsonSerializerContext
    {
    }
}
