// -----------------------------------------------------------------------
// <copyright file="SampleEfCoreSpannerRepository.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CssWebApi.CssWebApi.Features.Sample;
using CssWebApi.CssWebApi.Features.Sample.Models;

namespace CssWebApi.CssWebApi.Features.EfSample
{
    public class SampleEfCoreSpannerRepository : ISampleRepository
    {
        public Task<SampleEntity> AddAsync(SampleEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<SampleEntity?> FindAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<SearchResponseEntity> SearchSampleAsync(SearchRequestEntity searchRequest, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(SampleEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
