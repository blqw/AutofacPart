using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.Autofac
{
    /// <summary>
    /// 可导入零件集合
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class ImportManyAttribute : ImportAttribute
    {
        /// <summary>
        /// 使用推断类型来初始化导入零件集合的契约
        /// </summary>
        public ImportManyAttribute()
        {
        }

        /// <summary>
        /// 使用契约名称和推断类型来初始化导入零件集合的契约
        /// </summary>
        /// <param name="contractName">契约名称</param>
        public ImportManyAttribute(string contractName) : base(contractName)
        {
        }

        /// <summary>
        /// 使用显式契约类型来初始化导入零件集合的契约
        /// </summary>
        /// <param name="contractType">契约类型</param>
        public ImportManyAttribute(Type contractType) : base(contractType)
        {
        }

        /// <summary>
        /// 使用契约名称和显式契约类型来初始化导入零件集合的契约
        /// </summary>
        /// <param name="contractName">契约名称</param>
        /// <param name="contractType">契约类型</param>
        public ImportManyAttribute(string contractName, Type contractType) : base(contractName, contractType)
        {
        }
    }
}
