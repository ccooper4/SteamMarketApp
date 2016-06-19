using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using Datamodel;
using DataModel.Interfaces;
using DataModel.Repositories;
using SteamAppEntities = DataModel.Custom.SteamAppEntities;

namespace DataModel
{
    /// <summary>
    ///     Implementation of the unit of work interface for database access.
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        /// <summary>
        ///     Single database context for data access
        /// </summary>
        protected readonly SteamAppEntities DatabaseContext;

        /// <summary>
        ///     private bool for disposed.
        /// </summary>
        private bool _disposed;

        public UnitOfWork() : this(new SteamAppEntities())
        {
        }

        public UnitOfWork(SteamAppEntities databaseContext)
        {
            DatabaseContext = databaseContext;
        }

        #region Private variables

        // List of ISimple Repositories
        private ISimpleRepository<App_MarketItem> _marketItemRepository;
        private ISimpleRepository<App_Image> _imageRepository;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the MarketItems repository
        /// </summary>
        public ISimpleRepository<App_MarketItem> MarketItems
        {
            get
            {
                if (_marketItemRepository == null)
                {
                    _marketItemRepository = new SimpleRepository<App_MarketItem>(DatabaseContext);
                }

                return _marketItemRepository;
            }
        }

        /// <summary>
        ///     Gets the MarketItems repository
        /// </summary>
        public ISimpleRepository<App_Image> ImageItems
        {
            get
            {
                if (_imageRepository == null)
                {
                    _imageRepository = new SimpleRepository<App_Image>(DatabaseContext);
                }

                return _imageRepository;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Save changes made in the Unit Of Work
        /// </summary>
        public void Save()
        {
            try
            {
                DatabaseContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var validationError in e.EntityValidationErrors)
                {
                    // _logger.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", validationError.Entry.Entity.GetType().Name, validationError.Entry.State);
                    foreach (var ve in validationError.ValidationErrors)
                    {
                        //_logger.ErrorFormat("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }

                throw;
            }
            catch (DbUpdateConcurrencyException concurrencyException)
            {
                foreach (var entry in concurrencyException.Entries)
                {
                    //_logger.ErrorFormat("Saving entity of type \"{0}\" in state \"{1}\" has failed due to a concurrency conflict", entry.Entity.GetType().Name, entry.State);
                }

                throw new ApplicationException("Concurrency conflict when saving entity", concurrencyException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                //_logger.ErrorFormat("Db Update Failure: {0}", dbUpdateException);

                throw new ApplicationException("Failed to update the entity", dbUpdateException);
            }
        }

        /// <summary>
        ///     method to dispospose the database context if not currently disposing
        /// </summary>
        /// <param name="disposing">indicate if the database should be disposed</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DatabaseContext.Dispose();
                }
            }

            _disposed = true;
        }

        /// <summary>
        ///     Method to dispose the database context
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Set the CommandTimeout on the database context
        /// </summary>
        /// <param name="timeoutInSeconds">The number of seconds to set the DatabaseContect CommandTimeout to</param>
        public void SetCommandTimout(int timeoutInSeconds)
        {
            var adapter = (IObjectContextAdapter)DatabaseContext;
            var objectContext = adapter.ObjectContext;
            objectContext.CommandTimeout = timeoutInSeconds; //default is 30 seconds.
            //_logger.InfoFormat("Unit of Work CommandTimeout set to '{0}' seconds", timeoutInSeconds);
        }

        #endregion
    }
}