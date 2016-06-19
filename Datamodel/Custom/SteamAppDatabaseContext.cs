using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace DataModel.Custom
{
    /// <summary>
    ///     Override the EF DB context so we can perform custom operations on the save method of the object graph
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SteamAppEntities : DbContext
    {
        /// <summary>
        ///     Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>
        ///     The number of objects written to the underlying database.
        /// </returns>
        /// <summary>
        ///     Method to get an entity table name
        /// </summary>
        /// <param name="entity">entity to get name for</param>
        /// <returns>table name of entity.</returns>
        private string GetEntityTableName(DbEntityEntry entity)
        {
            var entityType = entity.Entity.GetType();

            // get the actual name of the entity type not the name of the instance of the entity.
            if (entityType.BaseType != null && entityType.Namespace == "System.Data.Entity.DynamicProxies")
            {
                entityType = entityType.BaseType;
            }

            return entityType.Name;
        }
    }
}