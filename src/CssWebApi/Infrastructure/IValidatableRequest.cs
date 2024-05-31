// -----------------------------------------------------------------------
// <copyright file="IValidatableRequest.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using JackHenry.CSS.AspNetCore;

namespace CssWebApi.CssWebApi.Infrastructure
{
    public interface IValidatableRequest
    {
        public (bool IsValid, StandardErrorHttpResult? Error) Validate(HttpRequest httpRequest);
    }
}
