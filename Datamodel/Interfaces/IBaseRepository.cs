using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataModel.Interfaces
{
    /// <summary>
    ///     Base Repository Class
    /// </summary>
    /// <typeparam name="T">the type of object</typeparam>
    public interface IBaseRepository<T> where T : class
    {
        /// <summary>
        ///     Retrieve an objecby it's id
        /// </summary>
        /// <param name="id">the object's unique id</param>
        /// <returns>The matching object</returns>
        T Get(int id);

        /// <summary>
        ///     Retrieve all objects of a particular type
        /// </summary>
        /// <returns>all objects</returns>
        IQueryable<T> GetAll();

        /// <summary>
        ///     Retrieve objects ordered by a key
        /// </summary>
        /// <typeparam name="TKey">the type of object acting as key</typeparam>
        /// <param name="orderBy">the value to order by</param>
        /// <returns>all objects ordered</returns>
        IQueryable<T> GetAllOrderBy<TKey>(Func<T, TKey> orderBy);

        /// <summary>
        ///     Adds an object
        /// </summary>
        /// <param name="entity">The object</param>
        void Add(T entity);

        /// <summary>
        ///     Remove an object
        /// </summary>
        /// <param name="entity">The object</param>
        void Remove(T entity);

        /// <summary>
        ///     Update an object
        /// </summary>
        /// <param name="entity">The object</param>
        void Update(T entity);

        /// <summary>
        ///     Update an object
        /// </summary>
        /// <param name="entity">The object</param>
        /// <param name="properties">the properties to be updated</param>
        void Update(T entity, params Expression<Func<T, object>>[] properties);

        /// <summary>
        ///     Bulk Insert
        /// </summary>
        /// <param name="entities">the object</param>
        /// <param name="tableName">the table</param>
        /// <param name="dbConnectionString">the connection string</param>
        /// <param name="enableIdentityInsert">flag to indicate of identity insert should be enabled</param>
        void BulkInsert(IEnumerable<T> entities, string tableName, string dbConnectionString, bool enableIdentityInsert);

        /// <summary>
        ///     Truncate a table
        /// </summary>
        /// <param name="tableName">The table name</param>
        void TruncateTable(string tableName);

        /// <summary>
        ///     Method to drop the object out of the Context
        ///     Allows us to reload from the db and get the proxy
        ///     rather than object back
        /// </summary>
        /// <param name="entity">Entity to drop</param>
        void DetachObjectFromContext(T entity);
    }
}