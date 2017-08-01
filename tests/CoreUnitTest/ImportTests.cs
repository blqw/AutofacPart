using blqw.Autofac;
using System;
using Xunit;

namespace CoreUnitTest
{
    public class ImportTests
    {
        interface ITestInterface
        {
            string ID { get; }
        }

        [Export(typeof(ITestInterface))]
        class TestClass1 : ITestInterface
        {
            public string ID => "TestClass1";
        }

        class TestClass2
        {
            [Import]
            public ITestInterface TestInterface { get; private set; }


            [Import]
            public readonly ITestInterface TestInterface2 = null;
        }

        static class TestClass3
        {
            [Import]
            public static ITestInterface TestInterface { get; private set; }
            [Import]
            public static readonly ITestInterface TestInterface2 = null;
        }

        [Fact]
        public void InstanceProperty()
        {
            var test2 = new TestClass2();
            Assert.Null(test2.TestInterface);
            PartContainer.Fill(test2);
            Assert.NotNull(test2.TestInterface);
            Assert.IsType<TestClass1>(test2.TestInterface);
            Assert.Equal("TestClass1", test2.TestInterface.ID);

        }

        [Fact]
        public void StaticPropertyAndField()
        {
            Assert.Null(TestClass3.TestInterface);
            Assert.Null(TestClass3.TestInterface2);

            PartContainer.Fill(typeof(TestClass3));

            Assert.NotNull(TestClass3.TestInterface);
            Assert.IsType<TestClass1>(TestClass3.TestInterface);
            Assert.Equal("TestClass1", TestClass3.TestInterface.ID);

            Assert.NotNull(TestClass3.TestInterface2);
            Assert.IsType<TestClass1>(TestClass3.TestInterface2);
            Assert.Equal("TestClass1", TestClass3.TestInterface2.ID);
        }

        [Fact]
        public void InstanceField()
        {
            var test2 = new TestClass2();
            Assert.Null(test2.TestInterface2);
            PartContainer.Fill(test2);
            Assert.NotNull(test2.TestInterface2);
            Assert.IsType<TestClass1>(test2.TestInterface2);
            Assert.Equal("TestClass1", test2.TestInterface2.ID);
        }
    }
}
