using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.Autofac
{
    /// <summary>
    /// 可导入零件
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class ImportAttribute : Attribute
    {
        /// <summary>
        /// 使用推断类型来初始化可导入零件契约
        /// </summary>
        public ImportAttribute() { }

        /// <summary>
        /// 使用契约名称和推断类型来初始化可导入零件契约
        /// </summary>
        /// <param name="contractName">契约名称</param>
        public ImportAttribute(string contractName) => ContractName = contractName?.Trim();

        /// <summary>
        /// 使用显式契约类型来初始化可导入零件契约
        /// </summary>
        /// <param name="contractType">契约类型</param>
        public ImportAttribute(Type contractType) => ContractType = contractType;

        /// <summary>
        /// 使用契约名称和显式契约类型来初始化可导入零件契约
        /// </summary>
        /// <param name="contractName">契约名称</param>
        /// <param name="contractType">契约类型</param>
        public ImportAttribute(string contractName, Type contractType)
        {
            ContractName = contractName?.Trim();
            ContractType = contractType;
        }

        /// <summary>
        /// 契约名称
        /// </summary>
        public string ContractName { get; }

        /// <summary>
        /// 契约名称
        /// </summary>
        public Type ContractType { get; }

    }
}
