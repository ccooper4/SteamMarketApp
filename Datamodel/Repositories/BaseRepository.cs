using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using DataModel.Custom;

namespace DataModel.Repositories
{
    /// <summary>
    ///     Base repository class for data acess
    /// </summary>
    /// <typeparam name="T">the type of object being persisted</typeparam>
    public abstract class BaseRepository<T> where T : class
    {
        /// <summary>
        ///     The object set
        /// </summary>
        private IDbSet<T> _objectSet;

        /// <summary>
        ///     The database context
        /// </summary>
        protected SteamAppEntities DatabaseContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="{T}" /> class
        /// </summary>
        /// <param name="databaseContext">the database context</param>
        protected BaseRepository(SteamAppEntities databaseContext)
        {
            DatabaseContext = databaseContext;
        }

        /// <summary>
        ///     Gets the object set
        /// </summary>
        protected IDbSet<T> ObjectSet
        {
            get
            {
                if (_objectSet == null)
                {
                    _objectSet = DatabaseContext.Set<T>();
                }

                return _objectSet;
            }
        }

        /// <summary>
        ///     Retrieves an object
        /// </summary>
        /// <param name="id">id of object to be retrieved</param>
        /// <returns>corresponding object</returns>
        public T Get(int id)
        {
            return ObjectSet.Find(id);
        }

        /// <summary>
        ///     Retrieves a collection of objects
        /// </summary>
        /// <param name="pageSize">the page size</param>
        /// <param name="pageNumber">the page number</param>
        /// <param name="orderBy">criteria to order them by</param>
        /// <returns>a collection of objects</returns>
        public IEnumerable<T> Get(int pageSize, int pageNumber, Expression<Func<T, bool>> orderBy = null)
        {
            IQueryable<T> query = ObjectSet;

            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }

            query = query
                .Skip(pageSize * pageNumber)
                .Take(pageSize);

            return query;
        }

        /// <summary>
        ///     Retrieves all objects of a given type
        /// </summary>
        /// <returns>a collection of objects</returns>
        public virtual IQueryable<T> GetAll()
        {
            return ObjectSet.AsQueryable();
        }

        /// <summary>
        ///     Retrieves all objects of a given type in a certain order
        /// </summary>
        /// <typeparam name="TKey">the type</typeparam>
        /// <param name="orderBy">criteria to order by</param>
        /// param>
        /// <returns>a collection of objects</returns>
        public virtual IQueryable<T> GetAllOrderBy<TKey>(Func<T, TKey> orderBy)
        {
            return ObjectSet.OrderBy(orderBy).AsQueryable();
        }

        /// <summary>
        ///     Add an object
        /// </summary>
        /// <param name="entity">object to be added</param>
        public void Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            ObjectSet.Add(entity);
        }

        /// <summary>
        ///     Remove an object
        /// </summary>
        /// <param name="entity">object to be removed</param>
        public virtual void Remove(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            ObjectSet.Remove(entity);
        }

        /// <summary>
        ///     Allows us to update properties of an entity entity without having to
        ///     retrieve it first. This should not be used if you have retrieved an item
        ///     from the context already and simply want to save it. This will assume all
        ///     properties of the entity you are passing here are the up to date values.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Update(T entity)
        {
            ObjectSet.Attach(entity);
            DatabaseContext.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="properties">The properties that should be updated.</param>
        public void Update(T entity, params Expression<Func<T, object>>[] properties)
        {
            ObjectSet.Attach(entity);
            var entry = DatabaseContext.Entry(entity);
            foreach (var selector in properties)
            {
                entry.Property(selector).IsModified = true;
            }
        }

        /// <summary>
        ///     Bulk insert
        /// </summary>
        /// <param name="list">list of objects to be inserted</param>
        /// <param name="tableName">the name of the table to insert them into</param>
        /// <param name="dbConnectionString">the database connection string</param>
        /// <param name="enableIdentityInsert">whether to enable identity insert</param>
        public void BulkInsert(IEnumerable<T> list, string tableName, string dbConnectionString,
            bool enableIdentityInsert)
        {
            SqlBulkCopy bulkCopy;

            if (enableIdentityInsert)
            {
                bulkCopy = new SqlBulkCopy(dbConnectionString, SqlBulkCopyOptions.KeepIdentity);
            }
            else
            {
                bulkCopy = new SqlBulkCopy(dbConnectionString);
            }

            using (bulkCopy)
            {
                bulkCopy.BatchSize = list.Count();
                bulkCopy.DestinationTableName = tableName;

                var table = new DataTable();

                // Filter out the relationships/collections to make sure we only have system data types
                var props = TypeDescriptor.GetProperties(typeof(T))
                    .Cast<PropertyDescriptor>()
                    .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                    .ToArray();

                foreach (var propertyInfo in props)
                {
                    bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                    table.Columns.Add(propertyInfo.Name,
                        Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }

                var values = new object[props.Length];

                foreach (var item in list)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }

                    table.Rows.Add(values);
                }

                bulkCopy.WriteToServer(table);
            }
        }

        /// <summary>
        ///     Method to truncate a table
        /// </summary>
        /// <param name="tableName">Name of table to truncate</param>
        public void TruncateTable(string tableName)
        {
            var sqlCommand = string.Format("TRUNCATE TABLE {0}", tableName);
            DatabaseContext.Database.ExecuteSqlCommand(sqlCommand);
        }

        /// <summary>
        ///     Method to drop the object out of the Context
        ///     Allows us to reload from the db and get the proxy
        ///     rather than object back
        /// </summary>
        /// <param name="entity">Entity to drop</param>
        public void DetachObjectFromContext(T entity)
        {
            DatabaseContext.Entry(entity).State = EntityState.Detached;
        }
    }
}