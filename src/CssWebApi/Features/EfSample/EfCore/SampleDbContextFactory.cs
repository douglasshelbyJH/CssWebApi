// -----------------------------------------------------------------------
// <copyright file="SampleDbContextFactory.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Google.Cloud.EntityFrameworkCore.Spanner.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CssWebApi.CssWebApi.Features.EfSample.EfCore
{
    public class SampleDbContextFactory : IDesignTimeDbContextFactory<SampleDbContext>
    {
        public SampleDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SampleDbContext>();
            optionsBuilder.UseSpanner(
                "Data Source=projects/sdb-dig-core-jheisessb977/instances/sdb-dig-core-jheisessb977-01/databases/dshelby-sampledb");

            return new SampleDbContext(optionsBuilder.Options);
        }
    }
}
