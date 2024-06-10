// -----------------------------------------------------------------------
// <copyright file="SearchResponseEfModel.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using JackHenry.CSS.Jxr.Models;

namespace CssWebApi.CssWebApi.Features.EfSample.Models
{
    [ExcludeFromCodeCoverage]
    public class SearchResponseEfModel
    {
        public List<SampleEfModel>? Samples { get; set; }

        public PagingModel Paging { get; set; } = new PagingModel();
    }
}
