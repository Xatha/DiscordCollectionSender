using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestingConsoleApp
{
    public class FactoryContainer
    {
        private static readonly ConcurrentDictionary<Type, Type> _types = new();
        private static readonly ConcurrentDictionary<Type, ObjectActivator> _transientCache = new();
        private static readonly ConcurrentDictionary<Type, ObjectActivator> _singletonCache = new();

        private delegate object ObjectActivator();

        public FactoryContainer()
        {

        }

        public void Register<TImplementation, TInterface>() where TImplementation : class where TInterface : class
        {
            if (!typeof(TInterface).IsInterface || !typeof(TImplementation).IsClass)
            {
                throw new ArgumentException("Interface and concrete class implentation was not correctly assigned.");
            }

            _types.TryAdd(typeof(TInterface), typeof(TImplementation));
        }
        public void RegisterSingleton<TImplementation, TInterface>() where TImplementation : class where TInterface : class
        {
            if (!typeof(TInterface).IsInterface || !typeof(TImplementation).IsClass)
            {
                throw new ArgumentException("Interface and concrete class implentation was not correctly assigned.");
            }

            _types.TryAdd(typeof(TInterface), typeof(TImplementation));
        }

        public static T GetImplementation<T>() where T : class
        {
            return _transientCache.TryGetValue(typeof(T), out var cachedResult) ? (T)cachedResult() : GenerateImplementationByLambda<T>();
        }

        //Insanely slow, but allow us to cache a delegate which allow us to do use the cached delegate very fast.
        private static T GenerateImplementationByLambda<T>() where T : class
        {
            Type myType = _types[typeof(T)] ?? throw new NullReferenceException("Could not retrieve type.");
            ConstructorInfo constructor = myType.GetConstructor(Type.EmptyTypes) ?? throw new NullReferenceException("Could not retrieve ctor.");

            // Make a NewExpression that calls the ctor with the args we just created
            NewExpression newExp = Expression.New(constructor);

            // Create a lambda with the New expression as body and our param object[] as arg
            LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator), newExp);

            // Compile it
            var compiled = (ObjectActivator)lambda.Compile() ?? throw new NullReferenceException("Could not compile ctor.");

            //Add the delegate to the cache.
            _transientCache.TryAdd(typeof(T), compiled);

            return (T)compiled();
        }

        private T? GetImplementation_Private<T>()
        {
            //Console.WriteLine($"Trying to get key {typeof(T)}");
            try
            {
                Type concreteImpType = _types[typeof(T)];
                return (T?)Activator.CreateInstance(concreteImpType);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
