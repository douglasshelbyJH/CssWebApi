// -----------------------------------------------------------------------
// <copyright file="ISampleRepository.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CssWebApi.CssWebApi.Features.Sample.Models;

namespace CssWebApi.CssWebApi.Features.Sample
{
    public interface ISampleRepository
    {
        Task<SampleEntity> AddAsync(SampleEntity entity, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(SampleEntity entity, CancellationToken cancellationToken);

        Task<SampleEntity?> FindAsync(Guid id, CancellationToken cancellationToken);

        Task<SearchResponseEntity> SearchSampleAsync(SearchRequestEntity searchRequest, int offset, int count, CancellationToken cancellationToken);
    }
}
