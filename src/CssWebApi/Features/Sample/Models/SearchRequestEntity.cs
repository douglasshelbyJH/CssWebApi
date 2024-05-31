// -----------------------------------------------------------------------
// <copyright file="SearchRequestEntity.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace CssWebApi.CssWebApi.Features.Sample.Models
{
    [ExcludeFromCodeCoverage]
    public record SearchRequestEntity(string? Name, string? InstitutionUniversalId)
    {
    }
}
