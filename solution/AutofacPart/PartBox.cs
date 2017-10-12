using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace blqw.Autofac
{
    /// <summary>
    /// 零件盒子
    /// </summary>
    public class PartBox
    {
        public static readonly PartBox Default = new PartBox();

        private readonly ConcurrentDictionary<object, ConcurrentDictionary<Type, object>> _items
            = new ConcurrentDictionary<object, ConcurrentDictionary<Type, object>>();

        private ConcurrentDictionary<Type, object> CreateItem(object key)
            => new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// 获取一个新零件
        /// </summary>
        public T Get<T>(object key, Func<T> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }
            var cache = _items.GetOrAdd(key, CreateItem);
            return (T)cache.GetOrAdd(typeof(T), t => valueFactory());
        }

        /// <summary>
        /// 获取一个新零件
        /// </summary>
        public T Get<T>(object key, Func<object, T> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }
            var cache = _items.GetOrAdd(key, CreateItem);
            return (T)cache.GetOrAdd(typeof(T), t => valueFactory(key));
        }

        /// <summary>
        /// 获取一个新零件
        /// </summary>
        public T Get<T, TArg>(object key, Func<TArg, T> valueFactory, TArg arg)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }
            var cache = _items.GetOrAdd(key, CreateItem);
            return (T)cache.GetOrAdd(typeof(T), t => valueFactory(arg));
        }

        /// <summary>
        /// 获取一个新零件
        /// </summary>
        public T Get<T, TArg>(object key, Func<object, TArg, T> valueFactory, TArg arg)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }
            var cache = _items.GetOrAdd(key, CreateItem);
            return (T)cache.GetOrAdd(typeof(T), t => valueFactory(key, arg));
        }
    }
}
