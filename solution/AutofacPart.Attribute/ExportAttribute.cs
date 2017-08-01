using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.Autofac
{
    /// <summary>
    /// 可导出零件契约
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ExportAttribute : Attribute
    {
        /// <summary>
        /// 使用契约名称和推断类型来初始化可导出零件契约
        /// </summary>
        /// <param name="contractName">契约名称</param>
        public ExportAttribute(string contractName) => ContractName = contractName?.Trim();

        /// <summary>
        /// 使用显式契约类型来初始化可导出零件契约
        /// </summary>
        /// <param name="contractType">契约类型</param>
        public ExportAttribute(Type contractType) => ContractType = contractType;

        /// <summary>
        /// 使用契约名称和显式契约类型来初始化可导出零件契约
        /// </summary>
        /// <param name="contractName">契约名称</param>
        /// <param name="contractType">契约类型</param>
        public ExportAttribute(string contractName, Type contractType)
        {
            ContractName = contractName?.Trim();
            ContractType = contractType;
        }

        /// <summary>
        /// 契约名称
        /// </summary>
        public string ContractName { get; }

        /// <summary>
        /// 契约类型
        /// </summary>
        public Type ContractType { get; }
    }
}
