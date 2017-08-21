using blqw.Autofac;
using System;
using System.Collections.Generic;
using Xunit;

namespace CoreUnitTest
{
    public class ImportGenericTests
    {
        //[Export(typeof(TestClass1<>))]
        //class TestClass1<T>
        //{
        //    public Type ID => typeof(T);

        //}


        //class TestClass2
        //{
        //    [Import]
        //    public TestClass1<string> Test1 { get; private set; }


        //    [Import]
        //    public readonly TestClass1<int> Test2 = null;


        //}

        //[Fact]
        //public void Test()
        //{
        //    var test2 = new TestClass2();
        //    Assert.Null(test2.Test1);
        //    Assert.Null(test2.Test2);
        //    PartContainer.Fill(test2);
        //    Assert.NotNull(test2.Test1);
        //    Assert.NotNull(test2.Test2);
        //    Assert.Equal(typeof(string), test2.Test1.ID);
        //    Assert.Equal(typeof(int), test2.Test2.ID);
        //}




        class TestClass3
        {

            [Export("GetT")]
            public static Type Get<T>() => typeof(T);
        }

        delegate Type GetHandler<T>();
        
        class TestClass4
        {
            [Import("GetT")]
            public readonly GetHandler<string> Get1 = null;

            [Import("GetT")]
            public readonly GetHandler<int> Get2 = null;
        }

        [Fact]
        public void Test2()
        {
            var test = new TestClass4();
            Assert.Null(test.Get1);
            Assert.Null(test.Get2);
            PartContainer.Fill(test);
            Assert.NotNull(test.Get1);
            Assert.NotNull(test.Get2);
            Assert.Equal(typeof(string), test.Get1());
            Assert.Equal(typeof(int), test.Get2());
        }
    }
}
