// -----------------------------------------------------------------------
// <copyright file="PagingModel.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Functional.Tests.Domain.Response
{
    public class PagingModel
    {
        /// <summary>
        /// Gets or sets the next offset is the start position of the read pointer for pagination
        /// </summary>
        public string? NextOffset { get; set; }

        /// <summary>
        /// Gets or sets the number of records sent for pagination
        /// </summary>
        public int Results { get; set; }

        /// <summary>
        /// Gets or sets the total number of records requested for pagination
        /// </summary>
        public long Total { get; set; }
    }
}
