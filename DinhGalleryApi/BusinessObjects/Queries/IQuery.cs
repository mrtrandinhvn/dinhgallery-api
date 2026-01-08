namespace dinhgallery_api.BusinessObjects.Queries;

/// <summary>
/// Marker interface for all queries.
/// Queries represent read-only operations that don't change system state.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the query</typeparam>
public interface IQuery<TResult>
{
}
