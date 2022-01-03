using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyInjection.DependencyConfiguration.ImplementationData;

namespace DependencyInjection.DependencyConfiguration
{
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
