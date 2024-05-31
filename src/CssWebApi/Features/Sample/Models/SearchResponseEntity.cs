// -----------------------------------------------------------------------
// <copyright file="SearchResponseEntity.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using JackHenry.CSS.Jxr.Models;

namespace CssWebApi.CssWebApi.Features.Sample.Models
{
    [ExcludeFromCodeCoverage]
    public class SearchResponseEntity
    {
        public List<SampleEntity>? Samples { get; set; }

        public PagingModel Paging { get; set; } = new PagingModel();

        public SearchResponseModel ToModel()
        {
            return new SearchResponseModel
            {
                Samples = this.Samples?.Select(x => x.ToModel()).ToList(),
                Paging = this.Paging
            };
        }
    }
}
