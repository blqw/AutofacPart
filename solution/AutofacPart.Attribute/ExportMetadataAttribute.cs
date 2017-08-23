using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.Autofac
{
    /// <summary>
    /// 指定的类型、 属性、 字段或方法，标记有元数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class ExportMetadataAttribute : Attribute
    {
        /// <summary>
        /// 新实例初始化具有指定名称和元数据值。
        /// </summary>
        /// <param name="name"> 一个字符串，包含元数据值的名称或 null。</param>
        /// <param name="value">一个包含元数据值的对象。 这可能是 null。</param>
        public ExportMetadataAttribute(string name, object value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// 获取元数据值的名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取元数据值。
        /// </summary>
        public object Value { get; }
    }
}
