// -----------------------------------------------------------------------
// <copyright file="SampleEntityConfiguration.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using CssWebApi.CssWebApi.Features.Sample.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CssWebApi.CssWebApi.Features.EfSample.EfCore.Configuration
{
    public class SampleEntityConfiguration : IEntityTypeConfiguration<SampleEntity>
    {
        public void Configure(EntityTypeBuilder<SampleEntity> builder)
        {
            builder.ToTable("SampleEntity");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).IsRequired();
        }
    }
}
