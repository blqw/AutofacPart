using System.Linq;
using blqw.Autofac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace CoreUnitTest
{
    public class InheritedTests
    {
        
        [InheritedExport]
        abstract class TestAbstractClass2
        {
            public abstract string ID { get; }
        }

        [Export(typeof(TestAbstractClass))]
        abstract class TestAbstractClass
        {
            public abstract string ID { get; }
        }


        class TestClass1 : TestAbstractClass2
        {
            public override string ID => "TestClass1";
        }

        class TestClass2 : TestAbstractClass2
        {
            public override string ID => "TestClass2";
        }

        class TestClass3 : TestAbstractClass
        {
            public override string ID => "TestClass3";
        }

        class TestClass4
        {
            [ImportMany]
            public static List<TestAbstractClass2> TestProp { get; private set; }
            [ImportMany]
            public static readonly TestAbstractClass2[] TestProp2 = null;
            [ImportMany]
            public IList<TestAbstractClass2> TestProp3 { get; private set; }
            [ImportMany]
            public readonly ReadOnlyCollection<TestAbstractClass2> TestProp4 = null;
            [ImportMany]
            public static Collection<TestAbstractClass> TestProp5 = null;
            [ImportMany]
            public readonly IEnumerable<TestAbstractClass> TestProp6 = null;
        }

        [Fact]
        public void Test()
        {
            var test3 = new TestClass4();
            PartContainer.Fill(test3);
            PartContainer.Fill(typeof(TestClass4));
            Assert.Null(TestClass4.TestProp5);
            Assert.Null(test3.TestProp6);
            foreach (var test in new[] { TestClass4.TestProp, TestClass4.TestProp2, test3.TestProp3, test3.TestProp4 })
            {
                Assert.NotNull(test);
                Assert.Equal(2, test.Count());
                if (test.First() is TestClass1)
                {
                    Assert.IsType<TestClass1>(test.First());
                    Assert.Equal("TestClass1", test.First().ID);
                    Assert.IsType<TestClass2>(test.Last());
                    Assert.Equal("TestClass2", test.Last().ID);
                }
                else
                {
                    Assert.IsType<TestClass2>(test.First());
                    Assert.Equal("TestClass2", test.First().ID);
                    Assert.IsType<TestClass1>(test.Last());
                    Assert.Equal("TestClass1", test.Last().ID);
                }
            }
        }
    }
}
