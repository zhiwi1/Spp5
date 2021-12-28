using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer.UnitTests.AccessoryClasses
{
    class Class2 : Interface2
    {
        public Interface1 interface1 { get; set; }
        public Class2(Interface1 class1)
        {
            this.interface1 = interface1;
        }
    }
}
