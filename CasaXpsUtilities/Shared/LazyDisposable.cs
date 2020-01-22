namespace CasaXpsUtilities.Shared
{
    using System;


    // https://stackoverflow.com/a/30664665/633098

    public sealed class LazyDisposable<T> : Lazy<T>, IDisposable
        where T : IDisposable
    {
        public LazyDisposable(Func<T> valueFactory) : base(valueFactory) { }

        public void Dispose()
        {
            if (IsValueCreated)
            {
                Value.Dispose();
            }
        }
    }
}
