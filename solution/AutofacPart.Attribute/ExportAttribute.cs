using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.Autofac
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ExportAttribute : Attribute
    {
        public ExportAttribute()
        {
        }
        public ExportAttribute(string contractName) => ContractName = contractName?.Trim();
        public ExportAttribute(Type contractType) => ContractType = contractType;
        public ExportAttribute(string contractName, Type contractType)
        {
            ContractName = contractName?.Trim();
            ContractType = contractType;
        }

        public string ContractName { get; }
        public Type ContractType { get; }

    }
}
