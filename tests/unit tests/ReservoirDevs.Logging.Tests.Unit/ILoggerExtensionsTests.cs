using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using ReservoirDevs.Logging.Extensions;
using Xunit;

namespace ReservoirDevs.Logging.Tests.Unit
{
    // ReSharper disable once InconsistentNaming
    public class ILoggerExtensionsTests
    {
        private const string TypeKey = "type";
        private const string MethodKey = "method";

        private readonly Mock<ILogger<ILoggerExtensionsTests>> _typedLogger;

        public ILoggerExtensionsTests()
        {
            _typedLogger = new Mock<ILogger<ILoggerExtensionsTests>>();
        }

        [Fact]
        public void CreateScope_CallsBeginScope_PassingInDictionaryContainingClassAndMethodNames()
        {
            _typedLogger.Object.CreateScope(nameof(CreateScope_CallsBeginScope_PassingInDictionaryContainingClassAndMethodNames));
            _typedLogger.Verify(logger => logger.BeginScope(It.Is<IDictionary<string, string>>(dictionary => ContainsKeyAndValue(dictionary, TypeKey, typeof(ILoggerExtensionsTests).ToString()) && ContainsKeyAndValue(dictionary, MethodKey, nameof(CreateScope_CallsBeginScope_PassingInDictionaryContainingClassAndMethodNames)))), Times.Once);
        }

        [Fact]
        public void CreateScope_CallsBeginScope_PassingInDictionaryContainingClassAndMethodNamesAndParamValues()
        {
            _typedLogger.Object.CreateScope(nameof(CreateScope_CallsBeginScope_PassingInDictionaryContainingClassAndMethodNames), new KeyValuePair<string, string>("A", "B"), new KeyValuePair<string, string>("C", "D"));
            _typedLogger.Verify(logger => logger.BeginScope(It.Is<IDictionary<string, string>>(dictionary => ContainsKeyAndValue(dictionary, TypeKey, typeof(ILoggerExtensionsTests).ToString()) && ContainsKeyAndValue(dictionary, MethodKey, nameof(CreateScope_CallsBeginScope_PassingInDictionaryContainingClassAndMethodNames)) && ContainsKeyAndValue(dictionary, "A", "B") && ContainsKeyAndValue(dictionary, "C", "D"))), Times.Once);
        }

        [Fact]
        public void CreateScope_CallsBeginScope_PassingInDictionaryContainingClassAndMethodNamesAndDictionaryValues()
        {
            _typedLogger.Object.CreateScope(nameof(CreateScope_CallsBeginScope_PassingInDictionaryContainingClassAndMethodNames), new Dictionary<string, string> { { "A", "B" }, { "C", "D" } });
            _typedLogger.Verify(logger => logger.BeginScope(It.Is<IDictionary<string, string>>(dictionary => ContainsKeyAndValue(dictionary, TypeKey, typeof(ILoggerExtensionsTests).ToString()) && ContainsKeyAndValue(dictionary, MethodKey, nameof(CreateScope_CallsBeginScope_PassingInDictionaryContainingClassAndMethodNames)) && ContainsKeyAndValue(dictionary, "A", "B") && ContainsKeyAndValue(dictionary, "C", "D"))), Times.Once);
        }

        private static bool ContainsKeyAndValue(IDictionary<string, string> dictionary, string key, string value)
        {
            return dictionary.ContainsKey(key) && dictionary[key].Equals(value, StringComparison.InvariantCulture);
        }
    }
}