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
        /// 尝试获取契约特性
        /// </summary>
        /// <param name="member">需要获取特性的成员</param>
        /// <param name="attributeName">特性的类名, 不包含 "Attribute" </param>
        /// <param name="name">契约名称</param>
        /// <param name="type">契约类型</param>
        /// <returns></returns>
        private static bool TryGetContractAttribute(MemberInfo member, string attributeName, out string name, out Type type)
        {
            foreach (var attr in member.GetCustomAttributes())
            {
                var attrType = attr.GetType().GetTypeInfo();
                if (string.Equals(attrType.Name, $"{attributeName}Attribute", StringComparison.Ordinal))
                {
                    var p1 = attrType.GetProperty("ContractName", typeof(string));
                    var p2 = attrType.GetProperty("ContractType", typeof(Type));
                    if (p1 != null)
                    {
                        name = (string)p1.GetValue(attr);
                        type = (Type)p2?.GetValue(attr);
                        return true;
                    }
                }
            }
            name = null;
            type = null;
            return false;
        }

        /// <summary>
        /// 导出零件契约
        /// </summary>
        /// <param name="type"></param>
        public static Contract Export(Type type)
        {
            if (type != null && TryGetContractAttribute(type, "Export", out var contractName, out var contractType))
            {
                return new Contract(type, contractName, contractType ?? type);
            }
            return new Contract();
        }

        /// <summary>
        /// 导出零件契约
        /// </summary>
        /// <param name="method"></param>
        public static Contract Export(MethodInfo method)
        {
            if (method != null && TryGetContractAttribute(method, "Export", out var contractName, out var contractType))
            {
                return new Contract(method, contractName, typeof(MethodInfo));
            }
            return new Contract();
        }

        /// <summary>
        /// 导入零件契约
        /// </summary>
        /// <param name="property"></param>
        public static Contract Import(PropertyInfo property)
        {
            if (property != null && TryGetContractAttribute(property, "Import", out var contractName, out var contractType))
            {
                return new Contract(property, contractName, contractType ?? property.PropertyType);
            }
            return new Contract();
        }

        /// <summary>
        /// 导入零件契约
        /// </summary>
        /// <param name="field"></param>
        public static Contract Import(FieldInfo field)
        {
            if (field != null && TryGetContractAttribute(field, "Import", out var contractName, out var contractType))
            {
                return new Contract(field, contractName, contractType ?? field.FieldType);
            }
            return new Contract();
        }

        /// <summary>
        /// 契约是否有效
        /// </summary>
        public bool Valid { get; }

        /// <summary>
        /// 契约名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 契约类型
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 契约成员
        /// </summary>
        public MemberInfo Member { get; }

        /// <summary>
        /// 契约是一个方法
        /// </summary>
        public bool IsMethod => Member is MethodInfo;
    }
}
