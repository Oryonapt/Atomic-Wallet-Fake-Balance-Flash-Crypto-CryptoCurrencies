namespace Microsoft.EntityFrameworkCore;

public class JoinedClass<TOuter, TInner>
{
    public TOuter Outer { get; internal set; } = default!;

    public TInner? Inner { get; internal init; } = default!;
}

public class Pagination<TEntity>
{
    public List<TEntity> Data { get; init; } = [];

    public int TotalCount { get; init; } = 0;
}
