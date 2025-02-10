namespace SoundexDifferenceDemo.SharedKernel;

public interface IPaginationSpecification<T> : ISpecification<T> where T : class
{
    int? Take { get; }
    int? Skip { get; }
}
