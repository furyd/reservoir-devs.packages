namespace ReservoirDevs.Correlation.Models
{
    public class CorrelationHeader
    {
        private readonly string _value;

        public override string ToString() => _value;

        public CorrelationHeader(string value)
        {
            _value = value;
        }

        public static implicit operator CorrelationHeader(string value) => new CorrelationHeader(value);

        public static implicit operator string(CorrelationHeader header) => header.ToString();
    }
}