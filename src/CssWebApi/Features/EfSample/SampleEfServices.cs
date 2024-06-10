// -----------------------------------------------------------------------
// <copyright file="SampleEfServices.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using JackHenry.CSS.Jxr.Interfaces;

namespace CssWebApi.CssWebApi.Features.EfSample
{
    public class SampleEfServices(
        ISampleEfRepository repository,
        IJxrContext jxrContext,
        ILogger<SampleEfServices> logger)
    {
        public ISampleEfRepository Repository { get; } = repository;

        public ILogger<SampleEfServices> Logger { get; } = logger;

        public IJxrContext JxrContext { get; } = jxrContext;
    }
}
