namespace CasaXpsUtilities.Shared
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    // https://stackoverflow.com/a/30664665/633098

    public sealed class LazyDisposable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]T> : Lazy<T>, IDisposable
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
