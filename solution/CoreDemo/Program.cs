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
            builder.RegisterInstance(new TraceSource("default")).As<TraceSource>();
            var container = builder.Build();


            var x = container.Resolve<MyClass<string>>();
            
            Console.WriteLine(x.Data);
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
