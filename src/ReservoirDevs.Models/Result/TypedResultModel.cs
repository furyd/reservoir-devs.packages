namespace ReservoirDevs.Models.Result
{
    public record ResultModel<T> : ResultModel
    {
        public T Item { get; init; }

        public bool HasItem => Item != null;
    }
}
