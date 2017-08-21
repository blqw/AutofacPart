using blqw.Autofac;
using System;
using System.Collections.Generic;
using Xunit;

namespace CoreUnitTest
{
    public class ImportMethodTests
    {
        class TestClass1
        {
            [Export("GetID")]
            public static string GetID_A() => "A";


            [Export("GetID")]
            public static string GetID_B() => "B";
        }

        class TestClass2
        {
            [Import]
            public Func<object> GetID { get; private set; }


            [Import("GetID")]
            public Func<string> GetID2 { get; private set; }


            [ImportMany("GetID")]
            public List<Func<string>> GetID3 { get; private set; }
        }

        [Fact]
        public void Test()
        {
            var test2 = new TestClass2();
            Assert.Null(test2.GetID);
            Assert.Null(test2.GetID2);
            PartContainer.Fill(test2);
            Assert.NotNull(test2.GetID);
            Assert.NotNull(test2.GetID2);
            Assert.Equal(test2.GetID2(), test2.GetID());
        }

        [Fact]
        public void TestMany()
        {
            var test2 = new TestClass2();
            Assert.Null(test2.GetID3);
            PartContainer.Fill(test2);
            Assert.NotNull(test2.GetID3);
            Assert.Equal(2, test2.GetID3.Count);
            var a = test2.GetID3[0]();
            var b = test2.GetID3[1]();
            Assert.True((a == "A" && b == "B") || (a == "B" && b == "A"));
        }
    }
}
