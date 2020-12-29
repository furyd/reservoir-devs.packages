namespace ReservoirDevs.Correlation.Models
{
    public class CorrelationToken
    {
        private readonly string _tokenValue;

        public override string ToString() => _tokenValue;

        public CorrelationToken(string value)
        {
            _tokenValue = value;
        }

        public static implicit operator CorrelationToken(string value) => new CorrelationToken(value);

        public static implicit operator string(CorrelationToken correlationToken) => correlationToken.ToString();
    }
}
