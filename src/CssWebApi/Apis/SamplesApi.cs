// -----------------------------------------------------------------------
// <copyright file="SamplesApi.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using CssWebApi.CssWebApi.Extensions;
using CssWebApi.CssWebApi.Features.Sample;
using CssWebApi.CssWebApi.Features.Sample.Models;
using JackHenry.CSS.AspNetCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CssWebApi.CssWebApi.Apis
{
    public static class SamplesApi
    {
        [ExcludeFromCodeCoverage]
        public static RouteGroupBuilder MapSamplesApi(this RouteGroupBuilder app)
        {
            app.MapGet("/{sampleId}", GetSampleAsync);
            app.MapPost("/search", SearchSamplesAsync);
            app.MapPost("/", CreateSampleAsync);
            app.MapPut("/{sampleId}", UpdateSampleAsync);
            app.MapDelete("/{sampleId}", DeleteSampleAsync);
            return app;
        }

        public static async Task<Results<Ok<SampleModel>, NotFound>> GetSampleAsync(
            string sampleId,
            SampleServices services,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(sampleId, out Guid id))
            {
                return TypedResults.NotFound();
            }

            SampleEntity? entity = await services.Repository.FindAsync(id, cancellationToken);

            if (entity == null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(new SampleModel { Id = entity.Id, Name = entity.Name });
        }

        public static async Task<Results<NoContent, NotFound>> DeleteSampleAsync(
            string sampleId,
            SampleServices services,
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
            SampleServices services,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(sampleId, out Guid id))
            {
                return TypedResults.NotFound();
            }

            (var isValid, UpdateRequestModel? request, StandardErrorHttpResult? error) =
                await httpContext.Request.GetValidatedRequestAsync<UpdateRequestModel>(cancellationToken);

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

        public static async Task<Results<Created<CreateResponseModel>, StandardErrorHttpResult>> CreateSampleAsync(
            HttpContext httpContext,
            SampleServices services,
            CancellationToken cancellationToken)
        {
            (var isValid, CreateRequestModel? request, StandardErrorHttpResult? error) =
                await httpContext.Request.GetValidatedRequestAsync<CreateRequestModel>(cancellationToken);

            if (!isValid || request == null)
            {
                return error ?? StandardErrorResults.BadRequest(["Invalid request."]);
            }

            request.InstitutionUniversalId = services.JxrContext.InstitutionUniversalId;

            SampleEntity entity = await services.Repository.AddAsync(request.ToEntity(), cancellationToken);

            return TypedResults.Created(string.Empty, new CreateResponseModel(entity.Id));
        }

        public static async Task<Results<Ok<SearchResponseModel>, NotFound, StandardErrorHttpResult>> SearchSamplesAsync(
            HttpContext httpContext,
            SampleServices services,
            CancellationToken cancellationToken)
        {
            (var isValid, SearchRequestModel? request, StandardErrorHttpResult? error) =
                await httpContext.Request.GetValidatedRequestAsync<SearchRequestModel>(cancellationToken);

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

            SearchResponseEntity searchResults = await services.Repository.SearchSampleAsync(
                new SearchRequestEntity(request.Name, services.JxrContext.InstitutionUniversalId),
                offset,
                count,
                cancellationToken);

            return TypedResults.Ok(searchResults.ToModel());
        }
    }
}
