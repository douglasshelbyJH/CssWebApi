// -----------------------------------------------------------------------
// <copyright file="SampleServices.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using JackHenry.CSS.Jxr.Interfaces;

namespace CssWebApi.CssWebApi.Features.Sample
{
    public class SampleServices(
        ISampleRepository repository,
        IJxrContext jxrContext,
        ILogger<SampleServices> logger)
    {
        public ISampleRepository Repository { get; } = repository;

        public ILogger<SampleServices> Logger { get; } = logger;

        public IJxrContext JxrContext { get; } = jxrContext;
    }
}
