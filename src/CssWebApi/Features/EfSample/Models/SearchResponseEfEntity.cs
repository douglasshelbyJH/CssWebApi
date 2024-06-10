// -----------------------------------------------------------------------
// <copyright file="SearchResponseEfEntity.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using CssWebApi.CssWebApi.Features.EfSample.EfCore.Entities;

using JackHenry.CSS.Jxr.Models;

namespace CssWebApi.CssWebApi.Features.EfSample.Models
{
    [ExcludeFromCodeCoverage]
    public class SearchResponseEfEntity
    {
        public List<SampleEfEntity>? Samples { get; set; }

        public PagingModel Paging { get; set; } = new PagingModel();

        public SearchResponseEfModel ToModel()
        {
            return new SearchResponseEfModel
            {
                Samples = this.Samples?.Select(x => x.ToModel()).ToList(),
                Paging = this.Paging
            };
        }
    }
}
