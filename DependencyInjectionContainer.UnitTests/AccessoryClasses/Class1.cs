using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer.UnitTests.AccessoryClasses
{
    class Class1 : Interface1
    {
        public Interface2 interface2 { get; set; }
        public Class1(Interface2 class2)
        {
            this.interface2 = interface2;
        }
    }
}
