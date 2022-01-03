using System;
using System.Collections.Generic;
using DependencyInjection.DependencyConfiguration.ImplementationData;
using System.Reflection;

namespace DependencyInjection.DependencyProvider
{
     class ResolveABAService
    {
        private static ResolveABAService instance;
        private DependencyProvider provider;
        private ResolveABAService(DependencyProvider provider)
        {
            this.provider = provider;
        }

        public static ResolveABAService getInstance(DependencyProvider provider)
        {
            if (instance == null)
                instance = new ResolveABAService( provider);
            return instance;
        }
        /**
      * function replaceParametersOfNullObject change null on object in circular dependency
      * @param Type replaceType
     **/
        public void replaceParametersOfNullObject(Type replaceType, Dictionary<Type, Type> nullParameters, Stack<Type> _recursionStack)
        {
         
            foreach (KeyValuePair<Type, Type> keyValuePair in nullParameters)
            {
                if (replaceType == keyValuePair.Value)
                {
                    object objectWithNull = provider.Resolve(keyValuePair.Key, ImplNumber.Any);
                    PropertyInfo[] propertyInfos = objectWithNull.GetType().GetProperties();
                    foreach (PropertyInfo property in propertyInfos)
                    {
                        object replaceObject = provider.Resolve(replaceType, ImplNumber.Any);
                        if (property.PropertyType.IsAssignableFrom(replaceObject.GetType()) && property.CanWrite)
                        {
                            property.SetValue(objectWithNull, replaceObject);
                        }
                    }
                 //   for (int i = 0; i < propertyInfos.Length; i++)
                 //   {
                 //       if (propertyInfos[i].PropertyType == keyValuePair.Value)
                 //       {//сравниниваем что в диктионари и проперти
                 //           _recursionStack.Pop();//достаем ненужное
                 //           object replaceObject = provider.Resolve(replaceType, ImplNumber.Any);
                 //           objectWithNull.GetType().GetProperty(propertyInfos[i].Name)?.SetValue(objectWithNull, replaceObject);
                 //           break;
                 //       }
                 //   }
                }
            }
        }
        public void replaceParametersOfNullObject2(Type replaceType,Type replaceType2, Dictionary<Type, Type> nullParameters, Stack<Type> _recursionStack)
        {

            foreach (KeyValuePair<Type, Type> keyValuePair in nullParameters)
            {
                if (replaceType == keyValuePair.Value)
                {
                    object objectWithNull = provider.Resolve(keyValuePair.Key, ImplNumber.Any);
                    PropertyInfo[] propertyInfos = objectWithNull.GetType().GetProperties();
                    for (int i = 0; i < propertyInfos.Length; i++)
                    {
                        if (propertyInfos[i].PropertyType == keyValuePair.Value)
                        {//сравниниваем что в диктионари и проперти
                            _recursionStack.Pop();
                            object replaceObject = provider.Resolve(replaceType, ImplNumber.Any);
                            objectWithNull.GetType().GetProperty(propertyInfos[i].Name)?.SetValue(objectWithNull, replaceObject);
                            break;
                        }
                    }
                }
            }
        }
    }
}
