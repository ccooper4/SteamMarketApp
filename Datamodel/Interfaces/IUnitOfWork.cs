using Datamodel;

namespace DataModel.Interfaces
{
    /// <summary>
    ///     Interface for the unitofwork
    /// </summary>
    public interface IUnitOfWork
    {
        ISimpleRepository<App_MarketItem> MarketItems { get; }

        ISimpleRepository<App_Image> ImageItems { get; }

        /// <summary>
        ///     Save method
        /// </summary>
        void Save();
    }
}