using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace blqw.Autofac
{
    /// <summary>
    /// 导入零件
    /// </summary>
    internal interface IImprot
    {
        /// <summary>
        /// 尝试解析零件
        /// </summary>
        /// <param name="container">IOC容器</param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryResolve(IContainer container, out object value);
    }
}
