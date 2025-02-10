using System.Linq.Expressions;

namespace SoundexDifferenceDemo.SharedKernel;

public interface ISpecification<T> where T : class
{
    List<Expression<Func<T, bool>>>? Criteria { get; }

    List<Expression<Func<T, object?>>>? Includes { get; }

    Expression<Func<T, object?>>? OrderBy { get; }

    Expression<Func<T, object?>>? OrderByDescending { get; }
}