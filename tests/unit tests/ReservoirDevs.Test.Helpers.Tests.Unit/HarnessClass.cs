namespace ReservoirDevs.Test.Helpers.Tests.Unit
{
    public sealed class HarnessClass
    {
        // ReSharper disable once NotAccessedField.Local
        private string _privateField;

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private string PrivateProperty { get; set; }

        // ReSharper disable once UnusedMember.Local
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