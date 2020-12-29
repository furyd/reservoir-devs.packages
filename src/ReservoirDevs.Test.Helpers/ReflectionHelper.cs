using System;
using System.Reflection;

namespace ReservoirDevs.Test.Helpers
{
    public static class ReflectionHelper
    {
        public static TOutput GetField<TInput, TOutput>(TInput input, string fieldName)
        {
            var field = typeof(TInput).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                throw new Exception($"{fieldName} not found");
            }

            return (TOutput)field.GetValue(input);
        }

        public static TOutput GetProperty<TInput, TOutput>(TInput input, string fieldName)
        {
            var field = typeof(TInput).GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                throw new Exception($"{fieldName} not found");
            }

            return (TOutput)field.GetValue(input);
        }

        public static MethodInfo GetMethod<TInput>(string methodName)
        {
            var method = typeof(TInput).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            
            return method ?? throw new Exception($"{methodName} not found");
        }
    }
}