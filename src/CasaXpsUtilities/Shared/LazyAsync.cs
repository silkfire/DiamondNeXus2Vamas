namespace CasaXpsUtilities.Shared;

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// https://devblogs.microsoft.com/pfxteam/asynclazyt/

/// <summary>
/// Provides support for asynchronous lazy initialization. The asynchronous operation is started on demand and its
/// result is cached for subsequent accesses.
/// </summary>
/// <remarks>Use this class to defer the creation of an object until it is needed, while supporting asynchronous
/// initialization patterns. The initialization function is invoked only once, and the resulting task is cached for all
/// future accesses. This class is thread-safe and can be awaited directly using the <see langword="await"/> keyword.</remarks>
/// <typeparam name="T">The type of the object that is being asynchronously initialized.</typeparam>
/// <param name="taskFunc">A function that returns a task representing the asynchronous initialization operation. The function is invoked only
/// once, when the value is first needed.</param>
public sealed class LazyAsync<T>(Func<Task<T>> taskFunc) : Lazy<Task<T>>(() => Task.Factory.StartNew(taskFunc).Unwrap())
{
    /// <summary>
    /// Gets an awaiter used to await this <see cref="LazyAsync{T}"/> instance.
    /// </summary>
    public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
}
