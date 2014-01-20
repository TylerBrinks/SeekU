using System;

namespace SeekU
{
    internal static class Extensions
    {
        internal static object InvokeGenericMethod(this object instance, string methodName, Type genericType, params object[] parameters)
        {
            var method = instance.GetType().GetMethod("SaveSnapshot").MakeGenericMethod(genericType);
            return method.Invoke(instance, parameters);
        }
    }
}
