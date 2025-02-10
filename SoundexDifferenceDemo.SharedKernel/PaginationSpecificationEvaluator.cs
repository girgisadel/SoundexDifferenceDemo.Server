namespace SoundexDifferenceDemo.SharedKernel;

public static class PaginationSpecificationEvaluator<T> where T : class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, IPaginationSpecification<T> specification)
    {
        var query = SpecificationEvaluator<T>.GetQuery(inputQuery, specification);
        
        if (specification.Skip.HasValue && specification.Skip.Value > 0)
        {
            query = query.Skip(specification.Skip.Value);
        }

        if (specification.Take.HasValue && specification.Take.Value > 0)
        {
            query = query.Take(specification.Take.Value);
        }

        return query;
    }
}
