namespace DataModel.Interfaces
{
    /// <summary>
    ///     Simple Repository Interface
    ///     This can be used for base operations without the need to create individual repositories
    /// </summary>
    /// <typeparam name="T">the type of object</typeparam>
    public interface ISimpleRepository<T> : IBaseRepository<T> where T : class
    {
    }
}