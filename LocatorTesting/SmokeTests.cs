using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyLocator;

namespace LocatorTesting
{
    [TestClass]
    public class SmokeTests
    {
        [TestMethod]
        public void FactoryCreatesILocator()
        {
            var locator = LocatorFactory.GetLocator();
            Assert.IsNotNull(locator);
            Assert.IsInstanceOfType(locator, typeof(ILocator));
        }

        [TestMethod]
        public void ILocatorCanRegister()
        {
            var locator = LocatorFactory.GetLocator();
            locator.Register(typeof(ITestData), typeof(TestData));
        }

        [TestMethod]
        public void ILocatorCanBuildParameterless()
        {
            var locator = LocatorFactory.GetLocator();
            locator.Register(typeof(ITestData), typeof(TestData));
            var testData = locator.Locate(typeof(ITestData));
            Assert.IsNotNull(testData);
            Assert.IsInstanceOfType(testData, typeof(TestData));
        }

        [TestMethod]
        public void ILocatorCanBuildParameterized()
        {
            var locator = LocatorFactory.GetLocator();
            locator.Register(typeof(ITestData), typeof(TestData));
            locator.Register(typeof(IComplexTestData), typeof(ComplexTestData));
            var complexData = locator.Locate<IComplexTestData>();
            Assert.IsNotNull(complexData);
            Assert.IsInstanceOfType(complexData, typeof(ComplexTestData));
            Assert.IsNotNull(complexData.SimpleData);
            Assert.IsInstanceOfType(complexData.SimpleData, typeof(TestData));
        }

        [TestMethod]
        public void ReRegisteredTypesOverride()
        {
            var locator = LocatorFactory.GetLocator();
            locator.Register(typeof(ITestData), typeof(TestData));
            var testData = locator.Locate(typeof(ITestData));
            Assert.IsNotNull(testData);
            Assert.IsInstanceOfType(testData, typeof(TestData));
            locator.Register(typeof(ITestData), typeof(SecondTestData));
            var secondTestData = locator.Locate(typeof(ITestData));
            Assert.IsNotNull(secondTestData);
            Assert.IsInstanceOfType(secondTestData, typeof(SecondTestData));
        }
    }

    public interface ITestData
    {
        void Foo();
    }

    public interface IComplexTestData
    {
        ITestData SimpleData { get; set; }
        void Foo();
    }

    public class SecondTestData : ITestData
    {
        public void Foo()
        {
            // different code from TestData
        }
    }

    public class ComplexTestData : IComplexTestData
    {
        public ITestData SimpleData { get; set; }

        public ComplexTestData(ITestData data)
        {
            this.SimpleData = data;
        }

        public void Foo()
        {
            // some code right up in hurrr
        }
    }

    public class TestData : ITestData
    {
        public void Foo()
        {
            //some code or something
        }
    }
}
