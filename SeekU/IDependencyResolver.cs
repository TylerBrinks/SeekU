using System;
using System.Collections.Generic;

namespace SeekU
{
    /// <summary>
    /// Represents a runtime dependency resolver 
    /// </summary>
    public interface IDependencyResolver : IDisposable
    {
        T Resolve<T>();
        IEnumerable<T> ResolveAll<T>();
        IEnumerable<object> ResolveAll(Type type); 
        object Resolve(Type type);

        void Register<TInterface, TConcrete>() where TInterface : class where TConcrete : TInterface;
        void Register<TInterface, TConcrete>(Action<TConcrete> configurationAction) where TInterface : class where TConcrete : TInterface;
        void Register<T>(T instance);
    }
}