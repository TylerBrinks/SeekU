using System;
using System.Linq;
using System.Reflection;
using SeekU.Eventing;

namespace SeekU
{
    internal static class Extensions
    {
        /// <summary>
        /// Invokes a method with a generic signature
        /// </summary>
        /// <param name="instance">Object instance to invoke the method on</param>
        /// <param name="methodName">Name of the method to invoke</param>
        /// <param name="genericType">Type of generic parameter</param>
        /// <param name="parameters">Method parameters</param>
        /// <returns>Method's return value if any.</returns>
        internal static object InvokeGenericMethod(this object instance, string methodName, Type genericType, params object[] parameters)
        {
            var method = instance.GetType().GetMethod("SaveSnapshot").MakeGenericMethod(genericType);
            return method.Invoke(instance, parameters);
        }

        /// <summary>
        /// Get's a convention-based method name for a given domain event by finding a
        /// method with a given name and a single parameter of a given type.
        /// </summary>
        /// <param name="instance">Instance to query</param>
        /// <param name="methodName">Name of the method to query</param>
        /// <param name="parameterType">Type of the method parameter</param>
        /// <returns>Method info if it exists</returns>
        internal static MethodInfo GetAppliedEventMethodNamed(this object instance, string methodName, Type parameterType)
        {
            return instance.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase) &&
                    m.GetParameters().Count() == 1 &&
                    m.GetParameters()[0].ParameterType == parameterType);
        }

        /// <summary>
        /// Gets a convention-based method name for a domain event.
        /// 
        /// SomethingHappenedEvent returns a method name of OnSomethingHappend
        /// </summary>
        /// <param name="domainEvent">Domain event</param>
        /// <returns>Name of the associated method</returns>
        internal static string GetEventMethodName(this DomainEvent domainEvent)
        {
            var name = domainEvent.GetType().Name;
            if (name.EndsWith("Event", StringComparison.InvariantCultureIgnoreCase))
            {
                name = name.Substring(0, name.Length - 5);
            }

            return "On" + name;
        }
    }
}
