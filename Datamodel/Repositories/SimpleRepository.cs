using DataModel.Custom;
using DataModel.Interfaces;

namespace DataModel.Repositories
{
    /// <summary>
    ///     Simple Repository class to provide base operations without having to create specific instances
    /// </summary>
    public class SimpleRepository<T> : BaseRepository<T>, ISimpleRepository<T> where T : class
    {
        /// <summary>
        ///     Initializes a new instance of the ReadOnlyRepository class.
        /// </summary>
        /// <param name="databaseContext">The database context</param>
        public SimpleRepository(SteamAppEntities databaseContext)
            : base(databaseContext)
        {
        }
    }
}