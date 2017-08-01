using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace blqw.Autofac
{
    /// <summary>
    /// 导出零件接口
    /// </summary>
    internal interface IExport
    {
        /// <summary>
        /// 注册导出的零件
        /// </summary>
        /// <param name="builder"></param>
        void Register(ContainerBuilder builder);
    }
}
