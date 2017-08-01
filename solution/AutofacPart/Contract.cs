using System.Linq;
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
            IsMany = false;
            IsMethod = false;
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
            if (type == null)
            {
                return new Contract();
            }
            if (TryGetContractAttribute(type, "Export", out var contractName, out var contractType))
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
            if (method == null)
            {
                return new Contract();
            }
            if (TryGetContractAttribute(method, "Export", out var contractName, out var contractType))
            {
                return new Contract(method, contractName, typeof(MethodInfo)) { IsMethod = true };
            }
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
            if (TryGetContractAttribute(property, "Import", out var contractName, out var contractType))
            {
                return new Contract(property, contractName, contractType ?? property.PropertyType);
            }
            if (TryGetContractAttribute(property, "ImportMany", out contractName, out contractType))
            {
                if (contractType == null)
                {
                    var enumerable = property.PropertyType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                    if (enumerable == null)
                    {
                        return new Contract();
                    }
                    contractType = enumerable.GetGenericArguments()[0];
                }
                return new Contract(property, contractName, contractType) { IsMany = true };
            }
            return new Contract();
        }

        /// <summary>
        /// 导入零件契约
        /// </summary>
        /// <param name="field"></param>
        public static Contract Import(FieldInfo field)
        {
            if (field == null)
            {
                return new Contract();
            }
            if (TryGetContractAttribute(field, "Import", out var contractName, out var contractType))
            {
                return new Contract(field, contractName, contractType ?? field.FieldType);
            }
            if (TryGetContractAttribute(field, "ImportMany", out contractName, out contractType))
            {
                return new Contract(field, contractName, contractType ?? field.FieldType) { IsMany = true };
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
        public bool IsMethod { get; private set; }

        /// <summary>
        /// 导入插件集合
        /// </summary>
        public bool IsMany { get; private set; }
    }
}
