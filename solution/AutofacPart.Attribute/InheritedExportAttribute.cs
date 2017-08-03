using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.Autofac
{
    /// <summary>
    /// 可继承导出零件契约
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
    public sealed class InheritedExportAttribute : ExportAttribute
    {
        /// <summary>
        /// 使用推断类型来初始化可继承导出零件契约
        /// </summary>
        /// <param name="contractType">显式契约类型</param>
        public InheritedExportAttribute() : base(null, null) { }

        /// <summary>
        /// 使用契约名称和推断类型来初始化可继承导出零件契约
        /// </summary>
        /// <param name="contractName">契约名称</param>
        public InheritedExportAttribute(string contractName) : base(contractName) { }

        /// <summary>
        /// 使用显式契约类型来初始化可继承导出零件契约
        /// </summary>
        /// <param name="contractType">显式契约类型</param>
        public InheritedExportAttribute(Type contractType) : base(contractType) { }

        /// <summary>
        /// 使用契约名称和显式契约类型来初始化可继承导出零件契约
        /// </summary>
        /// <param name="contractName">契约名称</param>
        /// <param name="contractType">显式契约类型</param>
        public InheritedExportAttribute(string contractName, Type contractType) : base(contractName, contractType) { }
    }
}
