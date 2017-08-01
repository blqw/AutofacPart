using System.Collections.Generic;
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
            PartContainer.Fill(my);
            my.Call();

            //var builder = new ContainerBuilder();

            //builder.RegisterType(typeof(MyClass)).Named<IInterfacePart>("xxx");
            //builder.RegisterType(typeof(MyClass3)).Named<IInterfacePart>("xxx");

            //var container = builder.Build();

            //var my2 = container.ResolveNamed<IEnumerable<IInterfacePart>>("xxx");
            //Console.WriteLine(my2);

        }
    }


    class MyClass2
    {
        [ImportMany]
        public IInterfacePart[] Part { get; set; }

        public void Call()
        {
            foreach (var p in Part)
            {
                Console.WriteLine(p.Call());
            }
        }
    }

    [Export(typeof(IInterfacePart))]
    class MyClass3 : IInterfacePart
    {
        public object Call() => "MyClass3";
    }

    [Export(typeof(IInterfacePart))]
    class MyClass : IInterfacePart
    {
        public object Call() => "MyClass";
    }

    //[InheritedExport]
    interface IInterfacePart
    {
        object Call();
    }
}
