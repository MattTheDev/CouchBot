using System;
using System.Collections.Generic;
using System.Text;

namespace MTD.CouchBot.Domain.Models.Piczel
{
    public class PiczelBanner
    {
        public Banner banner { get; set; }
    }

    public class Banner
    {
        public string Url { get; set; }
    }
}
