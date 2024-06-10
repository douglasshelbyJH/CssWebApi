// -----------------------------------------------------------------------
// <copyright file="SamplesEfApi.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using CssWebApi.CssWebApi.Extensions;
using CssWebApi.CssWebApi.Features.EfSample;
using CssWebApi.CssWebApi.Features.EfSample.EfCore.Entities;
using CssWebApi.CssWebApi.Features.EfSample.Models;

using JackHenry.CSS.AspNetCore;

using Microsoft.AspNetCore.Http.HttpResults;

namespace CssWebApi.CssWebApi.Apis
{
    public static class SamplesEfApi
    {
        [ExcludeFromCodeCoverage]
        public static RouteGroupBuilder MapSamplesEfApi(this RouteGroupBuilder app)
        {
            app.MapGet("/{sampleId}", GetSampleAsync);
            app.MapPost("/search", SearchSamplesAsync);
            app.MapPost("/", CreateSampleAsync);
            app.MapPut("/{sampleId}", UpdateSampleAsync);
            app.MapDelete("/{sampleId}", DeleteSampleAsync);
            return app;
        }

        public static async Task<Results<Ok<SampleEfModel>, NotFound>> GetSampleAsync(
            string sampleId,
            SampleEfServices services,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(sampleId, out Guid id))
            {
                return TypedResults.NotFound();
            }

            SampleEfEntity? entity = await services.Repository.FindAsync(id, cancellationToken);

            if (entity == null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(new SampleEfModel { Id = entity.Id, Name = entity.Name });
        }

        public static async Task<Results<NoContent, NotFound>> DeleteSampleAsync(
            string sampleId,
            SampleEfServices services,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(sampleId, out Guid id))
            {
                return TypedResults.NotFound();
            }

            return await services.Repository.DeleteAsync(id, cancellationToken) ?
                TypedResults.NoContent() :
                TypedResults.NotFound();
        }

        public static async Task<Results<NoContent, NotFound, StandardErrorHttpResult>> UpdateSampleAsync(
            string sampleId,
            HttpContext httpContext,
            SampleEfServices services,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(sampleId, out Guid id))
            {
                return TypedResults.NotFound();
            }

            (var isValid, UpdateRequestEfModel? request, StandardErrorHttpResult? error) =
                await httpContext.Request.GetValidatedRequestAsync<UpdateRequestEfModel>(cancellationToken);

            if (!isValid || request == null)
            {
                return error ?? StandardErrorResults.BadRequest(["Invalid request."]);
            }

            request.Id = id;
            request.InstitutionUniversalId = services.JxrContext.InstitutionUniversalId;

            return await services.Repository.UpdateAsync(request.ToEntity(), cancellationToken) ?
                TypedResults.NoContent() :
                TypedResults.NotFound();
        }

        public static async Task<Results<Created<CreateResponseEfModel>, StandardErrorHttpResult>> CreateSampleAsync(
            HttpContext httpContext,
            SampleEfServices services,
            CancellationToken cancellationToken)
        {
            (var isValid, CreateRequestEfModel? request, StandardErrorHttpResult? error) =
                await httpContext.Request.GetValidatedRequestAsync<CreateRequestEfModel>(cancellationToken);

            if (!isValid || request == null)
            {
                return error ?? StandardErrorResults.BadRequest(["Invalid request."]);
            }

            request.InstitutionUniversalId = services.JxrContext.InstitutionUniversalId;

            SampleEfEntity entity = await services.Repository.AddAsync(request.ToEntity(), cancellationToken);

            return TypedResults.Created(string.Empty, new CreateResponseEfModel(entity.Id));
        }

        public static async Task<Results<Ok<SearchResponseEfModel>, NotFound, StandardErrorHttpResult>> SearchSamplesAsync(
            HttpContext httpContext,
            SampleEfServices services,
            CancellationToken cancellationToken)
        {
            (var isValid, SearchRequestEfModel? request, StandardErrorHttpResult? error) =
                await httpContext.Request.GetValidatedRequestAsync<SearchRequestEfModel>(cancellationToken);

            if (!isValid || request == null)
            {
                return error ?? StandardErrorResults.BadRequest(["Invalid request."]);
            }

            var offsetResult = int.TryParse(httpContext.Request.Query["offset"], out var offset);
            var countResult = int.TryParse(httpContext.Request.Query["count"], out var count);

            if (!(offsetResult && countResult))
            {
                var errors = new List<string>();
                if (!offsetResult)
                {
                    errors.Add("Invalid offset.");
                }

                if (!countResult)
                {
                    errors.Add("Invalid count.");
                }

                return StandardErrorResults.BadRequest(errors);
            }

            SearchResponseEfEntity searchResults = await services.Repository.SearchSampleAsync(
                new SearchRequestEfEntity(request.Name, services.JxrContext.InstitutionUniversalId),
                offset,
                count,
                cancellationToken);

            return TypedResults.Ok(searchResults.ToModel());
        }
    }
}
