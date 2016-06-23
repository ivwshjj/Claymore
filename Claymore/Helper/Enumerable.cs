using System;
using System.Collections.Generic;
using System.Text;

namespace Claymore.Helper
{
    public delegate TResult Func<T, TResult>(T t);
    public delegate TResult Func<T,T2, TResult>(T t,T2 t2);
    /*
      for (int i = 0; i < 100000; i++)
            {
                Test u = new Test();
                u.ID = i % 100;
                u.XM = (i % 100).ToString();
                list2.Add(u);
            }
            Func<Test, Test, bool> func = delegate(Test t1, Test t2)
             {
                 return t1.ID == t2.ID && t1.XM == t2.XM;
             };
            
            list2 =(new Enumerable<Test>(list2).Distinct(func)).ToList();
     */
    /// <summary>
    /// http://www.cnblogs.com/JeffreyZhao/archive/2009/06/27/try-to-make-a-better-csharp-2.html
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Enumerable<T>
    {
        private IEnumerable<T> m_source;

        public Enumerable(IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            this.m_source = source;
        }

        public List<T> ToList()
        {
            return ToList<T>(m_source);
        }
        public static List<T> ToList<T>(IEnumerable<T> m_source)
        {
            return new List<T>(m_source);
        }

        public Enumerable<T> Where(Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            return new Enumerable<T>(Where(this.m_source, predicate));
        }

        private static IEnumerable<T> Where(IEnumerable<T> source, Func<T, bool> predicate)
        {
            foreach (T item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        public Enumerable<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            return new Enumerable<TResult>(Select(this.m_source, selector));
        }

        private static IEnumerable<TResult> Select<TResult>(IEnumerable<T> source, Func<T, TResult> selector)
        {
            foreach (T item in source)
            {
                yield return selector(item);
            }
        }
        /*
        public Enumerable<T> Distinct(Func<T,T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            return new Enumerable<T>(Distinct(this.m_source, predicate));
        }

        private static IEnumerable<T> Distinct(IEnumerable<T> source, Func<T, bool> predicate)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    **/

        public Enumerable<T> Distinct(Func<T, T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            return (new Enumerable<T>(Distincts(this.m_source, predicate)));
        }
        public static IEnumerable<T> Distincts(IEnumerable<T> m_source, Func<T, T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            List<T> list = new List<T>();
            bool flag = false;
            foreach (T item in m_source)
            {
                foreach (T x in list) {
                    flag = predicate(x, item);
                    if (flag) {
                        break;
                    }
                }
                if (flag)
                    continue;

                list.Add(item);
                yield return item;
            }
        }
    }

    internal class DistinctEqualityComparer<T> : IEqualityComparer<T>
    {
        public Func<T, T, bool> EqualityPredicate;// { get; private set; }
        public Func<T, int> HashCodeFunction; //{ get; private set; }

        public DistinctEqualityComparer(Func<T, T, bool> equality, Func<T, int> hashCode)
        {
            EqualityPredicate = equality;
            HashCodeFunction = hashCode;
        }

        public bool Equals(T x, T y)
        {
            return EqualityPredicate.Invoke(x, y);
        }
        public int GetHashCode(T obj)
        {
            return HashCodeFunction.Invoke(obj);
        }
    }
}
