using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;

namespace LewisTech.Utils.Entity
{
    public static class DbContextExtensions
    {

        public static ICollection<TElement> EnsureLoaded<TEntity, TElement>(this DbContext dbContext, TEntity entity,
                                                                            Expression<Func<TEntity, ICollection<TElement>>> navigationProperty)
            where TEntity : class
            where TElement : class
        {
            var collection = dbContext.Entry(entity).Collection(navigationProperty);
            if (!collection.IsLoaded)
                collection.Load();

            return collection.CurrentValue;
        }

        public static TElement EnsureLoaded<TEntity, TElement>(this DbContext dbContext, TEntity entity, Expression<Func<TEntity, TElement>> referenceProperty)
            where TEntity : class
            where TElement : class
        {
            var reference = dbContext.Entry(entity).Reference(referenceProperty);
            if (!reference.IsLoaded)
                reference.Load();

            return reference.CurrentValue;
        }


        public static T Add<T>(this DbContext dbContext, T entity) where T : class
        {
            return dbContext.Set<T>().Add(entity);
        }

        public static T Remove<T>(this DbContext dbContext, T entity) where T : class
        {
            return dbContext.Set<T>().Remove(entity);
        }

        public static void UndoChanges(this DbContext dbContext)
        {
            var changes = dbContext.ChangeTracker.Entries().ToArray();

            foreach (var change in changes)
            {
                change.State = EntityState.Unchanged;
            }
        }

    }

    public static class EntityFrameworkExtensions
    {


        public static string FormatValidationErrors(this DbEntityValidationException ex)
        {
            return string.Join("\n", ex.EntityValidationErrors.SelectMany(m => m.ValidationErrors).Select(m => m.PropertyName + ": " + m.ErrorMessage));
        }

    }
}
