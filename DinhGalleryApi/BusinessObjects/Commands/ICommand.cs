namespace dinhgallery_api.BusinessObjects.Commands;

/// <summary>
/// Marker interface for all commands.
/// Commands represent operations that change system state.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the command</typeparam>
public interface ICommand<TResult>
{
}
