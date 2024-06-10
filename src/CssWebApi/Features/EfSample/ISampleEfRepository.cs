// -----------------------------------------------------------------------
// <copyright file="ISampleEfRepository.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CssWebApi.CssWebApi.Features.EfSample.EfCore.Entities;
using CssWebApi.CssWebApi.Features.EfSample.Models;

namespace CssWebApi.CssWebApi.Features.EfSample
{
    public interface ISampleEfRepository
    {
        Task<SampleEfEntity> AddAsync(SampleEfEntity entity, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(SampleEfEntity entity, CancellationToken cancellationToken);

        Task<SampleEfEntity?> FindAsync(Guid id, CancellationToken cancellationToken);

        Task<SearchResponseEfEntity> SearchSampleAsync(SearchRequestEfEntity searchRequest, int offset, int count, CancellationToken cancellationToken);
    }
}
