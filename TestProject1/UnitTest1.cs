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
            Assert.IsNotEmpty(DependenciesConfiguration1._dependencies);
            Assert.NotNull( DependenciesConfiguration2);
            Assert.IsNotEmpty(DependenciesConfiguration2._dependencies);
        }
        [Test]
        public void RegisteringDependencies()
        {
            bool keyOfISomeInterface =DependenciesConfiguration1._dependencies.ContainsKey(typeof(ISomeInterface));
            bool keyOfITestClass =DependenciesConfiguration1._dependencies.ContainsKey(typeof(ITestClass));
            int numberOfKeys=DependenciesConfiguration1._dependencies.Keys.Count;
            Assert.IsTrue(keyOfISomeInterface, "Dependency dictionary hasn't key ISomeInterface.");
            Assert.IsTrue(keyOfITestClass, "Dependency dictionary hasn't key ITestClass.");
            Assert.AreEqual(numberOfKeys, 2,"Dependency dictionary has another number of keys.");
        }

        [Test]
        public void RegisterDoubleDependency()
        {
            var containers = DependenciesConfiguration2._dependencies[typeof(ISomeInterface)];
            var firstType = containers[0].Type;
            var secondType = containers[1].Type;
            int numberOfKeys = DependenciesConfiguration2._dependencies.Keys.Count;
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



       


        // А-Б-А
        [Test]
        public void ABA_test()
        {
            var dependencies = new DependencyConfig();
            var provider = new DependencyProvider(dependencies);
            dependencies.Register<IA, ClassA>(LifeCycle.Singleton, ImplNumber.First);
            dependencies.Register<IB, ClassB>(LifeCycle.Singleton, ImplNumber.First);
            dependencies.Register<IC, ClassC>(LifeCycle.Singleton, ImplNumber.First);
            ClassA a = (ClassA)provider.Resolve<IA>(ImplNumber.First);
            ClassB b = (ClassB)provider.Resolve<IB>(ImplNumber.First);
            ClassC c = (ClassC)provider.Resolve<IC>(ImplNumber.First);
            Assert.AreSame(a,b.ia);
            Assert.AreSame(b.ic, c);
            Assert.AreSame(c.ib, b);
        }
        
   
    
    }
    
}