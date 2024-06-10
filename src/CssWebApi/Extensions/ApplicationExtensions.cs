// -----------------------------------------------------------------------
// <copyright file="ApplicationExtensions.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CssWebApi.CssWebApi.Features.EfSample;
using CssWebApi.CssWebApi.Features.EfSample.EfCore;
using CssWebApi.CssWebApi.Features.Sample;
using CssWebApi.CssWebApi.Infrastructure;

using Google.Cloud.EntityFrameworkCore.Spanner.Extensions;
using Google.Cloud.Spanner.Data;

namespace CssWebApi.CssWebApi.Extensions
{
    public static class ApplicationExtensions
    {
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            IServiceCollection services = builder.Services;
            services.AddSpannerConnection("Spanner");
            services.AddScoped<ISampleRepository, SampleSpannerRepository>();
            services.AddScoped<ISampleEfRepository, SampleSpannerEfRepository>();
            services.AddScoped<SampleServices>();
            services.AddScoped<SampleEfServices>();
            services.AddDbContext<SampleDbContext>((sp, options) =>
            {
                options.EnableDetailedErrors();
                options.UseSpanner(sp.GetRequiredService<SpannerConnection>());
            });
            services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Insert(0, CssWebApiSerializerContext.Default));
        }
    }
}
