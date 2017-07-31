using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.Autofac
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class ImportAttribute : Attribute
    {
        public ImportAttribute()
        {
        }
        public ImportAttribute(string contractName) => ContractName = contractName?.Trim();
        public ImportAttribute(Type contractType) => ContractType = contractType;
        public ImportAttribute(string contractName, Type contractType)
        {
            ContractName = contractName?.Trim();
            ContractType = contractType;
        }

        public string ContractName { get; }
        public Type ContractType { get; }

    }
}
