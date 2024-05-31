// -----------------------------------------------------------------------
// <copyright file="ApplicationExtensions.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CssWebApi.CssWebApi.Features.Sample;
using CssWebApi.CssWebApi.Infrastructure;

namespace CssWebApi.CssWebApi.Extensions
{
    public static class ApplicationExtensions
    {
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            IServiceCollection services = builder.Services;
            services.AddSpannerConnection("Spanner");
            services.AddScoped<ISampleRepository, SampleSpannerRepository>();
            services.AddScoped<SampleServices>();
            services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Insert(0, CssWebApiSerializerContext.Default));
        }
    }
}
