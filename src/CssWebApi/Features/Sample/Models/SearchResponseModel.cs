// -----------------------------------------------------------------------
// <copyright file="SearchResponseModel.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using JackHenry.CSS.Jxr.Models;

namespace CssWebApi.CssWebApi.Features.Sample.Models
{
    [ExcludeFromCodeCoverage]
    public class SearchResponseModel
    {
        public List<SampleModel>? Samples { get; set; }

        public PagingModel Paging { get; set; } = new PagingModel();
    }
}
