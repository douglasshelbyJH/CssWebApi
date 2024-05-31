// -----------------------------------------------------------------------
// <copyright file="SampleEntity.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace CssWebApi.CssWebApi.Features.Sample.Models
{
    [ExcludeFromCodeCoverage]
    public class SampleEntity
    {
        public Guid Id { get; set; }

        public string? InstitutionUniversalId { get; set; }

        public string? Name { get; set; }

        public SampleModel ToModel()
        {
            return new SampleModel
            {
                Id = this.Id,
                Name = this.Name
            };
        }
    }
}
