using System.Threading;
using blqw.Autofac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace CoreUnitTest
{
    public class ImportManyTests
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

        [Export(typeof(ITestInterface))]
        class TestClass2 : ITestInterface
        {
            public string ID => "TestClass2";
        }

        class TestClass3
        {
            [ImportMany]
            public static List<ITestInterface> TestInterface { get; private set; }
            [ImportMany]
            public static readonly ITestInterface[] TestInterface2 = null;
            [ImportMany]
            public IList<ITestInterface> TestInterface3 { get; private set; }
            [ImportMany]
            public readonly Collection<ITestInterface> TestInterface4 = null;
        }

        [Fact]
        public void Test()
        {
            var test3 = new TestClass3();
            PartContainer.Fill(test3);
            PartContainer.Fill(typeof(TestClass3));
            
            foreach (var test in new[] { TestClass3.TestInterface, TestClass3.TestInterface2 , test3.TestInterface3, test3.TestInterface4 })
            {
                Assert.NotNull(test);
                Assert.Equal(2, test.Count);
                if (test[0] is TestClass1)
                {
                    Assert.IsType<TestClass1>(test[0]);
                    Assert.Equal("TestClass1", test[0].ID);
                    Assert.IsType<TestClass2>(test[1]);
                    Assert.Equal("TestClass2", test[1].ID);
                }
                else
                {
                    Assert.IsType<TestClass2>(test[0]);
                    Assert.Equal("TestClass2", test[0].ID);
                    Assert.IsType<TestClass1>(test[1]);
                    Assert.Equal("TestClass1", test[1].ID);
                }
            }
        }
        
    }
}
