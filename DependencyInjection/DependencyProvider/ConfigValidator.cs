using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection.DependencyConfiguration;
using System.Reflection;

namespace DependencyInjection.DependencyProvider
{
    public class ConfigValidator
    {
        private readonly Stack<Type> _nestedTypes;
        private readonly DependencyConfig _configuration;

        public ConfigValidator(DependencyConfig configuration)
        {
            this._configuration = configuration;
            this._nestedTypes = new Stack<Type>();
        }
        /**
         *  function that  checks if the type is in the container
         * @param Type type
         * @return bool
         **/
        private bool IsInContainer(Type type)
        {
            return this._configuration._dependencies.ContainsKey(type);
        }
        /**
        *  function that  checks if the type can be created
        * @param Type type
        * @return bool
        **/
        private bool CanBeCreated(Type instanceType)
        {
            this._nestedTypes.Push(instanceType);
            var constructors = instanceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            foreach (var constructor in constructors)
            {
                var requiredParams = constructor.GetParameters();
                foreach (var parameter in requiredParams)
                {
                    Type parameterType;
                    if (parameter.ParameterType.ContainsGenericParameters)
                    {
                        parameterType = parameter.ParameterType.GetInterfaces()[0];
                    }
                    else if (parameter.ParameterType.GetInterfaces().Any(i => i.Name == "IEnumerable"))
                    {
                        parameterType = parameter.ParameterType.GetGenericArguments()[0];
                    }
                    else
                    {
                        parameterType = parameter.ParameterType;
                    }

                    if (parameterType.IsInterface && IsInContainer(parameterType)) continue;
                    this._nestedTypes.Pop();
                    return false;
                }
            }

            this._nestedTypes.Pop();
            return true;
        }
        /**
        *  function that  checks if the all types can be created
        * @return bool
        **/
        public bool Validate()
        {
            return this._configuration._dependencies.Values.
                All(implementations => implementations.
                    All(implementation => CanBeCreated(implementation.Type)));
        }
    }
}
