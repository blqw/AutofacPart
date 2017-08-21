using System.Collections;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Builder;

namespace blqw.Autofac
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class Units
    {
        /// <summary>
        /// 判断类型是否可以被实例化
        /// </summary>
        /// <param name="type">待判断的类型</param>
        /// <returns></returns>
        public static bool IsInstantiable(Type type)
        {
            if (type == null || type.IsAbstract || !type.IsClass)
            {
                return false;
            }
            return !type.IsGenericType || type.IsGenericTypeDefinition;
        }

        /// <summary>
        /// 转换集合类型
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="collectionType"></param>
        /// <returns></returns>
        internal static object ConvertToCollection(IEnumerable enumerable, Type eleType, Type collectionType)
        {
            if (collectionType.IsInstanceOfType(enumerable))
            {
                return enumerable;
            }
            //Array
            if (collectionType.IsArray)
            {
                var elementType = collectionType.GetElementType();
                if (eleType.IsAssignableFrom(elementType))
                {
                    var array = new ArrayList();
                    foreach (var item in enumerable)
                    {
                        array.Add(item);
                    }
                    return array.ToArray(elementType);
                }
                throw new InvalidCastException($"数组类型不兼容");
            }
            //ReadOnlyCollection<T>
            if (collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>))
            {
                var list = ConvertToCollection(enumerable, eleType, typeof(List<>).MakeGenericType(collectionType.GetGenericArguments()[0]));
                return Activator.CreateInstance(collectionType, list);
            }

            if (collectionType.GetInterfaces().Any(x => x == typeof(IList)))
            {
                var list = collectionType.IsInterface ? new ArrayList() : (IList)Activator.CreateInstance(collectionType);
                foreach (var item in enumerable)
                {
                    list.Add(item);
                }
                return list;
            }

            var collection = collectionType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));
            if (collection != null)
            {
                var elementType = collection.GetGenericArguments()[0];
                if (eleType.IsAssignableFrom(elementType))
                {
                    if (collectionType.IsInterface)
                    {
                        return Activator.CreateInstance(collectionType, typeof(List<>).MakeGenericType(elementType));
                    }
                    var list = Activator.CreateInstance(collectionType);
                    var add = collectionType.GetMethod("Add");
                    var args = new object[1];
                    foreach (var item in enumerable)
                    {
                        args[0] = item;
                        add.Invoke(list, args);
                    }
                    return list;
                }
                throw new InvalidCastException($"集合类型不兼容");
            }

            throw new InvalidCastException($"不支持的集合类型:{collectionType.FullName}");
        }

        /// <summary>
        /// 根据type的泛型属性选择合适的注册方法
        /// </summary>
        public static IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle>  SmartRegisterTypes(this ContainerBuilder builder, Type type)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsGenericTypeDefinition)
            {
                return builder.RegisterGeneric(type);
            }
            else
            {
                return builder.RegisterTypes(type);
            }
        }
    }
}
