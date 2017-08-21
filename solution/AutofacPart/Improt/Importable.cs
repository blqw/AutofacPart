using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Autofac;
using System.Collections;

namespace blqw.Autofac
{
    /// <summary>
    /// 可导入零件
    /// </summary>
    internal sealed class Improtable : IImprotable
    {
        /// <summary>
        /// 解析零件的委托声明
        /// </summary>
        private delegate bool TryResolveHandler(IContainer container, out object part);

        /// <summary>
        /// 尝试解析零件的委托
        /// </summary>
        private readonly TryResolveHandler _tryResolve;

        /// <summary>
        /// 初始化可导入零件
        /// </summary>
        /// <param name="tryResolve"></param>
        private Improtable(TryResolveHandler tryResolve) => _tryResolve = tryResolve;

        /// <summary>
        /// 尝试解析零件
        /// </summary>
        /// <param name="container">IOC容器</param>
        /// <param name="part">零件实体</param>
        public bool TryResolve(IContainer container, out object part) => _tryResolve(container, out part);

        /// <summary>
        /// 根据属性特性返回导入零件操作
        /// </summary>
        public static IImprotable ByProperty(PropertyInfo property)
            => ByContract(Contract.Import(property), property.PropertyType);

        /// <summary>
        /// 根据字段特性返回导入零件操作
        /// </summary>
        public static IImprotable ByField(FieldInfo field)
            => ByContract(Contract.Import(field), field.FieldType);

        /// <summary>
        /// 根据契约和实际类型返回导入零件操作
        /// </summary>
        private static IImprotable ByContract(Contract contract, Type returnType)
        {
            if (!contract.Valid)
            {
                return null;
            }
            if (typeof(Delegate).IsAssignableFrom(contract.Type)) //委托
            {
                if (contract.IsMany)
                {
                    if (contract.Name != null)
                    {
                        return new Improtable(NamedDelegateToMany(contract.Name, contract.Type, returnType));
                    }
                    return new Improtable(DelegateToMany(contract.Member.Name, contract.Type, returnType));
                }
                if (contract.Name != null)
                {
                    return new Improtable(NamedDelegateToOne(contract.Name, contract.Type, returnType));
                }
                return new Improtable(DelegateToOne(contract.Member.Name, contract.Type, returnType));
            }
            if (contract.IsMany)
            {
                if (contract.Name != null)
                {
                    return new Improtable(NamedTypeToMany(contract.Name, contract.Type, returnType));
                }
                return new Improtable(TypeToMany(contract.Type, returnType));
            }
            if (contract.Name != null)
            {
                return new Improtable(NamedTypeToOne(contract.Name, contract.Type, returnType));
            }
            return new Improtable(TypeToOne(contract.Type, returnType));
        }

        /// <summary>
        /// 根据契约名称和契约类型得到一个委托类型的零件
        /// </summary>
        private static TryResolveHandler NamedDelegateToOne(string contractName, Type contractType, Type returnType)
        {
            return (IContainer container, out object value) =>
            {
                if (container.TryResolveNamed(contractName, contractType, out value))
                {
                    return true;
                }
                if (container.TryResolveNamed(contractName, typeof(MethodInfo), out value))
                {
                    try
                    {
                        value = ((MethodInfo)value).CreateDelegate(contractType);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return false;
            };
        }

        /// <summary>
        /// 根据契约类型得到一个委托类型的零件
        /// </summary>
        private static TryResolveHandler DelegateToOne(string memberName, Type contractType, Type returnType)
        {
            return (IContainer container, out object value) =>
            {
                if (container.TryResolve(contractType, out value))
                {
                    return true;
                }
                if (container.TryResolveNamed(memberName, typeof(MethodInfo), out value))
                {
                    try
                    {
                        value = ((MethodInfo)value).CreateDelegate(contractType);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return false;
            };
        }
        
        /// <summary>
        /// 根据契约类型得到多个委托类型的零件
        /// </summary>
        private static TryResolveHandler DelegateToMany(string memberName, Type contractType, Type returnType)
        {
            return (IContainer container, out object value) =>
            {
                var type = typeof(IEnumerable<>).MakeGenericType(contractType);
                if (container.TryResolve(type, out value) == false)
                {
                    if (container.TryResolveNamed(memberName, typeof(IEnumerable<MethodInfo>), out value))
                    {
                        try
                        {
                            value = ((IEnumerable<MethodInfo>)value).Select(x => x.CreateDelegate(contractType));
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                try
                {
                    value = Units.ConvertToCollection((IEnumerable)value, contractType, returnType);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            };
        }


        /// <summary>
        /// 根据契约名称和契约类型得到多个委托类型的零件
        /// </summary>
        private static TryResolveHandler NamedDelegateToMany(string contractName, Type contractType, Type returnType)
        {
            return (IContainer container, out object value) =>
            {
                var type = typeof(IEnumerable<>).MakeGenericType(contractType);
                if (container.TryResolveNamed(contractName, type, out value) == false || ((IEnumerable)value).Cast<object>().Any() == false)
                {
                    if (container.TryResolveNamed(contractName, typeof(IEnumerable<MethodInfo>), out value))
                    {
                        try
                        {
                            value = ((IEnumerable<MethodInfo>)value).Select(x => x.CreateDelegate(contractType));
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                try
                {
                    value = Units.ConvertToCollection((IEnumerable)value, contractType, returnType);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            };
        }

        /// <summary>
        /// 根据契约类型得到一个零件
        /// </summary>
        private static TryResolveHandler TypeToOne(Type contractType, Type returnType)
        {
            return (IContainer container, out object value) => container.TryResolve(contractType, out value);
        }
        /// <summary>
        /// 根据契约名称和契约类型得到一个零件
        /// </summary>
        private static TryResolveHandler NamedTypeToOne(string contractName, Type contractType, Type returnType)
        {
            return (IContainer container, out object value) => container.TryResolveNamed(contractName, contractType, out value);
        }
        /// <summary>
        /// 根据契约类型得到多个零件
        /// </summary>
        private static TryResolveHandler TypeToMany(Type contractType, Type returnType)
        {
            return (IContainer container, out object value) =>
            {
                var type = typeof(IEnumerable<>).MakeGenericType(contractType);
                if (container.TryResolve(type, out value) == false)
                {
                    return false;
                }
                try
                {
                    value = Units.ConvertToCollection((IEnumerable)value, contractType, returnType);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            };
        }
        /// <summary>
        /// 根据契约名称和契约类型得到多个零件
        /// </summary>
        private static TryResolveHandler NamedTypeToMany(string contractName, Type contractType, Type returnType)
        {
            return (IContainer container, out object value) =>
            {
                var type = typeof(IEnumerable<>).MakeGenericType(contractType);
                if (container.TryResolveNamed(contractName, type, out value) == false)
                {
                    return false;
                }
                try
                {
                    value = Units.ConvertToCollection((IEnumerable)value, contractType, returnType);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            };
        }
    }
}
