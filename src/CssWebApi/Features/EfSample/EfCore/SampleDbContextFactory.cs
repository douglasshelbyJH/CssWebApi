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
    /// <summary>
    /// This factory is used when executing dotnet ef CLI commands.
    /// </summary>
    public class SampleDbContextFactory : IDesignTimeDbContextFactory<SampleDbContext>
    {
        public SampleDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();

            var optionsBuilder = new DbContextOptionsBuilder<SampleDbContext>();
            optionsBuilder.UseSpanner(configuration.GetConnectionString("Spanner"));

            return new SampleDbContext(optionsBuilder.Options);
        }
    }
}
