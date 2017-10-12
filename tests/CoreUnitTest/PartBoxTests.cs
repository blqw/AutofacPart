using System.Reflection;
using blqw.Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CoreUnitTest
{
    public class PartBoxTests
    {
        class TestClass1
        {
            [Export("GetValue")]
            public static Func<object, object> GetValue(PropertyInfo property)
            {
                if (property == null)
                {
                    throw new ArgumentNullException(nameof(property));
                }
                var method = typeof(TestClass1).GetMethod("CreateGetter", BindingFlags.Static | BindingFlags.NonPublic);
                var call = method.MakeGenericMethod(property.DeclaringType, property.PropertyType);
                return (Func<object, object>)call.Invoke(null, new object[] { property });
            }

            private static Func<object, object> CreateGetter<T, TResult>(PropertyInfo property)
            {
                var func = (Func<T, TResult>)property.GetGetMethod().CreateDelegate(typeof(Func<T, TResult>));
                return o => func((T)o);
            }

            [Export("SetValue")]
            public static Action<object, object> SetValue(PropertyInfo property)
            {
                if (property == null)
                {
                    throw new ArgumentNullException(nameof(property));
                }
                var method = typeof(TestClass1).GetMethod("CreateSetter", BindingFlags.Static | BindingFlags.NonPublic);
                var call = method.MakeGenericMethod(property.DeclaringType, property.PropertyType);
                return (Action<object, object>)call.Invoke(null, new object[] { property });
            }

            private static Action<object, object> CreateSetter<T, TArg>(PropertyInfo property)
            {
                var act = (Action<T, TArg>)property.GetSetMethod().CreateDelegate(typeof(Action<T, TArg>));
                return (o, v) => act((T)o, (TArg)v);
            }
        }

        [Fact]
        public void TestInvoke()
        {
            PartContainer.Noop();
            var test = new MyClass { Name = "blqw" };
            var p = test.GetType().GetProperty("Name");
            var getter = PartContainer.Invoke<Func<object, object>>("GetValue", p);
            var setter = PartContainer.Invoke<Action<object, object>>("SetValue", p);
            Assert.NotNull(getter);
            Assert.NotNull(setter);

            Assert.Equal(getter(test), test.Name);
            test.Name = "xxxx";
            Assert.Equal(getter(test), test.Name);
            setter(test, "yyyy");
            Assert.Equal(getter(test), test.Name);
            Assert.Equal(test.Name, "yyyy");
        }

        [Fact]
        public void PartBoxGetTest()
        {
            PartContainer.Noop();
            var test = new MyClass { Name = "blqw" };
            var p = test.GetType().GetProperty("Name");
            var part = PartBox.Default.Get(p, x => new
            {
                Get = PartContainer.Invoke<Func<object, object>>("GetValue", x),
                Set = PartContainer.Invoke<Action<object, object>>("SetValue", x),
            }, p);
            Assert.NotNull(part);
            Assert.NotNull(part.Get);
            Assert.NotNull(part.Set);

            var part2 = PartBox.Default.Get(p, x => new
            {
                Get = PartContainer.Invoke<Func<object, object>>("GetValue", x),
                Set = PartContainer.Invoke<Action<object, object>>("SetValue", x),
            }, p);

            Assert.NotNull(part2);
            Assert.NotNull(part2.Get);
            Assert.NotNull(part2.Set);

            Assert.Equal(part.Get, part2.Get);
            Assert.Equal(part.Set, part2.Set);
        }

        class MyClass
        {
            public string Name { get; set; }
        }
    }
}
