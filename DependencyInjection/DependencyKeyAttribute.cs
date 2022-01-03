using System;
using DependencyInjection.DependencyConfiguration.ImplementationData;

namespace DependencyInjection
{
    //Задает элементы приложения, к которым допустимо применить атрибут
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyKeyAttribute : Attribute
    {
        public ImplNumber ImplNumber { get; }
        public object Key { get; internal set; }

        public DependencyKeyAttribute(ImplNumber number)
        {
            this.ImplNumber = number;
        }
    }
}
