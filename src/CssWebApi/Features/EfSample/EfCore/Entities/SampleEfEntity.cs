// -----------------------------------------------------------------------
// <copyright file="SampleEfEntity.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CssWebApi.CssWebApi.Features.EfSample.Models;

namespace CssWebApi.CssWebApi.Features.EfSample.EfCore.Entities
{
    public class SampleEfEntity
    {
        public Guid Id { get; set; }

        public string? InstitutionUniversalId { get; set; }

        public string? Name { get; set; }

        public string? UpdatedBy { get; set; }

        public SampleEfModel ToModel()
        {
            return new SampleEfModel
            {
                Id = this.Id,
                Name = this.Name
            };
        }
    }
}
