using Microsoft.EntityFrameworkCore;

namespace SoundexDifferenceDemo.SharedKernel;

public static class SpecificationEvaluator<T> where T : class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        var query = inputQuery;

        if (specification.Criteria is not null)
        {
            foreach (var criterion in specification.Criteria)
            {
                query = query.Where(criterion);
            }
        }

        if (specification.Includes is not null)
        {
            query = specification.Includes.Aggregate(query, (c, ex) => c.Include(ex));
        }

        if (specification.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        return query;
    }
}