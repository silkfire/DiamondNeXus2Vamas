namespace CasaXpsUtilities.Common
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;


    // https://devblogs.microsoft.com/pfxteam/asynclazyt/

    public sealed class LazyAsync<T> : Lazy<Task<T>>
    {
        public LazyAsync(Func<Task<T>> taskFunc) : base(() => Task.Factory.StartNew(taskFunc).Unwrap()) { }

        public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
    }
}
