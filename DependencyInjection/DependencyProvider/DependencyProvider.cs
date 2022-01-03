using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DependencyInjection.DependencyConfiguration;
using DependencyInjection.DependencyConfiguration.ImplementationData;

namespace DependencyInjection.DependencyProvider
{
    public class DependencyProvider
    {
        private readonly DependencyConfig _configuration;
        public readonly Dictionary<Type, List<SingletonContainer>> _singletons;
        private readonly Stack<Type> _recursionStack = new Stack<Type>();
        private Dictionary<Type, Type> nullParameters = new Dictionary<Type, Type>();
        private static List<object> toFill = new List<object>();
        private ResolveABAService resolveService;
        
        public DependencyProvider(DependencyConfig configuration)
        {
            ConfigValidator configValidator = new ConfigValidator(configuration);
            if (!configValidator.Validate())
            {
                throw new ArgumentException("Wrong configuration");
            }
         
            this._singletons = new Dictionary<Type, List<SingletonContainer>>();
            this._configuration = configuration;
            this.resolveService = ResolveABAService.getInstance(this);
        }
        private void fillWithSingleton(object singleton)
        {
            foreach (object o in toFill)
            {
                var fields = o.GetType().GetFields();
                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType.IsAssignableFrom(singleton.GetType()))
                    {
                        field.SetValue(o, singleton);
                    }
                }
                var properties = o.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType.IsAssignableFrom(singleton.GetType()) && property.CanWrite)
                    {
                        property.SetValue(o, singleton);
                    }
                }

            }
        }
        public static object CreateInstance(Type type, DependencyConfig dependencyConfiguration)
        {

            var constructors = ChooseConstructors(type).ToList();
            if (constructors.Count == 0) throw new Exception($"{type} has no injectable constructor");
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                var arguments = ProvideParameters(parameters, dependencyConfiguration);
                return constructor.Invoke(arguments.ToArray());
            }

            throw new Exception($"Can't create instance of {type}");
        }

        private static IEnumerable<object> ProvideParameters(IEnumerable<ParameterInfo> parameters,
            DependencyConfig dependencyConfiguration)
        {
            var provider = new DependencyProvider(dependencyConfiguration);
            return parameters.Select(provider.Resolve);
        }
        /*        private static IEnumerable<object> ProvideFields(IEnumerable<FieldInfo> fields, DependenciesConfiguration dependencyConfiguration)
                {

                    var provider = new DependencyProvider(dependencyConfiguration);
                    return fields.Select(provider.Resolve);
                }*/

        private static IEnumerable<ConstructorInfo> ChooseConstructors(Type type)
        {
            return type.GetConstructors()
                .Where(HasConstructedParameters);
        }

        private static bool HasConstructedParameters(ConstructorInfo constructor)
        {
            return constructor.GetParameters()
                .All(IsParameterConstructable);
        }

        private static bool IsParameterConstructable(ParameterInfo parameter)
        {
            var parameterType = parameter.GetType();
            return parameterType.IsClass;
        }
    
    private object ResolveDependency(Dependency dependency)
        {
            object result = null;

            if (_configuration.IsExcluded(dependency.Type))
                //throw new DependencyException($"Dependency type {dependency.Type} leads to recursion!");
                return null;
            _configuration.ExcludeType(dependency.Type);

            if (dependency.LifeCycle == LifeCycle.InstancePerDependency)
            {
                result = CreateInstance(dependency.Type, _configuration);
            }
            else if (dependency.LifeCycle == LifeCycle.Singleton)
            {
                lock (dependency)
                {
                    if (dependency.Instance == null)
                    {
                        result =CreateInstance(dependency.Type, _configuration);
                        dependency.Instance = result;
                        //singletons.Add(result);
                        fillWithSingleton(result);
                    }
                    else
                    {
                        result = dependency.Instance;
                    }
                }
            }
            toFill.Add(result);
            //fillObjects();
            _configuration.RemoveFromExcluded(dependency.Type);

            return result;
        }
        /* public void FinalInit(Type type )
         {
             foreach (var s in _singletons)
             {

              PropertyInfo[] prp=  s.Value[0].Instance.GetType().GetProperties();
                 foreach(var p in prp)
                 {
                     if (p.PropertyType.IsAssignableFrom(s.Value[0].Instance.GetType()) && p.CanWrite)
                     {
                         p.SetValue(o, s.Value[0].Instance);
                     }
                     //  Type myTypeB =s.Value[0].Instance.GetType();
                     //  Console.WriteLine(myTypeB);
                     //  FieldInfo myFieldInfo1 = myTypeB.GetField("instance",
                     //      BindingFlags.NonPublic | BindingFlags.Instance);

                     if (p.GetValue(s.Value[0].Instance) == null ){
                         SingletonContainer container;
                         object replaceObject = Resolve(type, ImplNumber.Any);
                         p.SetValue(s.Value[0].Instance, replaceObject);
                     }
                 }

             }
         }*/
        /*  private void fillWithSingleton(object singleton)
          {
              foreach (object o in toFill)
              {
                  var fields = o.GetType().GetFields();
                  foreach (FieldInfo field in fields)
                  {
                      if (field.FieldType.IsAssignableFrom(singleton.GetType()))
                      {
                          field.SetValue(o, singleton);
                      }
                  }
                  var properties = o.GetType().GetProperties();
                  foreach (PropertyInfo property in properties)
                  {
                      if (property.PropertyType.IsAssignableFrom(singleton.GetType()) && property.CanWrite)
                      {
                          property.SetValue(o, singleton);
                      }
                  }

              }
          }*/
        /**
         *  functions that  resolves dependencies using recursion
         * @param ImplNumber number 
         * @return TDependency
        **/
        public TDependency Resolve<TDependency>(ImplNumber number = ImplNumber.Any)
            where TDependency : class
        {
            TDependency dep = (TDependency)Resolve(typeof(TDependency), number);
            return dep;
        }

        public object Resolve(Type dependencyType, ImplNumber number = ImplNumber.Any)
        {

            if (_recursionStack.Contains(dependencyType))
            {
                return null;
            }

            _recursionStack.Push(dependencyType);

            object result;
            if (IsIEnumerable(dependencyType))
            {
                result = CreateEnumerable(dependencyType.GetGenericArguments()[0]);
            }
            else
            {
                ImplContainer container = GetImplContainerByDependencyType(dependencyType, number);
      
                
                    Type requiredType = GetGeneratedType(dependencyType, container.ImplementationsType);

                    result = this.ResolveNonIEnumerable(requiredType, container.TimeToLive, dependencyType, container.ImplNumber);

              
            }

           

            return result;
        }

        private object ResolveNonIEnumerable(Type implType, LifeCycle ttl, Type dependencyType,
            ImplNumber number)
        {
            
            if (ttl != LifeCycle.Singleton)
            {
                return CreateInstance(dependencyType,implType);
            }

            if (!IsInSingletons(dependencyType, implType, number))
            {
               
                var result = CreateInstance(dependencyType,implType);
                AddToSingletons(dependencyType, result, number);
                    _recursionStack.Pop();
               
                resolveService.replaceParametersOfNullObject(dependencyType,nullParameters,_recursionStack);
            }
            //FinalInit(dependencyType);
            return _singletons[dependencyType]
                   .Find(singletonContainer => number.HasFlag(singletonContainer.ImplNumber))
                   ?.Instance;
        }
      

        private object CreateInstance(Type dependecyType, Type implementationType)
        {
            var constructors = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            foreach (var constructor in constructors)
            {
                var constructorParams = constructor.GetParameters();
                var generatedParams = new List<dynamic>();
                foreach (var parameterInfo in constructorParams)
                {
                    dynamic parameter;
                    if (parameterInfo.ParameterType.IsInterface)
                    {
                        var number = parameterInfo.GetCustomAttribute<DependencyKeyAttribute>()?.ImplNumber ?? ImplNumber.Any;
                        parameter = Resolve(parameterInfo.ParameterType, number);
                        if(parameter == null)
                        {
                            if (!nullParameters.ContainsKey(dependecyType))
                            {
                                nullParameters.Add(dependecyType, parameterInfo.ParameterType);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }

                    generatedParams.Add(parameter);
                }

                return constructor.Invoke(generatedParams.ToArray());
            }

            throw new ArgumentException("Cannot create instance of class");
        }

        private ImplContainer GetImplContainerByDependencyType(Type dependencyType, ImplNumber number)
        {
            ImplContainer container;
            if (dependencyType.IsGenericType)
            {
                container = GetImplementationsContainerLast(dependencyType, number);
                container ??= GetImplementationsContainerLast(dependencyType.GetGenericTypeDefinition(), number);
            }
            else
            {
                container = GetImplementationsContainerLast(dependencyType, number);
            }

            return container;
        }

        private bool IsIEnumerable(Type dependencyType)
        {
            return dependencyType.GetInterfaces().Any(i => i.Name == "IEnumerable");
        }



        private Type GetGeneratedType(Type dependencyType, Type implementationType)
        {
            if (dependencyType.IsGenericType && implementationType.IsGenericTypeDefinition)
            {
                return implementationType.MakeGenericType(dependencyType.GetGenericArguments());
            }

            return implementationType;
        }

        private IList CreateEnumerable(Type dependencyType)
        {
            var implementationList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(dependencyType));
            var implementationsContainers = this._configuration.DependenciesDictionary[dependencyType];
            foreach (var implementationContainer in implementationsContainers)
            {
                var instance = this.ResolveNonIEnumerable(implementationContainer.ImplementationsType,
                    implementationContainer.TimeToLive, dependencyType, implementationContainer.ImplNumber);
                implementationList.Add(instance);
            }

            return implementationList;
        }

        private ImplContainer GetImplementationsContainerLast(Type dependencyType, ImplNumber number)
        {
            if (this._configuration.DependenciesDictionary.ContainsKey(dependencyType))
            {
                return this._configuration.DependenciesDictionary[dependencyType]
                    .FindLast(container => number.HasFlag(container.ImplNumber));
            }

            return null;
        }

        private void AddToSingletons(Type dependencyType, object implementation, ImplNumber number)
        {
            if (this._singletons.ContainsKey(dependencyType))
            {
                this._singletons[dependencyType].Add(new SingletonContainer(implementation, number));
            }
            else
            {
                this._singletons.Add(dependencyType, new List<SingletonContainer>()
                {
                    new SingletonContainer(implementation, number)
                });
            }
        }

        private bool IsInSingletons(Type dependencyType, Type implType, ImplNumber number)
        {
            var lst = this._singletons.ContainsKey(dependencyType) ? this._singletons[dependencyType] : null;
            return lst?.Find(container => number.HasFlag(container.ImplNumber) && container.Instance.GetType() == implType) is not null;
        }
    }
    public class Dependency
    {
        public Type Type { get; }

        public LifeCycle LifeCycle { get; }

        public object Key { get; }

        public object Instance { get; set; }

        public Dependency(Type type, LifeCycle lifeCycle, object key)
        {
            Key = key;
            Type = type;
            LifeCycle = lifeCycle;
        }
    }
}