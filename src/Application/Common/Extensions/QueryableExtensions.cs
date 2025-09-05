// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Ardalis.Specification.EntityFrameworkCore;
using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplySpecification<T>(
        this IQueryable<T> query,
        ISpecification<T> spec,
        bool evaluateCriteriaOnly = false
    )
        where T : class, IEntity
    {
        return SpecificationEvaluator.Default.GetQuery(query, spec, evaluateCriteriaOnly);
    }

    public static async Task<PaginatedData<TResult>> ProjectToPaginatedDataAsync<T, TResult>(
        this IOrderedQueryable<T> query,
        ISpecification<T> spec,
        int pageNumber,
        int pageSize,
        IConfigurationProvider configuration,
        CancellationToken cancellationToken = default
    )
        where T : class
    {
        var specificationEvaluator = SpecificationEvaluator.Default;
        var count = await specificationEvaluator
            .GetQuery(query, spec)
            .CountAsync(cancellationToken: cancellationToken);
        var data = await specificationEvaluator
            .GetQuery(query.AsNoTracking(), spec)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<TResult>(configuration)
            .ToListAsync(cancellationToken);
        return new PaginatedData<TResult>(data, count, pageNumber, pageSize);
    }       
}