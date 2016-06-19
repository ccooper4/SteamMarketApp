using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel;

namespace Services.Interfaces
{
    public interface ISteamService
    {
        App_MarketItem GetMarketItem(int id);

        IQueryable<App_MarketItem> GetMarketItems(string userId);

        bool AddMarketItem(App_MarketItem item);

        bool AddImage(App_Image image);
    }
}
