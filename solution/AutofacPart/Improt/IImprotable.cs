using Autofac;

namespace blqw.Autofac
{
    /// <summary>
    /// 可导入零件接口
    /// </summary>
    internal interface IImprotable
    {
        /// <summary>
        /// 尝试解析零件
        /// </summary>
        /// <param name="container">IOC容器</param>
        /// <param name="part">零件实体</param>
        /// <returns></returns>
        bool TryResolve(IContainer container, out object part);
    }
}
