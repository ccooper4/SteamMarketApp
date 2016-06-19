using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SteamApp.Models
{
    public class MarketItemModel 
    {
        public string Description { get; set; }

        public double? LastKnownPrice { get; set; }

        public int Id { get; set; }

        public string ItemUrl { get; set; }

        public string ImageUrl { get; set; }

        public byte[] Image { get; set; }
    }
}