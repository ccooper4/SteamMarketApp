using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Datamodel;
using HtmlAgilityPack;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using Services.Interfaces;
using SteamApp.Models;

namespace SteamApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISteamService _steamService;

        public HomeController(ISteamService steamService)
        {
            _steamService = steamService;
        }

        public ActionResult Index()
        {
            var items = _steamService.GetMarketItems(User.Identity.GetUserId());
            var itemsList = new List<MarketItemModel>();

            foreach (var i in items)
            {
                var temp = new MarketItemModel
                {
                    Description = i.ItemName,
                    Id = i.Id,
                    ItemUrl = i.ItemUrl,
                    LastKnownPrice = i.LastKnownPrice,
                    ImageUrl = i.ImageUrl,
                    Image = i.App_Image.Image
                };

                itemsList.Add(temp);
            }

            return View(itemsList);
        }

        public ActionResult AddItemPage()
        {
            return View("AddItem", new MarketItemModel());
        }

        public ActionResult UpdatePrices(List<MarketItemModel> items)
        {
            foreach (var item in items)
            {
                var marketItem = _steamService.GetMarketItem(item.Id);
                marketItem = GetLatestPrice(marketItem);
                _steamService.AddMarketItem(marketItem);
            }
            Index();
            return Json(new { success = true });
        }

        public ActionResult AddItem(MarketItemModel item)
        {
            var marketItem = new App_MarketItem
            {
                UserId = User.Identity.GetUserId(),
                ItemUrl = Server.UrlDecode(item.ItemUrl),
                App_Image = new App_Image()
            };

            marketItem = GetAppId(marketItem);
            try
            {
                marketItem = GetItemImage(marketItem);
                marketItem = GetLatestPrice(marketItem);
                marketItem.App_Image.Image = GetBytesFromUrl(marketItem.ImageUrl);
                marketItem.App_Image.ImageName = "test";
            }
            catch (Exception)
            {
                return View("Error");
            }
            _steamService.AddMarketItem(marketItem);

            return RedirectToAction("Index");
        }

        private App_MarketItem GetAppId(App_MarketItem item)
        {
            var split = Regex.Split(item.ItemUrl, "http://steamcommunity.com/market/listings/");
            split = Regex.Split(split[1], "/");
            item.AppId = split[0];
            item.ItemName = split[1];
            return item;
        }

        private App_MarketItem GetItemImage(App_MarketItem item)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(item.ItemUrl);

            foreach (HtmlNode div in doc.DocumentNode.SelectNodes("//div[contains(@class,'market_listing_largeimage')]"))
            {
                var test = from Match match in Regex.Matches(div.InnerHtml, "\"([^\"]*)\"")
                           select match.ToString();
                item.ImageUrl = test.First().Replace("\"", "");
                return item;
            }

            return item;
        }

        private App_MarketItem GetLatestPrice(App_MarketItem item)
        {
            WebClient client = new WebClient();
            string itemResult = client.DownloadString("http://steamcommunity.com/market/priceoverview/?country=DE&currency=2&appid=" + item.AppId + "&market_hash_name=" + item.ItemName);

            var o = JObject.Parse(itemResult);
            var price = o.GetValue("lowest_price");
            item.LastKnownPrice = Convert.ToDouble(price.ToString().Replace("{", "").Replace("}", "").Replace("£", ""));
            return item;
        }

        static public byte[] GetBytesFromUrl(string url)
        {
            byte[] b;
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            WebResponse myResp = myReq.GetResponse();

            Stream stream = myResp.GetResponseStream();
            using (BinaryReader br = new BinaryReader(stream))
            {
                b = br.ReadBytes(500000);
                br.Close();
            }
            myResp.Close();
            return b;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}