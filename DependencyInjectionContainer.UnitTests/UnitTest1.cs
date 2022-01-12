using System;
using NUnit.Framework;
using DependencyInjection.DependencyConfiguration;
using DependencyInjection.DependencyConfiguration.ImplementationData;
using DependencyInjection.DependencyProvider;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using LifeCycle = DependencyInjection.DependencyConfiguration.ImplementationData.LifeCycle;

namespace TestProject1
{
    public class Tests
    {
        private DependencyConfig DependenciesConfiguration1;
        private DependencyConfig DependenciesConfiguration2;

        [SetUp]
        public void Setup()
        {
            DependenciesConfiguration1 = new DependencyConfig();
            DependenciesConfiguration1.Register<ISomeInterface, Class>();
            DependenciesConfiguration1.Register<ITestClass, TestClass>();
            DependenciesConfiguration2 = new DependencyConfig();
            DependenciesConfiguration2.Register<ISomeInterface, Class>();
            DependenciesConfiguration2.Register<ISomeInterface, Class2>();
            DependenciesConfiguration2.Register<ITestClass, TestClass>(LifeCycle.InstancePerDependency,ImplNumber.First);
            DependenciesConfiguration2.Register<ITestClass, TestClass2>(LifeCycle.InstancePerDependency,ImplNumber.Second);
        }
        [Test]
        public void Init_DependenciesConfigurationCreatedSuccessfully()
        {
            Assert.NotNull(DependenciesConfiguration1);
            Assert.IsNotEmpty(DependenciesConfiguration1.DependenciesDictionary);
            Assert.NotNull( DependenciesConfiguration2);
            Assert.IsNotEmpty(DependenciesConfiguration2.DependenciesDictionary);
        }
        [Test]
        public void RegisteringDependencies()
        {
            bool keyOfISomeInterface =DependenciesConfiguration1.DependenciesDictionary.ContainsKey(typeof(ISomeInterface));
            bool keyOfITestClass =DependenciesConfiguration1.DependenciesDictionary.ContainsKey(typeof(ITestClass));
            int numberOfKeys=DependenciesConfiguration1.DependenciesDictionary.Keys.Count;
            Assert.IsTrue(keyOfISomeInterface, "Dependency dictionary hasn't key ISomeInterface.");
            Assert.IsTrue(keyOfITestClass, "Dependency dictionary hasn't key ITestClass.");
            Assert.AreEqual(numberOfKeys, 2,"Dependency dictionary has another number of keys.");
        }

        [Test]
        public void RegisterDoubleDependency()
        {
            var containers = DependenciesConfiguration2.DependenciesDictionary[typeof(ISomeInterface)];
            var firstType = containers[0].ImplementationsType;
            var secondType = containers[1].ImplementationsType;
            int numberOfKeys = DependenciesConfiguration2.DependenciesDictionary.Keys.Count;
            Assert.AreEqual(containers.Count, 2, "Wrong number of dependencies of IInterface.");
            Assert.AreEqual(firstType, typeof(Class), "Another type of class Class in container.");
            Assert.AreEqual(secondType, typeof(Class2), "Another type of class Class2 in container.");
            Assert.AreEqual(numberOfKeys, 2,"Dependency dictionary has another number of keys.");
        }

        [Test]
        public void SimpleDependencyProvider()
        {
            var provider = new DependencyProvider(DependenciesConfiguration1);
            var result = provider.Resolve<ITestClass>();
            var innerInterface = ((TestClass)result).isomeInterface;
            Assert.AreEqual(result.GetType(), typeof(TestClass),"Wrong type of resolving result.");
            Assert.AreEqual(innerInterface == null, false, "Error in creating an instance of dependency.");
            Assert.AreEqual(innerInterface.GetType(), typeof(Class), "Wrong type of created dependency.");
        }

        [Test]
        public void DoubleDependencyProvider()
        {
            var provider = new DependencyProvider(DependenciesConfiguration2);
            var result = provider.Resolve<ITestClass>();
            var innerInterface = ((TestClass2)result).isomeInterface;
            Assert.AreEqual(innerInterface.GetType(),typeof(Class2),"Wrong type of created instance.");
        }

        [Test]
        public void SingletonObj()
        {
            var dep1 = new DependencyConfig();
            dep1.Register<ISomeInterface, Class>(LifeCycle.Singleton);
            dep1.Register<ITestClass, TestClass>(LifeCycle.Singleton);
            var provider = new DependencyProvider(dep1);
            var obj11 = provider.Resolve<ITestClass>();
            var obj12 = provider.Resolve<ITestClass>();
            var b1 = obj11 == obj12;
            
            int count1 = provider._singletons.Count;
            Assert.AreEqual(count1, 2, "Wrong number of Singleton objects in Dictionary for Singleton");
            var dep2 = new DependencyConfig();
            dep2.Register<ISomeInterface, Class>(LifeCycle.InstancePerDependency);
            dep2.Register<ITestClass, TestClass>(LifeCycle.InstancePerDependency);
            var provider2 = new DependencyProvider(dep2);
            var obj21 = provider2.Resolve<ITestClass>();
            var obj22 = provider2.Resolve<ITestClass>();
            var b2 = obj21 == obj22;

            int count2 = provider2._singletons.Count;
            Assert.AreEqual(count2, 0, "Wrong number of Singleton objects in Dictionary for InstancePerDependency");
            Assert.IsTrue(b1, "Different objects for singleton object.");
            Assert.IsFalse(b2, "The same object using InstancePerDependency");
        }
        // А-Б-А
        [Test]
        public void ABA_test()
        {
            var dependencies = new DependencyConfig();
            var provider = new DependencyProvider(dependencies);
            dependencies.Register<IA, ClassA>(LifeCycle.Singleton, ImplNumber.First);
            dependencies.Register<IB, ClassB>(LifeCycle.Singleton, ImplNumber.First);
            ClassA a = (ClassA)provider.Resolve<IA>(ImplNumber.First);
            ClassB b = (ClassB)provider.Resolve<IB>(ImplNumber.First);
            Assert.AreSame(a,b.ia);
        }
        
        
        [Test]
        public void selfdependency_Test()
        {
            var dependencies = new DependencyConfig();
            var provider = new DependencyProvider(dependencies);
            dependencies.Register<ISelf, Self>(LifeCycle.Singleton, ImplNumber.First);
            Self self = (Self)provider.Resolve<ISelf>(ImplNumber.First);
            Assert.IsTrue(self.iself.GetType().Equals(typeof(Self)));
            Assert.AreSame(self,self.iself);
        }
        
        [Test]
        public void ABCA_test()
        {
            var dependencies = new DependencyConfig();
            var provider = new DependencyProvider(dependencies);
            dependencies.Register<IQ, Q>(LifeCycle.Singleton, ImplNumber.First);
            dependencies.Register<IW, W>(LifeCycle.Singleton, ImplNumber.First);
            dependencies.Register<IE, E>(LifeCycle.Singleton, ImplNumber.First);
            Q q = (Q)provider.Resolve<IQ>(ImplNumber.First);
            W w = (W)provider.Resolve<IW>(ImplNumber.First);
            E e = (E)provider.Resolve<IE>(ImplNumber.Any);
            Assert.AreSame(q,e.iq);
        }
    }
    
}