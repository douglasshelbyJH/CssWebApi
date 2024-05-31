// -----------------------------------------------------------------------
// <copyright file="SampleModel.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Functional.Tests.Domain.Response
{
    public class SampleModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        [JsonIgnore]
        public Guid ETag { get; set; }
    }
}
