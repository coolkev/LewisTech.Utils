using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using LewisTech.Utils.Collections;

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


        public static void SaveChangesWithRetry(this DbContext dbContext, SqlExceptionCode retryOnExceptionCode, Action beforeRetry, int maxRetries = 1)
        {

            var retries = 0;
            while (true)
            {
                try
                {
                    dbContext.SaveChanges();
                    break;
                }
                catch (DbUpdateException ex)
                {

                    var sqlException = ex.GetBaseException() as SqlException;
                    if (sqlException != null && sqlException.Number == (int)retryOnExceptionCode && retries < maxRetries)
                    {
                        retries++;

                        beforeRetry();
                        continue;
                    }

                    throw;
                }
            }
        }

        public static TKey GetOrAddRecord<T, TKey>(this DbContext dbContext, Expression<Func<T, bool>> uniqueIndexPredicate, Expression<Func<T, TKey>> keyValue, Func<T> createFunc) where T : class where TKey : struct, IEquatable<TKey>
        {

            Func<TKey> queryFunc = dbContext.Set<T>().Where(uniqueIndexPredicate).Select(keyValue).FirstOrDefault;

            var recordID = queryFunc();
            
            if (recordID.Equals(default(TKey)))
            {
                var newRecord = createFunc();

                dbContext.Set<T>().Add(newRecord);

                try
                {
                    dbContext.SaveChanges();
                    recordID = keyValue.Compile()(newRecord);
                }
                catch (DbUpdateException ex)
                {

                    if (ex.IsSqlKeyViolation())
                    {
                        dbContext.Set<T>().Remove(newRecord);

                        recordID = queryFunc();

                        if (!recordID.Equals(default(TKey)))
                        {
                            return recordID;

                        }
                    }
                    throw;

                }
            }

            return recordID;
        }


        public static T GetOrAddRecord<T>(this DbContext dbContext, Expression<Func<T, bool>> uniqueIndexPredicate, Func<T> createFunc) where T : class
        {

            bool added;
            return GetOrAddRecord(dbContext, uniqueIndexPredicate, createFunc, out added);

        }

        public static T GetOrAddRecord<T>(this DbContext dbContext, Expression<Func<T, bool>> uniqueIndexPredicate, Func<T> createFunc, out bool added) where T : class
        {

            Func<T> queryFunc = dbContext.Set<T>().Where(uniqueIndexPredicate).FirstOrDefault;
            return GetOrAddRecordInternal(dbContext, queryFunc, createFunc, out added);
        }


        private static T GetOrAddRecordInternal<T>(this DbContext dbContext, Func<T> queryFunc, Func<T> createFunc, out bool added) where T : class
        {
            added = false;

            var record = queryFunc();


            if (record == null)
            {
                var newRecord = createFunc();

                dbContext.Set<T>().Add(newRecord);

                try
                {
                    dbContext.SaveChanges();
                    added = true;
                    return newRecord;
                }
                catch (DbUpdateException ex)
                {

                    if (ex.IsSqlKeyViolation())
                    {
                        dbContext.Set<T>().Remove(newRecord);

                        record = queryFunc();

                        if (record != null)
                        {
                            return record;
                        }
                    }

                    throw;
                }
            }
            return record;
        }

        public static T GetOrAddRecord<T, TKey>(this DbContext dbContext, TKey key, Func<T> createFunc) where T : class where TKey : struct, IEquatable<TKey>
        {

            bool added;
            return GetOrAddRecord(dbContext, key, createFunc, out added);

        }

        public static T GetOrAddRecord<T, TKey>(this DbContext dbContext, TKey key, Func<T> createFunc, out bool added) where T : class where TKey : struct, IEquatable<TKey>
        {

            Func<T> queryFunc = () => dbContext.Set<T>().Find(key);
            return GetOrAddRecordInternal(dbContext, queryFunc, createFunc, out added);

        }

        public static T TryAddRecord<T>(this DbContext dbContext, T newRecord, Expression<Func<T, bool>> uniqueIndexPredicate) where T : class
        {

            bool added;
            return TryAddRecord(dbContext, newRecord, uniqueIndexPredicate, out added);
        }

        public static T TryAddRecord<T>(this DbContext dbContext, T newRecord, Expression<Func<T, bool>> uniqueIndexPredicate, out bool added) where T : class
        {


            dbContext.Set<T>().Add(newRecord);

            try
            {
                dbContext.SaveChanges();
                added = true;
                return newRecord;
            }
            catch (DbUpdateException ex)
            {

                var sqlException = ex.GetBaseException() as SqlException;
                if (sqlException != null && (sqlException.Number == (int)SqlExceptionCode.UniqueKeyConstraintViolation || sqlException.Number == (int)SqlExceptionCode.PrimaryKeyConstraintViolation))
                {
                    dbContext.Set<T>().Remove(newRecord);

                    added = false;

                    return dbContext.Set<T>().Where(uniqueIndexPredicate).FirstOrDefault(uniqueIndexPredicate);

                }
                throw;
            }
        }

        public static void TryAddRecord<T>(this DbContext dbContext, T newRecord) where T : class
        {


            dbContext.Set<T>().Add(newRecord);

            try
            {
                dbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {

                var sqlException = ex.GetBaseException() as SqlException;
                if (sqlException != null && (sqlException.Number == (int)SqlExceptionCode.UniqueKeyConstraintViolation || sqlException.Number == (int)SqlExceptionCode.PrimaryKeyConstraintViolation))
                {
                    dbContext.Set<T>().Remove(newRecord);
                    return;
                }
                throw;
            }
        }

        /// <summary>
        /// Add record to DbSet if it doesn't already exist for given uniqueIndexPredicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="newRecord"></param>
        /// <param name="uniqueIndexPredicate"></param>
        /// <returns>True if record was added, False if it already exists</returns>
        public static bool AddIfNotFound<T>(this DbContext dbContext, T newRecord, Expression<Func<T, bool>> uniqueIndexPredicate) where T : class
        {

            if (!dbContext.Set<T>().Any(uniqueIndexPredicate))
            {

                dbContext.Set<T>().Add(newRecord);

                try
                {
                    dbContext.SaveChanges();
                    return true;
                }
                catch (DbUpdateException ex)
                {

                    var sqlException = ex.GetBaseException() as SqlException;
                    if (sqlException != null
                        && (sqlException.Number == (int)SqlExceptionCode.UniqueKeyConstraintViolation || sqlException.Number == (int)SqlExceptionCode.PrimaryKeyConstraintViolation))
                    {
                        dbContext.Set<T>().Remove(newRecord);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return false;
        }


        public static bool IsSqlKeyViolation(this DbUpdateException exception)
        {
            var sqlException = exception.GetBaseException() as SqlException;
            return sqlException != null
                   && (sqlException.Number == (int)SqlExceptionCode.UniqueKeyConstraintViolation || sqlException.Number == (int)SqlExceptionCode.PrimaryKeyConstraintViolation);

        }
    }


    public static class DbSetExtensions
    {

        public static void AddRange<T>(this DbSet<T> dbSet, IEnumerable<T> values) where T : class
        {
            values.ForEach(v => dbSet.Add(v));
        }

        
    }

    public static class EntityFrameworkExtensions
    {


        public static string FormatValidationErrors(this DbEntityValidationException ex)
        {
            return string.Join("\n", ex.EntityValidationErrors.SelectMany(m => m.ValidationErrors).Select(m => m.PropertyName + ": " + m.ErrorMessage));
        }

    }


    public enum SqlExceptionCode
    {
        ForeignKeyViolation = 547,
        UniqueKeyConstraintViolation = 2601,
        PrimaryKeyConstraintViolation = 2627

    }
}
