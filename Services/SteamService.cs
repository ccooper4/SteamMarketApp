using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Datamodel;
using DataModel.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class SteamService : ISteamService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Steamapp service constructor injecting unitofwork
        /// </summary>
        /// <param name="unitOfWork">The unit of work</param>
        public SteamService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public App_MarketItem GetMarketItem(int id)
        {
            return _unitOfWork.MarketItems.Get(id);
        }

        public IQueryable<App_MarketItem> GetMarketItems(string userId)
        {
            return _unitOfWork.MarketItems.GetAll().Where(x => x.UserId == userId);
        }

        public bool AddMarketItem(App_MarketItem item)
        {
            if(_unitOfWork.MarketItems.Get(item.Id) != null)
            {
                var temp = _unitOfWork.MarketItems.Get(item.Id);
                temp.LastKnownPrice = item.LastKnownPrice;
                _unitOfWork.Save();
            }

            else
            {
                _unitOfWork.MarketItems.Add(item);
                _unitOfWork.Save();
            }
            return true;
        }

        public bool AddImage(App_Image image)
        {
            _unitOfWork.ImageItems.Add(image);
            _unitOfWork.Save();

            return true;
        }
    }
}
