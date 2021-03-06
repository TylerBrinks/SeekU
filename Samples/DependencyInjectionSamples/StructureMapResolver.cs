﻿using System;
using System.Collections.Generic;
using System.Linq;
using SampleDomain.Domain;
using SeekU;
using SeekU.Commanding;
using SeekU.Eventing;
using StructureMap;
using StructureMap.Graph;

namespace DependencyInjectionSamples
{
    public class StructureMapResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public StructureMapResolver()
        {
            ObjectFactory.Initialize(x => x.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType<BankAccount>();
                scan.WithDefaultConventions();
                scan.ConnectImplementationsToTypesClosing(typeof(IHandleCommands<>));
                scan.ConnectImplementationsToTypesClosing(typeof(IHandleDomainEvents<>));
            }));

            _container = ObjectFactory.Container;
        }

        public T Resolve<T>()
        {
            return _container.GetInstance<T>();
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return _container.GetAllInstances<T>();
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            var instances = _container.GetAllInstances(type);

            return instances.Cast<object>();
        }

        public object Resolve(Type type)
        {
            return _container.GetInstance(type);
        }

        public void Register<T, TK>()
            where T : class
            where TK : T
        {
            _container.Configure(x => x.For<T>().Use<TK>());
        }

        public void Register<T, TK>(Action<TK> configurationAction)
            where T : class
            where TK : T
        {
            _container.Configure(x => x.For<T>().Use<TK>().OnCreation(typeof(TK).FullName, configurationAction));
        }

        public void Register<T>(T instance) where T : class
        {
            _container.Configure(x => x.For<T>().Use(instance));
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}