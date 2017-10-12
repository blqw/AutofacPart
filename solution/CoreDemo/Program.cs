using System.Runtime.InteropServices.WindowsRuntime;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using blqw.Autofac;
using System;
using Autofac;
using System.Composition;
using Autofac.Features.Metadata;
using System.Diagnostics;

namespace CoreDemo
{

    class Program
    {

        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType(typeof(MyClass<>));
            builder.RegisterInstance(new TraceSource("default")).As<TraceSource>().WithMetadata(new ResolveMetadata("x", 1, 2));
            var container = builder.Build();

            var x = container.Resolve<IEnumerable<Lazy<TraceSource, ResolveMetadata>>>();
            Console.WriteLine(x.FirstOrDefault().Value);
            Console.WriteLine(x);

        }

        class MyClass2
        {
            public string Name { get; set; }

        }

        class MyClass<T>
        {
            public MyClass(TraceSource logger = null)
            {
                Logger = logger;
            }

            public T Data { get; set; }

            public TraceSource Logger { get; }
        }


    }


}
