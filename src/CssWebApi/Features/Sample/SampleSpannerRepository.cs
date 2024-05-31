// -----------------------------------------------------------------------
// <copyright file="SampleSpannerRepository.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using CssWebApi.CssWebApi.Features.Sample.Models;

using Google.Cloud.Spanner.Data;

using JackHenry.CSS.Jxr.Interfaces;

namespace CssWebApi.CssWebApi.Features.Sample
{
    [ExcludeFromCodeCoverage(Justification = "SpannerConnection and SpannerCommand are sealed classes and cannot be mocked, extra effort required to make these dependencies mockable. May look into this in the future.")]
    public class SampleSpannerRepository(SpannerConnection connection, IJxrContext jxrContext) : ISampleRepository
    {
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await using SpannerCommand cmd = connection.CreateDmlCommand("DELETE FROM Samples WHERE Id = @Id AND InstitutionUniversalId = @InstitutionUniversalId");
            cmd.Parameters.Add("Id", SpannerDbType.String, id.ToString());
            cmd.Parameters.Add("InstitutionUniversalId", SpannerDbType.String, jxrContext.InstitutionUniversalId);
            var recordCount = await cmd.ExecuteNonQueryAsync(cancellationToken);

            return recordCount > 0;
        }

        public async Task<SampleEntity> AddAsync(SampleEntity entity, CancellationToken cancellationToken)
        {
            var spannerParameterCollection = new SpannerParameterCollection()
                    {
                         { "Id", SpannerDbType.String, entity.Id.ToString() },
                         { "InstitutionUniversalId", SpannerDbType.String, entity.InstitutionUniversalId },
                         { "Name", SpannerDbType.String, entity.Name },
                    };

            await using SpannerCommand? cmd = connection.CreateInsertCommand("Samples", spannerParameterCollection);
            await cmd.ExecuteNonQueryAsync(cancellationToken);

            return entity;
        }

        public async Task<bool> UpdateAsync(SampleEntity entity, CancellationToken cancellationToken)
        {
            await using SpannerCommand cmd = connection.CreateDmlCommand("UPDATE Samples SET Name = @Name " +
                "WHERE Id = @Id AND InstitutionUniversalId = @InstitutionUniversalId");
            cmd.Parameters.Add("Id", SpannerDbType.String, entity.Id.ToString());
            cmd.Parameters.Add("Name", SpannerDbType.String, entity.Name);
            cmd.Parameters.Add("InstitutionUniversalId", SpannerDbType.String, entity.InstitutionUniversalId);

            var recordCount = await cmd.ExecuteNonQueryAsync(cancellationToken);

            return recordCount > 0;
        }

        public async Task<SampleEntity?> FindAsync(Guid id, CancellationToken cancellationToken)
        {
            var institutionUniversalId = jxrContext.InstitutionUniversalId;
            var sql = "SELECT Id, InstitutionUniversalId, Name FROM Samples WHERE InstitutionUniversalId = @InstitutionUniversalId and Id = @Id";

            SampleEntity? response = null;

            await using SpannerCommand? cmd = connection.CreateSelectCommand(sql);
            cmd.Parameters.Add("Id", SpannerDbType.String, id.ToString());
            cmd.Parameters.Add("InstitutionUniversalId", SpannerDbType.String, institutionUniversalId);
            using (SpannerDataReader? reader = await cmd.ExecuteReaderAsync(cancellationToken))
            {
                if (reader.Read())
                {
                    response = ReadTransaction(reader);
                }
            }

            return response;
        }

        public async Task<SearchResponseEntity> SearchSampleAsync(SearchRequestEntity searchRequest, int offset, int count, CancellationToken cancellationToken)
        {
            var countSql = "SELECT COUNT(1) AS Count";
            var selectSql = "SELECT Id, InstitutionUniversalId, Name";
            var whereClause = " FROM Samples WHERE InstitutionUniversalId = @InstitutionUniversalId";

            var countParameters = new SpannerParameterCollection
            {
                { "InstitutionUniversalId", SpannerDbType.String, searchRequest.InstitutionUniversalId }
            };

            if (!string.IsNullOrEmpty(searchRequest.Name))
            {
                whereClause += $" AND Name LIKE CONCAT('%', @Name, '%')";
                countParameters.Add("Name", SpannerDbType.String, searchRequest.Name);
            }

            var searchParameters = new SpannerParameterCollection(countParameters)
            {
                { "Count", SpannerDbType.Int64, count },
                { "Offset", SpannerDbType.Int64, offset }
            };

            var response = new SearchResponseEntity();

            await connection.OpenAsync(cancellationToken);

            var tasks = new List<Task>
            {
                Task.Run(
                async () =>
                {
                    await using SpannerCommand cmd = connection.CreateSelectCommand(countSql + whereClause, countParameters);
                    response.Paging.Total = await cmd.ExecuteScalarAsync<long>(cancellationToken);
                },
                cancellationToken),
                Task.Run(
                async () =>
                {
                    await using SpannerCommand cmd = connection.CreateSelectCommand(selectSql + whereClause + " LIMIT @Count OFFSET @Offset", searchParameters);

                    using (SpannerDataReader? reader = await cmd.ExecuteReaderAsync(cancellationToken))
                    {
                        response.Samples = ReadTransactions(reader);
                    }
                },
                cancellationToken)
            };

            await Task.WhenAll(tasks);

            if (response.Paging.Total > 0)
            {
                response.Paging.Results = response.Samples?.Count ?? 0;
                response.Paging.NextOffset = response.Paging.Total <= offset + count ? null : (offset + count).ToString();
            }

            return response;
        }

        protected static List<SampleEntity> ReadTransactions(SpannerDataReader reader)
        {
            var result = new List<SampleEntity>();
            while (reader.Read())
            {
                result.Add(ReadTransaction(reader));
            }

            return result;
        }

        protected static SampleEntity ReadTransaction(SpannerDataReader reader)
        {
            return new SampleEntity()
            {
                InstitutionUniversalId = reader.GetFieldValue<string>("InstitutionUniversalId"),
                Name = reader.GetFieldValue<string>("Name"),
                Id = new Guid(reader.GetFieldValue<string>("Id"))
            };
        }
    }
}
