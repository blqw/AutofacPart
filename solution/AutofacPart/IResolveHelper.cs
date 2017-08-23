using System.Security.Cryptography.X509Certificates;
using System.Linq;
using Autofac;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
#if NET461
using  System.ComponentModel.Composition;
#endif

using System.Text;

namespace blqw.Autofac
{
    interface IResolveHelper
    {
        bool TryResolve(IContainer container, Contract contract, out object part);
    }

    class ResolveHelper<T> : IResolveHelper
    {
        protected virtual object Convert(T value) => value;
        protected virtual Type Type => typeof(T);
        public bool TryResolve(IContainer container, Contract contract, out object part)
        {
            var value = contract.ContractName != null
                        ? container.ResolveOptionalNamed<IEnumerable<Lazy<T, ResolveMetadata>>>(contract.ContractName)
                        : container.ResolveOptional<IEnumerable<Lazy<T, ResolveMetadata>>>();

            if (value == null || value.Any() == false)
            {
                part = null;
                return false;
            }
            if (contract.IsMany)
            {
                var list = value?.GroupBy(x => x.Metadata.PartName)
                            .Select(y => y.OrderByDescending(x => x.Metadata.Priority).FirstOrDefault().Value)
                            .ToList();
                if (list == null || list.Count == 0)
                {
                    part = null;
                    return false;
                }
                try
                {
                    part = Units.ConvertToCollection(list.Select(Convert), contract.PartType, contract.ActualType);
                    return true;
                }
                catch(Exception e)
                {
                    part = null;
                    return false;
                }
            }
            part = Convert(value.OrderByDescending(x => x.Metadata.Priority).FirstOrDefault().Value);
            return true;
        }
    }

    class ResolveMethodHelper : ResolveHelper<MethodInfo>
    {
        public Type DelegateType { get; }
        protected override Type Type => DelegateType;

        protected override object Convert(MethodInfo value) => CreateDelegate(value, DelegateType);

        public ResolveMethodHelper(Type delegateType)
        {
            DelegateType = delegateType;
        }

        private static object CreateDelegate(MethodInfo method, Type delegateType)
        {
            try
            {
                if (method.IsGenericMethodDefinition)
                {
                    var part = new GenericMethodInfo(method);
                    var target = new GenericMethodInfo(delegateType);
                    if (!part.Compatible(target) || !part.ResetGenericArgumentsWithTarget(target))
                    {
                        return false;
                    }
                    method = method.MakeGenericMethod(part.GenericArguments);
                }
                return method.CreateDelegate(delegateType);
            }
            catch
#if DEBUG
            (Exception e)
            {
                throw;
#else
            {
                return null;
#endif
            }
        }
    }

    public class ResolveMetadata : IEnumerable<KeyValuePair<string, object>>
    {
        public ResolveMetadata()
        {

        }

        public ResolveMetadata(IDictionary<string, object> metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (metadata.TryGetValue("PartName", out var name) && name is string s)
            {
                PartName = s;
            }
            if (metadata.TryGetValue("Version", out var version) && version is decimal ver)
            {
                Version = ver;
            }
            if (metadata.TryGetValue("Priority", out var priority) && priority is int pri)
            {
                Priority = pri;
            }
        }

        public ResolveMetadata(string partName, decimal version, int priority)
        {
            PartName = partName;
            Version = version;
            Priority = priority;
        }

        public ResolveMetadata(params ICustomAttributeProvider[] attributeProviders)
        {
            if (attributeProviders == null)
            {
                throw new ArgumentNullException(nameof(attributeProviders));
            }

            foreach (var part in attributeProviders)
            {
                PartName = part.ToString();
                foreach (var attr in part.GetCustomAttributes(false))
                {
                    var attrType = attr.GetType().GetTypeInfo();
                    if (string.Equals(attrType.Name, "ExportMetadataAttribute", StringComparison.Ordinal))
                    {
                        var p1 = attrType.GetProperty("Name", typeof(string));
                        var p2 = attrType.GetProperty("Value", typeof(object));
                        if (p1 != null && p1.PropertyType == typeof(string) && p2 != null)
                        {
                            var name = (string)p1.GetValue(attr);
                            var value = p2.GetValue(attr);
                            if (value != null)
                            {
                                switch (name)
                                {
                                    case "PartName":
                                        PartName = value.ToString();
                                        break;
                                    case "Version":
                                        if (value is decimal version)
                                        {
                                            Version = version;
                                        }
                                        else if (decimal.TryParse(value.ToString(), out version))
                                        {
                                            Version = version;
                                        }
                                        break;
                                    case "Priority":
                                        if (value is int priority)
                                        {
                                            Priority = priority;
                                        }
                                        else if (int.TryParse(value.ToString(), out priority))
                                        {
                                            Priority = priority;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }



        [System.ComponentModel.DefaultValue(null)]
        public string PartName { get; } = null;

        [System.ComponentModel.DefaultValue(typeof(decimal), "1")]
        public decimal Version { get; } = 1;

        [System.ComponentModel.DefaultValue(0)]
        public int Priority { get; } = 0;

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            yield return new KeyValuePair<string, object>("PartName", PartName);
            yield return new KeyValuePair<string, object>("Version", Version);
            yield return new KeyValuePair<string, object>("Priority", Priority);
        }
    }
}
