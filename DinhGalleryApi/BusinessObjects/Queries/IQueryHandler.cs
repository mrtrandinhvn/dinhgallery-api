namespace dinhgallery_api.BusinessObjects.Queries;

/// <summary>
/// Defines a handler for a query.
/// Each query should have exactly one handler that contains the logic for executing that query.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle</typeparam>
/// <typeparam name="TResult">The type of result returned by the handler</typeparam>
public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Handles the specified query.
    /// </summary>
    /// <param name="query">The query to handle</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of executing the query</returns>
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
