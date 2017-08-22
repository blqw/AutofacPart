using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Collections;

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
        private Contract(string contractName, Type contractType, MemberInfo part, string partName, Type partType)
        {
            Part = part;
            ContractName = string.IsNullOrWhiteSpace(contractName) ? null : contractName.Trim();
            ContractType = contractType;
            PartName = partName;
            PartType = partType;
            ActualType = partType;
            Valid = true;
            IsMany = false;
            IsMethod = false;
            HasAttribute = true;
        }

        /// <summary>
        /// 尝试获取契约特性
        /// </summary>
        /// <param name="part">需要获取特性的成员</param>
        /// <param name="attributeName">特性的类名, 不包含 "Attribute" </param>
        /// <param name="name">契约名称</param>
        /// <param name="type">契约类型</param>
        /// <returns></returns>
        private static bool TryGetContractAttribute(ICustomAttributeProvider part, string attributeName, out string name, out Type type)
        {
            foreach (var attr in part.GetCustomAttributes(false))
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
        public static Contract Export(Type type, bool inherit)
        {
            if (type == null)
            {
                return new Contract();
            }
            if (inherit)
            {
                if (TryGetContractAttribute(type, "InheritedExport", out var contractName, out var contractType))
                {
                    return new Contract(contractName, contractType, type, type.Name, type);
                }
            }
            else if (TryGetContractAttribute(type, "Export", out var contractName, out var contractType))
            {
                return new Contract(contractName, contractType, type, type.Name, type);
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
                return new Contract(contractName, typeof(MethodInfo), method, method.Name, typeof(MethodInfo)) { IsMethod = true };
            }
            return new Contract();
        }

        private static Contract Import(MemberInfo propertyOrField, Type type)
        {
            if (propertyOrField == null)
            {
                return new Contract();
            }
            //尝试从 ImportAttribute 中获取契约
            if (TryGetContractAttribute(propertyOrField, "Import", out var contractName, out var contractType))
            {
                return new Contract(contractName, contractType, propertyOrField, propertyOrField.Name, type);
            }
            //尝试从 ImportMany 中获取集合契约, 并且属性必须是 IEnumerable 的实现
            if (TryGetContractAttribute(propertyOrField, "ImportMany", out contractName, out contractType) && typeof(IEnumerable).IsAssignableFrom(type))
            {
                //获取属性的集合泛型
                var enumerable = type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                ? type
                                : type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                //如果不是实现泛型接口 IEnumerable<T> 则零件类型为 object
                return new Contract(contractName, contractType, propertyOrField, propertyOrField.Name, enumerable?.GetGenericArguments()[0] ?? typeof(object))
                {
                    IsMany = true,
                    ActualType = type
                };
            }
            return new Contract();
        }

        /// <summary>
        /// 导入零件契约
        /// </summary>
        /// <param name="property"></param>
        public static Contract Import(PropertyInfo property) => Import(property, property?.PropertyType);

        /// <summary>
        /// 导入零件契约
        /// </summary>
        /// <param name="field"></param>
        public static Contract Import(FieldInfo field) => Import(field, field?.FieldType);

        /// <summary>
        /// 获取参数的契约
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static Contract Import(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                return new Contract();
            }
            var type = parameter.ParameterType;
            //尝试从 ImportAttribute 中获取契约
            if (TryGetContractAttribute(parameter, "Import", out var contractName, out var contractType))
            {
                return new Contract(contractName, contractType, parameter.Member, parameter.Name, type);
            }
            //尝试从 ImportMany 中获取集合契约, 并且属性必须是 IEnumerable 的实现
            if (TryGetContractAttribute(parameter, "ImportMany", out contractName, out contractType) && typeof(IEnumerable).IsAssignableFrom(type))
            {
                //获取属性的集合泛型
                var enumerable = type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                ? type
                                : type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                //如果不是实现泛型接口 IEnumerable<T> 则零件类型为 object
                return new Contract(contractName, contractType, parameter.Member, parameter.Name, enumerable?.GetGenericArguments()[0] ?? typeof(object))
                {
                    IsMany = true,
                    ActualType = type
                };
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
        public string ContractName { get; }

        /// <summary>
        /// 契约类型
        /// </summary>
        public Type ContractType { get; }

        /// <summary>
        /// 契约零件
        /// </summary>
        public MemberInfo Part { get; }

        /// <summary>
        /// 零件名称
        /// </summary>
        public string PartName { get; }

        /// <summary>
        /// 零件类型
        /// </summary>
        public Type PartType { get; }

        /// <summary>
        /// 实际类型
        /// </summary>
        public Type ActualType { get; private set; }

        /// <summary>
        /// 方法零件
        /// </summary>
        public bool IsMethod { get; private set; }

        /// <summary>
        /// 集合契约
        /// </summary>
        public bool IsMany { get; private set; }

        public bool HasAttribute { get; }
    }
}
