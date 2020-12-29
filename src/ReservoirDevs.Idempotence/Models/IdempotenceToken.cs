namespace ReservoirDevs.Idempotence.Models
{
    public class IdempotenceToken
    {
        private readonly string _tokenValue;

        public override string ToString() => _tokenValue;

        public IdempotenceToken(string value)
        {
            _tokenValue = value;
        }

        public static implicit operator IdempotenceToken(string value) => new IdempotenceToken(value);

        public static implicit operator string(IdempotenceToken idempotenceToken) => idempotenceToken.ToString();
    }
}