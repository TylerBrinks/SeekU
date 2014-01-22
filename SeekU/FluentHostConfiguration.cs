using System;

namespace SeekU
{
    /// <summary>
    /// Fluent configuration helper class
    /// </summary>
    /// <typeparam name="TIoC">Depencency resolver type</typeparam>
    /// <typeparam name="T">Type to register</typeparam>
    public class FluentHostConfiguration<TIoC, T>
        where TIoC : IDependencyResolver, new()
        where T : class
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly SeekUHostConfiguration<TIoC> _configuration;

        public FluentHostConfiguration(SeekUHostConfiguration<TIoC> configuration, IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
            _configuration = configuration;
        }

        /// <summary>
        /// Register a specific instance with the dependency resolver.
        /// </summary>
        /// <param name="instance">Instance to register</param>
        /// <returns>Configuration</returns>
        public SeekUHostConfiguration<TIoC> Use(T instance)
        {
            _dependencyResolver.Register(instance);
            return _configuration;
        }
        
        /// <summary>
        /// Registers types with the dependency resolver
        /// </summary>
        /// <typeparam name="TConcrete">Type to resolve</typeparam>
        /// <returns>Configuration</returns>
        public SeekUHostConfiguration<TIoC> Use<TConcrete>() where TConcrete : T
        {
            return Use<TConcrete>(instance => { });
        }

        /// <summary>
        /// Registers types with the dependency resolver
        /// </summary>
        /// <typeparam name="TConcrete">Type to resolve</typeparam>
        /// <param name="configurationAction">Configuration action to perform upon object creation</param>
        /// <returns>Configuration</returns>
        public SeekUHostConfiguration<TIoC> Use<TConcrete>(Action<TConcrete> configurationAction) where TConcrete : T
        {
            _dependencyResolver.Register<T, TConcrete>(configurationAction);
            return _configuration;
        }
    }
}