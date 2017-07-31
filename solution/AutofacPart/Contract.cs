using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace blqw.Autofac
{
    /// <summary>
    /// 契约
    /// </summary>
    internal struct Contract
    {
        /// <summary>
        /// 私有构造函数
        /// </summary>
        private Contract(MemberInfo member, string contractName, Type contractType)
        {
            Member = member;
            Name = string.IsNullOrWhiteSpace(contractName) ? null : contractName.Trim();
            Type = contractType;
            Valid = true;
        }
        /// <summary>
        /// 导出零件契约
        /// </summary>
        /// <param name="type"></param>
        public static Contract Export(Type type)
        {
            if (type == null)
            {
                return new Contract();
            }
            var attr = type.GetCustomAttribute<ExportAttribute>();
            if (attr != null)
            {
                return new Contract(type, attr.ContractName, attr.ContractType ?? type);
            }
#if NET461
            var attr2 = type.GetCustomAttribute<System.ComponentModel.Composition.ExportAttribute>();
            if (attr2 != null)
            {
                return new Contract(type, attr2.ContractName, attr2.ContractType ?? type) ;
            }
#endif
            return new Contract();
        }

        /// <summary>
        /// 导入零件契约
        /// </summary>
        /// <param name="property"></param>
        public static Contract Import(PropertyInfo property)
        {
            if (property == null)
            {
                return new Contract();
            }
            var attr = property.GetCustomAttribute<ImportAttribute>();
            if (attr != null)
            {
                return new Contract(property, attr.ContractName, attr.ContractType ?? property.PropertyType);
            }
#if NET461
            var attr2 = property.GetCustomAttribute<System.ComponentModel.Composition.ImportAttribute>();
            if (attr2 != null)
            {
                return new Contract(property, attr2.ContractName, attr2.ContractType) ;
            }
#endif
            return new Contract();
        }

        public bool Valid { get; }
        public string Name { get; }
        public Type Type { get; }
        public MemberInfo Member { get; }

    }
}
