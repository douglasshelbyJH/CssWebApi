// -----------------------------------------------------------------------
// <copyright file="SampleSearchResponseModel.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Functional.Tests.Domain.Response
{
    public class SampleSearchResponseModel
    {
        public SampleModel[]? Samples { get; set; }

        public PagingModel? Paging { get; set; }
    }
}
