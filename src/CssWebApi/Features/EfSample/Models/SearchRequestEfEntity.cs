// -----------------------------------------------------------------------
// <copyright file="SearchRequestEfEntity.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace CssWebApi.CssWebApi.Features.EfSample.Models
{
    [ExcludeFromCodeCoverage]
    public record SearchRequestEfEntity(string? Name, string? InstitutionUniversalId)
    {
    }
}
