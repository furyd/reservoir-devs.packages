namespace ReservoirDevs.Idempotence.Models
{
    public class IdempotenceHeader
    {
        private readonly string _value;

        public override string ToString() => _value;

        public IdempotenceHeader(string value)
        {
            _value = value;
        }

        public static implicit operator IdempotenceHeader(string value) => new IdempotenceHeader(value);

        public static implicit operator string(IdempotenceHeader header) => header.ToString();
    }
}