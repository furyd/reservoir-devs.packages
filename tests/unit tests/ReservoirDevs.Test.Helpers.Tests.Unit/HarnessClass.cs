namespace ReservoirDevs.Test.Helpers.Tests.Unit
{
    public sealed class HarnessClass
    {
        private string _privateField;

        private string PrivateProperty { get; set; }

        private static bool PrivateStaticMethod(bool value) => value;

        public void UpdatePrivateField(string value)
        {
            _privateField = value;
        }

        public void UpdatePrivateProperty(string value)
        {
            PrivateProperty = value;
        }
    }
}