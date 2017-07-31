using System.ComponentModel.DataAnnotations;
using blqw.Autofac;
using System;

namespace CoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var my = new MyClass2();
            PartContainer.PropertiesAutowired(my);
            Console.WriteLine("Hello World!");
        }
    }


    class MyClass2
    {
        public IInterfacePart Part { get; set; }
    }

    
    class MyClass : IInterfacePart
    {

    }

    
    interface IInterfacePart
    {

    }
}
