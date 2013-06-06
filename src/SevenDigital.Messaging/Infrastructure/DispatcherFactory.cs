using DispatchSharp;

namespace SevenDigital.Messaging.Infrastructure
{
	/// <summary>
	/// Factory for work dispatchers
	/// </summary>
	public interface IDispatcherFactory
	{
		/// <summary>
		/// Create a dispatcher for the given queue and pool
		/// </summary>
		IDispatch<T> Create<T>(IWorkQueue<T> queue, IWorkerPool<T> pool);
	}

	/// <summary>
	/// Default factory for work dispatchers
	/// </summary>
	public class DispatcherFactory:IDispatcherFactory
	{
		/// <summary>
		/// Create a dispatcher for the given queue and pool
		/// </summary>
		public IDispatch<T> Create<T>(IWorkQueue<T> queue, IWorkerPool<T> pool)
		{
			return new Dispatch<T>(
				queue, pool
				);
		}
	}
}