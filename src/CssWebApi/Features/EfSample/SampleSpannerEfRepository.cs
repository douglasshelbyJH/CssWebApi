// -----------------------------------------------------------------------
// <copyright file="SampleSpannerEfRepository.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CssWebApi.CssWebApi.Features.EfSample.EfCore;
using CssWebApi.CssWebApi.Features.EfSample.EfCore.Entities;
using CssWebApi.CssWebApi.Features.EfSample.Models;

using JackHenry.CSS.Jxr.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace CssWebApi.CssWebApi.Features.EfSample
{
    public class SampleSpannerEfRepository(SampleDbContext dbContext, IJxrContext jxrContext) : ISampleEfRepository
    {
        public async Task<SampleEfEntity> AddAsync(SampleEfEntity entity, CancellationToken cancellationToken)
        {
            await dbContext.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var sampleEntity = await this.FindAsync(id, cancellationToken);
            if (sampleEntity == null)
            {
                return false;
            }

            dbContext.Remove(sampleEntity);
            return await dbContext.SaveChangesAsync(cancellationToken) != 0;
        }

        public async Task<SampleEfEntity?> FindAsync(Guid id, CancellationToken cancellationToken)
        {
            return await dbContext.FindAsync<SampleEfEntity>(id);
        }

        public async Task<SearchResponseEfEntity> SearchSampleAsync(SearchRequestEfEntity searchRequest, int offset, int count, CancellationToken cancellationToken)
        {
            var response = new SearchResponseEfEntity();

            var query = dbContext
                .SampleEntities
                .Where(e => e.InstitutionUniversalId == searchRequest.InstitutionUniversalId);

            if (!string.IsNullOrEmpty(searchRequest.Name))
            {
                query = query.Where(e => e.Name.Contains(searchRequest.Name));
            }

            response.Samples = await query.Skip(offset).Take(count).AsNoTrackingWithIdentityResolution().ToListAsync();
            response.Paging.Total = await query.CountAsync(cancellationToken);
            response.Paging.NextOffset = response.Paging.Total <= offset + count ? null : (offset + count).ToString();

            return response;
        }

        public async Task<bool> UpdateAsync(SampleEfEntity entity, CancellationToken cancellationToken)
        {
            var entityToUpdate = await dbContext.FindAsync<SampleEfEntity>(entity.Id);

            if (entityToUpdate == null)
            {
                return false;
            }

            entityToUpdate.Name = entity.Name;
            return await dbContext.SaveChangesAsync(cancellationToken) != 0;
        }
    }
}
