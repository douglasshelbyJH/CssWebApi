// -----------------------------------------------------------------------
// <copyright file="SampleCreateResponseModel.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Functional.Tests.Domain.Response
{
    public class SampleCreateResponseModel
    {
        public SampleCreateResponseModel(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; set; }
    }
}
