using System;
using FluentAssertions;
using Xunit;

namespace ReservoirDevs.Test.Helpers.Tests.Unit
{
    public class ReflectionHelperTests
    {
        private string _privateField = nameof(_privateField);

        private string PrivateProperty => nameof(PrivateProperty);

        private string PrivateStaticMethod = nameof(PrivateStaticMethod);

        [Fact]
        public void GetField_DoesNotThrowException_WhenFieldExists()
        {
            var sut = new HarnessClass();
            
            Action action = () => ReflectionHelper.GetField<HarnessClass, string>(sut, _privateField);

            action.Should().NotThrow();
        }

        [Fact]
        public void GetField_ThrowsException_WhenFieldDoesNotExist()
        {
            var sut = new HarnessClass();

            Action action = () => ReflectionHelper.GetField<HarnessClass, string>(sut, $"{_privateField}a");

            action.Should().Throw<Exception>();
        }

        [Fact]
        public void GetField_ReturnsPrivateFieldValue()
        {
            const string testValue = "A";
            
            var sut = new HarnessClass();

            var result = ReflectionHelper.GetField<HarnessClass, string>(sut, _privateField);

            result.Should().BeNullOrWhiteSpace();
            
            sut.UpdatePrivateField(testValue);

            result = ReflectionHelper.GetField<HarnessClass, string>(sut, _privateField);

            result.Should().Be(testValue);
        }

        [Fact]
        public void GetProperty_DoesNotThrowException_WhenPropertyExists()
        {
            var sut = new HarnessClass();

            Action action = () => ReflectionHelper.GetProperty<HarnessClass, string>(sut, PrivateProperty);

            action.Should().NotThrow();
        }

        [Fact]
        public void GetProperty_ThrowsException_WhenPropertyDoesNotExist()
        {
            var sut = new HarnessClass();

            Action action = () => ReflectionHelper.GetProperty<HarnessClass, string>(sut, $"{PrivateProperty}a");

            action.Should().Throw<Exception>();
        }

        [Fact]
        public void GetProperty_ReturnsPrivatePropertyValue()
        {
            const string testValue = "A";

            var sut = new HarnessClass();

            var result = ReflectionHelper.GetProperty<HarnessClass, string>(sut, PrivateProperty);

            result.Should().BeNullOrWhiteSpace();

            sut.UpdatePrivateProperty(testValue);

            result = ReflectionHelper.GetProperty<HarnessClass, string>(sut, PrivateProperty);

            result.Should().Be(testValue);
        }

        [Fact]
        public void GetMethod_DoesNotThrowException_WhenMethodExists()
        {
            Action action = () => ReflectionHelper.GetMethod<HarnessClass>(PrivateStaticMethod);

            action.Should().NotThrow();
        }

        [Fact]
        public void GetMethod_ThrowsException_WhenMethodDoesNotExist()
        {
            Action action = () => ReflectionHelper.GetMethod<HarnessClass>($"{PrivateStaticMethod}a");

            action.Should().Throw<Exception>();
        }

        [Fact]
        public void GetMethod_ReturnsValue()
        {
            var sut = ReflectionHelper.GetMethod<HarnessClass>(PrivateStaticMethod);

            sut.Should().NotBeNull();
            
            Action action = () => sut.Invoke(new HarnessClass(), new object[] { true });
            
            action.Should().NotThrow();
        }
    }
}
