// -----------------------------------------------------------------------
// <copyright file="GlobalSetup.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using JackHenry.CSS.Authentication.Extensions;

using Microsoft.Extensions.DependencyInjection;
using SolidToken.SpecFlow.DependencyInjection;

namespace Functional.Tests.Support
{
    public static class GlobalSetup
    {
        [ScenarioDependencies]
        public static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();
            services.AddTokenRetriever();
            services.AddSingleton<HeaderProvider>();

            return services;
        }
    }
}
