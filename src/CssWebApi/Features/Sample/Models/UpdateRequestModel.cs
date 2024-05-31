// -----------------------------------------------------------------------
// <copyright file="UpdateRequestModel.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

using CssWebApi.CssWebApi.Infrastructure;

using JackHenry.CSS.AspNetCore;

namespace CssWebApi.CssWebApi.Features.Sample.Models
{
    public record UpdateRequestModel(string Name) : IValidatableRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        [JsonIgnore]
        public string? InstitutionUniversalId { get; set; }

        public (bool IsValid, StandardErrorHttpResult? Error) Validate(HttpRequest httpRequest)
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                return (false, StandardErrorResults.BadRequest(["Name is required."]));
            }

            if (this.Name.Length < 2)
            {
                return (false, StandardErrorResults.BadRequest(["Name must be at least 2 characters."]));
            }

            if (this.Name.Length > 20)
            {
                return (false, StandardErrorResults.BadRequest(["Name must be 20 characters or less."]));
            }

            return (true, null);
        }

        public SampleEntity ToEntity()
        {
            return new SampleEntity
            {
                Id = this.Id,
                Name = this.Name,
                InstitutionUniversalId = this.InstitutionUniversalId
            };
        }
    }
}
