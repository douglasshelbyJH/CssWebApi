// -----------------------------------------------------------------------
// <copyright file="SampleUpdateRequestModel.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Functional.Tests.Domain.Request
{
    public class SampleUpdateRequestModel
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string? Name { get; set; }
    }
}
