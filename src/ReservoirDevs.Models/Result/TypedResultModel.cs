using System;

namespace ReservoirDevs.Models.Result
{
    public class TypedResultModel<T> : ResultModelBase where T : class
    {
        public T Item { get; }

        public bool HasItem => Item != null;

        public TypedResultModel(T item)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
        }

        public TypedResultModel(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public TypedResultModel(T item, Exception exception)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }
    }
}
