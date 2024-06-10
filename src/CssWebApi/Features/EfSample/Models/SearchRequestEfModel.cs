// -----------------------------------------------------------------------
// <copyright file="SearchRequestEfModel.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using CssWebApi.CssWebApi.Infrastructure;

using JackHenry.CSS.AspNetCore;

namespace CssWebApi.CssWebApi.Features.EfSample.Models
{
    [ExcludeFromCodeCoverage]
    public record SearchRequestEfModel(string? Name) : IValidatableRequest
    {
        public (bool IsValid, StandardErrorHttpResult? Error) Validate(HttpRequest httpRequest)
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                return (false, StandardErrorResults.BadRequest(["Name is required."]));
            }
            else
            {
                return (true, null);
            }
        }
    }
}
