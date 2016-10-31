using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _24ayar.Data.Utility
{
    public static class Extensions
    {
        public static void Edit<T, TKey>(this ICollection<T> oldCollection, DbContext dataContext, DbSet<T> dbset, ICollection<T> newCollection, Func<T, TKey> getKey) where T : class
        {
            if (newCollection != null)
            {
                var addedMaterials = newCollection.Except(oldCollection, getKey).ToList();
                var deletedMaterials = oldCollection.Except(newCollection, getKey).ToList();
                deletedMaterials.ForEach(u => oldCollection.Remove(u));
                addedMaterials.ForEach(u =>
                {
                    //dataContext.Entry(u).State = EntityState.;
                    //dbset.Attach(u);
                    oldCollection.Add(u);
                });
            }
            else
            {
                oldCollection.Clear();
            }
        }
        /// <summary>
        /// Token input haricinde detaylı bilgiler içeren child modelller için... dbset içindekileri direk buradan kaldırır.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="oldCollection"></param>
        /// <param name="dataContext"></param>
        /// <param name="dbset"></param>
        /// <param name="newCollection"></param>
        /// <param name="getKey"></param>
        public static void EditDetailed<T, TKey>(this ICollection<T> oldCollection, DbContext dataContext, DbSet<T> dbset, ICollection<T> newCollection, Func<T, TKey> getKey) where T : class
        {
            if (newCollection != null)
            {
                var addedMaterials = newCollection.Except(oldCollection, getKey).ToList();
                var deletedMaterials = oldCollection.Except(newCollection, getKey).ToList();
                deletedMaterials.ForEach(u => oldCollection.Remove(u));
                deletedMaterials.ForEach(u => dbset.Remove(u));
                //dataContext.SaveChanges();
                addedMaterials.ForEach(u =>
                {
                    //dataContext.Entry(u).State = EntityState.;
                    //dbset.Attach(u);
                    oldCollection.Add(u);
                });
            }
            else
            {
                var deletedMaterials = oldCollection.ToList();
                deletedMaterials.ForEach(u => dbset.Remove(u));
                deletedMaterials.ForEach(u => oldCollection.Remove(u));
                
                //oldCollection.Clear();
            }
        }
        public static IEnumerable<T> Except<T, TKey>(this IEnumerable<T> items, IEnumerable<T> other, Func<T, TKey> getKey)
        {
            return from item in items
                   join otherItem in other on getKey(item)
                   equals getKey(otherItem) into tempItems
                   from temp in tempItems.DefaultIfEmpty()
                   where ReferenceEquals(null, temp) || temp.Equals(default(T))
                   select item;

        }
        public static IEnumerable<TSource> ToPaged<TSource>(this IEnumerable<TSource> source, int pageSize, int pageId, out int totalCount)
        {
            if (source != null)
            {
                totalCount = source.Count();
                return source.Skip((pageId) * pageSize).Take(pageSize);
            }
            else
            {
                totalCount = 0;
                return new List<TSource>();
            }
        }
        public static IQueryable<TSource> ToPaged<TSource>(this IQueryable<TSource> source, int pageSize, int pageId, out int totalCount)
        {
            totalCount = source.Count();
            if (source.Expression.Type != typeof(IOrderedQueryable<TSource>))
                source = source.OrderBy(u => "");
            return source.Skip((pageId) * pageSize).Take(pageSize);
        }
    }
}
