// -----------------------------------------------------------------------
// <copyright file="RequestExtensions.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using CssWebApi.CssWebApi.Infrastructure;

using JackHenry.CSS.AspNetCore;

namespace CssWebApi.CssWebApi.Extensions
{
    public static class RequestExtensions
    {
        public static async ValueTask<(bool IsValid, TValue? Request, StandardErrorHttpResult? Error)> GetValidatedRequestAsync<TValue>(
            this HttpRequest httpRequest,
            CancellationToken cancellationToken)
            where TValue : IValidatableRequest
        {
            var deserializationResult = await httpRequest.TryGetRequestAsync<TValue>(cancellationToken: cancellationToken);

            if (deserializationResult.Error?.Exception != null)
            {
                ILoggerFactory? loggerFactory = httpRequest.HttpContext.RequestServices.GetService<ILoggerFactory>();
                ILogger? logger = loggerFactory?.CreateLogger(typeof(RequestExtensions));

                if (logger is not null)
                {
                    Log.RequestDeserializationException(logger, deserializationResult.Error.Exception);
                }

                return (false, deserializationResult.Request, deserializationResult.Error);
            }

            if (deserializationResult.Request == null)
            {
                return (false, deserializationResult.Request, deserializationResult.Error);
            }

            (bool IsValid, StandardErrorHttpResult? Error) validationResult = deserializationResult.Request.Validate(httpRequest);

            if (!validationResult.IsValid)
            {
                return (false, default, validationResult.Error);
            }

            return (true, deserializationResult.Request, deserializationResult.Error);
        }
    }

    [ExcludeFromCodeCoverage]
#pragma warning disable SA1402 // File may only contain a single type
    internal static partial class Log
#pragma warning restore SA1402 // File may only contain a single type
    {
        [LoggerMessage(LogLevel.Error, "Error deserializing request body.", EventName = "RequestDeserializationException")]
        public static partial void RequestDeserializationException(ILogger logger, Exception ex);
    }
}
