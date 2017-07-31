using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.Autofac
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
    public sealed class InheritedExportAttribute : Attribute
    {
        public InheritedExportAttribute()
        {
        }
        public InheritedExportAttribute(string contractName) => ContractName = contractName?.Trim();
        public InheritedExportAttribute(Type contractType) => ContractType = contractType;
        public InheritedExportAttribute(string contractName, Type contractType)
        {
            ContractName = contractName?.Trim();
            ContractType = contractType;
        }

        public string ContractName { get; }
        public Type ContractType { get; }

    }
}
