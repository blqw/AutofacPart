using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using blqw.Autofac;
using System;
using Autofac;
using System.Composition;

namespace CoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var my = new MyClass2();
            PartContainer.Fill(my);
            Console.WriteLine(my.Part.Call());

            //var builder = new ContainerBuilder();

            //builder.RegisterType(typeof(MyClass)).As<IInterfacePart>().WithParameter("name", "xxx");

            //var container = builder.Build();

            //var my = container.Resolve<IInterfacePart>();
            //Console.WriteLine(my);

        }
    }


    class MyClass2
    {
        [Import]
        public IInterfacePart Part { get; set; }
    }

    class MyClass3
    {

    }

    [Export(typeof(IInterfacePart))]
    class MyClass : MyClass3, IInterfacePart
    {
        public object Call() => "MyClass";
    }

    //[InheritedExport]
    interface IInterfacePart
    {
        object Call();
    }
}
