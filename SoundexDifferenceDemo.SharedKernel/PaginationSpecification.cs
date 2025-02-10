using System.Linq.Expressions;

namespace SoundexDifferenceDemo.SharedKernel;

public abstract class PaginationSpecification<T> : Specification<T>, IPaginationSpecification<T>
    where T : class
{
    public int? Take { get; private set; }

    public int? Skip { get; private set; }

    public virtual void ApplyPagination(int skip, int take)
    {
        Take = take;
        Skip = skip;
    }
}
