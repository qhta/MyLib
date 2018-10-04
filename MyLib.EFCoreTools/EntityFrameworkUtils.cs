using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyLib.EFTools
{
  public static class EntityFrameworkUtils
  {

    public static void Clear<T>(this DbSet<T> dbSet) where T : class
    {
      dbSet.GetContext().RemoveRange(dbSet);
    }

    public static DbContext GetContext<TEntity>(this DbSet<TEntity> dbSet)
            where TEntity : class
    {
      object internalSet = dbSet
          .GetType()
          .GetField("_internalSet", BindingFlags.NonPublic | BindingFlags.Instance)
          .GetValue(dbSet);
      object internalContext = internalSet
          .GetType()
          .BaseType
          .GetField("_internalContext", BindingFlags.NonPublic | BindingFlags.Instance)
          .GetValue(internalSet);
      return (DbContext)internalContext
          .GetType()
          .GetProperty("Owner", BindingFlags.Instance | BindingFlags.Public)
          .GetValue(internalContext, null);
    }


    //public static ObjectContext GetObjectContextFromEntity(this object entity)
    //{
    //  var field = entity.GetType().GetField("_entityWrapper");

    //  if (field == null)
    //    return null;

    //  var wrapper = field.GetValue(entity);
    //  var property = wrapper.GetType().GetProperty("Context");
    //  var context = (ObjectContext)property.GetValue(wrapper, null);

    //  return context;
    //}

    public static IEnumerable<T> QueryInChunksOf<T>(this IQueryable<T> queryable, int chunkSize)
    {
      return queryable.QueryChunksOfSize(chunkSize).SelectMany(chunk => chunk);
    }

    public static IEnumerable<T[]> QueryChunksOfSize<T>(this IQueryable<T> queryable, int chunkSize)
    {
      int chunkNumber = 0;
      while (true)
      {
        var query = (chunkNumber == 0)
            ? queryable
            : queryable.Skip(chunkNumber * chunkSize);
        var chunk = query.Take(chunkSize).ToArray();
        if (chunk.Length == 0)
          yield break;
        yield return chunk;
        chunkNumber++;
      }
    }
  }
}
