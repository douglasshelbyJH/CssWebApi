// -----------------------------------------------------------------------
// <copyright file="SampleDbContext.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CssWebApi.CssWebApi.Features.EfSample.EfCore.Configuration;
using CssWebApi.CssWebApi.Features.Sample.Models;

using Microsoft.EntityFrameworkCore;

namespace CssWebApi.CssWebApi.Features.EfSample.EfCore
{
    public class SampleDbContext : DbContext
    {
        public DbSet<SampleEntity> SampleEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new SampleEntityConfiguration());
        }
    }
}
