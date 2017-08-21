using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw.Autofac
{

    /// <summary>
    /// 用于操作泛型方法
    /// </summary>
    internal struct GenericMethodInfo
    {
        public GenericMethodInfo(MethodInfo method)
        {
            Method = method;
            Parameters = method.GetParameters();
            if (!method.IsGenericMethod)
            {
                GenericDefinitionArguments = null;
                GenericArguments = method.GetGenericArguments();
            }
            else if (method.IsGenericMethodDefinition)
            {
                GenericDefinitionArguments = method.GetGenericArguments();
                GenericArguments = null;
            }
            else
            {
                GenericDefinitionArguments = method.GetGenericMethodDefinition().GetGenericArguments();
                GenericArguments = method.GetGenericArguments();
            }
        }

        public GenericMethodInfo(Type type)
            : this()
        {
            if (!typeof(Delegate).IsAssignableFrom(type))
            {
                return;
            }
            Method = type.GetMethod("Invoke");
            Parameters = Method.GetParameters();
            if (!type.IsGenericType)
            {
                GenericDefinitionArguments = null;
                GenericArguments = Method.GetGenericArguments();
            }
            else if (type.IsGenericTypeDefinition)
            {
                GenericDefinitionArguments = Method.GetGenericArguments();
                GenericArguments = null;
            }
            else
            {
                GenericDefinitionArguments = type.GetGenericTypeDefinition().GetGenericArguments();
                GenericArguments = type.GetGenericArguments();
            }
        }
        /// <summary>
        /// 方法信息
        /// </summary>
        public MethodInfo Method { get; }
        /// <summary>
        /// 参数信息
        /// </summary>
        public ParameterInfo[] Parameters { get; }
        /// <summary>
        /// 泛型定义参数
        /// </summary>
        public Type[] GenericDefinitionArguments { get; }
        /// <summary>
        /// 泛型参数
        /// </summary>
        public Type[] GenericArguments { get; private set; }

        /// <summary>
        /// 检查兼容性
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        internal bool Compatible(GenericMethodInfo target)
        {
            if (target.Parameters == null || Parameters == null)
            {
                return false;
            }
            if (target.Parameters.Length != Parameters.Length
                || target.GenericArguments == null
                || (Method.ReturnType == typeof(void) && target.Method.ReturnType != typeof(void))
                || (Method.ReturnType != typeof(void) && target.Method.ReturnType == typeof(void)))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据Target重新定义当前方法的泛型参数
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        internal bool ResetGenericArgumentsWithTarget(GenericMethodInfo target)
        {
            var genericArguments = new Type[GenericDefinitionArguments.Length];
            Array.Copy(GenericDefinitionArguments, genericArguments, GenericDefinitionArguments.Length);
            for (var i = 0; i < Parameters.Length; i++)
            {
                genericArguments.CompareExchange(Parameters[i].ParameterType, target.Parameters[i].ParameterType);
            }
            genericArguments.CompareExchange(Method.ReturnType, target.Method.ReturnType);

            if (genericArguments.Any(x => x.IsGenericParameter))
            {
                if (target.GenericDefinitionArguments == null)
                {
                    return false;
                }
                for (var i = 0; i < genericArguments.Length; i++)
                {
                    var arg = genericArguments[i];
                    if (arg.IsGenericParameter)
                    {
                        var newArg = target.GetGenericArgumentByDefineName(arg.Name);
                        if (newArg == null)
                        {
                            return false;
                        }
                        genericArguments[i] = newArg;
                    }
                }
            }
            GenericArguments = genericArguments;
            return true;
        }

        /// <summary>
        /// 根据泛型参数的定义名称返回泛型参数
        /// </summary>
        private Type GetGenericArgumentByDefineName(string defineName)
        {
            if (GenericDefinitionArguments == null || GenericArguments == null)
            {
                return null;
            }
            for (var i = 0; i < GenericDefinitionArguments.Length; i++)
            {
                if (GenericDefinitionArguments[i].Name == defineName)
                {
                    return GenericArguments[i];
                }
            }
            return null;
        }
    }
}
