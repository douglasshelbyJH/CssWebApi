// -----------------------------------------------------------------------
// <copyright file="SampleEfCoreEntityConfiguration.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using CssWebApi.CssWebApi.Features.EfSample.EfCore.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CssWebApi.CssWebApi.Features.EfSample.EfCore.Configuration
{
    public class SampleEfCoreEntityConfiguration : IEntityTypeConfiguration<SampleEfEntity>
    {
        public void Configure(EntityTypeBuilder<SampleEfEntity> builder)
        {
            builder.ToTable("SampleEntity");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(36).IsRequired();
            builder.HasIndex(e => e.InstitutionUniversalId);
            builder.Property(e => e.InstitutionUniversalId).HasMaxLength(36).IsRequired();
            builder.Property(e => e.Name).HasMaxLength(256).IsRequired();
            builder.Property(e => e.UpdatedBy).HasMaxLength(256);
        }
    }
}
