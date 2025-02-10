using System.Linq.Expressions;

namespace SoundexDifferenceDemo.SharedKernel;

public abstract class Specification<T> : ISpecification<T>
    where T : class
{
    public List<Expression<Func<T, bool>>>? Criteria { get; private set; }

    public List<Expression<Func<T, object?>>>? Includes { get; private set; }

    public Expression<Func<T, object?>>? OrderBy { get; private set; }

    public Expression<Func<T, object?>>? OrderByDescending { get; private set; }

    public virtual void AddCriterion(Expression<Func<T, bool>> expression)
    {
        Criteria ??= [];
        Criteria.Add(expression);
    }

    public virtual void AddInclude(Expression<Func<T, object?>> expression)
    {
        Includes ??= [];
        Includes.Add(expression);
    }

    public virtual void AddOrderBy(Expression<Func<T, object?>> expression)
    {
        OrderBy = expression;
    }

    public virtual void AddOrderByDescending(Expression<Func<T, object?>> expression)
    {
        OrderByDescending = expression;
    }
}